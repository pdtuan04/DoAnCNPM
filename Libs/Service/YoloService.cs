using Libs.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
namespace Libs.Service
{
    public class YoloService : IDisposable
    {
        private readonly InferenceSession _session;

        // CẤU HÌNH YOLO11 (Thường là 640x640)
        private const int ImgSize = 640;
        private const float ConfidenceThreshold = 0.5f; // Giảm xuống 0.25 để bắt nhạy hơn
        private const float IouThreshold = 0.45f;      // Ngưỡng lọc trùng

        // DANH SÁCH 51 CLASS CỦA BẠN
        private readonly string[] _labels = new string[]
        {
            "Đường người đi bộ cắt ngang",                // 0
            "Đường giao nhau (ngã ba bên phải)",          // 1
            "Cấm đi ngược chiều",                         // 2
            "Phải đi vòng sang bên phải",                 // 3
            "Giao nhau với đường đồng cấp",               // 4
            "Giao nhau với đường không ưu tiên",          // 5
            "Chỗ ngoặt nguy hiểm vòng bên trái",          // 6
            "Cấm rẽ trái",                                // 7
            "Bến xe buýt",                                // 8
            "Nơi giao nhau chạy theo vòng xuyến",         // 9
            "Cấm dừng và đỗ xe",                          // 10
            "Chỗ quay xe",                                // 11
            "Biển gộp làn đường theo phương tiện",        // 12
            "Đi chậm",                                    // 13
            "Cấm xe tải",                                 // 14
            "Đường bị thu hẹp về phía phải",              // 15
            "Giới hạn chiều cao",                         // 16
            "Cấm quay đầu",                               // 17
            "Cấm ô tô khách và ô tô tải",                 // 18
            "Cấm rẽ phải và quay đầu",                    // 19
            "Cấm ô tô",                                   // 20
            "Đường bị thu hẹp về phía trái",              // 21
            "Gồ giảm tốc phía trước",                     // 22
            "Cấm xe hai và ba bánh",                      // 23
            "Kiểm tra",                                   // 24
            "Chỉ dành cho xe máy*",                       // 25
            "Chướng ngoại vật phía trước",                // 26
            "Trẻ em",                                     // 27
            "Xe tải và xe công*",                         // 28
            "Cấm mô tô và xe máy",                        // 29
            "Chỉ dành cho xe tải*",                       // 30
            "Đường có camera giám sát",                   // 31
            "Cấm rẽ phải",                                // 32
            "Nhiều chỗ ngoặt nguy hiểm liên tiếp, chỗ đầu tiên sang phải", // 33
            "Cấm xe sơ-mi rơ-moóc",                       // 34
            "Cấm rẽ trái và phải",                        // 35
            "Cấm đi thẳng và rẽ phải",                    // 36
            "Đường giao nhau (ngã ba bên trái)",          // 37
            "Giới hạn tốc độ",                   // 38
            "Giới hạn tốc độ",                   // 39
            "Giới hạn tốc độ",                   // 40
            "Giới hạn tốc độ",                   // 41
            "Các xe chỉ được rẽ trái",                    // 42
            "Chiều cao tĩnh không thực tế",               // 43
            "Nguy hiểm khác",                             // 44
            "Đường một chiều",                            // 45
            "Cấm đỗ xe",                                  // 46
            "Cấm ô tô quay đầu xe (được rẽ trái)",        // 47
            "Giao nhau với đường sắt có rào chắn",        // 48
            "Cấm rẽ trái và quay đầu xe",                 // 49
            "Chỗ ngoặt nguy hiểm vòng bên phải",          // 50
            "Chú ý chướng ngại vật – vòng tránh sang bên phải" // 51
        };

        public YoloService()
        {
            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "best.onnx");
            _session = new InferenceSession(modelPath);
        }

        public List<YoloPrediction> Detect(Stream imageStream)
        {
            // 1. PRE-PROCESSING
            using var image = Image.Load<Rgb24>(imageStream);
            int originalW = image.Width;
            int originalH = image.Height;

            image.Mutate(x => x.Resize(ImgSize, ImgSize));

            var input = new DenseTensor<float>(new[] { 1, 3, ImgSize, ImgSize });
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        input[0, 0, y, x] = row[x].R / 255.0f;
                        input[0, 1, y, x] = row[x].G / 255.0f;
                        input[0, 2, y, x] = row[x].B / 255.0f;
                    }
                }
            });

            // 2. INFERENCE
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("images", input) };

            using var outputs = _session.Run(inputs);
            var outputTensor = outputs.First().AsTensor<float>();

            // 3. POST-PROCESSING
            return ParseOutput(outputTensor, originalW, originalH);
        }

        private List<YoloPrediction> ParseOutput(Tensor<float> output, int orgW, int orgH)
        {
            var boxes = new List<YoloPrediction>();

            // Output YOLO: [1, 4 + 51, 8400]
            int dimensions = output.Dimensions[1]; // 55
            int anchors = output.Dimensions[2];    // 8400

            for (int i = 0; i < anchors; i++)
            {
                float maxScore = 0;
                int maxClassId = -1;

                // Loop từ index 4 đến 54 (51 classes)
                for (int j = 4; j < dimensions; j++)
                {
                    float score = output[0, j, i];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxClassId = j - 4;
                    }
                }

                if (maxScore < ConfidenceThreshold) continue;

                float cx = output[0, 0, i];
                float cy = output[0, 1, i];
                float w = output[0, 2, i];
                float h = output[0, 3, i];

                float x = cx - (w / 2);
                float y = cy - (h / 2);

                float xFactor = (float)orgW / ImgSize;
                float yFactor = (float)orgH / ImgSize;

                // Sửa lỗi RectangleF bằng cách dùng constructor tường minh
                boxes.Add(new YoloPrediction
                {
                    Label = _labels[maxClassId],
                    Confidence = maxScore,
                    BBox = new RectangleF(x * xFactor, y * yFactor, w * xFactor, h * yFactor)
                });
            }

            return NonMaxSuppression(boxes);
        }

        private List<YoloPrediction> NonMaxSuppression(List<YoloPrediction> boxes)
        {
            var result = new List<YoloPrediction>();
            var sortedBoxes = boxes.OrderByDescending(b => b.Confidence).ToList();

            while (sortedBoxes.Count > 0)
            {
                var current = sortedBoxes[0];
                result.Add(current);
                sortedBoxes.RemoveAt(0);

                for (int i = sortedBoxes.Count - 1; i >= 0; i--)
                {
                    // CalculateIoU dùng RectangleF của SixLabors vẫn OK
                    if (CalculateIoU(current.BBox, sortedBoxes[i].BBox) > IouThreshold)
                    {
                        sortedBoxes.RemoveAt(i);
                    }
                }
            }
            return result;
        }

        private float CalculateIoU(RectangleF b1, RectangleF b2)
        {
            float x1 = Math.Max(b1.X, b2.X);
            float y1 = Math.Max(b1.Y, b2.Y);
            float x2 = Math.Min(b1.X + b1.Width, b2.X + b2.Width);
            float y2 = Math.Min(b1.Y + b1.Height, b2.Y + b2.Height);

            float w = Math.Max(0, x2 - x1);
            float h = Math.Max(0, y2 - y1);
            float intersection = w * h;

            float area1 = b1.Width * b1.Height;
            float area2 = b2.Width * b2.Height;

            return intersection / (area1 + area2 - intersection);
        }

        public void Dispose() => _session?.Dispose();
    }
}

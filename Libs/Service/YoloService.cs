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
        private const float ConfidenceThreshold = 0.25f; // Giảm xuống 0.25 để bắt nhạy hơn
        private const float IouThreshold = 0.45f;      // Ngưỡng lọc trùng

        // DANH SÁCH 51 CLASS CỦA BẠN
        private readonly string[] _labels = new string[]
        {
            "Ben xe buyt",
            "Bien gop lan duong theo phuong tien",
            "Cac xe chi duoc re trai",
            "Cam di nguoc chieu",
            "Cam di thang va re phai",
            "Cam do xe",
            "Cam dung va do xe",
            "Cam mo to va xe may",
            "Cam o to",
            "Cam o to khach va o to tai",
            "Cam o to quay dau xe -duoc re trai-",
            "Cam quay dau",
            "Cam re phai",
            "Cam re phai va quay dau",
            "Cam re trai",
            "Cam re trai va phai",
            "Cam re trai va quay dau xe",
            "Cam xe hai va ba banh",
            "Cam xe so-mi ro-mooc",
            "Cam xe tai",
            "Chi danh cho xe may-",
            "Chi danh cho xe tai-",
            "Chieu cao tinh khong thuc te",
            "Cho ngoat nguy hiem vong ben phai",
            "Cho ngoat nguy hiem vong ben trai",
            "Cho quay xe",
            "Chu y chuong ngai vat - vong tranh sang ben phai",
            "Chuong ngoai vat phia truoc",
            "Di cham",
            "Duong bi thu hep ve phia phai",
            "Duong bi thu hep ve phia trai",
            "Duong co camera giam sat",
            "Duong giao nhau -nga ba ben phai-",
            "Duong giao nhau -nga ba ben trai-",
            "Duong mot chieu",
            "Duong nguoi di bo cat ngang",
            "Giao nhau voi duong dong cap",
            "Giao nhau voi duong khong uu tien",
            "Giao nhau voi duong sat co rao chan",
            "Gioi han chieu cao",
            "Gioi han toc do -40km-h-",
            "Gioi han toc do -50km-h-",
            "Gioi han toc do -60km-h-",
            "Gioi han toc do -80km-h-",
            "Go giam toc phia truoc",
            "Kiem tra",
            "Nguy hiem khac",
            "Noi giao nhau chay theo vong xuyen",
            "Phai di vong sang ben phai",
            "Tre em",
            "Xe tai va xe cong-"
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

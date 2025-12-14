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

        private readonly string[] _labels = new string[]
        {
            "BenXeBuyt",
            "BienMotChieu",
            "CamDiNguocChieu",
            "CamDungCamDo",
            "CamXeHaiBaBanh"
        };

        public YoloService()
        {
            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "last_final.onnx");
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

            // Kiểm tra output shape
            int dimensions = output.Dimensions[1]; // Nên là 4 + _labels.Length
            int anchors = output.Dimensions[2];

            for (int i = 0; i < anchors; i++)
            {
                float maxScore = 0;
                int maxClassId = -1;

                // Chỉ loop qua số class thực tế
                int numClasses = Math.Min(dimensions - 4, _labels.Length);
                for (int j = 4; j < 4 + numClasses; j++)
                {
                    float score = output[0, j, i];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxClassId = j - 4;
                    }
                }

                if (maxScore < ConfidenceThreshold || maxClassId >= _labels.Length)
                    continue;

                float cx = output[0, 0, i];
                float cy = output[0, 1, i];
                float w = output[0, 2, i];
                float h = output[0, 3, i];

                float x = Math.Max(0, cx - (w / 2));
                float y = Math.Max(0, cy - (h / 2));

                float xFactor = (float)orgW / ImgSize;
                float yFactor = (float)orgH / ImgSize;

                boxes.Add(new YoloPrediction
                {
                    Label = _labels[maxClassId],
                    Confidence = maxScore,
                    BBox = new RectangleF(
                        x * xFactor,
                        y * yFactor,
                        Math.Min(w * xFactor, orgW - x * xFactor),
                        Math.Min(h * yFactor, orgH - y * yFactor)
                    )
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

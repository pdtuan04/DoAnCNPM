using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Models
{
    public class YoloPrediction
    {
        public string Label { get; set; }
        public float Confidence { get; set; }
        public RectangleF BBox { get; set; } // Nó sẽ tự dùng của SixLabors.ImageSharp
    }
}

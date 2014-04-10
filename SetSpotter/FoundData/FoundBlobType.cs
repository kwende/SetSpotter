using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.FoundData
{
    public class FoundBlobType
    {
        public ShapeTypeEnum ShapeType { get; set; }
        public FillTypeEnum FillType { get; set; }
        public ColorTypeEnum ColorType { get; set; }
        public Blob Blob { get; set; }

        public Color AverageColor { get; set; }
        public Bitmap StrippedBitmap { get; set; }
        public float AverageHue { get; set; }

        public int[] Histogram { get; set; }
    }
}

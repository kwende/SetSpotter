using AForge.Imaging.Filters;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class ColorSpaceFinder
    {
        public static FoundColorSpaces Find(string imagePath)
        {
            FoundColorSpaces ret = new FoundColorSpaces(); 

            // load bmp
            ret.OriginalColorSpace = (Bitmap)System.Drawing.Image.FromFile(imagePath);
            // gray bmp
            ret.GrayColorSpace = (new Grayscale(0.2125, 0.7154, 0.0721)).Apply(ret.OriginalColorSpace);
            // threshold
            ret.BinaryColorSpace = (new Threshold(135)).Apply(ret.GrayColorSpace);

            return ret; 
        }
    }
}

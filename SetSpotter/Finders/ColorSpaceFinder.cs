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
            float averageBrightness = 0f;
            float count = 0f;
            for (int y = 0; y < ret.OriginalColorSpace.Height; y += 10)
            {
                for (int x = 0; x < ret.OriginalColorSpace.Width; x += 10)
                {
                    averageBrightness += ret.GrayColorSpace.GetPixel(x, y).GetBrightness();
                    count++;
                }
            }
            averageBrightness /= count; 
            int computedThreshold = (int)Math.Floor(150 * (averageBrightness / .48f)); 
            ret.BinaryColorSpace = (new Threshold(computedThreshold).Apply(ret.GrayColorSpace));

            return ret; 
        }
    }
}

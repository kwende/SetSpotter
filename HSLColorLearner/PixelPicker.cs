using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSLColorLearner
{
    public class LeastLuminousPixelPicker : IPixelPicker
    {
        public HSL[] Get(Bitmap bmp, int take)
        {
            List<HSL> pixels = new List<HSL>();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color = bmp.GetPixel(x, y);
                    pixels.Add(HSL.FromRGB(new RGB(color.R, color.G, color.B)));
                }
            }
            return pixels.OrderBy(m => m.Luminance).Take(take).ToArray(); 
        }
    }
}

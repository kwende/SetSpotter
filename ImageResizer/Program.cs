using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceDirectory = Path.GetDirectoryName(args[0]); 

            string[] bmpPaths = Directory.GetFiles(args[0]);
            foreach (string bmpPath in bmpPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(bmpPath); 

                Bitmap bmp = (Bitmap)Image.FromFile(bmpPath);
                float scaler = 816 / (bmp.Width * 1.0f); 

                ResizeBicubic resizer = new ResizeBicubic(
                    (int)Math.Floor(scaler * bmp.Width),
                    (int)Math.Floor(scaler * bmp.Height));

                resizer.Apply(bmp).Save(Path.Combine(sourceDirectory, fileName + "_resized.bmp")); 
            }
        }
    }
}

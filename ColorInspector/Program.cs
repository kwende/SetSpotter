using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using AForge.Imaging;
using AForge.Imaging.Filters;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorInspector
{
    class Program
    {
        static void DumpHistogram()
        {
            string[] files = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\diamonds");
            Dictionary<int, int> histogram = new Dictionary<int, int>();

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName.StartsWith("purple"))
                {
                    using (Bitmap bmp = (Bitmap)Bitmap.FromFile(file))
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            for (int x = 0; x < bmp.Width; x++)
                            {
                                int hue = (int)(bmp.GetPixel(x, y).GetHue());
                                if (histogram.ContainsKey(hue))
                                {
                                    histogram[hue]++;
                                }
                                else
                                {
                                    histogram[hue] = 1;
                                }
                            }
                        }
                    }
                }
            }

            for (int c = 0; c < 360; c++)
            {
                if (histogram.ContainsKey(c))
                {
                    File.AppendAllText(@"c:\users\brush\desktop\purplefile.csv", histogram[c].ToString() + "\r\n");
                }
                else
                {
                    File.AppendAllText(@"c:\users\brush\desktop\purplefile.csv", "0\r\n");
                }
            }
        }

        static void Main(string[] args)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\diamonds\red__6714dc3d2605484b9416e6ff7a8c09a1.bmp");

            List<Color> brightnessList = new List<Color>();
            for (int y = 0; y < bmp.Height; y++)
            {
                if (y == 0 || y == bmp.Height - 1)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        brightnessList.Add(bmp.GetPixel(x, y));
                    }
                }
                else
                {
                    brightnessList.Add(bmp.GetPixel(0, y));
                    brightnessList.Add(bmp.GetPixel(bmp.Width - 1, y));
                }
            }
            Color[] whites = brightnessList.OrderByDescending(m => m.GetBrightness()).Take(10).ToArray();
            double averageR = whites.Average(m => m.R);
            double averageG = whites.Average(m => m.G);
            double averageB = whites.Average(m => m.B);

            double mean = (averageR + averageG + averageB) / 3.0;
            double gScaler = mean / averageG;
            double bScaler = mean / averageB;
            double rScaler = mean / averageR;

            bmp.Save(@"c:\users\brush\desktop\original.bmp"); 
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    HSL hsl = HSL.FromRGB(new RGB(c.R, c.G, c.B));
                    if (hsl.Hue < 0) hsl.Hue = 0;
                    RGB rgb = hsl.ToRGB();
                    byte b = (byte)Math.Floor(rgb.Blue * bScaler);
                    byte g = (byte)Math.Floor(rgb.Green * gScaler);
                    byte r = (byte)Math.Floor(rgb.Red * rScaler);
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            bmp.Save(@"C:\users\brush\desktop\test.bmp");

            return;
        }
    }
}

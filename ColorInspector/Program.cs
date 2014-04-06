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
            
        }
    }
}

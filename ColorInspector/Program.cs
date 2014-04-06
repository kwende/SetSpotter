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
            string[] squiggles = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\squiggle\");
            string[] pills = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\pill\");
            string[] diamonds = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\diamonds\");

            List<string> all = new List<string>();
            all.AddRange(squiggles);
            all.AddRange(pills);
            all.AddRange(diamonds);

            int bucketSize = 15;
            foreach (string file in all.Where(m => Path.GetFileName(m).StartsWith("red")))
            {
                int[] histogram = new int[360 / bucketSize];

                using (Bitmap bmp = (Bitmap)Bitmap.FromFile(file))
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color color = bmp.GetPixel(x, y);
                            int hue = (int)(color.GetHue() / bucketSize);
                            histogram[hue]++;
                        }
                    }
                }

                File.Delete(@"c:\users\brush\desktop\hist.csv");
                for (int c = 0; c < histogram.Length; c++)
                {
                    File.AppendAllText(@"c:\users\brush\desktop\hist.csv", histogram[c].ToString() + "\r\n");
                    //if (histogram[c] < minHistogram[c])
                    //{
                    //    minHistogram[c] = histogram[c];
                    //}
                }

                Console.WriteLine("Poop"); 
            }
        }
    }
}

using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using AForge.Imaging;
using AForge.Imaging.Filters;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            foreach (string file in Directory.GetFiles(@"C:\repos\SetSpotter\SetSpotter\SetSpotter\testimages"))
            {
                FoundColorSpaces foundColorSpaces = ColorSpaceFinder.Find(file);
                FoundBlobs foundBlobs = BlobFinder.Find(foundColorSpaces, 80, 25, 90, 50, 1.2, 2.2);

                string destinationDirectory = @"C:\Users\brush\Desktop\out\" + Path.GetFileNameWithoutExtension(file) + "\\";
                Directory.CreateDirectory(destinationDirectory);

                foreach (Blob blob in foundBlobs.Blobs)
                {
                    foundColorSpaces.OriginalColorSpace.Clone(blob.Rectangle, PixelFormat.Format24bppRgb).Save(
                         destinationDirectory + Guid.NewGuid().ToString() + ".bmp");
                }
            }

            return;
        }
    }
}

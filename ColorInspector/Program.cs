using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using AForge.Imaging;
using AForge.Imaging.Filters;
using LibSVMWrapper;
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

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\testimages\");
            int i = 0;
            Predict red = new Predict(@"C:\Users\brush\Desktop\round2\red.txt.model");
            Predict purple = new Predict(@"C:\Users\brush\Desktop\round2\purple.txt.model");
            Predict green = new Predict(@"C:\Users\brush\Desktop\round2\green.txt.model");

            //foreach (string file in files)
            {
                FoundColorSpaces foundColorSpaces = ColorSpaceFinder.Find(@"C:\Users\brush\Desktop\IMAG2112.jpg");

                FoundBlobs foundBlobs = BlobFinder.Find(foundColorSpaces, 80, 25, 90, 50, 1.2, 2.2);

                foreach (Blob blob in foundBlobs.Blobs)
                {
                    //Bitmap color = foundColorSpaces.OriginalColorSpace.Clone(blob.Rectangle, PixelFormat.Format24bppRgb);
                    Bitmap color = ColorSpaceFinder.FindColorCorrectedForBlob(foundColorSpaces, blob);
                    ColorTypeEnum colorType = ColorSpaceFinder.FindShapeColor(color, red, purple, green);

                    using (Graphics g = Graphics.FromImage(color))
                    {
                        g.DrawString(colorType.ToString(), new Font("Arial", 12), Brushes.Black, new PointF(0, 5));
                    }
                    color.Save(@"C:\users\brush\desktop\blobs\" + (i++).ToString() + ".bmp");
                }
            }
            return;
        }
    }
}

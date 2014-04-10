using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
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

namespace SetSpotter
{
    class Program
    {
        //http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        static bool pnpoly(int nvert, float[] vertx, float[] verty, float testx, float testy)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((verty[i] > testy) != (verty[j] > testy)) &&
                 (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
                    c = !c;
            }
            return c;
        }

        static void DoIt(string image)
        {
            FoundColorSpaces foundColorSpaces = ColorSpaceFinder.Find(image);
            FoundBlobs foundBlobs = BlobFinder.Find(foundColorSpaces, 80, 25, 90, 50, 1.2, 2.2); 

            using (Graphics g = Graphics.FromImage(foundColorSpaces.OriginalColorSpace))
            {
                foreach (Blob blob in foundBlobs.Blobs)
                {
                    Bitmap axisAlignedBitmap = AxisAlignedBitmapFinder.Find(blob, foundBlobs.BlobCounter, foundColorSpaces);
                    FoundBlobType foundType = ShapeTypeFinder.Find(axisAlignedBitmap, foundColorSpaces);
                }
            }

            return; 
        }

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles("testimages");
            foreach (string file in files)
            {
                DoIt(file);
            }
            return;
        }
    }
}

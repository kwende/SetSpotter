using AForge.Imaging;
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
    public static class BlobFinder
    {
        public static FoundBlobs Find(FoundColorSpaces colorSpaces)
        {
            FoundBlobs foundBlobs = new FoundBlobs(); 

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(colorSpaces.BinaryColorSpace);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            foundBlobs.Blobs = blobs.OrderByDescending(m => m.Area).Take(12).ToArray();

            return foundBlobs; 
        }

        public static void DebugDrawBlobs(Bitmap debugBitmap, FoundBlobs foundBlobs)
        {
            using(Graphics g = Graphics.FromImage(debugBitmap))
            {
                foreach(Blob blob in foundBlobs.Blobs)
                {
                    g.DrawRectangle(new Pen(Brushes.Red, 3), blob.Rectangle); 
                }
            }
        }
    }
}

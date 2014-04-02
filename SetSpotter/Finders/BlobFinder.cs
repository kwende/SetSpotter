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

            SobelEdgeDetector edge = new SobelEdgeDetector();
            Bitmap edges = edge.Apply(colorSpaces.GrayColorSpace);

            Threshold threshold = new Threshold(50);
            threshold.ApplyInPlace(edges);

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(edges);
            foundBlobs.Blobs = blobCounter.GetObjects(colorSpaces.GrayColorSpace, false).Where(
                m => m.Rectangle.Width < 80 &&
                m.Rectangle.Width > 25 &&
                m.Rectangle.Height < 90 &&
                m.Rectangle.Height > 50 &&
                m.Rectangle.Height / (m.Rectangle.Width * 1.0f) > 1.2 &&
                m.Rectangle.Height / (m.Rectangle.Width * 1.0f) < 2.2).ToArray();

            foundBlobs.BlobCounter = blobCounter;

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

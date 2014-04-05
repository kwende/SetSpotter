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
        public static FoundBlobs FindAny(FoundColorSpaces colorSpaces)
        {
            FoundBlobs foundBlobs = new FoundBlobs();

            SobelEdgeDetector edge = new SobelEdgeDetector();
            Bitmap edges = edge.Apply(colorSpaces.GrayColorSpace);

            Threshold threshold = new Threshold(50);
            threshold.ApplyInPlace(edges);

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(edges);
            foundBlobs.Blobs = blobCounter.GetObjects(colorSpaces.GrayColorSpace, false).ToArray();

            foundBlobs.BlobCounter = blobCounter;

            return foundBlobs;
        }

        public static FoundBlobs FindLargest(FoundColorSpaces colorSpaces)
        {
            FoundBlobs foundBlobs = FindAny(colorSpaces); 
            if(foundBlobs.Blobs.Length > 0)
            {
                foundBlobs.Blobs = new Blob[] { foundBlobs.Blobs.OrderByDescending(m => m.Area).First() }; 
            }
            return foundBlobs; 
        }

        public static FoundBlobs Find(FoundColorSpaces colorSpaces, int maxWidth, int minWidth, int maxHeight, int minHeight,
            double minRatio, double maxRatio)
        {
            FoundBlobs foundBlobs = FindAny(colorSpaces);
            foundBlobs.Blobs = foundBlobs.Blobs.Where(
                m => m.Rectangle.Width < maxWidth &&
                m.Rectangle.Width > minWidth &&
                m.Rectangle.Height < maxHeight &&
                m.Rectangle.Height > minHeight &&
                m.Rectangle.Height / (m.Rectangle.Width * 1.0f) > minRatio &&
                m.Rectangle.Height / (m.Rectangle.Width * 1.0f) < maxRatio).ToArray();

            return foundBlobs;
        }

        public static void DebugDrawBlobs(Bitmap debugBitmap, FoundBlobs foundBlobs)
        {
            using (Graphics g = Graphics.FromImage(debugBitmap))
            {
                foreach (Blob blob in foundBlobs.Blobs)
                {
                    g.DrawRectangle(new Pen(Brushes.Red, 3), blob.Rectangle);
                }
            }
        }
    }
}

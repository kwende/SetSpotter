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

            Bitmap debugBitmap = foundColorSpaces.OriginalColorSpace.Clone(
                new Rectangle(0, 0, 
                    foundColorSpaces.OriginalColorSpace.Width, 
                    foundColorSpaces.OriginalColorSpace.Height), 
                    foundColorSpaces.OriginalColorSpace.PixelFormat); 

            FoundBlobs foundBlobs = BlobFinder.Find(foundColorSpaces);
            BlobFinder.DebugDrawBlobs(debugBitmap, foundBlobs);

            debugBitmap.Save(@"C:\users\brush\desktop\debug\" + Path.GetFileNameWithoutExtension(image) + "_debug.bmp");

            return; 
            //// load bmp
            //Bitmap originalBmp = (Bitmap)System.Drawing.Image.FromFile(image);
            //// gray bmp
            //Bitmap grayBmp = (new Grayscale(0.2125, 0.7154, 0.0721)).Apply(originalBmp);
            //// threshold
            //Bitmap thresholded = (new Threshold(135)).Apply(grayBmp);

            //BlobCounter blobCounter = new BlobCounter();
            //blobCounter.ProcessImage(thresholded);
            //Blob[] blobs = blobCounter.GetObjectsInformation();

            //blobs = blobs.OrderByDescending(m => m.Area).Take(12).ToArray();

            //using (Graphics g = Graphics.FromImage(originalBmp))
            //{
            //    foreach (Blob blob in blobs)
            //    {
            //        List<IntPoint> points = blobCounter.GetBlobsEdgePoints(blob);
            //        List<IntPoint> corners = PointsCloud.FindQuadrilateralCorners(points);

            //        //float averageR = 0, averageG = 0, averageB = 0;
            //        //float count = 0; 
            //        List<IntPoint> cardPixels = new List<IntPoint>();
            //        for (int y = blob.Rectangle.Y; y < blob.Rectangle.Y + blob.Rectangle.Height; y++)
            //        {
            //            for (int x = blob.Rectangle.X; x < blob.Rectangle.X + blob.Rectangle.Width; x++)
            //            {
            //                if (pnpoly(corners.Count, corners.Select(m => (float)m.X).ToArray(),
            //                    corners.Select(m => (float)m.Y).ToArray(), x, y))
            //                {
            //                    cardPixels.Add(new IntPoint(x, y));
            //                }
            //            }
            //        }

            //        Bitmap shapeBlobBmp = new Bitmap(blob.Rectangle.Width, blob.Rectangle.Height, PixelFormat.Format24bppRgb);
            //        float averageR = 0, averageG = 0, averageB = 0;
            //        float count = 0;
            //        foreach (IntPoint pixel in cardPixels)
            //        {
            //            if (thresholded.GetPixel(pixel.X, pixel.Y).GetBrightness() != 1.0f)
            //            {
            //                Color color = originalBmp.GetPixel(pixel.X, pixel.Y);
            //                averageR += color.R;
            //                averageG += color.G;
            //                averageB += color.B;
            //                count++;

            //                const int FillSize = 2;
            //                for (int y1 = pixel.Y - FillSize; y1 < pixel.Y + FillSize; y1++)
            //                {
            //                    for (int x1 = pixel.X - FillSize; x1 < pixel.X + FillSize; x1++)
            //                    {
            //                        shapeBlobBmp.SetPixel(x1 - blob.Rectangle.Left, y1 - blob.Rectangle.Top, Color.White);
            //                    }
            //                }
            //            }
            //        }

            //        //GaussianBlur blur = new GaussianBlur(2, 3); 
            //        //blur.ApplyInPlace(shapeBlobBmp);

            //        //shapeBlobBmp.Save(@"c:\users\brush\desktop\image.bmp"); 

            //        averageR /= count;
            //        averageG /= count;

            //        if (averageR > 150)
            //        {
            //            g.DrawString("RED", new Font("Arial", 18, FontStyle.Bold), Brushes.Red, blob.Rectangle.X, blob.Rectangle.Y);
            //        }
            //        else if (averageG > 100)
            //        {
            //            g.DrawString("GREEN", new Font("Arial", 18, FontStyle.Bold), Brushes.Green, blob.Rectangle.X, blob.Rectangle.Y);
            //        }
            //        else
            //        {
            //            g.DrawString("PURPLE", new Font("Arial", 18, FontStyle.Bold), Brushes.Purple, blob.Rectangle.X, blob.Rectangle.Y);
            //        }

            //        shapeBlobBmp = Grayscale.CommonAlgorithms.BT709.Apply(shapeBlobBmp);

            //        BlobCounter shapeBlobCounter = new BlobCounter();
            //        shapeBlobCounter.ProcessImage(shapeBlobBmp);
            //        Blob[] shapeBlobs = shapeBlobCounter.GetObjects(shapeBlobBmp, false).Where(m => m.Area > 50).ToArray();

            //        g.DrawString(shapeBlobs.Length.ToString(), new Font("Arial", 18, FontStyle.Bold), Brushes.Purple, blob.Rectangle.X, blob.Rectangle.Y + 18);

            //        Blob sampleBlob = shapeBlobs[0];
            //        List<IntPoint> sampleBlobEdgePoints = shapeBlobCounter.GetBlobsEdgePoints(sampleBlob);
            //        SimpleShapeChecker simpleShapeChecker = new SimpleShapeChecker();
            //        bool isQuad = simpleShapeChecker.IsQuadrilateral(sampleBlobEdgePoints);

            //        Console.WriteLine(isQuad.ToString());

            //        if (isQuad)
            //        {
            //            g.DrawString("Diamond", new Font("Arial", 18, FontStyle.Bold), Brushes.Purple, blob.Rectangle.X, blob.Rectangle.Y + 36);
            //        }
            //        else
            //        {
            //            sampleBlob.Image.ToManagedImage().Save(@"c:\users\brush\desktop\shape.bmp");
            //            // compute a normalized euclidean distance from the centroid of the blob to the edge points, use this
            //            // to form a histogram which can hopefully be used as a means of final shape classification. 
            //            AForge.Point centerOfGravity = sampleBlob.CenterOfGravity;
            //            //File.Delete(@"c:\users\brush\desktop\histogram.csv");
            //            List<float> distances = new List<float>();
            //            foreach (IntPoint edgePoint in sampleBlobEdgePoints)
            //            {
            //                float distance = centerOfGravity.SquaredDistanceTo(edgePoint);
            //                distances.Add(distance);
            //                //File.AppendAllText(@"c:\users\brush\desktop\histogram.csv", distance.ToString() + "\r\n"); 
            //            }
            //            float max = distances.Max();
            //            float[] scaledDistances = distances.Select(m => m / max).ToArray();
            //            if (scaledDistances.Where(m => m < .2).Count() > 20)
            //            {
            //                g.DrawString("Squiggle", new Font("Arial", 18, FontStyle.Bold), Brushes.Purple, blob.Rectangle.X, blob.Rectangle.Y + 36);
            //            }
            //            else
            //            {
            //                g.DrawString("Pill", new Font("Arial", 18, FontStyle.Bold), Brushes.Purple, blob.Rectangle.X, blob.Rectangle.Y + 36);
            //            }
            //        }
            //    }
            //}

            //originalBmp.Save(image + "_processed.bmp"); 
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

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class BlobTypeFinder
    {
        public static FoundBlobType Find(Blob blob, FoundColorSpaces foundColorSpaces, BlobCounter blobCounter)
        {
            FoundBlobType foundBlobType = new FoundBlobType();

            List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);

            int indexOfPoint1 = 0, indexOfPoint2 = 0;
            float longestDistance = 0f;
            for (int a = 0; a < edgePoints.Count; a++)
            {
                for (int b = 0; b < edgePoints.Count; b++)
                {
                    if (a != b)
                    {
                        float distance = edgePoints[a].DistanceTo(edgePoints[b]);
                        if (distance > longestDistance)
                        {
                            indexOfPoint1 = a;
                            indexOfPoint2 = b;
                            longestDistance = distance;
                        }
                    }
                }
            }

            //foundColorSpaces.OriginalColorSpace.Clone(blob.Rectangle, PixelFormat.Format24bppRgb);

            List<IntPoint> leftPoints, rightPoints;
            blobCounter.GetBlobsLeftAndRightEdges(blob, out leftPoints, out rightPoints);

            if (leftPoints.Count != rightPoints.Count) throw new Exception(); 

            Bitmap trimmedColor = new Bitmap(blob.Rectangle.Width, blob.Rectangle.Height, PixelFormat.Format24bppRgb);
            for (int c = 0; c < leftPoints.Count; c++)
            {
                IntPoint leftPoint = leftPoints[c];
                IntPoint rightPoint = rightPoints[c];

                for (int x = leftPoint.X - blob.Rectangle.Left; x <= rightPoint.X - blob.Rectangle.Left; x++)
                {
                    trimmedColor.SetPixel(x, leftPoint.Y - blob.Rectangle.Top, Color.Red); 
                }
            }

            IntPoint origin = new IntPoint(blob.Rectangle.X, blob.Rectangle.Y);
            int point1X = edgePoints[indexOfPoint1].X - origin.X;
            int point1Y = edgePoints[indexOfPoint1].Y - origin.Y;
            int point2X = edgePoints[indexOfPoint2].X - origin.X;
            int point2Y = edgePoints[indexOfPoint2].Y - origin.Y;

            IntPoint topPoint;
            if (point2Y < point1Y)
            {
                topPoint = new IntPoint(point2X, point2Y);
            }
            else
            {
                topPoint = new IntPoint(point1X, point1Y);
            }

            float errorAngle = AForge.Math.Geometry.GeometryTools.GetAngleBetweenLines(
                new AForge.Point(trimmedColor.Width / 2, trimmedColor.Height / 2), new AForge.Point(trimmedColor.Width / 2, 0),
                new AForge.Point(trimmedColor.Width / 2, trimmedColor.Height / 2), new AForge.Point(topPoint.X, topPoint.Y));

            if (topPoint.X < trimmedColor.Width / 2) errorAngle = -errorAngle; 

            //using (Graphics g = Graphics.FromImage(trimmedColor))
            //{
            //    g.DrawLine(new Pen(Brushes.Black, 5), new PointF(point1X, point1Y), new PointF(point2X, point2Y));

            //    //g.DrawRectangle(new Pen(Brushes.Red, 5), new Rectangle(point1X - 2, point1Y - 2, 4, 4));
            //    //g.DrawRectangle(new Pen(Brushes.Red, 5), new Rectangle(point2X - 2, point2Y - 2, 4, 4)); 
            //}

            RotateBicubic rotate = new RotateBicubic(errorAngle, false);
            trimmedColor = rotate.Apply(trimmedColor);

            BlobCounter blobCounter2 = new BlobCounter();
            blobCounter2.ProcessImage(trimmedColor);
            Blob rotatedBlob = blobCounter2.GetObjects(trimmedColor, false).First();
            blobCounter2.GetBlobsLeftAndRightEdges(rotatedBlob, out leftPoints, out rightPoints);
            edgePoints = blobCounter2.GetBlobsEdgePoints(rotatedBlob);


            int scan1Y = (int)Math.Floor((blob.Rectangle.Height * .17) + rotatedBlob.Rectangle.Top);
            int scan2Y = (int)Math.Floor((blob.Rectangle.Height * .83) + rotatedBlob.Rectangle.Top);
            int scanMidY = (int)Math.Floor((blob.Rectangle.Height * .5) + rotatedBlob.Rectangle.Top);

            IntPoint[] edges1 = edgePoints.Where(m => m.Y == scan1Y).ToArray();
            IntPoint[] edges2 = edgePoints.Where(m => m.Y == scan2Y).ToArray();
            IntPoint[] edges3 = edgePoints.Where(m => m.Y == scanMidY).ToArray();
            IntPoint center = new IntPoint((int)blob.CenterOfGravity.X, (int)blob.CenterOfGravity.Y);

            if(edges1.Length == 0)
            {
                trimmedColor.Save(@"C:\users\brush\desktop\blobs\" + Guid.NewGuid().ToString().Replace("-", "") + ".bmp");
                return foundBlobType; 
            }

            float distance1 = Math.Abs(edges1[0].X - edges1[1].X) / (trimmedColor.Width * 1.0f);
            float distance2 = Math.Abs(edges2[0].X - edges2[1].X) / (trimmedColor.Width * 1.0f);
            float distance3 = Math.Abs(edges3[0].X - edges3[1].X) / (trimmedColor.Width * 1.0f);

            using (Graphics g = Graphics.FromImage(trimmedColor))
            {
                if (distance3 > .9 && distance1 < .5 && distance2 < .5)
                {
                    g.DrawString("Diamond", new Font("Arial", 12), Brushes.White, new PointF(10, 10));
                }
            }

            Console.WriteLine(distance1.ToString());
            Console.WriteLine(distance2.ToString());
            Console.WriteLine(distance3.ToString()); 
            trimmedColor.Save(@"C:\users\brush\desktop\blobs\" + Guid.NewGuid().ToString().Replace("-", "") + ".bmp");

            //Console.ReadLine(); 
            return foundBlobType; 
            //SimpleShapeChecker simpleShapeChecker = new SimpleShapeChecker();
            //if (simpleShapeChecker.IsQuadrilateral(edgePoints))
            //{
            //    foundBlobType.ShapeType = ShapeTypeEnum.Diamond;
            //}
            //else
            //{

            //}

            return foundBlobType;
        }
    }
}

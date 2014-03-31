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

            Bitmap trimmedColor = foundColorSpaces.OriginalColorSpace.Clone(blob.Rectangle, PixelFormat.Format24bppRgb);

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
            trimmedColor.Save(@"C:\users\brush\desktop\blobs\" + Guid.NewGuid().ToString().Replace("-", "") + ".bmp");

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

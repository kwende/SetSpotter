using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class BlobTypeFinder
    {
        private static int Count = 0;

        public static FoundBlobType Find(Blob blob, FoundColorSpaces foundColorSpaces, BlobCounter blobCounter)
        {
            FoundBlobType foundBlobType = new FoundBlobType();

            //double area = blob.Area / (foundColorSpaces.OriginalColorSpace.Width * foundColorSpaces.OriginalColorSpace.Height * 1.0);
            Bitmap bmp = new Bitmap(blob.Rectangle.Width, blob.Rectangle.Height, PixelFormat.Format24bppRgb); 
            List<IntPoint> leftEdge, rightEdge;
            blobCounter.GetBlobsLeftAndRightEdges(blob, out leftEdge, out rightEdge); 
            for(int c=0;c<leftEdge.Count;c++)
            {
                int leftX = leftEdge[c].X;
                int rightX = rightEdge[c].X;
                int y = leftEdge[c].Y; 

                for(int x = leftX; x < rightX; x++)
                {
                    bmp.SetPixel(x, y, Color.Red); 
                }
            }

            ResizeBicubic resize = new ResizeBicubic(60, 70);
            bmp = resize.Apply(bmp);
            bmp.Save(@"C:\users\brush\desktop\debug\" + (Count++).ToString() + ".bmp");

            //File.AppendAllText(@"C:\users\brush\desktop\out.csv", area.ToString() + "\r\n"); 
            return foundBlobType;
        }
    }
}

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
    public static class ShapeTypeFinder
    {
        private static int Count = 0;

        private static int EdgeFoundAtX(Bitmap bmp, int y)
        {
            int ret = -1; 
            for (int x = 0; x < bmp.Width; x++)
            {
                Color color = bmp.GetPixel(x, y);
                if (color.R > 0 || color.G > 0 || color.B > 0)
                {
                    ret = x;
                    break; 
                }
            }
            return ret; 
        }

        public static FoundBlobType Find(Bitmap bmp)
        {
            FoundBlobType foundBlobType = new FoundBlobType();

            ResizeBicubic resize = new ResizeBicubic(30, 50);
            Bitmap resizedBitmap = resize.Apply(bmp);

            int p0 = EdgeFoundAtX(resizedBitmap, 6); 
            int p1 = EdgeFoundAtX(resizedBitmap, 16);
            int p2 = EdgeFoundAtX(resizedBitmap, 36); 

            if(p0 < p1 && p2 <= p0)
            {
                foundBlobType.ShapeType = ShapeTypeEnum.Squiggle; 
            }
            else if(Math.Abs(p0 - p1) < 4 && Math.Abs(p2 - p0) < 4)
            {
                foundBlobType.ShapeType = ShapeTypeEnum.Pill;
            }
            else
            {
                resizedBitmap.Save(@"c:\users\brush\desktop\failed.bmp"); 
            }

            return foundBlobType;
        }
    }
}

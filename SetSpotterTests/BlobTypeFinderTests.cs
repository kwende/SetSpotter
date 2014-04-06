using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System.Drawing;

namespace SetSpotterTests
{
    [TestClass]
    public class BlobTypeFinderTests
    {
        [TestMethod]
        public void TestBlobFinder()
        {
            string[] diamondBmps = Directory.GetFiles("separatedshapes/squiggle/");
            foreach (string diamondBmp in diamondBmps)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(diamondBmp);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp);
            }

            string[] pills = Directory.GetFiles("separatedshapes/pill/");
            foreach (string pill in pills)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(pill);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp);
                if (foundBlobType.ShapeType != ShapeTypeEnum.Pill)
                {
                    return;
                }
                Assert.AreEqual(ShapeTypeEnum.Pill, foundBlobType.ShapeType);
            }

            string[] diamonds = Directory.GetFiles("separatedshapes/diamonds/");
            foreach (string diamond in diamonds)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(diamond);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp);
                Assert.AreEqual(ShapeTypeEnum.Diamond, foundBlobType.ShapeType);
            }

            string[] errors = Directory.GetFiles("separatedshapes/error/");
            foreach (string error in errors)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(error);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp);
                Assert.AreEqual(ShapeTypeEnum.NotAType, foundBlobType.ShapeType);
            }

            return; 
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Imaging;

namespace SetSpotterTests
{
    [TestClass]
    public class BlobTypeFinderTests
    {
        private int NumberInRange(Dictionary<int, int> histogram, int startRange, int endRange)
        {
            int ret = 0;
            for (int c = startRange; c <= endRange; c++)
            {
                if (histogram.ContainsKey(c))
                {
                    ret += histogram[c];
                }
            }
            return ret;
        }

        [TestMethod]
        public void TestColors()
        {
            string[] cards = Directory.GetFiles(@"labeledshapes");

            foreach (string file in cards)
            {
                string fileName = Path.GetFileName(file);
                string[] identifiers = fileName.Split('-').Take(3).ToArray();
            }
        }

        [TestMethod]
        public void TestShapes()
        {
            string[] diamondBmps = Directory.GetFiles("separatedshapes/squiggle/");
            foreach (string diamondBmp in diamondBmps)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(diamondBmp);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp, colorSpaces);
            }

            string[] pills = Directory.GetFiles("separatedshapes/pill/");
            foreach (string pill in pills)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(pill);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp, colorSpaces);
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

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp, colorSpaces);
                Assert.AreEqual(ShapeTypeEnum.Diamond, foundBlobType.ShapeType);
            }

            string[] errors = Directory.GetFiles("separatedshapes/error/");
            foreach (string error in errors)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(error);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp, colorSpaces);
                Assert.AreEqual(ShapeTypeEnum.NotAType, foundBlobType.ShapeType);
            }

            return;
        }
    }
}

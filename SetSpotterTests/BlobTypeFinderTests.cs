﻿using System;
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
                Assert.AreEqual(ShapeTypeEnum.Pill, foundBlobType.ShapeType);
            }

            //string[] squiggles = Directory.GetFiles("separatedshapes/squiggle/");
            //foreach (string squiggle in squiggles)
            //{
            //    FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(squiggle);
            //    FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
            //    BlobTypeFinder.Find(foundBlobs.Blobs[0], colorSpaces, foundBlobs.BlobCounter);
            //}

            return; 
        }
    }
}
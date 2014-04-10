using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System.Drawing;
using System.Collections.Generic;

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
            string[] squiggles = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\squiggle\");
            string[] pills = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\pill\");
            string[] diamonds = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\diamonds\");

            List<string> all = new List<string>();
            all.AddRange(squiggles);
            all.AddRange(pills);
            all.AddRange(diamonds);

            double[] redHistogram = new double[360];
            double[] purpleHistogram = new double[360];
            double[] greenHistogram = new double[360];

            double numGreen = 0, numPurple = 0, numRed = 0;
            foreach (string file in all)
            {
                string fileName = Path.GetFileName(file);
                string color = fileName.Substring(0, fileName.IndexOf('_'));

                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(file);
                FoundBlobs foundBlobs = BlobFinder.FindLargest(colorSpaces);
                Bitmap bmp = AxisAlignedBitmapFinder.Find(foundBlobs.Blobs[0], foundBlobs.BlobCounter, colorSpaces);

                FoundBlobType foundBlobType = ShapeTypeFinder.Find(bmp, colorSpaces);

                switch (color)
                {
                    case "red":
                        numRed++;
                        for (int c = 0; c < redHistogram.Length; c++)
                        {
                            redHistogram[c] += foundBlobType.Histogram[c];
                        }
                        break;
                    case "green":
                        numGreen++;
                        for (int c = 0; c < greenHistogram.Length; c++)
                        {
                            greenHistogram[c] += foundBlobType.Histogram[c];
                        }
                        break;
                    case "purple":
                        numPurple++;
                        for (int c = 0; c < purpleHistogram.Length; c++)
                        {
                            purpleHistogram[c] += foundBlobType.Histogram[c];
                        }
                        break;
                }

                switch (color)
                {
                    case "red":
                        foundBlobType.StrippedBitmap.Save(@"c:\users\brush\desktop\errorStripped.bmp");
                        colorSpaces.OriginalColorSpace.Save(@"c:\users\brush\desktop\errorOriginal.bmp");
                        Assert.AreEqual(ColorTypeEnum.Red, foundBlobType.ColorType);
                        break;
                    case "green":
                        foundBlobType.StrippedBitmap.Save(@"c:\users\brush\desktop\errorStripped.bmp");
                        colorSpaces.OriginalColorSpace.Save(@"c:\users\brush\desktop\errorOriginal.bmp");
                        Assert.AreEqual(ColorTypeEnum.Green, foundBlobType.ColorType);
                        break;
                    case "purple":
                        foundBlobType.StrippedBitmap.Save(@"c:\users\brush\desktop\errorStripped.bmp");
                        colorSpaces.OriginalColorSpace.Save(@"c:\users\brush\desktop\errorOriginal.bmp");
                        Assert.AreEqual(ColorTypeEnum.Purple, foundBlobType.ColorType);
                        break;
                }
            }

            //for (int c = 0; c < redHistogram.Length; c++)
            //{
            //    redHistogram[c] = Math.Round(redHistogram[c] / numRed);
            //    File.AppendAllText(@"c:\users\brush\desktop\red.csv", redHistogram[c].ToString() + "\r\n");
            //}

            //for (int c = 0; c < purpleHistogram.Length; c++)
            //{
            //    purpleHistogram[c] = Math.Round(purpleHistogram[c] / numPurple);
            //    File.AppendAllText(@"c:\users\brush\desktop\purple.csv", purpleHistogram[c].ToString() + "\r\n");
            //}

            //for (int c = 0; c < greenHistogram.Length; c++)
            //{
            //    greenHistogram[c] = Math.Round(greenHistogram[c] / numGreen);
            //    File.AppendAllText(@"c:\users\brush\desktop\green.csv", greenHistogram[c].ToString() + "\r\n");
            //}

            //File.Delete(@"C:\users\brush\desktop\test.csv");
            //using (FileStream fs = File.OpenWrite(@"C:\users\brush\desktop\test.csv"))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        for (int c = 0; c < histogram.Length; c++)
            //        {
            //            sw.WriteLine(histogram[c].ToString());
            //        }
            //    }
            //}
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

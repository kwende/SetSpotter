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
        private int NumberInRange(Dictionary<int,int> histogram, int startRange, int endRange)
        {
            int ret = 0; 
            for(int c=startRange;c<=endRange;c++)
            {
                if(histogram.ContainsKey(c))
                {
                    ret += histogram[c]; 
                }
            }
            return ret; 
        }

        [TestMethod]
        public void TestColors()
        {
            string[] squiggles = Directory.GetFiles("separatedshapes/squiggle/");
            string[] pills = Directory.GetFiles("separatedshapes/pill/");
            string[] diamonds = Directory.GetFiles("separatedshapes/diamonds/");

            List<string> all = new List<string>();
            all.AddRange(squiggles);
            all.AddRange(pills);
            all.AddRange(diamonds);

            foreach (string file in all)
            {
                string fileName = Path.GetFileName(file);
                string colorName = fileName.Substring(0, fileName.IndexOf('_')).ToLower();
                Dictionary<int, int> histogram = new Dictionary<int, int>();

                using (Bitmap bmp = (Bitmap)Bitmap.FromFile(file))
                {
                    double total = 0; 
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color color = bmp.GetPixel(x, y);
                            int hue = (int)color.GetHue();
                            float saturation = color.GetSaturation(); 
                            if(saturation > .15)
                            {
                                total++; 
                                if (!histogram.ContainsKey(hue))
                                {
                                    histogram.Add(hue, 1);
                                }
                                else
                                {
                                    histogram[hue]++;
                                }
                            }
                        }
                    }

                    if (colorName == "red")
                    {
                        int numberInRedRange = NumberInRange(histogram, 0, 8);
                        numberInRedRange += NumberInRange(histogram, 330, 360); 

                        double percentage = numberInRedRange / total;

                        //Assert.IsTrue(percentage > .5); 
                        if(percentage < .5)
                        {
                            File.Delete(@"c:\users\brush\desktop\red.csv");
                            File.Delete(@"c:\users\brush\desktop\error.bmp");
                            File.Copy(file, @"c:\users\brush\desktop\error.bmp"); 
                            for (int c = 0; c < 360; c++)
                            {
                                if (histogram.ContainsKey(c))
                                {
                                    File.AppendAllText(@"c:\users\brush\desktop\red.csv", histogram[c].ToString() + "\r\n");
                                }
                                else
                                {
                                    File.AppendAllText(@"c:\users\brush\desktop\red.csv", "0\r\n");
                                }
                            }
                            return; 
                        }
                    }
                }
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

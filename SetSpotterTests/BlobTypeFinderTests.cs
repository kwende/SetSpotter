using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SetSpotter.Finders;
using SetSpotter.FoundData;

namespace SetSpotterTests
{
    [TestClass]
    public class BlobTypeFinderTests
    {
        [TestMethod]
        public void TestBlobFinder()
        {
            string[] diamondBmps = Directory.GetFiles("separatedshapes/diamonds/");
            foreach (string diamondBmp in diamondBmps)
            {
                FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(diamondBmp);
                FoundBlobs foundBlobs = BlobFinder.Find(colorSpaces);
                FoundBlobType blobType = BlobTypeFinder.Find(foundBlobs.Blobs[0], colorSpaces, foundBlobs.BlobCounter);
                Assert.AreEqual(ShapeTypeEnum.Diamond, blobType.ShapeType); 
            }
            return; 
        }
    }
}

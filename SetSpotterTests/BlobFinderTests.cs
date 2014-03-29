using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SetSpotter.Finders;
using SetSpotter.FoundData;

namespace SetSpotterTests
{
    [TestClass]
    public class BlobFinderTests
    {
        [TestMethod]
        public void TestBlobFinder()
        {
            string[] bmps = Directory.GetFiles("testimages");
            foreach (string bmp in bmps)
            {
                FoundColorSpaces foundColorSpaces = ColorSpaceFinder.Find(bmp);
                FoundBlobs foundBlobs = BlobFinder.Find(foundColorSpaces); 
            }
            return; 
        }
    }
}

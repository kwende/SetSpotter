using AForge;
using AForge.Imaging;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class AxisAlignedBlobFinder
    {
        public static Bitmap Find(Blob blob, BlobCounter blobCounter)
        {
            List<IntPoint> blobPoints = new List<IntPoint>(); 
            List<IntPoint> leftPoints, rightPoints;
            blobCounter.GetBlobsTopAndBottomEdges(blob, out leftPoints, out rightPoints);

            for (int c = 0; c < leftPoints.Count; c++)
            {
                IntPoint startLeft = leftPoints[c];
                IntPoint endRight = rightPoints[c];

                for (int x = startLeft.X; x <= endRight.X; x++)
                {
                    blobPoints.Add(new IntPoint(x, startLeft.Y)); 
                }
            }

            double[,] aData = new double[blobPoints.Count,2]; 
            double[] bData = new double[blobPoints.Count]; 
            for(int c=0;c<blobPoints.Count;c++)
            {
                aData[c,0] = 1; 
                aData[c,1] = blob
            }

            DenseMatrix A = new DenseMatrix(
        }
    }
}

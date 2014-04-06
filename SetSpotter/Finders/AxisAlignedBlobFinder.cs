using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class AxisAlignedBitmapFinder
    {
        private static int Count = 0; 

        public static Bitmap Find(Blob blob, BlobCounter blobCounter, FoundColorSpaces colorSpaces)
        {
            List<IntPoint> blobPoints = new List<IntPoint>();
            List<IntPoint> leftPoints, rightPoints;
            blobCounter.GetBlobsLeftAndRightEdges(blob, out leftPoints, out rightPoints);

            Bitmap bmp = new Bitmap(blob.Rectangle.Width, blob.Rectangle.Height, PixelFormat.Format24bppRgb);
            for (int c = 0; c < leftPoints.Count; c++)
            {
                IntPoint startLeft = leftPoints[c];
                IntPoint endRight = rightPoints[c];

                for (int x = startLeft.X; x <= endRight.X; x++)
                {
                    blobPoints.Add(new IntPoint(x - blob.Rectangle.Left, startLeft.Y - blob.Rectangle.Top));
                    bmp.SetPixel(x - blob.Rectangle.Left, startLeft.Y - blob.Rectangle.Top,
                        colorSpaces.OriginalColorSpace.GetPixel(x, startLeft.Y)); 
                }
            }

            double[,] aData = new double[blobPoints.Count, 2];
            double[,] eData = new double[blobPoints.Count, 2];
            double[] bData = new double[blobPoints.Count];

            double centroidX = 0;
            double centroidY = 0;
            double total = 0;
            for (int c = 0; c < blobPoints.Count; c++)
            {
                centroidX += blobPoints[c].X;
                centroidY += blobPoints[c].Y;

                total++;
            }

            centroidX /= total;
            centroidY /= total;

            for (int c = 0; c < blobPoints.Count; c++)
            {
                eData[c, 0] = blobPoints[c].X - centroidX;
                eData[c, 1] = blobPoints[c].Y - centroidY;

                aData[c, 0] = 1;
                aData[c, 1] = blobPoints[c].X;

                bData[c] = blobPoints[c].Y;
            }

            DenseMatrix E = DenseMatrix.OfArray(eData);
            DenseMatrix Eprime = (DenseMatrix)E.TransposeThisAndMultiply(E);
            DenseEvd eigen = new DenseEvd(Eprime);
            Vector<Complex> eigenValues = eigen.EigenValues();
            int maxIndex = 0;
            double max = 0.0;
            for (int c = 0; c < eigenValues.Count; c++)
            {
                double eigenvalue = eigenValues[c].Real;
                if (eigenvalue > max)
                {
                    max = eigenvalue;
                    maxIndex = c;
                }
            }
            Matrix<double> eigenVectors = eigen.EigenVectors();
            Vector<double> maxEigenVector = eigenVectors.Column(maxIndex);
            Matrix<double> xHat = DenseMatrix.OfColumns(2, 1, new List<Vector<double>>(new Vector<double>[] { maxEigenVector }));

            double radians = System.Math.Asin(xHat[0, 0]);
            double degrees = radians * (180 / System.Math.PI); 

            // Why doesn't this work?
            //DenseMatrix A = DenseMatrix.OfArray(aData);
            //DenseVector b = new DenseVector(bData);
            //Matrix<double> xHat = (A.TransposeThisAndMultiply(A).Inverse() * A.Transpose() * b).ToColumnMatrix();

            RotateBicubic rotate = new RotateBicubic(-degrees);
            bmp = rotate.Apply(bmp); 

            return bmp;
        }
    }
}

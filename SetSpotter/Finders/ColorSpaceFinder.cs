using Accord.MachineLearning.VectorMachines;
using AForge.Imaging;
using AForge.Imaging.Filters;
using LibSVMWrapper;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class ColorSpaceFinder
    {
        public static Bitmap FindColorCorrectedForBlob(Bitmap bmp)
        {
            Bitmap corrected = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);
            List<Color> brightnessList = new List<Color>();
            for (int y = 0; y < bmp.Height; y++)
            {
                if (y == 0 || y == bmp.Height - 1)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        brightnessList.Add(bmp.GetPixel(x, y));
                    }
                }
                else
                {
                    brightnessList.Add(bmp.GetPixel(0, y));
                    brightnessList.Add(bmp.GetPixel(bmp.Width - 1, y));
                }
            }

            Color[] whites = brightnessList.OrderByDescending(m => m.GetBrightness()).Take(10).ToArray();
            double averageR = whites.Average(m => m.R);
            double averageG = whites.Average(m => m.G);
            double averageB = whites.Average(m => m.B);

            //double mean = (averageR + averageG + averageB) / 3.0;
            double mean = 255; //
            double gScaler = mean / averageG;
            double bScaler = mean / averageB;
            double rScaler = mean / averageR;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    HSL hsl = HSL.FromRGB(new RGB(c.R, c.G, c.B));
                    if (hsl.Hue < 0) hsl.Hue = 0;
                    RGB rgb = hsl.ToRGB();
                    double correctedBlue = rgb.Blue * bScaler;
                    if (correctedBlue > 255) correctedBlue = 255;
                    byte b = (byte)Math.Floor(correctedBlue);

                    byte g = (byte)Math.Floor(rgb.Green * gScaler);
                    double correctedGreen = rgb.Green * gScaler;
                    if (correctedGreen > 255) correctedGreen = 255;

                    byte r = (byte)Math.Floor(rgb.Red * rScaler);
                    double correctedRed = rgb.Red * rScaler;
                    if (correctedRed > 255) correctedRed = 255;

                    corrected.SetPixel(x, y,
                        Color.FromArgb((byte)correctedRed, (byte)correctedGreen, (byte)correctedBlue));
                }
            }

            return corrected;
        }

        public static Bitmap FindColorCorrectedForBlob(FoundColorSpaces foundColorSpaces, Blob blob)
        {
            Bitmap bmp = foundColorSpaces.OriginalColorSpace.Clone(blob.Rectangle,
                foundColorSpaces.OriginalColorSpace.PixelFormat);

            return FindColorCorrectedForBlob(bmp); 
        }

        public static FoundColorSpaces Find(Bitmap bmp)
        {
            FoundColorSpaces ret = new FoundColorSpaces();

            ret.OriginalColorSpace = bmp;
            ret.GrayColorSpace = Grayscale.CommonAlgorithms.BT709.Apply(ret.OriginalColorSpace);

            CannyEdgeDetector edges = new CannyEdgeDetector();
            Threshold threshold = new Threshold();
            ret.Edges = threshold.Apply(edges.Apply(ret.GrayColorSpace));

            ret.BinaryColorSpace = threshold.Apply(ret.GrayColorSpace);

            //ret.CorrectedRGBColorSpace = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb); 

            return ret;
        }

        public static FoundColorSpaces Find(string imagePath)
        {
            return Find((Bitmap)System.Drawing.Image.FromFile(imagePath));
        }

        public static ColorTypeEnum FindShapeColor(Bitmap shapeBitmap,
            Predict red,
            Predict green,
            Predict purple)
        {
            List<HSL> pixels = new List<HSL>();
            for (int y = 0; y < shapeBitmap.Height; y++)
            {
                for (int x = 0; x < shapeBitmap.Width; x++)
                {
                    Color color = shapeBitmap.GetPixel(x, y);
                    pixels.Add(HSL.FromRGB(new RGB(color.R, color.G, color.B)));
                }
            }
            HSL[] subset = pixels.OrderBy(m => m.Luminance).Take(10).ToArray(); 

            int redCount = 0, greenCount = 0, purpleCount = 0; 
            foreach(HSL pixel in subset)
            {
                double[] input = new double[] { pixel.Hue, pixel.Saturation * 360.0, pixel.Luminance * 360.0};
                double isRed = red.Compute(input);
                double isPurple = purple.Compute(input);
                double isGreen = green.Compute(input);

                if (isRed > isPurple && isRed > isGreen)
                    redCount++;
                else if (isPurple > isRed && isPurple > isGreen)
                    purpleCount++;
                else if (isGreen > isRed && isGreen > isPurple)
                    greenCount++; 
            }

            if (redCount > greenCount && redCount > purpleCount)
                return ColorTypeEnum.Red;
            else if (greenCount > redCount && greenCount > purpleCount)
                return ColorTypeEnum.Green;
            else if (purpleCount > redCount && purpleCount > greenCount)
                return ColorTypeEnum.Purple;
            else
                return ColorTypeEnum.Unknown; 
        }
    }
}

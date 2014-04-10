using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetSpotter.Finders
{
    public static class ShapeTypeFinder
    {
        private static int Count = 0;

        private static int EdgeFoundAtX(Bitmap bmp, int y)
        {
            int ret = -1;
            for (int x = 0; x < bmp.Width; x++)
            {
                Color color = bmp.GetPixel(x, y);
                if (color.R > 0 || color.G > 0 || color.B > 0)
                {
                    ret = x;
                    break;
                }
            }
            return ret;
        }

        public static FoundBlobType Find(Bitmap bmp, FoundColorSpaces foundColorSpaces)
        {
            FoundBlobType foundBlobType = new FoundBlobType();

            ResizeBicubic resize = new ResizeBicubic(30, 50);
            Bitmap resizedBitmap = resize.Apply(bmp);

            double blackPixelCount = 0;
            for (int y = 0; y < resizedBitmap.Height; y++)
            {
                for (int x = 0; x < resizedBitmap.Width; x++)
                {
                    Color color = resizedBitmap.GetPixel(x, y);
                    if (color.R == 0 && color.G == 0 && color.B == 0)
                    {
                        blackPixelCount++;
                    }
                }
            }

            double total = (resizedBitmap.Width * resizedBitmap.Height * 1.0);

            double percentageBlack = blackPixelCount / total;

            int p0 = EdgeFoundAtX(resizedBitmap, 6);
            int p1 = EdgeFoundAtX(resizedBitmap, 16);
            int p2 = EdgeFoundAtX(resizedBitmap, 36);
            int p3 = EdgeFoundAtX(resizedBitmap, 43);

            if (percentageBlack < .6)
            {
                if (Math.Abs(p0 - p1) < 4 && Math.Abs(p2 - p0) < 4 && p0 > 0)
                {
                    foundBlobType.ShapeType = ShapeTypeEnum.Pill;
                }
                else if (p0 < p1 && p2 <= p0)
                {
                    foundBlobType.ShapeType = ShapeTypeEnum.Squiggle;
                }
                else if (p0 > p1 && p3 > p1)
                {
                    foundBlobType.ShapeType = ShapeTypeEnum.Diamond;
                }
            }
            else
            {
                foundBlobType.ShapeType = ShapeTypeEnum.NotAType;
            }

            float darkestPixelBrightnessValue = float.MaxValue;
            float lightestPixelBrightnessValue = 0;
            float lightestHue = float.MaxValue;
            Bitmap corrected = foundColorSpaces.CorrectedRGBColorSpace;
            for (int y = 0; y < corrected.Height; y++)
            {
                for (int x = 0; x < corrected.Width; x++)
                {
                    Color color = corrected.GetPixel(x, y);
                    float brightness = color.GetBrightness();
                    float hue = color.GetHue();
                    if (brightness < darkestPixelBrightnessValue)
                    {
                        darkestPixelBrightnessValue = brightness;
                    }
                    if (brightness > lightestPixelBrightnessValue)
                    {
                        lightestPixelBrightnessValue = brightness;
                    }

                    if (hue < lightestHue)
                    {
                        lightestHue = hue;
                    }
                }
            }

            float delta = (lightestPixelBrightnessValue - darkestPixelBrightnessValue);
            float darknessRange = lightestPixelBrightnessValue - (delta * .4f);

            foundBlobType.StrippedBitmap = new Bitmap(corrected.Width, corrected.Height, PixelFormat.Format24bppRgb);
            int redPixel = 0, greenPixel = 0, purplePixel = 0;
            foundBlobType.Histogram = new int[360];
            for (int y = 0; y < corrected.Height; y++)
            {
                for (int x = 0; x < corrected.Width; x++)
                {
                    Color color = corrected.GetPixel(x, y);
                    if (color.GetBrightness() < darknessRange)
                    {
                        foundBlobType.StrippedBitmap.SetPixel(x, y, color);
                        float hue = color.GetHue();
                        foundBlobType.Histogram[(int)hue]++;
                        if (hue < 17 && hue > 329)
                        {
                            redPixel++;
                        }
                        else if (hue > 105 && hue < 185)
                        {
                            greenPixel++;
                        }
                        else if (hue > 200 && hue < 329)
                        {
                            purplePixel++;
                        }
                    }
                }
            }

            if (redPixel > greenPixel && redPixel > purplePixel)
            {
                foundBlobType.ColorType = ColorTypeEnum.Red;
            }
            else if (greenPixel > redPixel && greenPixel > purplePixel)
            {
                foundBlobType.ColorType = ColorTypeEnum.Green;
            }
            else if (purplePixel > redPixel && purplePixel > greenPixel)
            {
                foundBlobType.ColorType = ColorTypeEnum.Purple;
            }

            //bmp2.Save(@"c:\users\brush\desktop\brokenout\" + (Count++).ToString() + ".bmp"); 

            return foundBlobType;
        }
    }
}

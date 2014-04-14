using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using AForge.Imaging;
using AForge.Imaging.Filters;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSLColorLearner
{
    class Program
    {
        static void PrintAccuracy(string colorName, KernelSupportVectorMachine svm, HSL[] positives, HSL[] negatives)
        {
            int numberCorrect = 0;
            for (int c = 0; c < positives.Length; c++)
            {
                double result = svm.Compute(HSLToDouble(positives[c]));
                if (Math.Sign(result) == 1)
                {
                    numberCorrect++;
                }
            }
            for (int c = 0; c < negatives.Length; c++)
            {
                double result = svm.Compute(HSLToDouble(negatives[c]));
                if (Math.Sign(result) == -1)
                {
                    numberCorrect++;
                }
            }

            Console.WriteLine(colorName + " accuracy is " +
                (numberCorrect / (positives.Length + negatives.Length * 1.0)).ToString());
        }

        static double[] HSLToDouble(HSL hsl)
        {
            return new double[] { hsl.Hue, hsl.Saturation, hsl.Luminance };
        }

        static KernelSupportVectorMachine LearnSVM(HSL[] positives, HSL[] negatives,
            double throwExceptionWhenErrorGreaterThan)
        {
            int[] classes = new int[positives.Length + negatives.Length];
            double[][] vectors = new double[classes.Length][];

            int index = 0;
            for (int c = 0; c < positives.Length; c++, index++)
            {
                classes[index] = 1;
                vectors[index] = HSLToDouble(positives[c]);
            }
            for (int c = 0; c < negatives.Length; c++, index++)
            {
                classes[index] = -1;
                vectors[index] = HSLToDouble(negatives[c]);
            }

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Gaussian(.1), vectors[0].Length);
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, vectors.ToArray(), classes);
            //smo.Complexity = 1.0;
            double error = smo.Run();
            if (error > throwExceptionWhenErrorGreaterThan)
            {
                throw new Exception("Failed to get reasonable error value.");
            }

            return svm;
        }

        static void WriteToLibSVMFile(List<HSL> positives, List<HSL> negatives, string outputFile)
        {
            using (FileStream fs = File.OpenWrite(outputFile))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int c = 0; c < positives.Count; c++)
                    {
                        sw.WriteLine("+1 1:{0} 2:{1} 3:{2}", 
                            positives[c].Hue, 
                            positives[c].Saturation * 360.0,
                            positives[c].Luminance * 360.0); 
                    }
                    for (int c = 0; c < negatives.Count; c++)
                    {
                        sw.WriteLine("-1 1:{0} 2:{1} 3:{2}",
                            negatives[c].Hue,
                            negatives[c].Saturation * 360.0,
                            negatives[c].Luminance * 360.0);
                    }
                    sw.Flush(); 
                }
            }
        }

        static void Main(string[] args)
        {
            IPixelPicker iPixelPicker = new LeastLuminousPixelPicker();

            string[] labeledShapes = Directory.GetFiles("LabeledShapes");
            Console.WriteLine("Loaded " + labeledShapes.Length.ToString() + " labeled shapes.");

            List<HSL> redHSL = new List<HSL>();
            List<HSL> greenHSL = new List<HSL>();
            List<HSL> purpleHSL = new List<HSL>();

            Console.WriteLine("Selecting pixels from shapes...");
            foreach (string labeledShape in labeledShapes)
            {
                string fileName = Path.GetFileNameWithoutExtension(labeledShape);
                string[] information = fileName.Split('-').Take(3).ToArray();
                if (information[0] != "error")
                {
                    string color = information[0].ToLower();
                    string shape = information[1].ToLower();
                    string fill = information[2].ToLower();

                    using (Bitmap bmp = (Bitmap)Bitmap.FromFile(labeledShape))
                    {
                        Random rand = new Random();
                        //for (int c = 0; c < 6; c++)
                        {
                            //BrightnessCorrection brightness = new BrightnessCorrection(rand.Next(-5, 5));
                            //float adjust = 0f; 
                            //if (rand.Next() % 2 == 0)
                            //    adjust = (float)rand.NextDouble() / 8.0f; 
                            //else
                            //    adjust = -(float)rand.NextDouble() / 8.0f;
                            //SaturationCorrection saturation = new SaturationCorrection(adjust); 

                            //brightness.ApplyInPlace(bmp);
                            //saturation.ApplyInPlace(bmp); 

                            FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(bmp);
                            Bitmap corrected = ColorSpaceFinder.FindColorCorrectedForBlob(colorSpaces.OriginalColorSpace);
                            //corrected.Save(@"C:\users\brush\desktop\corrected\" + Guid.NewGuid().ToString() + ".bmp"); 

                            HSL[] pixels = iPixelPicker.Get(corrected, 100);

                            switch (color)
                            {
                                case "red":
                                    redHSL.AddRange(pixels);
                                    break;
                                case "green":
                                    greenHSL.AddRange(pixels);
                                    break;
                                case "purple":
                                    purpleHSL.AddRange(pixels);
                                    break;
                            }
                        }
                    }
                }
            }

            List<PLYFileVertex> verticies = new List<PLYFileVertex>();
            foreach (HSL pixel in purpleHSL)
            {
                verticies.Add(new PLYFileVertex
                    {
                        X = pixel.Hue,
                        Y = pixel.Luminance * 360,
                        Z = pixel.Saturation * 360,
                        PixelColor = System.Drawing.Color.Purple
                    });
            }
            foreach (HSL pixel in redHSL)
            {
                verticies.Add(new PLYFileVertex
                {
                    X = pixel.Hue,
                    Y = pixel.Luminance * 360,
                    Z = pixel.Saturation * 360,
                    PixelColor = System.Drawing.Color.Red
                });
            }
            foreach (HSL pixel in greenHSL)
            {
                verticies.Add(new PLYFileVertex
                {
                    X = pixel.Hue,
                    Y = pixel.Luminance * 360,
                    Z = pixel.Saturation * 360,
                    PixelColor = System.Drawing.Color.Green
                });
            }
            PLYFile ply = new PLYFile
            {
                Vertices = verticies,
                Faces = new List<PLYFileFace>(), //necessary to save 
                Header = new PLYHeader(), //not necessary to save
            };
            ply.Save(@"C:\Users\brush\Desktop\cloud.ply");

            List<HSL> positives = new List<HSL>();
            List<HSL> negatives = new List<HSL>();

            Console.WriteLine("Learning red...");
            positives.AddRange(purpleHSL);
            negatives.AddRange(greenHSL);
            negatives.AddRange(redHSL);

            WriteToLibSVMFile(positives, negatives, @"c:\users\brush\desktop\purple.txt"); 

            //KernelSupportVectorMachine redSVM = LearnSVM(positives.ToArray(), negatives.ToArray(), .1);
            //PrintAccuracy("red", redSVM, positives.ToArray(), negatives.ToArray()); 
            //redSVM.Save(@"C:\users\brush\desktop\red.svm"); 

            //negatives.Clear();
            //positives.Clear();

            //Console.WriteLine("Learning purple..."); 
            //positives.AddRange(purpleHSL);
            //negatives.AddRange(redHSL);
            //negatives.AddRange(greenHSL);
            //KernelSupportVectorMachine purpleSVM = LearnSVM(positives.ToArray(), negatives.ToArray(), .1);
            //PrintAccuracy("purple", purpleSVM, positives.ToArray(), negatives.ToArray()); 
            //purpleSVM.Save(@"C:\users\brush\desktop\purple.svm"); 

            //negatives.Clear();
            //positives.Clear();

            //Console.WriteLine("Learning green..."); 
            //positives.AddRange(greenHSL);
            //negatives.AddRange(redHSL);
            //negatives.AddRange(purpleHSL);
            //KernelSupportVectorMachine greenSVM = LearnSVM(positives.ToArray(), negatives.ToArray(), .1);
            //PrintAccuracy("green", greenSVM, positives.ToArray(), negatives.ToArray()); 
            //greenSVM.Save(@"C:\users\brush\desktop\green.svm");


            Console.WriteLine("Done!");
            Console.ReadLine();
            return;
        }
    }
}

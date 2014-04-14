using Accord.MachineLearning.VectorMachines;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using SetSpotter.Finders;
using SetSpotter.FoundData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCaptureDevice _device = null; 
        private WriteableBitmap _bmp = null;

        private KernelSupportVectorMachine _red, _purple, _green; 

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _green = KernelSupportVectorMachine.Load("resources/green.svm");
            _purple = KernelSupportVectorMachine.Load("resources/purple.svm");
            _red = KernelSupportVectorMachine.Load("resources/red.svm"); 

            FilterInfoCollection filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FilterInfo desired = null;
            foreach (FilterInfo info in filter)
            {
                if (info.Name == "QuickCam for Notebooks Deluxe")
                {
                    desired = info;
                    break;
                }
            }
            _device = new VideoCaptureDevice(desired.MonikerString);
            _device.NewFrame += _device_NewFrame;
            _device.Start();  

            return; 
        }

        void _device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = eventArgs.Frame;

            FoundColorSpaces colorSpaces = ColorSpaceFinder.Find(bmp);
            FoundBlobs foundBlobs = BlobFinder.Find(colorSpaces, 80, 25, 90, 50, 1.2, 2.2);
            
            foreach(Blob blob in foundBlobs.Blobs)
            {
                Bitmap correctedBlobBitmap = ColorSpaceFinder.FindColorCorrectedForBlob(colorSpaces, blob);
                ColorTypeEnum color = ColorSpaceFinder.FindShapeColor(correctedBlobBitmap,
                    _red, _green, _purple); 

                System.Drawing.Pen pen = null; 
                switch(color)
                {
                    case ColorTypeEnum.Green:
                        pen = new System.Drawing.Pen(System.Drawing.Brushes.Green, 5); 
                        break; 
                    case ColorTypeEnum.Red:
                        pen = new System.Drawing.Pen(System.Drawing.Brushes.Red, 5); 
                        break; 
                    case ColorTypeEnum.Purple:
                        pen = new System.Drawing.Pen(System.Drawing.Brushes.Purple, 5); 
                        break; 
                    case ColorTypeEnum.Unknown:
                        pen = new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 5); 
                        break; 
                }

                using(Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawRectangle(pen, blob.Rectangle); 
                }
            }

            BitmapData bmd = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            byte[] bytes = new byte[bmd.Stride * bmd.Height];
            Marshal.Copy(bmd.Scan0, bytes, 0, bytes.Length);

            Dispatcher.Invoke((Action)delegate()
            {
                if (_bmp == null)
                {
                    _bmp = new WriteableBitmap(bmp.Width, bmp.Height, 96, 96, PixelFormats.Bgr24, null);
                    VideoWindow.Source = _bmp;
                }

                _bmp.WritePixels(new Int32Rect(0, 0, bmp.Width, bmp.Height), bytes, bmd.Stride, 0); 
            }); 
            
            return; 
        }
    }
}

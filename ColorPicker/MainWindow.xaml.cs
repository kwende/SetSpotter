using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _fileIndex = 0; 
        string[] _files; 
        Bitmap _bmp;
        System.Drawing.Color _color; 
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.FullImage.MouseUp += FullImage_MouseUp;
        }

        void FullImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            BitmapImage bmp = (BitmapImage)FullImage.Source;

            double scalerWidth = bmp.PixelWidth / (FullImage.Width * 1.0);
            double scalerHeight = bmp.PixelHeight / (FullImage.Height * 1.0);

            System.Windows.Point clickdPoint = e.GetPosition((IInputElement)sender);
            _color = _bmp.GetPixel((int)Math.Round(clickdPoint.X * scalerWidth), (int)Math.Round(clickdPoint.Y * scalerHeight));

            Rect.Fill = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(_color.A, _color.R, _color.G, _color.B)); 

            return; 
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _files = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\testimages");
            string currentFile = _files[_fileIndex];

            _bmp = (Bitmap)Bitmap.FromFile(currentFile);
            ImageSource src = new BitmapImage(new Uri(currentFile));
            FullImage.Source = src;
        }

        private void Save(string colorName, System.Drawing.Color color)
        {
            float hue = color.GetHue();
            float brightness = color.GetBrightness();
            File.AppendAllText(@"c:\users\brush\desktop\" + colorName + ".csv",
                string.Format("{0},{1},{2},{3},{4}\r\n", color.R, color.G, color.B, hue, brightness)); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Save("red", _color); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Save("green", _color); 
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Save("purple", _color); 
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _fileIndex++;
            if (_files.Length > _fileIndex)
            {
                string currentFile = _files[_fileIndex];

                _bmp = (Bitmap)Bitmap.FromFile(currentFile);
                ImageSource src = new BitmapImage(new Uri(currentFile));
                FullImage.Source = src;
            }
        }
    }
}

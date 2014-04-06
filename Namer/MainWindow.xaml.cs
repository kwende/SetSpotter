using System;
using System.Collections.Generic;
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

namespace Namer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _index = 0;
        private string[] _files;
        private string _currentFile; 

        private void ShowFile(string fileName)
        {
            ImageSource image = new BitmapImage(new Uri(fileName));
            CheckImage.Source = image; 
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _files = Directory.GetFiles(@"C:\Users\brush\Documents\Visual Studio 2012\Projects\SetSpotter\SetSpotter\separatedshapes\pill");
            _currentFile = _files[_index];
            ShowFile(_currentFile); 
        }

        private void Green_Click(object sender, RoutedEventArgs e)
        {
            ChangeNameAndAdvance("green"); 
        }

        private void Red_Click(object sender, RoutedEventArgs e)
        {
            ChangeNameAndAdvance("red"); 
        }

        private void Purple_Click(object sender, RoutedEventArgs e)
        {
            ChangeNameAndAdvance("purple"); 
        }

        private void ChangeNameAndAdvance(string color)
        {
            string currentFileDirectory = System.IO.Path.GetDirectoryName(_currentFile);
            string newFileName = string.Format("{0}_{1}_{2}.bmp", color, "", Guid.NewGuid().ToString().Replace("-",""));
            File.Copy(_currentFile, @"C:\users\brush\desktop\pill\" + newFileName);
            _currentFile = _files[++_index];
            ShowFile(_currentFile); 
        }
    }
}

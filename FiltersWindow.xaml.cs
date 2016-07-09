using PhotoBooth.Common;
using PhotoBooth.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    /// <summary>
    /// Interaction logic for FiltersWindow.xaml
    /// </summary>
    public partial class FiltersWindow
    {
        private Bitmap _bitmap;
        public string _photoName { get; set; }
        private CloudService _cloudService;
        private IProcessing _proccessing;

        public FiltersWindow(string photoName, Bitmap bitmap)
        {
            InitializeComponent();
           

            _bitmap = bitmap;
            _photoName = photoName;
            _cloudService = new CloudService();
            _proccessing = new Processing(bitmap);

            loadPictures();
        }

        /// <summary>
        /// Load filtered image presets
        /// </summary>
        /// 

        private async void loadPictures()
        {
            await Task.Run(() => loadPhoto());
        }

        private void loadPhoto()
        {
            Dispatcher.Invoke((() =>
            {
                photo.Source = BitmapUtils.loadBitmap(_bitmap);
            }));
        }

        public void FilterOriginal(object sender, RoutedEventArgs e)
        {
            _proccessing = new Processing(_bitmap);
            photo.Source = BitmapUtils.loadBitmap(_bitmap);
        }

        public void FilterNegative(object sender, RoutedEventArgs e)
        {
            _proccessing = new Processing(_bitmap);
            photo.Source = BitmapUtils.loadBitmap(_proccessing.negativ());
        }

        public void FilterGray(object sender, RoutedEventArgs e)
        {
            _proccessing = new Processing(_bitmap);
            photo.Source = BitmapUtils.loadBitmap(_proccessing.grayScale());
        }

        public void FilterSepia(object sender, RoutedEventArgs e)
        {
            _proccessing = new Processing(_bitmap);
            photo.Source = BitmapUtils.loadBitmap(_proccessing.sepia());
        }
        public void FilterCartoon(object sender, RoutedEventArgs e)
        {
            _proccessing = new Processing(_bitmap);
            photo.Source = BitmapUtils.loadBitmap(_proccessing.cartoon());
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            saveLocal();
            saveCloud();
        }

        public void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Save photo to local storage
        /// </summary>
        private void saveLocal()
        {
            Bitmap temp = new Bitmap(_bitmap.Width, _bitmap.Height, _bitmap.PixelFormat);
            temp.Save(_photoName);
            temp.Dispose();
        }

        /// <summary>
        /// Save photo to cloud storage
        /// </summary>
        private void saveCloud()
        {
            _cloudService.Upload(_photoName);
        }

        private void LoadImageList(object sender, RoutedEventArgs e)
        {
            string root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] supportedExtensions = new[] { ".bmp", ".jpeg", ".jpg", ".png", ".tiff" };
            var files = Directory.GetFiles(Path.Combine(root), "*.*").Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));

            List<ImageModel> ListImage = new List<ImageModel>();
            foreach (var file in files)
            {

                ImageModel id = new ImageModel()
                {
                    Path = new BitmapImage(new Uri(file.ToString())),
                    FileName = Path.GetFileName(file),
                };
                ListImage.Add(id);
            }
            this.Thumbnails.ItemsSource = ListImage;


        }

        

        private void selectedChange(object sender, SelectionChangedEventArgs e)
        {
            ImageModel image = (ImageModel)((ListBox)sender).SelectedValue;
            System.IO.FileStream fileStream = new System.IO.FileStream(image.FileName.ToString(), System.IO.FileMode.Open, System.IO.FileAccess.Read);
            Bitmap img = new Bitmap(fileStream);
            fileStream.Close();
            _bitmap = img;
           
            _photoName = image.FileName.ToString();

            loadPictures();

        }


    }
}

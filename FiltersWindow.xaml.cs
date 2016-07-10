using PhotoBooth;
using PhotoBooth.Common;
using PhotoBooth.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        private string tagFileName = "tags.csv";
        List<Label> labelList = new List<Label>();
        List<string> labelinfos = new List<string>();

        public FiltersWindow(string photoName, Bitmap bitmap)
        {
            InitializeComponent();
           

            _bitmap = bitmap;
            _photoName = photoName;
            _cloudService = new CloudService();
            _proccessing = new Processing(bitmap);
            tag_display.IsChecked = true;
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

        private async void loadTags()
        {
            await Task.Run(() => loadTag());
        }
        private void loadTag()
        {
            Dispatcher.Invoke((() =>
            {
                checkExistedTag();
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

            _bitmap.Save(_photoName);

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
            loadTags();

        }
        private void add_tags(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label txt = new Label();
            txt.Name = "txt";
            //-45 taille souris , -230 je sais pas ...

            txt.Margin = new Thickness(e.GetPosition(this).X-230, e.GetPosition(this).Y - 45, 0, 0);
            var dialog = new Dialog();
            string tagName = "";
            if (dialog.ShowDialog() == true)
            {
                tagName = dialog.TagName;
                txt.Content = tagName;
                string lblInfo = $"{ txt.Margin.Left},{ txt.Margin.Top},{tagName}";
                labelinfos.Add(lblInfo);
                labelList.Add(txt);
                stackPanel.Children.Add(txt);
                RightCSV();
            }
            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Label lbl in labelList)
            {
                lbl.Visibility = Visibility.Visible;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Label lbl in labelList)
            {
                lbl.Visibility = Visibility.Hidden;
            }
        }

        private void RightCSV()
        {

            bool added = false;
            List<string> lines = new List<string>();
            
            if (File.Exists(tagFileName))
            {
                lines = File.ReadAllLines(tagFileName).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (line.Contains(","))
                    {
                        var split = line.Split(',');
                        if (split[0].Equals(_photoName))
                        {
                            string labels = "";
                            foreach (string lbl in labelinfos)
                            {
                                if (labels.Equals(""))
                                {
                                    labels = lbl;
                                }
                                else
                                {
                                    labels += "," + lbl;
                                }
                            }
                            lines[i] = $"{split[0]},{labels}";
                            added = true;
                        }
                    }
                }
            }
            if (!added)
            {
                string labels = "";
                foreach (string lbl in labelinfos)
                {
                    if (labelinfos.Count == 1)
                    {
                        labels = lbl;
                    }
                    else
                    {
                        labels += "," + lbl;
                    }
                }
                lines.Add($"{_photoName},{labels}");
            }

            //after your loop
            File.WriteAllLines(tagFileName, lines);
        }

        private void checkExistedTag()
        {
            for(int k = 0; k< stackPanel.Children.Count; k++)
            {
                if (k > 1)
                {
                    stackPanel.Children.RemoveAt(k);
                }
            }
            List<string> lines = new List<string>();
            if (File.Exists(tagFileName))
            {
                lines = File.ReadAllLines(tagFileName).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (line.Contains(","))
                    {
                        var split = line.Split(',');
                        if (split[0].Equals(_photoName))
                        {
                            string labels = "";
                            for(int j= 1; j< split.Length; j+=3)
                            {
                                addExistedTag(int.Parse(split[j]), int.Parse(split[j + 1]), split[j + 2]);
                            }
                           
                        }
                    }
                }
            }

        }
       
        private void addExistedTag(int X, int Y , string name)
        {

            Label txt = new Label();
            txt.Name = "txt";
            //-45 taille souris , -230 je sais pas ...

            txt.Margin = new Thickness(X, Y, 0, 0);
            txt.Content = name;
            string lblInfo = $"{X},{Y},{name}";
            labelinfos.Add(lblInfo);
            labelList.Add(txt);
            stackPanel.Children.Add(txt);
            MessageBox.Show("added" + X + " // " + Y);
        }
    }
}

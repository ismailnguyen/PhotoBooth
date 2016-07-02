using System;
using System.Windows;
using System.Windows.Controls;
using WebEye.Controls.Wpf;
using PhotoBooth.Services;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PhotoBooth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private CloudService _cloudService;

        public MainWindow()
        {
            InitializeComponent();

            InitializeComboBox();

            //startCamera();

            _cloudService = new CloudService();
        }

        private void InitializeComboBox()
        {
            comboBox.ItemsSource = webCameraControl.GetVideoCaptureDevices();

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedItem = comboBox.Items[0];
            }            
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            startButton.IsEnabled = e.AddedItems.Count > 0;
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var cameraId = (WebCameraId)comboBox.SelectedItem;
            webCameraControl.StartCapture(cameraId);
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            webCameraControl.StopCapture();
        }

        private void OnImageButtonClick(object sender, RoutedEventArgs e)
        {
            /*var dialog = new SaveFileDialog { Filter = "Bitmap Image|*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                webCameraControl.GetCurrentImage().Save(dialog.FileName);
            }*/

            string photoName = string.Format(
                        "{0}-{1}.jpg",
                        "Mariage",
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
                    );

            
            IProcessing process = new Processing(webCameraControl.GetCurrentImage());
            Bitmap result = process.negativ();

            result.Save(photoName);

            _cloudService.Upload(photoName);
        }

        private void startCamera()
        {
            List<WebCameraId> cameraIds = webCameraControl.GetVideoCaptureDevices().ToList();
            var first = cameraIds.FirstOrDefault();
            var second = cameraIds.First();
            webCameraControl.StartCapture(cameraIds.FirstOrDefault());
        }
    }
}

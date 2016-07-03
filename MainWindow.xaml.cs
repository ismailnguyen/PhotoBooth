using System;
using System.Windows;
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

            _cloudService = new CloudService();

            Loaded += new RoutedEventHandler(MainWindowLoaded);   
        }

        /// <summary>
        /// Auto start camera at application launch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                startCamera();
            }
        }

        private void startCamera()
        {
            List<WebCameraId> cameras = new List<WebCameraId>(webCameraControl.GetVideoCaptureDevices());
            webCameraControl.StartCapture(cameras.FirstOrDefault());

            webCameraControl.Visibility = Visibility.Visible;
            takePhotoButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Show "About" panel on right side
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void OnTakePhotoClick(object sender, RoutedEventArgs e)
        {
            if (webCameraControl.IsCapturing)
            {
                // Generate photo name with current date
                string photoName = string.Format(
                            "{0}-{1}.jpg",
                            "PhotoBooth",
                            DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
                        );

                // Apply filter on photo
                IProcessing process = new Processing(webCameraControl.GetCurrentImage());
                Bitmap result = process.negativ();

                // Save photo to local storage
                //result.Save(photoName);

                FiltersWindow filtersWindow = new FiltersWindow();
                filtersWindow.Show();

                // Save photo to cloud storage
                //_cloudService.Upload(photoName);
            }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using WebEye.Controls.Wpf;
using PhotoBooth.Services;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;

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

        /// <summary>
        /// On button start click, hide button, start camera and show it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            //MessageDialogResult result = this.ShowMessageAsync("Event name", "Some message").ConfigureAwait(false);

            startCamera();
        }

        private void startCamera()
        {
            var cameraId = (WebCameraId)comboBox.SelectedItem;
            webCameraControl.StartCapture(cameraId);

            startButton.Visibility = Visibility.Hidden;
            webCameraControl.Visibility = Visibility.Visible;
            takePhotoButton.Visibility = Visibility.Visible;
        }

        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void OnTakePhotoClick(object sender, RoutedEventArgs e)
        {
            if (webCameraControl.IsCapturing)
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
        }
    }
}

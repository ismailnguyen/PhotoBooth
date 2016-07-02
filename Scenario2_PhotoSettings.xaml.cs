using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PhotoBooth
{
    /// <summary>
    /// Scenario 2 Change camera preview and photo settings
    /// </summary>
    public sealed partial class Scenario2_PhotoSettings : Page
    {
        // Private MainPage object for status updates
        private MainPage rootPage = MainPage.Current;

        // Object to manage access to camera devices
        private MediaCapturePreviewer _previewer = null;

        // Folder in which the captures will be stored
        private StorageFolder _captureFolder = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario2_PhotoSettings"/> class.
        /// </summary>
        public Scenario2_PhotoSettings()
        {
            this.InitializeComponent();
            _previewer = new MediaCapturePreviewer(PreviewControl, Dispatcher);

            startCamera();
        }

        /// <summary>
        /// Initializes the camera and populates the UI
        /// </summary>
        private async void startCamera()
        {
            // Clear any previous message.
            rootPage.NotifyUser("", NotifyType.StatusMessage);

            await _previewer.InitializeCameraAsync();

            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

            // Fall back to the local app storage if the Pictures Library is not available
            _captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await _previewer.CleanupCameraAsync();
        }

        /// <summary>
        /// Takes a photo and saves to a StorageFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_previewer.IsPreviewing)
            {
                // Disable the photo button while taking a photo
                PhotoButton.IsEnabled = false;

                var stream = new InMemoryRandomAccessStream();

                try
                {
                    // Generation of photo name with current date and time
                    string photoName = string.Format(
                        "{0}-{1}.jpg",
                        MainPage.PHOTO_NAME,
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
                    );

                    // Take and save the photo
                    var file = await _captureFolder.CreateFileAsync(photoName, CreationCollisionOption.GenerateUniqueName);

                    await _previewer.MediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), file);

                    rootPage.NotifyUser("Photo taken, saved to: " + file.Path, NotifyType.StatusMessage);
                }
                catch (Exception ex)
                {
                    // File I/O errors are reported as exceptions.
                    rootPage.NotifyUser("Exception when taking a photo: " + ex.Message, NotifyType.ErrorMessage);
                }

                // Done taking a photo, so re-enable the button
                PhotoButton.IsEnabled = true;
            }
        }
    }
}

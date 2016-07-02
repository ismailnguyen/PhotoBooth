using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.Media.Capture;
using Windows.UI.Core;

namespace PhotoBooth
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Photo Booth";
        public const string PHOTO_NAME = "Mariage";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Camera", ClassType=typeof(Scenario2_PhotoSettings)},
            new Scenario() { Title="Album", ClassType=typeof(Scenario3_AspectRatio)},
            new Scenario() { Title="Help", ClassType=typeof(Scenario1_PreviewSettings)}     
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

    public class MediaCapturePreviewer
    {
        CoreDispatcher _dispatcher;
        CaptureElement _previewControl;

        public MediaCapturePreviewer(CaptureElement previewControl, CoreDispatcher dispatcher)
        {
            _previewControl = previewControl;
            _dispatcher = dispatcher;
        }

        public bool IsPreviewing { get; private set; }
        public bool IsRecording { get; set; }
        public MediaCapture MediaCapture { get; private set; }


        /// <summary>
        /// Initializes the MediaCapture, starts preview.
        /// </summary>
        public async Task InitializeCameraAsync()
        {
            MediaCapture = new MediaCapture();
            MediaCapture.Failed += MediaCapture_Failed;

            try
            {
                await MediaCapture.InitializeAsync();
                _previewControl.Source = MediaCapture;
                await MediaCapture.StartPreviewAsync();
                IsPreviewing = true;
            }
            catch (UnauthorizedAccessException)
            {
                // This can happen if access to the camera has been revoked.
                MainPage.Current.NotifyUser("The app was denied access to the camera", NotifyType.ErrorMessage);
                await CleanupCameraAsync();
            }
        }

        public async Task CleanupCameraAsync()
        {
            if (IsRecording)
            {
                await MediaCapture.StopRecordAsync();
                IsRecording = false;
            }

            if (IsPreviewing)
            {
                await MediaCapture.StopPreviewAsync();
                IsPreviewing = false;
            }

            _previewControl.Source = null;

            if (MediaCapture != null)
            {
                MediaCapture.Dispose();
                MediaCapture = null;
            }
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs e)
        {
            var task = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                MainPage.Current.NotifyUser("Preview stopped: " + e.Message, NotifyType.ErrorMessage);
                IsRecording = false;
                IsPreviewing = false;
                await CleanupCameraAsync();
            });
        }
    }
}

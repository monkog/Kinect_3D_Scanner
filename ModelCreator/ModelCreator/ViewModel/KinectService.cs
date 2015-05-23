using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace ModelCreator.ViewModel
{
    public class KinectService : ViewModelBase, IKinectService
    {
        #region Private Fields
        /// <summary>
        /// Current KinectSensor
        /// </summary>
        private KinectSensor _kinectSensor;
        /// <summary>
        /// WritableBitmap that source from Kinect camera is written to
        /// </summary>
        private WriteableBitmap _kinectCameraImage;
        /// <summary>
        /// Bounds of camera source
        /// </summary>
        private Int32Rect _cameraSourceBounds;
        /// <summary>
        /// Number of bytes per line
        /// </summary>
        private int _colorStride;
        /// <summary>
        /// Visibility of ErrorGrid 
        /// </summary>
        private Visibility _errorGridVisibility;
        /// <summary>
        /// The image width
        /// </summary>
        private double _imageWidth;
        /// <summary>
        /// The image height
        /// </summary>
        private double _imageHeight;
        /// <summary>
        /// The error grid message
        /// </summary>`
        private string _errorGridMessage;
        private DepthImagePixel[] _depthImage;
        #endregion Private Fields
        #region Public Properties
        /// <summary>
        /// Current KinectSensor
        /// </summary>
        public KinectSensor Kinect
        {
            get { return _kinectSensor; }
            set
            {
                if (_kinectSensor != null)
                {
                    UninitializeKinectSensor(_kinectSensor);
                    _kinectSensor = null;
                }
                if (value != null && value.Status == KinectStatus.Connected)
                {
                    _kinectSensor = value;
                    InitializeKinectSensor(_kinectSensor);
                }
            }
        }
        /// <summary>
        /// Gets or sets the Kinect camera image.
        /// </summary>
        /// <value>
        /// The Kinect camera image.
        /// </value>
        public WriteableBitmap KinectCameraImage
        {
            get { return _kinectCameraImage; }
            set
            {
                if (Equals(_kinectCameraImage, value))
                    return;
                _kinectCameraImage = value;
                OnPropertyChanged("KinectCameraImage");
            }
        }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get { return _imageWidth; }
            set
            {
                if (_imageWidth == value)
                    return;
                _imageWidth = value;
                OnPropertyChanged("Width");
            }
        }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get { return _imageHeight; }
            set
            {
                if (_imageHeight == value)
                    return;
                _imageHeight = value;
                OnPropertyChanged("Height");
            }
        }
        /// <summary>
        /// Gets or sets visibility of ErrorGrid
        /// </summary>
        /// <value>
        /// The visibility of ErrorGrid
        /// </value>
        public Visibility ErrorGridVisibility
        {
            get { return _errorGridVisibility; }
            set
            {
                if (_errorGridVisibility == value)
                    return;
                _errorGridVisibility = value;
                OnPropertyChanged("ErrorGridVisibility");
            }
        }
        /// <summary>
        /// Gets or sets the error grid message.
        /// </summary>
        /// <value>
        /// The error grid message.
        /// </value>
        public string ErrorGridMessage
        {
            get { return _errorGridMessage; }
            set
            {
                if (_errorGridMessage == value)
                    return;
                _errorGridMessage = value;
                OnPropertyChanged("ErrorGridMessage");
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Enables ColorStream from newly detected KinectSensor and sets output image
        /// </summary>
        /// <param name="sensor">Detected KinectSensor</param>
        private void InitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                ColorImageStream colorStream = sensor.ColorStream;
                colorStream.Enable();

                KinectCameraImage = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight
                    , 96, 96, PixelFormats.Bgr32, null);

                _cameraSourceBounds = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                _colorStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
                sensor.ColorFrameReady += KinectSensor_ColorFrameReady;

                sensor.DepthStream.Enable();
                sensor.DepthFrameReady += KinectSensor_DepthFrameReady;
                _depthImage = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
                try
                {
                    sensor.Start();
                }
                catch (Exception)
                {
                    UninitializeKinectSensor(sensor);
                    Kinect = null;
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Kinect jest używany przez inny proces." + Environment.NewLine +
                        "Spróbuj odłączyć i ponownie podłączyć urządzenie do komputera." + Environment.NewLine +
                        "Upewnij się, że wszystkie programy używajace Kinecta zostały wyłączone.";
                }
            }
        }
        /// <summary>
        /// Disables ColorStream from disconnected KinectSensor
        /// </summary>
        /// <param name="sensor">Disconnected KinectSensor</param>
        private void UninitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor == null) return;
            sensor.Stop();
            sensor.ColorFrameReady -= KinectSensor_ColorFrameReady;
            sensor.DepthFrameReady -= KinectSensor_DepthFrameReady;
            sensor.DepthStream.Disable();
        }
        private void KinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var frame = e.OpenDepthImageFrame())
            {
                if (frame == null || frame.PixelDataLength == 0)
                    return;
                frame.CopyDepthImagePixelDataTo(_depthImage);
            }
        }
        /// <summary>
        /// Handles ColorFrameReady event
        /// </summary>
        /// <remarks>
        /// Views image from the camera in KinectCameraImage
        /// </remarks>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments containing ImageFrame</param>
        private void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null) return;
                var pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                KinectCameraImage.WritePixels(_cameraSourceBounds, pixels, _colorStride, 0);
                OnPropertyChanged("KinectCameraImage");
            }
        }
        /// <summary>
        /// Subscribes for StatusChanged event and initializes KinectSensor
        /// </summary>
        private void DiscoverKinectSensors()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensor_StatusChanged;
            Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            if (Kinect == null)
            {
                ErrorGridVisibility = Visibility.Visible;
                ErrorGridMessage = "Proszę podłączyć Kinect";
            }
        }
        /// <summary>
        /// Updates KinectSensor
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void KinectSensor_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Inicjalizacja Kinecta...";
                    break;
                case KinectStatus.Connected:
                    ErrorGridVisibility = Visibility.Hidden;
                    if (Kinect == null)
                        Kinect = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    if (Kinect == e.Sensor)
                    {
                        Kinect = null;
                        Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (Kinect == null)
                        {
                            ErrorGridVisibility = Visibility.Visible;
                            ErrorGridMessage = "Podłącz Kinect do komputera.";
                        }
                    }
                    break;
                case KinectStatus.NotPowered:
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Podłącz kabel zasilający do gniazdka.";
                    break;
                default:
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Kinect nie może być uruchomiony. Poczekaj chwilę lub uruchom program ponownie.";
                    break;
            }
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            ErrorGridVisibility = Visibility.Hidden;
            DiscoverKinectSensors();
        }
        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        public void Cleanup()
        {
            Kinect = null;
        }
        #endregion Public Methods
    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace ModelCreator.ViewModel
{
    public class KinectService : ViewModelBase, IKinectService
    {
        #region Private Fields
        private KinectSensor _kinectSensor;
        private WriteableBitmap _kinectCameraImage;
        private WriteableBitmap _kinectDepthImage;
        private Int32Rect _cameraSourceBounds;
        private int _colorStride;
        private Int32Rect _depthSourceBounds;
        private int _depthStride;
        private Visibility _errorGridVisibility;
        private double _imageWidth;
        private double _imageHeight;
        private string _errorGridMessage;
        private DepthImagePixel[] _depthData;
        private readonly object _depthDataMutex = new object();
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
        /// Gets or sets the kinect depth image.
        /// </summary>
        public WriteableBitmap KinectDepthImage
        {
            get { return _kinectDepthImage; }
            set
            {
                if (Equals(_kinectDepthImage, value))
                    return;
                _kinectDepthImage = value;
                OnPropertyChanged("KinectDepthImage");
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

                var depthStream = sensor.DepthStream;
                depthStream.Enable();

                KinectDepthImage = new WriteableBitmap(depthStream.FrameWidth, depthStream.FrameHeight
                    , 96, 96, PixelFormats.Gray16, null);
                _depthSourceBounds = new Int32Rect(0, 0, depthStream.FrameWidth, depthStream.FrameHeight);
                _depthStride = depthStream.FrameWidth * depthStream.FrameBytesPerPixel;
                sensor.DepthFrameReady += KinectSensor_DepthFrameReady;
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
            using (var depthFrame = e.OpenDepthImageFrame())
            {
                lock (_depthDataMutex)
                {
                    if (depthFrame == null || depthFrame.PixelDataLength == 0)
                        return;
                    _depthData = new DepthImagePixel[depthFrame.PixelDataLength];
                    depthFrame.CopyDepthImagePixelDataTo(_depthData);

                    // Get the min and max reliable depth for the current frame
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    // Convert the depth to grayscale
                    int colorPixelIndex = 0;
                    byte[] colorPixels = new byte[_depthData.Length * 2];
                    for (int i = 0; i < _depthData.Length; ++i)
                    {
                        short depth = _depthData[i].Depth;
                        byte intensity = (byte)(Math.Max(Math.Min(depth, maxDepth), minDepth));

                        colorPixels[colorPixelIndex++] = intensity;
                        colorPixels[colorPixelIndex++] = intensity;
                    }

                    KinectDepthImage.WritePixels(_depthSourceBounds, colorPixels, _depthStride, 0);
                    OnPropertyChanged("KinectDepthImage");
                }
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
        /// <summary>
        /// Gets the depth data.
        /// </summary>
        /// <returns>The array of depth data</returns>
        public DepthImagePixel[] GetDepthData()
        {
            DepthImagePixel[] data;
            lock (_depthDataMutex)
            {
                data = new DepthImagePixel[_depthData.Length];
                _depthData.CopyTo(data, 0);
            }
            return data;
        }
        #endregion Public Methods
    }
}

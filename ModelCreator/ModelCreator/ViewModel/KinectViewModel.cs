
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace ModelCreator.ViewModel
{
    /// <summary>
    /// View model for MainWindow
    /// </summary>
    public class KinectViewModel : ViewModelBase
    {
        #region Private Fields
        private readonly KinectService _kinectService;
        private ModelBuilder _builder;
        private double _rotationAngle;
        private double _currentRotation;
        private Model3DGroup _model;
        private const double FullRotationAngle = 360;
        private const int CubeDivide = 5;
        #endregion Private Fields
        #region Public Properties
        /// <summary>
        /// Gets the kinect service.
        /// </summary>
        public KinectService KinectService
        {
            get { return _kinectService; }
        }
        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        public double RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                if (_rotationAngle == value) return;
                _rotationAngle = value;
                OnPropertyChanged("RotationAngle");
            }
        }
        /// <summary>
        /// Gets or sets the current rotation.
        /// </summary>
        public double CurrentRotation
        {
            get { return _currentRotation; }
            set
            {
                if (_currentRotation == value) return;
                _currentRotation = value;
                OnPropertyChanged("CurrentRotation");
            }
        }
        /// <summary>
        /// Gets or sets model bigest size.
        /// </summary>
        public double ModelSize { get; set; }
        /// <summary>
        /// Gets or sets the created model.
        /// </summary>
        public Model3DGroup Model
        {
            get { return _model; }
            set
            {
                if (_model == value) return;
                _model = value;
                OnPropertyChanged("Model");
            }
        }
        #endregion Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="KinectViewModel"/> class.
        /// </summary>
        /// <param name="kinectService">The kinect service.</param>
        public KinectViewModel(KinectService kinectService)
        {
            RotationAngle = 30;
            CurrentRotation = 0;
            _kinectService = kinectService;
            _kinectService.Initialize();
        }
        #endregion .ctor
        #region Commands
        /// <summary>
        /// The command, executed after clicking on capture button
        /// </summary>
        private ICommand _captureCommand;
        /// <summary>
        /// Gets the command.
        /// </summary>
        public ICommand CaptureCommand
        {
            get { return _captureCommand ?? (_captureCommand = new DelegateCommand(CaptureExecuted)); }
        }
        /// <summary>
        /// Executes when capture button was hit.
        /// </summary>
        public void CaptureExecuted()
        {
            DepthImagePixel[] data = KinectService.GetDepthData();
            if (_builder == null)
            {
                ModelSize = ModelBuilder.GetModelSize(data, KinectService.KinectDepthImage.PixelWidth);
                _builder = new ModelBuilder(ModelSize, CubeDivide);
            }

            _builder.CheckVerticesInCube(CurrentRotation, data);
            CurrentRotation = CurrentRotation + RotationAngle;

            if (CurrentRotation == FullRotationAngle)
            {
                CurrentRotation = 0;
                Model = _builder.CreateModel();
            }
        }
        #endregion Commands
    }

}

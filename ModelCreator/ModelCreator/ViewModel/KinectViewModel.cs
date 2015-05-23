
namespace ModelCreator.ViewModel
{
    /// <summary>
    /// View model for MainWindow
    /// </summary>
    public class KinectViewModel : ViewModelBase
    {
        #region Private Fields
        /// <summary>
        /// The Kinect service
        /// </summary>
        private readonly KinectService _kinectService;
        #endregion Private Fields
        #region Public Properties
        /// <summary>
        /// Gets the kinect service.
        /// </summary>
        /// <value>
        /// The kinect service.
        /// </value>
        public KinectService KinectService
        {
            get { return _kinectService; }
        }
        #endregion Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="KinectViewModel"/> class.
        /// </summary>
        /// <param name="kinectService">The kinect service.</param>
        public KinectViewModel(KinectService kinectService)
        {
            _kinectService = kinectService;
            _kinectService.Initialize();
        }
        #endregion .ctor
        #region Private Methods
        #endregion Private Methods
    }
}

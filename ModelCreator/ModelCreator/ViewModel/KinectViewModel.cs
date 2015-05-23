
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;

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

        }
        #endregion Commands
    }
}

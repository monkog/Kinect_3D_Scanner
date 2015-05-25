using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ModelCreator.View;

namespace ModelCreator.ViewModel
{
    public class ModelWindowViewModel : ViewModelBase
    {
        #region Private Members
        private Model3DGroup _model;
        private Transform3DGroup _cameraTransform;
        private bool _isMouseDown;
        private Point _mousePreviousPosition;
        private Point _mouseCurrentPosition;
        private Point _mouseDelta;
        private double _mouseScale;
        private const double Tolernce = 0.2;
        #endregion Private Members
        #region Public Properties
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
        /// <summary>
        /// Gets or sets the camera transform.
        /// </summary>
        public Transform3DGroup CameraTransform
        {
            get { return _cameraTransform; }
            set
            {
                if (_cameraTransform == value) return;
                _cameraTransform = value;
                OnPropertyChanged("CameraTransform");
            }
        }
        #endregion Public Properties
        #region Constructors
        public ModelWindowViewModel(Model3DGroup model)
        {
            Model = model;
        }
        #endregion Constructors
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
        #region Commands
        private ActionCommand<MouseButtonEventArgs> _mouseClickCommand;
        public ActionCommand<MouseButtonEventArgs> MouseClickCommand
        {
            get
            {
                return _mouseClickCommand ??
                       (_mouseClickCommand = new ActionCommand<MouseButtonEventArgs>(MouseClickExecuted));
            }
        }
        /// <summary>
        /// Captures mouse position
        /// </summary>
        private void MouseClickExecuted(MouseButtonEventArgs args)
        {
            _mouseCurrentPosition = args.GetPosition((Viewport3D)args.OriginalSource);
            _isMouseDown = true;
        }

        private ActionCommand<MouseButtonEventArgs> _mouseUpCommand;
        public ActionCommand<MouseButtonEventArgs> MouseUpCommand
        {
            get
            {
                return _mouseUpCommand ??
                       (_mouseUpCommand = new ActionCommand<MouseButtonEventArgs>(MouseUpExecuted));
            }
        }
        /// <summary>
        /// Captures mouse position
        /// </summary>
        private void MouseUpExecuted(MouseButtonEventArgs args)
        {
            _isMouseDown = false;
        }

        private ActionCommand<MouseEventArgs> _mouseMoveCommand;
        public ActionCommand<MouseEventArgs> MouseMoveCommand
        {
            get
            {
                return _mouseMoveCommand ??
                       (_mouseMoveCommand = new ActionCommand<MouseEventArgs>(MouseMoveExecuted));
            }
        }
        /// <summary>
        /// Updates mouse position
        /// </summary>
        private void MouseMoveExecuted(MouseEventArgs args)
        {
            if (!_isMouseDown || !(args.OriginalSource.GetType() == typeof(Viewport3D))) return;
            _mousePreviousPosition = _mouseCurrentPosition;
            _mouseCurrentPosition = args.GetPosition((Viewport3D)args.OriginalSource);

            _mouseDelta = new Point((_mouseCurrentPosition.X - _mousePreviousPosition.X) / 50, (_mouseCurrentPosition.Y - _mousePreviousPosition.Y) / 50);

            var transform = new Transform3DGroup();
            transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), _mouseDelta.X)));
            transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), _mouseDelta.Y)));
            CameraTransform = transform;
        }

        private ActionCommand<MouseWheelEventArgs> _mouseWheelCommand;
        public ActionCommand<MouseWheelEventArgs> MouseWheelCommand
        {
            get
            {
                return _mouseWheelCommand ??
                       (_mouseWheelCommand = new ActionCommand<MouseWheelEventArgs>(MouseWheelExecuted));
            }
        }
        /// <summary>
        /// Zooms in/out the viewport
        /// </summary>
        private void MouseWheelExecuted(MouseWheelEventArgs args)
        {
            _mouseScale = 1 + args.Delta / 1000f;

            var transform = new Transform3DGroup();
            transform.Children.Add(new ScaleTransform3D(_mouseScale, _mouseScale, _mouseScale));
            CameraTransform = transform;
        }
        #endregion Commands
    }
}

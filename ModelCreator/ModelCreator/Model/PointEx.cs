using System.Windows.Media.Media3D;

namespace ModelCreator.Model
{
    public class Point3DEx
    {
        #region Public Properties
        public Point3D Point { get; private set; }
        public bool IsChecked { get; set; }
        #endregion Public Properties
        #region Constructors
        public Point3DEx(double x, double y, double z)
        {
            Point = new Point3D(x, y, z);
            IsChecked = true;
        }
        #endregion Constructors
    }
}


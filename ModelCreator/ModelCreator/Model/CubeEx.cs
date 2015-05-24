using System.Collections.Generic;
using System.Windows.Media.Media3D;
using ModelCreator.ViewModel;

namespace ModelCreator.Model
{
    public class CubeEx
    {
        #region Public Properties
        public List<List<Point3DEx>> TetrahedronsList { get; set; }
        public List<List<Point3DEx>> HexahedronsList { get; set; }
        public Model3DGroup Cube { get; private set; }
        #endregion Public Properties
        #region Constructors
        public CubeEx(Model3DGroup cube)
        {
            Cube = cube;
        }
        #endregion Constructors
    }
}


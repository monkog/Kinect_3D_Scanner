using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ModelCreator.Model
{
    public class Triangle
    {
        #region Public Properties
        public Point A { get; set; }
        public Point B { get; set; }
        public Point C { get; set; }
        #endregion Public Properties
        #region Constructors
        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }
        #endregion Constructors
    }
}

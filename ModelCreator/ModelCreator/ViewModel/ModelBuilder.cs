using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using ModelCreator.Model;

namespace ModelCreator.ViewModel
{
    public class ModelBuilder
    {
        #region Private Members
        private CubeEx _modelCube;
        private const int Tolerance = 50;
        private Random rand = new Random();
        #endregion Private Members
        #region Constructors
        public ModelBuilder(double size, int divide)
        {
            // ModelVisual3D mv3d = new ModelVisual3D();
            // Model3DGroup c = CreateBigCube(0, 0, 0, size);   //<ta 5 trzeba zmienic z aktualnym rozmiarem skanowanego modelu
            // mv3d.Content = c;
            //  this.mainViewport.Children.Add(mv3d);


            _modelCube = DividedCube(divide, CreateBigCube(0, 0, 0, size));
            // Model3DGroup ourModel = CreatingModel(newCube);
        }
        #endregion Constructors
        #region Private Methods
        private List<Point3D> MarchingTetrahedrons(List<Point3DEx> tetrahedron)
        {
            int index = 0;
            for (int i = 0; i < 4; ++i)
                if (tetrahedron[i].IsChecked)
                    index |= (1 << i);
            List<Point3D> newPoints = new List<Point3D>();
            switch (index)
            {

                // we don't do anything if everyone is inside or outside
                case 0x00:
                case 0x0F:
                    break;

                // only vert 0 is inside
                case 0x01:
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[2].Point, -1));
                    break;

                // only vert 1 is inside
                case 0x02:
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    break;

                // only vert 2 is inside
                case 0x04:
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[1].Point, -1));
                    break;

                // only vert 3 is inside
                case 0x08:
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    break;

                // verts 0, 1 are inside
                case 0x03:
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));

                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    break;

                // verts 0, 2 are inside
                case 0x05:
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[0].Point, -1));

                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[3].Point, -1));
                    break;

                // verts 0, 3 are inside
                case 0x09:
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[2].Point, -1));

                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[2].Point, -1));
                    break;

                // verts 1, 2 are inside
                case 0x06:
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));

                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[2].Point, -1));
                    break;

                // verts 2, 3 are inside
                case 0x0C:
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));

                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[1].Point, -1));
                    break;

                // verts 1, 3 are inside
                case 0x0A:
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[2].Point, -1));

                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    break;

                // verts 0, 1, 2 are inside
                case 0x07:
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[3].Point, tetrahedron[1].Point, -1));
                    break;

                // verts 0, 1, 3 are inside
                case 0x0B:
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[2].Point, tetrahedron[0].Point, -1));
                    break;

                // verts 0, 2, 3 are inside
                case 0x0D:
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[0].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[3].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[1].Point, tetrahedron[2].Point, -1));
                    break;

                // verts 1, 2, 3 are inside
                case 0x0E:
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[1].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[2].Point, -1));
                    newPoints.Add(CalculateVert(tetrahedron[0].Point, tetrahedron[3].Point, -1));
                    break;
            }
            return newPoints;
        }
        private Point3D CalculateVert(Point3D p1, Point3D p2, double isolevel)
        {
            double v1 = Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z);
            double v2 = Math.Sqrt(p2.X * p2.X + p2.Y * p2.Y + p2.Z * p2.Z);

            double x, y, z;

            if (v2 == v1)
            {
                x = (p1.X + p2.X) / 2.0f;
                y = (p1.Y + p2.Y) / 2.0f;
                z = (p1.Z + p2.Z) / 2.0f;
            }
            else
            {
                /*
                 <----+-----+---+----->
                      v1    |   v2
                         isolevel            
                 <----+-----+---+----->
                      0     |   1
                          interp       
                 */
                // interp == 0: vert should be at p1
                // interp == 1: vert should be at p2
                double interp = (isolevel - v1) / (v2 - v1);
                double oneMinusInterp = 1 - interp;

                x = p1.X * oneMinusInterp + p2.X * interp;
                y = p1.Y * oneMinusInterp + p2.Y * interp;
                z = p1.Z * oneMinusInterp + p2.Z * interp;
            }

            return new Point3D(x, y, z);
        }
        //metoda tworzaca duzy szescian 
        private Model3DGroup CreateBigCube(double X, double Y, double Z, double size)
        {
            Model3DGroup cube = new Model3DGroup();
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(X - size / 2, Y - size / 2, Z - size / 2));
            mesh.Positions.Add(new Point3D(X + size / 2, Y - size / 2, Z - size / 2));
            mesh.Positions.Add(new Point3D(X + size / 2, Y - size / 2, Z + size / 2));
            mesh.Positions.Add(new Point3D(X - size / 2, Y - size / 2, Z + size / 2));
            mesh.Positions.Add(new Point3D(X - size / 2, Y + size / 2, Z - size / 2));
            mesh.Positions.Add(new Point3D(X + size / 2, Y + size / 2, Z - size / 2));
            mesh.Positions.Add(new Point3D(X + size / 2, Y + size / 2, Z + size / 2));
            mesh.Positions.Add(new Point3D(X - size / 2, Y + size / 2, Z + size / 2));

            // mesh.TriangleIndices.Add(new int[] { 3, 2, 6, 3, 6, 7, 2, 1, 5, 2, 5, 6, 1, 0, 4, 1, 4, 5, 0, 3, 7, 0, 7, 4, 7, 6, 5, 7, 5, 4, 2, 3, 0, 2, 0, 1 });



            ////front side triangles
            //cube.Children.Add(CreateTriangleModel(p3, p2, p6));
            //cube.Children.Add(CreateTriangleModel(p3, p6, p7));
            ////right side triangles
            //cube.Children.Add(CreateTriangleModel(p2, p1, p5));
            //cube.Children.Add(CreateTriangleModel(p2, p5, p6));
            ////back side triangles
            //cube.Children.Add(CreateTriangleModel(p1, p0, p4));
            //cube.Children.Add(CreateTriangleModel(p1, p4, p5));
            ////left side triangles
            //cube.Children.Add(CreateTriangleModel(p0, p3, p7));
            //cube.Children.Add(CreateTriangleModel(p0, p7, p4));
            ////top side triangles
            //cube.Children.Add(CreateTriangleModel(p7, p6, p5));
            //cube.Children.Add(CreateTriangleModel(p7, p5, p4));
            ////bottom side triangles
            //cube.Children.Add(CreateTriangleModel(p2, p3, p0));
            //cube.Children.Add(CreateTriangleModel(p2, p0, p1));

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkKhaki));
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            cube.Children.Add(model);
            cube.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-2, -3, -1)));
            return cube;
        }
        private CubeEx DividedCube(int divide, Model3DGroup cube)
        {
            double voxelWidth = cube.Bounds.SizeX / divide;
            List<List<Point3DEx>> cubevoxels = new List<List<Point3DEx>>();
            List<List<Point3DEx>> tetraVoxelsTriangles = new List<List<Point3DEx>>();
            List<List<Point3DEx>> tetraVoxelsVertices = new List<List<Point3DEx>>();
            var hexahedrons = new List<Point3DEx>[divide, divide, divide];

            for (int i = 0; i < divide; i++)
                for (int j = 0; j < divide; j++)
                    for (int k = 0; k < divide; k++)
                    {
                        List<Point3DEx> tmp = new List<Point3DEx>();
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        cubevoxels.Add(tmp);
                        hexahedrons[i, j, k] = tmp;
                    }


            //kazdy z malych szescianow dzielimy na 5 czworoscianow
            foreach (var c in cubevoxels)
            {
                //lista z trojkatami szescianow
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new[] { c[3], c[2], c[6], c[2], c[1], c[6], c[3], c[2], c[1], c[3], c[6], c[1] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new[] { c[1], c[5], c[6], c[1], c[4], c[5], c[5], c[4], c[6], c[1], c[4], c[6] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new[] { c[3], c[6], c[7], c[3], c[7], c[4], c[7], c[6], c[4], c[3], c[6], c[4] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new[] { c[0], c[3], c[4], c[0], c[1], c[4], c[0], c[3], c[1], c[4], c[3], c[1] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new[] { c[4], c[3], c[1], c[3], c[6], c[1], c[1], c[6], c[4], c[3], c[6], c[4] }));

                //lista z wierzcholkami szescianow
                tetraVoxelsVertices.Add(new List<Point3DEx>(new[] { c[6], c[3], c[2], c[1] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new[] { c[1], c[4], c[5], c[6] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new[] { c[3], c[6], c[7], c[4] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new[] { c[4], c[1], c[0], c[3] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new[] { c[1], c[3], c[4], c[6] }));
            }

            CubeEx myCube = new CubeEx(cube);
            myCube.Hexahedrons = hexahedrons;
            myCube.TetrahedronsList = tetraVoxelsVertices;
            return myCube;
        }
        private static int FindMidPixel(DepthImagePixel[] data, int midPixel)
        {
            for (int i = 0; i < data.Length / 2; i++)
                if (data[midPixel + i].IsKnownDepth) return data[midPixel + i].Depth;
                else if (data[midPixel - i].IsKnownDepth) return data[midPixel - i].Depth;
            return 0;
        }
        public void SelectModelPoints(DepthImagePixel[] data, int stride)
        {
            var midPixel = data.Length / 2;
            int leftDepth = FindMidPixel(data, midPixel);
            var rightDepth = leftDepth;
            int width = 1;

            for (int j = 0; j < stride / 2; j++)
            {
                bool changed = false;
                if (data[midPixel - j].IsKnownDepth && Math.Abs(data[midPixel - j].Depth - leftDepth) < Tolerance)
                {
                    width = j + 1;
                    leftDepth = data[midPixel - j].Depth;
                    changed = true;
                }
                if (data[midPixel + j].IsKnownDepth && Math.Abs(data[midPixel + j].Depth - rightDepth) < Tolerance)
                {
                    width = j + 1;
                    rightDepth = data[midPixel + j].Depth;
                    changed = true;
                }
                if (!changed && (data[midPixel + j].IsKnownDepth || data[midPixel - j].IsKnownDepth))
                    break;
            }
        }
        private void GetMinMaxValues(out int min, out int max)
        {
            throw new NotImplementedException();
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Fills the voksels with the data from depth stream from Kinect
        /// </summary>
        /// <param name="angle">The current rotation angle.</param>
        /// <param name="rawData">The depth data from Kinect.</param>
        /// <param name="f">The focal length for Kinect</param>
        public void CheckVerticesInCube(int angle, DepthImagePixel[] rawData, float f)
        {
            //najpierw obracamy model o podany kąt
            var myRotateTransform3D = new RotateTransform3D();
            var myAxisAngleRotation3D = new AxisAngleRotation3D
            {
                Axis = new Vector3D(0, 1, 0),
                Angle = angle
            };
            myRotateTransform3D.Rotation = myAxisAngleRotation3D;
            _modelCube.Cube.Transform = myRotateTransform3D;

            DepthImagePixel[,] data = new DepthImagePixel[640, 480];
            for (int j = 0; j < 480; j++)
                for (int i = 0; i < 640; i++)
                    data[i, j] = rawData[j * 640 + i];
            //int startIndex = 640 / 2 - (int)(_modelCube.Cube.Bounds.SizeX / 2);
            //int step = (int)_modelCube.Cube.Bounds.SizeX / _modelCube.Hexahedrons.GetLength(0);

            switch (angle)
            {
                case 0:
                    for (int i = 0; i < _modelCube.Hexahedrons.GetLength(0); i++)
                        for (int j = 0; j < _modelCube.Hexahedrons.GetLength(1); j++)
                            for (int k = 0; k < _modelCube.Hexahedrons.GetLength(2); k++)
                            {
                                var list = _modelCube.Hexahedrons[i, j, k];
                                foreach (var point in list)
                                {
                                    int x = (int)(point.Point.X / point.Point.Z * f);
                                    int y = (int)(point.Point.Y / point.Point.Z * f);
                                    if (x >= 0 && y >= 0 && x < data.GetLength(0) && y < data.GetLength(1) && data[x, y].IsKnownDepth && data[x, y].Depth > 1000)
                                        point.IsChecked = false;
                                }
                            }
                    break;
                case 90:
                    for (int i = 0; i < _modelCube.Hexahedrons.GetLength(0); i++)
                        for (int j = 0; j < _modelCube.Hexahedrons.GetLength(1); j++)
                            for (int k = 0; k < _modelCube.Hexahedrons.GetLength(2); k++)
                            {
                                var list = _modelCube.Hexahedrons[i, j, k];
                                foreach (var point in list)
                                {
                                    int x = (int)(point.Point.Z / (-point.Point.X) * f);
                                    int y = (int)(point.Point.Y / point.Point.Z * f);
                                    if (x >= 0 && y >= 0 && x < data.GetLength(0) && y < data.GetLength(1) && data[x, y].IsKnownDepth && data[x, y].Depth > 1000)
                                        point.IsChecked = false;
                                }
                            }
                    break;
                case 180:
                    for (int i = 0; i < _modelCube.Hexahedrons.GetLength(0); i++)
                        for (int j = 0; j < _modelCube.Hexahedrons.GetLength(1); j++)
                            for (int k = 0; k < _modelCube.Hexahedrons.GetLength(2); k++)
                            {
                                var list = _modelCube.Hexahedrons[i, j, k];
                                foreach (var point in list)
                                {
                                    int x = (int)(point.Point.X / point.Point.Z * f);
                                    int y = -(int)(point.Point.Y / point.Point.Z * f);
                                    if (x >= 0 && y >= 0 && x < data.GetLength(0) && y < data.GetLength(1) && data[x, y].IsKnownDepth && data[x, y].Depth > 1000)
                                        point.IsChecked = false;
                                }
                            }
                    break;
                case 270:
                    for (int i = 0; i < _modelCube.Hexahedrons.GetLength(0); i++)
                        for (int j = 0; j < _modelCube.Hexahedrons.GetLength(1); j++)
                            for (int k = 0; k < _modelCube.Hexahedrons.GetLength(2); k++)
                            {
                                var list = _modelCube.Hexahedrons[i, j, k];
                                foreach (var point in list)
                                {
                                    int x = -(int)(point.Point.Z / point.Point.X * f);
                                    int y = (int)(point.Point.Y / point.Point.X * f);
                                    if (x >= 0 && y >= 0 && x < data.GetLength(0) && y < data.GetLength(1) && data[x, y].IsKnownDepth && data[x, y].Depth > 1000)
                                        point.IsChecked = false;
                                }
                            }
                    break;
            }
        }
        public Model3DGroup CreateModel()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            List<Point3D> meshVertices = new List<Point3D>();
            foreach (var t in _modelCube.TetrahedronsList)
                foreach (var point in MarchingTetrahedrons(t))
                    mesh.Positions.Add(point);

            for (int i = 0; i < mesh.Positions.Count; i += 3)
            {
                mesh.TriangleIndices.Add(i);
                mesh.TriangleIndices.Add(i + 1);
                mesh.TriangleIndices.Add(i + 2);
            }

            Material material = new DiffuseMaterial(
                new SolidColorBrush(Colors.Chocolate));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }
        /// <summary>
        /// Gets the size of the model.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="stride">The stride.</param>
        /// <returns>Model size in pixels</returns>
        public static double GetModelSize(DepthImagePixel[] data, int stride)
        {
            var midPixel = data.Length / 2;
            int leftDepth = FindMidPixel(data, midPixel);
            var rightDepth = leftDepth;
            var topDepth = leftDepth;
            var bottomDepth = leftDepth;
            int width = 1;

            for (int j = 0; j < stride / 2; j++)
            {
                bool changed = false;
                if (data[midPixel - j].IsKnownDepth && Math.Abs(data[midPixel - j].Depth - leftDepth) < Tolerance)
                {
                    width = j + 1;
                    leftDepth = data[midPixel - j].Depth;
                    changed = true;
                }
                if (data[midPixel + j].IsKnownDepth && Math.Abs(data[midPixel + j].Depth - rightDepth) < Tolerance)
                {
                    width = j + 1;
                    rightDepth = data[midPixel + j].Depth;
                    changed = true;
                }
                //if (midPixel + (j * stride) < data.Length && data[midPixel + (j * stride)].IsKnownDepth && Math.Abs(data[midPixel + (j * stride)].Depth - bottomDepth) < Tolerance)
                //{
                //    width = j + 1;
                //    bottomDepth = data[midPixel + (j * stride)].Depth;
                //    changed = true;
                //}
                //if (midPixel - (j * stride) >= 0 && data[midPixel - (j * stride)].IsKnownDepth && Math.Abs(data[midPixel - (j * stride)].Depth - topDepth) < Tolerance)
                //{
                //    width = j + 1;
                //    topDepth = data[midPixel - (j * stride)].Depth;
                //    changed = true;
                //}
                if (!changed && (data[midPixel + j].IsKnownDepth || data[midPixel - j].IsKnownDepth || data[midPixel - (j * stride)].IsKnownDepth || data[midPixel + (j * stride)].IsKnownDepth))
                    break;
            }

            return 2 * width;
        }
        #endregion Public Methods
    }
}

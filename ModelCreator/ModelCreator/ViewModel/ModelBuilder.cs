using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using ModelCreator.Model;

namespace ModelCreator.ViewModel
{
    public class ModelBuilder
    {
        #region Private Members
        private CubeEx _modelCube;
        private const int ImageWidth = 640;
        private const int Tolerance = 50;
        private const int ImageHeight = 480;
        private List<Triangle> _triangleIndices;
        private const double Epsilon = 10;
        #endregion Private Members
        #region Constructors
        public ModelBuilder(double size, int divide)
        {
            _modelCube = DividedCube(divide, CreateBigCube(0, 0, 0, size));
            _triangleIndices = CreateTriangleIndicesList();
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
        private void RotateCube(int angle)
        {
            var myRotateTransform3D = new RotateTransform3D();
            var myAxisAngleRotation3D = new AxisAngleRotation3D
            {
                Axis = new Vector3D(0, 1, 0),
                Angle = angle
            };
            myRotateTransform3D.Rotation = myAxisAngleRotation3D;
            _modelCube.Cube.Transform = myRotateTransform3D;
        }
        private DepthImagePixel[,] CreateDepthArray(DepthImagePixel[] rawData)
        {
            var data = new DepthImagePixel[ImageWidth, ImageHeight];
            for (int j = 0; j < ImageHeight; j++)
                for (int i = 0; i < ImageWidth; i++)
                    data[i, j] = rawData[j * ImageWidth + i];
            return data;
        }
        private Point3D?[,] MapDepthDataTo3D(float f, DepthImagePixel[,] data)
        {
            var depthPoints = new Point3D?[ImageWidth, ImageHeight];
            for (int i = 0; i < ImageWidth; i++)
                for (int j = 0; j < ImageHeight; j++)
                {
                    if (!data[i, j].IsKnownDepth) continue;
                    var depth = data[i, j].Depth;
                    depthPoints[i, j] = new Point3D((i / f) * depth, (j / f) * depth, depth);
                }
            return depthPoints;
        }/// <summary>
        /// Checks if the specified ray hits the triagnlge descibed by p1, p2 and p3.
        /// Möller–Trumbore ray-triangle intersection algorithm implementation.
        /// </summary>
        /// <param name="p1">Vertex 1 of the triangle.</param>
        /// <param name="p2">Vertex 2 of the triangle.</param>
        /// <param name="p3">Vertex 3 of the triangle.</param>
        /// <param name="ray">The ray to test hit for.</param>
        /// <returns><c>true</c> when the ray hits the triangle, otherwise <c>false</c></returns>
        /// <remarks>http://answers.unity3d.com/questions/861719/a-fast-triangle-triangle-intersection-algorithm-fo.html</remarks>
        private bool Intersect(Point3D p1P, Point3D p2P, Point3D p3P, Point3D rayP)
        {
            Vector3D p1 = new Vector3D(p1P.X, p1P.Y, p1P.Z);
            Vector3D p2 = new Vector3D(p2P.X, p2P.Y, p2P.Z);
            Vector3D p3 = new Vector3D(p3P.X, p3P.Y, p3P.Z);
            Vector3D ray = new Vector3D(rayP.X, rayP.Y, rayP.Z);
            // Vectors from p1 to p2/p3 (edges)
            Vector3D e1, e2;

            Vector3D p, q, t;
            double det, invDet, u, v;

            //Find vectors for two edges sharing vertex/point p1
            e1 = p2 - p1;
            e2 = p3 - p1;

            // calculating determinant 
            p = Vector3D.CrossProduct(ray, e2);

            //Calculate determinat
            det = Vector3D.DotProduct(e1, p);

            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -Epsilon && det < Epsilon) { return false; }
            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = -p1;

            //Calculate u parameter
            u = Vector3D.DotProduct(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1) { return false; }

            //Prepare to test v parameter
            q = Vector3D.CrossProduct(t, e1);

            //Calculate v parameter
            v = Vector3D.DotProduct(ray, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1) { return false; }

            if ((Vector3D.DotProduct(e2, q) * invDet) > Epsilon)
                return true;

            // No hit at all
            return false;
        }
        private List<Triangle> CreateTriangleIndicesList()
        {
            var triangles = new List<Triangle>();

            for (int i = 0; i < ImageWidth - 1; i++)
                for (int j = 0; j < ImageHeight - 1; j++)
                {
                    triangles.Add(new Triangle(new Point(i, j), new Point(i, j + 1), new Point(i + 1, j + 1)));
                    //triangles.Add(new Triangle(new Point(i, j), new Point(i + 1, j), new Point(i + 1, j + 1)));
                }

            return triangles;
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
            RotateCube(angle);
            var data = CreateDepthArray(rawData);
            var triangles = MapDepthDataTo3D(f, data);

            switch (angle)
            {
                case 0:
                    //foreach (var tetrahedron in _modelCube.TetrahedronsList)
                    //{
                        //if (_modelCube.TetrahedronsList.IndexOf(tetrahedron) % 4 != 0) continue;
                        //Console.Write(" " + _modelCube.TetrahedronsList.IndexOf(tetrahedron));
                        //foreach (var vertex in tetrahedron)
                        //{
                        //    foreach (var triangle in _triangleIndices)
                        //    {
                        //        if (!triangles[(int)triangle.A.X, (int)triangle.A.Y].HasValue ||
                        //            !triangles[(int)triangle.B.X, (int)triangle.B.Y].HasValue
                        //            || !triangles[(int)triangle.C.X, (int)triangle.C.Y].HasValue) continue;
                        //        bool isIntersecting = Intersect(triangles[(int)triangle.A.X, (int)triangle.A.Y].Value,
                        //            triangles[(int)triangle.B.X, (int)triangle.B.Y].Value,
                        //            triangles[(int)triangle.C.X, (int)triangle.C.Y].Value, vertex.Point);
                        //        if (isIntersecting)
                        //            vertex.IsChecked = false;
                        //    }
                        //}
                    //}
                    var tetra = _modelCube.TetrahedronsList.First();
                    tetra[0].IsChecked = false;
                    //tetra[3].IsChecked = false;
                    break;
                case 90:
                    break;
                case 180:
                    break;
                case 270:
                    break;
            }
        }
        /// <summary>
        /// Creates the model.
        /// </summary>
        /// <returns>Model group containing the created model</returns>
        public Model3DGroup CreateModel()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
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
            GeometryModel3D model = new GeometryModel3D(mesh, material);
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

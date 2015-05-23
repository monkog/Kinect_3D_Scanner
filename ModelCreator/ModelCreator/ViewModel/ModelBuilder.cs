using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace ModelCreator.ViewModel
{
    public class ModelBuilder
    {
        private CubeEx _modelCube;
        public ModelBuilder(double size, int divide)
        {
           // ModelVisual3D mv3d = new ModelVisual3D();
           // Model3DGroup c = CreateBigCube(0, 0, 0, size);   //<ta 5 trzeba zmienic z aktualnym rozmiarem skanowanego modelu
           // mv3d.Content = c;
          //  this.mainViewport.Children.Add(mv3d);


            _modelCube = DividedCube(divide, CreateBigCube(0, 0, 0, size));
           // Model3DGroup ourModel = CreatingModel(newCube);
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

        ///TO TRZEBA UZUPELNIC DANYMI Z KINECTA!!!!!! :)
        public void CheckVerticesInCube(double angle)
        {
            //najpierw obracamy model o podany kąt
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(0, 1, 0);
            myAxisAngleRotation3d.Angle = angle;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            _modelCube.Cube.Transform = myRotateTransform3D;

            //sprawdzamy kazdy z wierzcholkow podzielonego szescianu
            for (int j = 0; j < _modelCube.HexahedronsList.Count; j++)
                for (int i = 0; i < _modelCube.HexahedronsList[j].Count; i++)
                    if (_modelCube.HexahedronsList[j][i].Point == null)//!!!!!!!!!!!!!!!!!!! tutaj trzeba ten warunek czy wierzcholek jest trafiony czy nie na podstawie danych z bufora
                        _modelCube.HexahedronsList[j][i].isChecked = false;
        }
        private CubeEx DividedCube(int divide, Model3DGroup cube)
        {
            double voxelWidth = cube.Bounds.SizeX / divide;
            List<List<Point3DEx>> Cubevoxels = new List<List<Point3DEx>>();
            List<List<Point3DEx>> tetraVoxelsTriangles = new List<List<Point3DEx>>();
            List<List<Point3DEx>> tetraVoxelsVertices = new List<List<Point3DEx>>();
            List<Point3DEx> tmp;

            for (int i = 0; i < divide; i++)
                for (int j = 0; j < divide; j++)
                    for (int k = 0; k < divide; k++)
                    {
                        tmp = new List<Point3DEx>();
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth - voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth - voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth + voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        tmp.Add(new Point3DEx(i * voxelWidth - voxelWidth / 2, j * voxelWidth + voxelWidth / 2, k * voxelWidth + voxelWidth / 2));
                        Cubevoxels.Add(tmp);
                    }


            //kazdy z malych szescianow dzielimy na 5 czworoscianow
            foreach (var c in Cubevoxels)
            {
                //lista z trojkatami szescianow
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new Point3DEx[] { c[3], c[2], c[6], c[2], c[1], c[6], c[3], c[2], c[1], c[3], c[6], c[1] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new Point3DEx[] { c[1], c[5], c[6], c[1], c[4], c[5], c[5], c[4], c[6], c[1], c[4], c[6] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new Point3DEx[] { c[3], c[6], c[7], c[3], c[7], c[4], c[7], c[6], c[4], c[3], c[6], c[4] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new Point3DEx[] { c[0], c[3], c[4], c[0], c[1], c[4], c[0], c[3], c[1], c[4], c[3], c[1] }));
                tetraVoxelsTriangles.Add(new List<Point3DEx>(new Point3DEx[] { c[4], c[3], c[1], c[3], c[6], c[1], c[1], c[6], c[4], c[3], c[6], c[4] }));

                //lista z wierzcholkami szescianow
                tetraVoxelsVertices.Add(new List<Point3DEx>(new Point3DEx[] { c[6], c[3], c[2], c[1] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new Point3DEx[] { c[1], c[4], c[5], c[6] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new Point3DEx[] { c[3], c[6], c[7], c[4] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new Point3DEx[] { c[4], c[1], c[0], c[3] }));
                tetraVoxelsVertices.Add(new List<Point3DEx>(new Point3DEx[] { c[1], c[3], c[4], c[6] }));
            }

            CubeEx myCube = new CubeEx(cube);
            myCube.HexahedronsList = Cubevoxels;
            myCube.TetrahedronsList = tetraVoxelsVertices;
            return myCube;
        }
        public Model3DGroup CreatingModel()
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
                new SolidColorBrush(Colors.DarkKhaki));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }
        private List<Point3D> MarchingTetrahedrons(List<Point3DEx> tetrahedron)
        {
            int index = 0;
            for (int i = 0; i < 4; ++i)
                if (tetrahedron[i].isChecked)
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
    }
    class CubeEx
    {
        public List<List<Point3DEx>> TetrahedronsList { get; set; }
        public List<List<Point3DEx>> HexahedronsList { get; set; }
        public Model3DGroup Cube { get; private set; }
        public CubeEx(Model3DGroup cube)
        {
            Cube = cube;
        }
    }

    class Point3DEx
    {
        public Point3D Point { get; private set; }
        public bool isChecked { get; set; }
        public Point3DEx(double x, double y, double z)
        {
            Point = new Point3D(x, y, z);
            isChecked = true;
        }
    }
}

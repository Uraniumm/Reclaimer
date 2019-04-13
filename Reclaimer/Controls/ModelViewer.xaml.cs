﻿using Adjutant.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Studio.Controls;
using System.Collections.ObjectModel;

namespace Reclaimer.Controls
{
    /// <summary>
    /// Interaction logic for ModelViewer.xaml
    /// </summary>
    public partial class ModelViewer : UserControl, ITabContent
    {
        public ObservableCollection<TreeViewItem> TreeViewItems { get; }

        public ModelViewer()
        {
            InitializeComponent();
            TreeViewItems = new ObservableCollection<TreeViewItem>();
            DataContext = this;
        }

        public object Header => "ModelViewer";

        public object Icon => null;

        public TabItemUsage Usage => TabItemUsage.Document;

        public void LoadGeometry(Adjutant.Utilities.IRenderGeometry geom)
        {
            var errorMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Gold));
            var model = geom.ReadGeometry(0);
            var group = new Model3DGroup();
            var vis = new ModelVisual3D();

            TreeViewItems.Clear();
            foreach (var region in model.Regions)
            {
                var regNode = new TreeViewItem { Header = region.Name };

                foreach (var perm in region.Permutations)
                {
                    var permNode = new TreeViewItem { Header = perm.Name };
                    regNode.Items.Add(permNode);

                    var mesh = model.Meshes[perm.MeshIndex];

                    foreach (var sub in perm.Submeshes)
                    {
                        var mg = new MeshGeometry3D();

                        var indices = mesh.Indicies.Skip(sub.IndexStart).Take(sub.IndexLength);
                        if (mesh.IndexFormat == IndexFormat.Stripped) indices = Unstrip(indices);

                        var verts = mesh.Vertices.Skip(sub.VertexStart).Take(sub.VertexLength);
                        var positions = verts.Select(v => new Point3D(v.Position[0].X, v.Position[0].Y, v.Position[0].Z));
                        var texcoords = verts.Select(v => new Point(v.TexCoords[0].X, v.TexCoords[0].Y));

                        mg.Positions = new Point3DCollection(positions);
                        mg.TextureCoordinates = new PointCollection(texcoords);
                        mg.TriangleIndices = new Int32Collection(indices);

                        var g = new GeometryModel3D(mg, errorMaterial) { BackMaterial = errorMaterial };
                        group.Children.Add(g);
                    }
                }

                TreeViewItems.Add(regNode);
            }

            vis.Content = group;

            renderer.AddChild(vis);

            renderer.ScaleToContent(new[] { group });
        }

        private IEnumerable<int> Unstrip(IEnumerable<int> strip)
        {
            var arr = strip.ToArray();

            for (int n = 0; n < arr.Length - 2; n++)
            {
                int i1 = arr[n + 0];
                int i2 = arr[n + 1];
                int i3 = arr[n + 2];

                if ((i1 != i2) && (i1 != i3) && (i2 != i3))
                {
                    yield return i1;

                    if (n % 2 == 0)
                    {
                        yield return i2;
                        yield return i3;
                    }
                    else
                    {
                        yield return i3;
                        yield return i2;
                    }
                }
            }
        }
    }
}

﻿using Adjutant.Spatial;
using Reclaimer.Blam.Utilities;
using Reclaimer.Geometry;
using System.Numerics;

namespace Adjutant.Geometry
{
    public class GeometryModel : IGeometryModel
    {
        public Matrix4x4 CoordinateSystem { get; set; }

        public string Name { get; }
        public List<IGeometryNode> Nodes { get; }
        public List<IGeometryMarkerGroup> MarkerGroups { get; }
        public List<IGeometryRegion> Regions { get; }
        public List<IGeometryMaterial> Materials { get; }
        public List<IRealBounds5D> Bounds { get; }
        public List<IGeometryMesh> Meshes { get; }

        public GeometryModel(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Nodes = new List<IGeometryNode>();
            MarkerGroups = new List<IGeometryMarkerGroup>();
            Regions = new List<IGeometryRegion>();
            Materials = new List<IGeometryMaterial>();
            Bounds = new List<IRealBounds5D>();
            Meshes = new List<IGeometryMesh>();
            CoordinateSystem = Matrix4x4.Identity;
        }

        public override string ToString() => Name;

        #region IGeometryModel

        IReadOnlyList<IGeometryNode> IGeometryModel.Nodes => Nodes;

        IReadOnlyList<IGeometryMarkerGroup> IGeometryModel.MarkerGroups => MarkerGroups;

        IReadOnlyList<IGeometryRegion> IGeometryModel.Regions => Regions;

        IReadOnlyList<IGeometryMaterial> IGeometryModel.Materials => Materials;

        IReadOnlyList<IRealBounds5D> IGeometryModel.Bounds => Bounds;

        IReadOnlyList<IGeometryMesh> IGeometryModel.Meshes => Meshes;

        #endregion

        public void Dispose()
        {
            Nodes.Clear();
            MarkerGroups.Clear();
            Regions.Clear();
            Materials.Clear();
            Bounds.Clear();
            foreach (var mesh in Meshes)
                mesh.Dispose();
            Meshes.Clear();
        }
    }

    public class GeometryNode : IGeometryNode
    {
        public string Name { get; set; }
        public short ParentIndex { get; set; }
        public short FirstChildIndex { get; set; }
        public short NextSiblingIndex { get; set; }
        public IVector3 Position { get; set; }
        public IVector4 Rotation { get; set; }
        public Matrix4x4 OffsetTransform { get; set; }

        public override string ToString() => Name;
    }

    public class GeometryMarkerGroup : IGeometryMarkerGroup
    {
        public string Name { get; set; }
        public List<IGeometryMarker> Markers { get; set; }

        public GeometryMarkerGroup()
        {
            Markers = new List<IGeometryMarker>();
        }

        public override string ToString() => Name;

        #region IGeometryMarkerGroup

        IReadOnlyList<IGeometryMarker> IGeometryMarkerGroup.Markers => Markers;

        #endregion
    }

    public class GeometryRegion : IGeometryRegion
    {
        public int SourceIndex { get; set; }
        public string Name { get; set; }
        public List<IGeometryPermutation> Permutations { get; set; }

        public GeometryRegion()
        {
            Permutations = new List<IGeometryPermutation>();
        }

        public override string ToString() => Name;

        #region IGeometryRegion

        IReadOnlyList<IGeometryPermutation> IGeometryRegion.Permutations => Permutations;

        #endregion
    }

    public class GeometryPermutation : IGeometryPermutation
    {
        public int SourceIndex { get; set; }
        public string Name { get; set; }
        public int MeshIndex { get; set; }
        public int MeshCount { get; set; }

        public float TransformScale { get; set; }
        public Matrix4x4 Transform { get; set; }

        public GeometryPermutation()
        {
            TransformScale = 1;
            Transform = Matrix4x4.Identity;
        }

        public override string ToString() => Name;
    }

    public class GeometryMarker : IGeometryMarker
    {
        public byte RegionIndex { get; set; }
        public byte PermutationIndex { get; set; }
        public byte NodeIndex { get; set; }
        public IVector3 Position { get; set; }
        public IVector4 Rotation { get; set; }

        public override string ToString() => Position.ToString();
    }

    public class GeometryMesh : IGeometryMesh
    {
        public bool IsInstancing { get; set; }

        public VertexWeights VertexWeights => NodeIndex < byte.MaxValue || VertexBuffer.HasBlendIndices ? VertexBuffer.HasBlendWeights ? VertexWeights.Skinned : VertexWeights.Rigid : VertexWeights.None;
        public IndexFormat IndexFormat => IndexBuffer.Layout;

        public VertexBuffer VertexBuffer { get; set; }
        public IIndexBuffer IndexBuffer { get; set; }

        public byte? NodeIndex { get; set; }
        public short? BoundsIndex { get; set; }
        public List<IGeometrySubmesh> Submeshes { get; set; }

        public GeometryMesh()
        {
            Submeshes = new List<IGeometrySubmesh>();
        }

        #region IGeometryMesh

        IReadOnlyList<IGeometrySubmesh> IGeometryMesh.Submeshes => Submeshes;

        #endregion

        public void Dispose()
        {
            VertexBuffer = null;
            IndexBuffer = null;
            Submeshes.Clear();
        }
    }

    public class GeometrySubmesh : IGeometrySubmesh
    {
        public short MaterialIndex { get; set; }
        public int IndexStart { get; set; }
        public int IndexLength { get; set; }
    }

    public class GeometryMaterial : IGeometryMaterial
    {
        public string Name { get; set; }
        public MaterialFlags Flags { get; set; }
        public List<ISubmaterial> Submaterials { get; set; }
        public List<TintColour> TintColours { get; set; }

        public GeometryMaterial()
        {
            Submaterials = new List<ISubmaterial>();
            TintColours = new List<TintColour>();
        }

        public override string ToString() => Name;

        IReadOnlyList<ISubmaterial> IGeometryMaterial.Submaterials => Submaterials;
        IReadOnlyList<TintColour> IGeometryMaterial.TintColours => TintColours;
    }

    public class SubMaterial : ISubmaterial
    {
        public MaterialUsage Usage { get; set; }
        public IBitmap Bitmap { get; set; }
        public IVector2 Tiling { get; set; }

        public override string ToString() => $"[{Usage}] {Bitmap.Name}";
    }

    public class TintColour
    {
        public TintUsage Usage { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
    }
}

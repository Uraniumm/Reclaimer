﻿using Adjutant.Geometry;
using Adjutant.Spatial;
using Reclaimer.Blam.Common;
using Reclaimer.Blam.Utilities;
using Reclaimer.Geometry;
using Reclaimer.IO;
using System.Globalization;
using System.Numerics;

namespace Reclaimer.Blam.Halo3
{
    public class scenario_structure_bsp : ContentTagDefinition, IRenderGeometry
    {
        public scenario_structure_bsp(IIndexItem item)
            : base(item)
        { }

        [Offset(72, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(60, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(64, MinVersion = (int)CacheType.MccHalo3U4)]
        public RealBounds XBounds { get; set; }

        [Offset(80, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(68, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(72, MinVersion = (int)CacheType.MccHalo3U4)]
        public RealBounds YBounds { get; set; }

        [Offset(88, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(76, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(80, MinVersion = (int)CacheType.MccHalo3U4)]
        public RealBounds ZBounds { get; set; }

        [Offset(180, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(196, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(184, MinVersion = (int)CacheType.Halo3ODST)]
        public BlockCollection<ClusterBlock> Clusters { get; set; }

        [Offset(192, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(208, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(196, MinVersion = (int)CacheType.Halo3ODST)]
        public BlockCollection<ShaderBlock> Shaders { get; set; }

        [Offset(444, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(432, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(448, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(436, MinVersion = (int)CacheType.Halo3ODST)]
        public BlockCollection<BspGeometryInstanceBlock> GeometryInstances { get; set; }

        [Offset(592, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(580, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(596, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(584, MinVersion = (int)CacheType.Halo3ODST)]
        public ResourceIdentifier ResourcePointer1 { get; set; }

        [Offset(776, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(740, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(756, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(744, MinVersion = (int)CacheType.Halo3ODST)]
        public BlockCollection<SectionBlock> Sections { get; set; }

        [Offset(788, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(752, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(768, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(756, MinVersion = (int)CacheType.Halo3ODST)]
        public BlockCollection<BspBoundingBoxBlock> BoundingBoxes { get; set; }

        [Offset(896, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(860, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(876, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(864, MinVersion = (int)CacheType.Halo3ODST)]
        public ResourceIdentifier ResourcePointer2 { get; set; }

        [Offset(892, MaxVersion = (int)CacheType.MccHalo3U4)]
        [Offset(908, MinVersion = (int)CacheType.MccHalo3U4, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(896, MinVersion = (int)CacheType.Halo3ODST)]
        public ResourceIdentifier ResourcePointer3 { get; set; }

        #region IRenderGeometry

        int IRenderGeometry.LodCount => 1;

        public IGeometryModel ReadGeometry(int lod)
        {
            Exceptions.ThrowIfIndexOutOfRange(lod, ((IRenderGeometry)this).LodCount);

            var scenario = Cache.TagIndex.GetGlobalTag("scnr").ReadMetadata<scenario>();
            var model = new GeometryModel(Item.FileName) { CoordinateSystem = CoordinateSystem.Default };

            var bspBlock = scenario.StructureBsps.First(s => s.BspReference.TagId == Item.Id);
            var bspIndex = scenario.StructureBsps.IndexOf(bspBlock);

            var lightmap = scenario.ScenarioLightmapReference.Tag.ReadMetadata<scenario_lightmap>();
            var lightmapData = Cache.CacheType < CacheType.MccHalo3U4
                ? lightmap.LightmapData.First(lbsp => lbsp.BspIndex == bspIndex)
                : lightmap.LightmapRefs.Where(t => t.TagId >= 0)
                    .Select(lbsp => lbsp.Tag.ReadMetadata<scenario_lightmap_bsp_data>())
                    .FirstOrDefault(lbsp => lbsp.BspIndex == bspIndex)
                    ?? Cache.TagIndex.FirstOrDefault(t => t.ClassCode == "Lbsp" && t.TagName == Item.TagName)?.ReadMetadata<scenario_lightmap_bsp_data>();

            model.Bounds.AddRange(BoundingBoxes);
            model.Materials.AddRange(Halo3Common.GetMaterials(Shaders));

            var clusterRegion = new GeometryRegion { Name = BlamConstants.SbspClustersGroupName };
            clusterRegion.Permutations.AddRange(
                Clusters.Select((c, i) => new GeometryPermutation
                {
                    SourceIndex = i,
                    Name = Clusters.IndexOf(c).ToString("D3", CultureInfo.CurrentCulture),
                    MeshIndex = c.SectionIndex,
                    MeshCount = 1
                })
            );
            model.Regions.Add(clusterRegion);

            foreach (var instanceGroup in BlamUtils.GroupGeometryInstances(GeometryInstances, i => i.Name))
            {
                var sectionRegion = new GeometryRegion { Name = instanceGroup.Key };
                sectionRegion.Permutations.AddRange(
                    instanceGroup.Select(i => new GeometryPermutation
                    {
                        SourceIndex = GeometryInstances.IndexOf(i),
                        Name = i.Name,
                        Transform = i.Transform,
                        TransformScale = i.TransformScale,
                        MeshIndex = i.SectionIndex,
                        MeshCount = 1
                    })
                );

                model.Regions.Add(sectionRegion);
            }

            model.Meshes.AddRange(Halo3Common.GetMeshes(Cache, lightmapData.ResourcePointer, lightmapData.Sections, (s, m) =>
            {
                var index = (short)lightmapData.Sections.IndexOf(s);
                m.BoundsIndex = index >= BoundingBoxes.Count ? null : index;
                m.IsInstancing = index < BoundingBoxes.Count;
            }));

            return model;
        }

        public IEnumerable<IBitmap> GetAllBitmaps() => Halo3Common.GetBitmaps(Shaders);

        public IEnumerable<IBitmap> GetBitmaps(IEnumerable<int> shaderIndexes) => Halo3Common.GetBitmaps(Shaders, shaderIndexes);

        #endregion
    }
    [FixedSize(192, MaxVersion = (int)CacheType.Halo3Alpha)]
    [FixedSize(236, MaxVersion = (int)CacheType.Halo3Retail)]
    [FixedSize(220, MinVersion = (int)CacheType.Halo3Retail, MaxVersion = (int)CacheType.MccHalo3)]
    [FixedSize(280, MinVersion = (int)CacheType.MccHalo3, MaxVersion = (int)CacheType.Halo3ODST)]
    [FixedSize(220, MinVersion = (int)CacheType.Halo3ODST, MaxVersion = (int)CacheType.MccHalo3ODST)]
    [FixedSize(280, MinVersion = (int)CacheType.MccHalo3ODST)]
    public class ClusterBlock
    {
        [Offset(0)]
        public RealBounds XBounds { get; set; }

        [Offset(8)]
        public RealBounds YBounds { get; set; }

        [Offset(16)]
        public RealBounds ZBounds { get; set; }

        [Offset(164, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(172, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(156, MinVersion = (int)CacheType.Halo3Retail, MaxVersion = (int)CacheType.MccHalo3)]
        [Offset(216, MinVersion = (int)CacheType.MccHalo3, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(156, MinVersion = (int)CacheType.Halo3ODST, MaxVersion = (int)CacheType.MccHalo3ODST)]
        [Offset(216, MinVersion = (int)CacheType.MccHalo3ODST)]
        public short SectionIndex { get; set; }
    }

    [FixedSize(108, MaxVersion = (int)CacheType.Halo3Alpha)]
    [FixedSize(120, MinVersion = (int)CacheType.Halo3Alpha)]
    public class BspGeometryInstanceBlock
    {
        [Offset(0)]
        public float TransformScale { get; set; }

        [Offset(4)]
        public Matrix4x4 Transform { get; set; }

        [Offset(52)]
        public short SectionIndex { get; set; }

        [Offset(84)]
        public StringId Name { get; set; }

        public override string ToString() => Name;
    }

    [FixedSize(44)]
    public class BspBoundingBoxBlock : IRealBounds5D
    {
        [Offset(4)]
        public RealBounds XBounds { get; set; }

        [Offset(12)]
        public RealBounds YBounds { get; set; }

        [Offset(20)]
        public RealBounds ZBounds { get; set; }

        [Offset(28)]
        public RealBounds UBounds { get; set; }

        [Offset(36)]
        public RealBounds VBounds { get; set; }
    }
}

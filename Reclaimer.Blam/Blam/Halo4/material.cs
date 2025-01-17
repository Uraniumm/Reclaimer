﻿using Reclaimer.Blam.Common;
using Reclaimer.Geometry.Vectors;
using Reclaimer.IO;

namespace Reclaimer.Blam.Halo4
{
    public class material
    {
        [Offset(0)]
        public TagReference BaseShaderReference { get; set; }

        [Offset(28)]
        public BlockCollection<ShaderPropertiesBlock> ShaderProperties { get; set; }
    }

    [FixedSize(140)]
    public class ShaderPropertiesBlock
    {
        [Offset(0)]
        public BlockCollection<ShaderMapBlock> ShaderMaps { get; set; }

        [Offset(12)]
        public BlockCollection<RealVector4> TilingData { get; set; }
    }

    [FixedSize(20, MaxVersion = (int)CacheType.Halo4Retail)]
    [FixedSize(24, MinVersion = (int)CacheType.Halo4Retail)]
    public class ShaderMapBlock
    {
        [Offset(0)]
        public TagReference BitmapReference { get; set; }

        [Offset(19)]
        public byte TilingIndex { get; set; }
    }
}

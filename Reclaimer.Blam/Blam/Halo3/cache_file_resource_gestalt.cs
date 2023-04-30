﻿using Reclaimer.Blam.Common;
using Reclaimer.Blam.Utilities;
using Reclaimer.IO;

namespace Reclaimer.Blam.Halo3
{
    public class cache_file_resource_gestalt
    {
        [Offset(24, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(36, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(88, MinVersion = (int)CacheType.Halo3Retail, MaxVersion = (int)CacheType.MccHalo3)]
        [Offset(100, MinVersion = (int)CacheType.MccHalo3, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(88, MinVersion = (int)CacheType.Halo3ODST, MaxVersion = (int)CacheType.MccHalo3ODST)]
        [Offset(100, MinVersion = (int)CacheType.MccHalo3ODST)]
        public BlockCollection<ResourceEntryBlock> ResourceEntries { get; set; }

        [Offset(48, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(132, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(316, MinVersion = (int)CacheType.Halo3Retail, MaxVersion = (int)CacheType.MccHalo3)]
        [Offset(328, MinVersion = (int)CacheType.MccHalo3, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(316, MinVersion = (int)CacheType.Halo3ODST, MaxVersion = (int)CacheType.MccHalo3ODST)]
        [Offset(328, MinVersion = (int)CacheType.MccHalo3ODST)]
        public int FixupDataSize { get; set; }

        [Offset(60, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(144, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(328, MinVersion = (int)CacheType.Halo3Retail, MaxVersion = (int)CacheType.MccHalo3)]
        [Offset(340, MinVersion = (int)CacheType.MccHalo3, MaxVersion = (int)CacheType.Halo3ODST)]
        [Offset(328, MinVersion = (int)CacheType.Halo3ODST, MaxVersion = (int)CacheType.MccHalo3ODST)]
        [Offset(340, MinVersion = (int)CacheType.MccHalo3ODST)]
        public Pointer FixupDataPointer { get; set; }
    }

    [FixedSize(88, MaxVersion = (int)CacheType.Halo3Alpha)]
    [FixedSize(96, MaxVersion = (int)CacheType.Halo3Retail)]
    [FixedSize(64, MinVersion = (int)CacheType.Halo3Retail)]
    public class ResourceEntryBlock
    {
        [Offset(0)]
        public TagReference OwnerReference { get; set; }

        [Offset(18, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(16, MinVersion = (int)CacheType.Halo3Alpha)]
        // [Offset(16)]
        public ResourceIdentifier ResourcePointer { get; set; }

        [Offset(24, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(20, MinVersion = (int)CacheType.Halo3Retail)]
        public int FixupOffset { get; set; }

        [Offset(28, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(24, MinVersion = (int)CacheType.Halo3Retail)]
        public int FixupSize { get; set; }

        [Offset(32)]
        [MinVersion((int)CacheType.Halo3Retail)]
        public short LocationType { get; set; }

        [Offset(34)]
        [MinVersion((int)CacheType.Halo3Retail)]
        public short SegmentIndex { get; set; }

        [Offset(36)]
        [MaxVersion((int)CacheType.Halo3Retail)]
        public int CacheIndex { get; set; }

        [Offset(36, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(40, MinVersion = (int)CacheType.Halo3Alpha)]
        public int PrimaryOffset { get; set; }

        [Offset(40, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(44, MinVersion = (int)CacheType.Halo3Alpha)]
        public int PrimarySize { get; set; }

        [Offset(44, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(52, MinVersion = (int)CacheType.Halo3Alpha)]
        public int CacheIndex2 { get; set; }

        [Offset(48, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(56, MinVersion = (int)CacheType.Halo3Alpha)]
        public int SecondaryOffset { get; set; }

        [Offset(52, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(60, MinVersion = (int)CacheType.Halo3Alpha)]
        public int SecondarySize { get; set; }

        [Offset(64, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(72, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(40, MinVersion = (int)CacheType.Halo3Retail)]
        public BlockCollection<ResourceFixupBlock> ResourceFixups { get; set; }

        [Offset(76, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(84, MaxVersion = (int)CacheType.Halo3Retail)]
        [Offset(52, MinVersion = (int)CacheType.Halo3Retail)]
        public BlockCollection<ResourceDefinitionFixupBlock> ResourceDefinitionFixups { get; set; }
    }

    [FixedSize(8)]
    public class ResourceFixupBlock
    {
        [Offset(0)]
        public int Unknown { get; set; }

        [Offset(4)]
        public int Offset { get; set; } //mask 0x0FFFFFFF (4 bit ?, 28 bit offset)
    }

    [FixedSize(8)]
    public class ResourceDefinitionFixupBlock
    {
        [Offset(0)]
        public int Offset { get; set; }

        [Offset(4)]
        public int Unknown { get; set; }
    }
}

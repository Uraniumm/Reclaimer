﻿using Reclaimer.Blam.Common;
using Reclaimer.Blam.Utilities;
using Reclaimer.IO;
using System.Globalization;
using System.IO;

namespace Reclaimer.Blam.HaloReach
{
    public enum PageType
    {
        Auto,
        Primary,
        Secondary
    }

    public readonly record struct ResourceIdentifier
    {
        private readonly ICacheFile cache;

        public ResourceIdentifier(int identifier, ICacheFile cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Value = identifier;
        }

        public ResourceIdentifier(DependencyReader reader, ICacheFile cache)
        {
            ArgumentNullException.ThrowIfNull(reader);
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Value = reader.ReadInt32();
        }

        public int Value { get; } //actually two shorts
        public int ResourceIndex => Value & ushort.MaxValue;

        public byte[] ReadData(PageType mode) => ReadData(mode, int.MaxValue);

        public byte[] ReadData(PageType mode, int maxLength)
        {
            var resourceGestalt = cache.TagIndex.GetGlobalTag("zone").ReadMetadata<cache_file_resource_gestalt>();
            var resourceLayoutTable = cache.TagIndex.GetGlobalTag("play").ReadMetadata<cache_file_resource_layout_table>();

            var entry = resourceGestalt.ResourceEntries[ResourceIndex];

            if (entry.SegmentIndex < 0)
                throw new InvalidOperationException("Data not found");

            var segment = resourceLayoutTable.Segments[entry.SegmentIndex];
            var useSecondary = mode == PageType.Secondary || (mode == PageType.Auto && segment.SecondaryPageIndex >= 0);

            var pageIndex = useSecondary ? segment.SecondaryPageIndex : segment.PrimaryPageIndex;
            var segmentOffset = useSecondary ? segment.SecondaryPageOffset : segment.PrimaryPageOffset;

            if (pageIndex < 0 || segmentOffset < 0)
                throw new InvalidOperationException("Data not found");

            var page = resourceLayoutTable.Pages[pageIndex];
            if (mode == PageType.Auto && (page.DataOffset < 0 || page.CompressedSize == 0))
            {
                pageIndex = segment.PrimaryPageIndex;
                segmentOffset = segment.PrimaryPageOffset;
                page = resourceLayoutTable.Pages[pageIndex];
            }

            var targetFile = cache.FileName;
            if (page.CacheIndex >= 0)
            {
                var directory = Directory.GetParent(cache.FileName).FullName;
                var mapName = Utils.GetFileName(resourceLayoutTable.SharedCaches[page.CacheIndex].FileName);
                targetFile = Path.Combine(directory, mapName);
            }

            using (var fs = new FileStream(targetFile, FileMode.Open, FileAccess.Read))
            using (var reader = new EndianReader(fs, cache.ByteOrder))
            {
                switch (cache.CacheType)
                {
                    case CacheType.MccHaloReach:
                        reader.Seek(1208, SeekOrigin.Begin);
                        break;
                    case CacheType.MccHaloReachU3:
                    case CacheType.MccHaloReachU8:
                        reader.Seek(1200, SeekOrigin.Begin);
                        break;
                    case CacheType.MccHaloReachU10:
                        reader.Seek(1232, SeekOrigin.Begin);
                        break;
                    default:
                        reader.Seek(1136, SeekOrigin.Begin); //xbox
                        break;
                }

                var dataTableAddress = reader.ReadUInt32();
                reader.Seek(dataTableAddress + page.DataOffset, SeekOrigin.Begin);
                return ContentFactory.GetResourceData(reader, cache.Metadata.ResourceCodec, maxLength, segmentOffset, page.CompressedSize, page.DecompressedSize);
            }
        }

        public byte[] ReadSoundData()
        {
            var directory = Directory.GetParent(cache.FileName).FullName;
            var resourceGestalt = cache.TagIndex.GetGlobalTag("zone").ReadMetadata<cache_file_resource_gestalt>();
            var resourceLayoutTable = cache.TagIndex.GetGlobalTag("play").ReadMetadata<cache_file_resource_layout_table>();
            var entry = resourceGestalt.ResourceEntries[ResourceIndex];

            if (entry.SegmentIndex < 0)
                throw new InvalidOperationException("Data not found");

            var segment = resourceLayoutTable.Segments[entry.SegmentIndex];
            var size1 = resourceLayoutTable.SizeGroups[segment.PrimarySizeIndex];
            var size2 = resourceLayoutTable.SizeGroups[segment.SecondarySizeIndex];
            var page1 = resourceLayoutTable.Pages[segment.PrimaryPageIndex];
            var page2 = resourceLayoutTable.Pages[segment.SecondaryPageIndex];

            if (page1.CompressedSize != page1.DecompressedSize || page2.CompressedSize != page2.DecompressedSize)
                throw new NotSupportedException("Compressed sound data");

            if (size2.Sizes.Count > 1)
                throw new NotSupportedException("Segmented sound data");

            var output = new byte[size1.TotalSize + size2.TotalSize];
            if (page1.CompressedSize > 0 && size1.TotalSize > 0)
            {
                var temp = ReadSoundData(directory, resourceLayoutTable, page1, size1.TotalSize);
                Array.Copy(temp, segment.PrimaryPageOffset, output, 0, size1.TotalSize);
            }

            if (page2.CompressedSize > 0 && size2.TotalSize > 0)
            {
                var temp = ReadSoundData(directory, resourceLayoutTable, page2, size2.TotalSize);
                Array.Copy(temp, segment.SecondaryPageOffset, output, size1.TotalSize, size2.TotalSize);
            }

            return output;
        }

        private byte[] ReadSoundData(string directory, cache_file_resource_layout_table resourceLayoutTable, PageBlock page, int size)
        {
            var targetFile = cache.FileName;
            if (page.CacheIndex >= 0)
            {
                var mapName = Utils.GetFileName(resourceLayoutTable.SharedCaches[page.CacheIndex].FileName);
                targetFile = Path.Combine(directory, mapName);
            }

            using (var fs = new FileStream(targetFile, FileMode.Open, FileAccess.Read))
            using (var reader = new EndianReader(fs, cache.ByteOrder))
            {
                reader.Seek(cache.CacheType >= CacheType.MccHaloReach ? 1208 : 1136, SeekOrigin.Begin);
                var dataTableAddress = reader.ReadUInt32();

                reader.Seek(dataTableAddress + page.DataOffset, SeekOrigin.Begin);
                return reader.ReadBytes(Math.Max(page.CompressedSize, size));
            }
        }

        public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);
    }
}

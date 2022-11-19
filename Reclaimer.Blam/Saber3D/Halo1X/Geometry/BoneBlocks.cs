﻿using Adjutant.Spatial;
using Reclaimer.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reclaimer.Saber3D.Halo1X.Geometry
{
    [DataBlock(0xE802)]
    public class BoneListBlock : CollectionDataBlock
    {
        public int BoneCount { get; set; }
        public List<BoneBlock> Bones { get; } = new List<BoneBlock>();

        internal override void Read(EndianReader reader)
        {
            BoneCount = reader.ReadInt32();
            ReadChildren(reader, BoneCount * 2); //empty block after every bone
            PopulateChildrenOfType(Bones);
        }
    }

    [DataBlock(0xE902)]
    public class BoneBlock : CollectionDataBlock
    {
        public float Unknown { get; set; }
        public int UnknownAsInt { get; set; }

        public RealVector3D Position => GetUniqueChild<PositionBlock>().Value;
        public RealVector4D Rotation => GetUniqueChild<RotationBlock>().Value;
        public RealVector3D UnknownVector0xFC02 => GetUniqueChild<VectorBlock0xFC02>().Value;
        public float Scale => GetUniqueChild<ScaleBlock0x0A03>().Value;

        internal override void Read(EndianReader reader)
        {
            UnknownAsInt = reader.PeekInt32();
            Unknown = reader.ReadSingle();

            ReadChildren(reader);
        }
    }

    [DataBlock(0xFA02, ExpectedSize = 4 * 3)]
    public class PositionBlock : DataBlock
    {
        [Offset(0)]
        public RealVector3D Value { get; set; }
    }

    [DataBlock(0xFB02, ExpectedSize = 4 * 4)]
    public class RotationBlock : DataBlock
    {
        [Offset(0)]
        public RealVector4D Value { get; set; }
    }

    [DataBlock(0xFC02, ExpectedSize = 4 * 3)]
    public class VectorBlock0xFC02 : DataBlock
    {
        [Offset(0)]
        public RealVector3D Value { get; set; } //usually 1,1,1 (maybe actually scale?)
    }

    [DataBlock(0x0A03, ExpectedSize = 4)]
    public class ScaleBlock0x0A03 : DataBlock
    {
        [Offset(0)]
        public float Value { get; set; } //usually 1
    }
}
using Reclaimer.Blam.Common;
using Reclaimer.IO;

namespace Reclaimer.Blam.Halo3
{
    //in Halo3Beta and Halo3Retail this is embedded within the scenario_lightmap tag.
    //in Halo3ODST it was separated into its own tag type.
    //MCC Halo3 Flight 3 and onwards also uses the ODST method

    [FixedSize(436, MaxVersion = (int)CacheType.MccHalo3U4)]
    public class scenario_lightmap_bsp_data
    {
        [Offset(2)]
        public short BspIndex { get; set; }

        [Offset(116, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(308, MinVersion = (int)CacheType.Halo3Alpha)]
        public BlockCollection<SectionBlock> Sections { get; set; }

        [Offset(236, MaxVersion = (int)CacheType.Halo3Alpha)]
        [Offset(428, MinVersion = (int)CacheType.Halo3Alpha)]
        public ResourceIdentifier ResourcePointer { get; set; }
    }
}

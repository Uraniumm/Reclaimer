﻿using Adjutant.IO;
using Adjutant.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adjutant.Blam.Definitions
{
    public interface ICacheFile
    {
        string FileName { get; }
        string BuildString { get; }
        CacheType CacheType { get; }

        ITagIndex<IIndexItem> TagIndex { get; }
        IStringIndex StringIndex { get; }
    }
}

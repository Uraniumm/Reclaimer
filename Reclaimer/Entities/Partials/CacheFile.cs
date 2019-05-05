﻿using Adjutant.Blam.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adjutant.IO;
using Adjutant.Utilities;
using Reclaimer.Utils;

namespace Reclaimer.Entities
{
    public partial class CacheFile : ICacheFile
    {
        CacheType ICacheFile.Type => CacheType;

        IStringIndex<IStringItem> ICacheFile.StringIndex => StringIndex;

        ITagIndex<IIndexItem> ICacheFile.TagIndex => TagIndex;

        private IAddressTranslator tagAddressTranslator;
        internal IAddressTranslator TagAddressTranslator
        {
            get
            {
                if (tagAddressTranslator == null)
                    tagAddressTranslator = new StandardAddressTranslator(TagIndex.Magic);

                return tagAddressTranslator;
            }
        }

        public DependencyReader CreateReader(IAddressTranslator translator)
        {
            throw new NotImplementedException();
        }
    }
}
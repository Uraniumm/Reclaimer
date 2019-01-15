﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adjutant.Utilities
{
    public struct Pointer
    {
        private readonly int _value;
        private readonly IAddressTranslator translator;

        public Pointer(int value, IAddressTranslator translator)
        {
            if (translator == null)
                throw new ArgumentNullException(nameof(translator));

            this._value = value;
            this.translator = translator;
        }

        public int Value => _value;
        public int Address => translator?.GetAddress(_value) ?? default(int);

        public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);

        #region Equality Operators

        public static bool operator ==(Pointer pointer1, Pointer pointer2)
        {
            return pointer1._value == pointer2._value;
        }

        public static bool operator !=(Pointer pointer1, Pointer pointer2)
        {
            return !(pointer1 == pointer2);
        }

        public static bool Equals(Pointer pointer1, Pointer pointer2)
        {
            return pointer1._value.Equals(pointer2._value);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Pointer))
                return false;

            return Pointer.Equals(this, (Pointer)obj);
        }

        public bool Equals(Pointer value)
        {
            return Pointer.Equals(this, value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion

        public static implicit operator int(Pointer pointer)
        {
            return pointer.Address;
        }
    }
}

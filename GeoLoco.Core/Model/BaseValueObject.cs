using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoLoco.Core.Model
{
    public abstract class BaseValueObject
    {
        public static bool operator ==(BaseValueObject a, BaseValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseValueObject a, BaseValueObject b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            var baseValueObject = (BaseValueObject)obj;

            return GetAtomicValues().SequenceEqual(baseValueObject.GetAtomicValues());
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var value in GetAtomicValues())
            {
                hash.Add(value);
            }
            return hash.ToHashCode();
        }

        protected abstract IEnumerable<object> GetAtomicValues();
    }
}

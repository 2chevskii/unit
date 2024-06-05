using System;

namespace Dvchevskii.Unit
{
    public readonly struct Unit : IEquatable<object>, IEquatable<Unit>, IComparable<Unit>
    {
        private const string STRING_REPRESENTATION = "()";

        // ReSharper disable once UnassignedReadonlyField
        public static readonly Unit Default;

        #region Operators

        public static bool operator ==(Unit lhs, Unit rhs) => lhs.Equals(rhs);

        public static bool operator ==(Unit lhs, object rhs) => lhs.Equals(rhs);

        public static bool operator ==(object lhs, Unit rhs) => rhs.Equals(lhs);

        public static bool operator !=(Unit lhs, Unit rhs) => !lhs.Equals(rhs);

        public static bool operator !=(Unit lhs, object rhs) => !lhs.Equals(rhs);

        public static bool operator !=(object lhs, Unit rhs) => !rhs.Equals(lhs);

        public static bool operator >(Unit lhs, Unit rhs) => lhs.CompareTo(rhs).Equals(1);

        public static bool operator <(Unit lhs, Unit rhs) => lhs.CompareTo(rhs).Equals(-1);

        public static bool operator >=(Unit lhs, Unit rhs)
        {
            int cmp = lhs.CompareTo(rhs);
            switch (cmp)
            {
                case 1:
                case 0:
                    return true;
                default:
                    return false;
            }
        }

        public static bool operator <=(Unit lhs, Unit rhs)
        {
            int cmp = lhs.CompareTo(rhs);
            switch (cmp)
            {
                case -1:
                case 0:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        public override string ToString() => STRING_REPRESENTATION;

        public override bool Equals(object obj)
        {
            if (obj is Unit unit)
            {
                return Equals(unit);
            }

            return false;
        }

        public override int GetHashCode() => 804741551;

        public bool Equals(Unit other) => true;

        public int CompareTo(Unit other) => 0;
    }
}

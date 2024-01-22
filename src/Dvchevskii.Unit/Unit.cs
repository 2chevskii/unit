using System;

namespace Dvchevskii.Unit
{
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        private const string STRING_REPRESENTATION = "()";

        public static readonly Unit Default;

        public static bool operator ==(Unit lhs, Unit rhs) => lhs.Equals(rhs);

        public static bool operator !=(Unit lhs, Unit rhs) => !lhs.Equals(rhs);

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

        public override string ToString() => STRING_REPRESENTATION;

        public override bool Equals(object obj)
        {
            if (obj is Unit unit)
            {
                return Equals(unit);
            }

            return false;
        }

        public override int GetHashCode() => 804741542;

        public bool Equals(Unit other) => true;

        public int CompareTo(Unit other) => 0;
    }
}

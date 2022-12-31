using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Golfscript
{
    class ItemComparer : IComparer<Item>, IEqualityComparer<Item>
    {
        public int Compare(Item? x, Item? y)
        {
            if (ReferenceEquals(x, y) || x == null || y == null)
                return 0;

            if (x.Type != y.Type)
                return 0;

            if (x.Type == ItemType.Integer)
                return ((BigInteger)x.Value).CompareTo((BigInteger)y.Value);
            else if (x.Type == ItemType.String || x.Type == ItemType.Block)
                return ((string)x.Value).CompareTo((string)y.Value);

            return 0;
        }

        public bool Equals(Item? x, Item? y)
        {
            if (ReferenceEquals(x, y) || x == null || y == null)
                return true;

            if (x.Type != y.Type)
                return false;

            if (x.Type == ItemType.Integer)
                return ((BigInteger)x.Value).Equals((BigInteger)y.Value);
            else if (x.Type == ItemType.String || x.Type == ItemType.Block)
                return ((string)x.Value).Equals((string)y.Value);

            return ((List<Item>)x.Value).SequenceEqual((List<Item>)y.Value, this);
        }

        public int GetHashCode([DisallowNull] Item obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
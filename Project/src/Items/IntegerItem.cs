using System.Numerics;

namespace Golfscript
{
    public class IntegerItem : Item
    {
        public override ItemType Type => ItemType.Integer;
        public override object Value => RealValue;
        public override bool Truthy => RealValue != 0;
        public override long Size => throw new NotImplementedException();

        public char CharValue => (char)RealValue;
        public BigInteger RealValue { get; }

        public IntegerItem(BigInteger value)
        {
            RealValue = value;
        }

        public override Item Coerce(ItemType type)
        {
            switch (type)
            {
                case ItemType.Integer:
                    //return new IntegerItem(m_value);
                    return this;
                case ItemType.String:
                    return new StringItem(NativeString());
                case ItemType.Array:
                    return new ArrayItem(new IntegerItem(RealValue));
                case ItemType.Block:
                    return new BlockItem(NativeString());
            }

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            return RealValue.ToString();
        }

        public override string NativeString()
        {
            return RealValue.ToString();
        }
    }
}
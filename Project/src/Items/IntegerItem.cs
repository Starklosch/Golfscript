using System.Numerics;

namespace Golfscript
{
    public class IntegerItem : Item
    {
        public override ItemType Type => ItemType.Integer;
        public override object Value => _value;
        public override bool Truthy => _value != 0;
        public override long Size => throw new NotImplementedException();

        public char CharValue => (char)_value;
        
        BigInteger _value;

        public IntegerItem(BigInteger value)
        {
            _value = value;
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
                    return new ArrayItem(new IntegerItem(_value));
                case ItemType.Block:
                    return new BlockItem(NativeString());
            }

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            return _value.ToString();
        }

        public override string NativeString()
        {
            return _value.ToString();
        }
    }
}
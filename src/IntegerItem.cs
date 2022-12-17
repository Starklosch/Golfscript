namespace Golfscript
{
    class IntegerItem : Item
    {
        public override ItemType Type => ItemType.Integer;
        public override object Value => m_value;
        public override bool Truthy => m_value != 0;

        public IntegerItem(int value)
        {
            m_value = value;
        }

        public override Item Coerce(ItemType type)
        {
            switch (type)
            {
                case ItemType.Integer:
                    //return new IntegerItem(m_value);
                    return this;
                case ItemType.String:
                    return new StringItem(m_value.ToString());
                case ItemType.Array:
                    return new ArrayItem(new IntegerItem(m_value));
                case ItemType.Block:
                    return new BlockItem(new IntegerItem(m_value));
                //case ItemType.Operation:
                //    break;
            }

            return null;
        }

        public override void Add(Item other)
        {
            m_value += (int)other.Value;
        }

        public override void Subtract(Item other)
        {
            m_value -= (int)other.Value;
        }

        public override void Multiply(Item other)
        {
            m_value *= (int)other.Value;
        }

        public override void Divide(Item other)
        {
            m_value /= (int)other.Value;
        }

        int m_value;
    }
}
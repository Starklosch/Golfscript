namespace Golfscript
{
    class StringItem : Item
    {
        public override ItemType Type => ItemType.String;
        public override object Value => m_value;
        public override int Truthy => m_value.Length > 0 ? 1 : 0;

        public StringItem(string value)
        {
            m_value = value;
        }

        public override Item Coerce(ItemType type)
        {
            switch (type)
            {
                case ItemType.Integer:
                    if (int.TryParse(m_value, out int number))
                        return new IntegerItem(number);
                    return null;
                case ItemType.String:
                    return this;
                case ItemType.Array:
                    return new ArrayItem(new StringItem(m_value));
                case ItemType.Block:
                    return new BlockItem(new StringItem(m_value));
            }

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string Print()
        {
            return m_value;
        }

        public override string? ToString()
        {
            return '"' + m_value + '"';
        }

        string m_value;
    }
}
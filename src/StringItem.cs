using System.Text;

namespace Golfscript
{
    class StringItem : Item
    {
        public override ItemType Type => ItemType.String;
        public override object Value => m_value;
        public override bool Truthy => m_value.Length > 0;

        public StringItem(string value)
        {
            m_value = value;
        }

        public override void Add(Item other)
        {
            m_value += (string)other.Value;
        }

        public override void Subtract(Item other)
        {
            // TODO Implement subtraction
            throw new NotImplementedException();
        }

        public override void Multiply(Item other)
        {
            // TODO Implement multiplication
            throw new NotImplementedException();
        }

        public override void Divide(Item other)
        {
            // TODO Implement division
            throw new NotImplementedException();
        }

        public override void Evaluate(Stack context)
        {
            context.Golfscript.Run(m_value);
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
                    return new StringItem('\"' + m_value + '\"');
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

        static Dictionary<char, string> Escaped = new()
        {
            { '\\', @"\\" },
            { '\a', @"\a" },
            { '\b', @"\b" },
            { '\t', @"\t" },
            { '\n', @"\n" },
            { '\r', @"\r" },
            { '"', "\\\"" },
        };

        public override string? ToString()
        {
            StringBuilder sb = new();
            foreach (var ch in m_value)
            {   
                if (Escaped.TryGetValue(ch, out string escaped))
                    sb.Append(escaped);
                else
                    sb.Append(ch);
            }
            
            return '"' + sb.ToString() + '"';
        }

        string m_value;
    }
}
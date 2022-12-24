using System.Text;

namespace Golfscript
{
    public class StringItem : Item
    {
        public override ItemType Type => ItemType.String;
        public override object Value => _value;
        public override bool Truthy => _value.Length > 0;
        public override long Size => _value.Length;

        protected string _value;

        public StringItem(char[] value)
        {
            _value = new string(value);
        }

        public StringItem(string value)
        {
            _value = value;
        }

        public StringItem(StringBuilder value)
        {
            _value = value.ToString();
        }

        public override Item Coerce(ItemType type)
        {
            switch (type)
            {
                case ItemType.Integer:
                    if (int.TryParse(_value, out int number))
                        return new IntegerItem(number);
                    break;
                case ItemType.String:
                    return new StringItem('\"' + _value + '\"');
                case ItemType.Array:
                    return new ArrayItem(new StringItem(_value));
                case ItemType.Block:
                    return new BlockItem(_value);
            }

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            StringBuilder sb = new();
            foreach (var ch in _value)
            {
                if (Escaped.TryGetValue(ch, out string escaped))
                    sb.Append(escaped);
                else if (ch < ' ' || ch > '~')
                    sb.Append(ch < 10 ? "\\x0" : "\\x").Append((byte)ch);
                else
                    sb.Append(ch);
            }

            return '"' + sb.ToString() + '"';
        }

        public override string NativeString()
        {
            return _value;
        }

        static Dictionary<char, string> Escaped = new()
        {

            { '\\', @"\\" },
            { '\'', "'" },
            { '\"', "\\\"" },
            { '\a', @"\a" },
            { '\b', @"\b" },
            { '\t', @"\t" },
            { '\n', @"\n" },
            { '\v', @"\v" },
            { '\f', @"\f"},
            { '\r', @"\r" },
            { '\u001B', @"\e" },
        };
    }
}
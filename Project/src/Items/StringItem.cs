using System.Diagnostics;
using System.Text;

namespace Golfscript
{
    public class StringItem : Item
    {
        public override ItemType Type => ItemType.String;
        public override object Value => RealValue;
        public override bool Truthy => RealValue.Length > 0;
        public override long Size => RealValue.Length;

        public string RealValue { get; }

        public StringItem(string value)
        {
            RealValue = value;
        }

        public override Item Coerce(ItemType type)
        {
            switch (type)
            {
                case ItemType.Integer:
                    if (int.TryParse(RealValue, out int number))
                        return new IntegerItem(number);
                    break;
                case ItemType.String:
                    return new StringItem('\"' + RealValue + '\"');
                case ItemType.Array:
                    return new ArrayItem(new StringItem(RealValue));
                case ItemType.Block:
                    return new BlockItem(RealValue);
            }

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            StringBuilder sb = new();
            foreach (var ch in RealValue)
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
            return RealValue;
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
            { '\f', @"\a"},
            { '\r', @"\r" },
            { '\u001B', @"\e" },
        };
    }
}
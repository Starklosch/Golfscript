using System.Collections;
using System.Numerics;
using System.Text;

namespace Golfscript
{
    public class ArrayItem : Item
    {
        public override ItemType Type => ItemType.Array;
        public override object Value => RealValue;
        public override bool Truthy => RealValue.Count > 0;
        public override long Size => RealValue.Count;

        public IReadOnlyList<Item> RealValue { get; }

        // Value shown when printing in console
        public string StringValue
        {
            get
            {
                StringBuilder sb = new();
                foreach (var item in RealValue)
                {
                    if (item is IntegerItem integerItem)
                        sb.Append(integerItem.CharValue);
                    else if (item is ArrayItem arrayItem)
                        sb.Append(arrayItem.StringValue);
                    else
                        sb.Append(item.NativeString());
                }
                return sb.ToString();
            }
        }

        public ArrayItem(IEnumerable<Item> values)
        {
            RealValue = values.ToList().AsReadOnly();
        }

        public ArrayItem(params Item[] values)
        {
            RealValue = values.ToList().AsReadOnly();
        }

        public override Item Coerce(ItemType type)
        {
            if (type == ItemType.Array)
                return this;
            // ["abc " [37 35] [33 31]]''*
            if (type == ItemType.String)
            {
                return new StringItem(StringValue);
                //StringBuilder sb = new();
                //foreach (var item in RealValue)
                //{
                //    if (item is IntegerItem integerItem)
                //        sb.Append(integerItem.CharValue);
                //    else if (item is ArrayItem arrayItem)
                //        sb.Append(arrayItem.Coerce(ItemType.String).NativeString());
                //    else
                //        sb.Append(item.NativeString());
                //}
                //return new StringItem(sb.ToString());
            }

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            if (RealValue.Count <= 0)
                return "[]";

            if (RealValue.Count == 1)
                return $"[{RealValue.First().StackString()}]";

            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var item in RealValue)
                sb.Append(item.StackString()).Append(' ');

            sb.Length--;
            sb.Append("]");

            return sb.ToString();
        }

        public override string NativeString()
        {
            if (RealValue.Count <= 0)
                return "";

            var sb = new StringBuilder();

            foreach (var item in RealValue)
                sb.Append(item.StackString());

            return sb.ToString();
        }
    }
}
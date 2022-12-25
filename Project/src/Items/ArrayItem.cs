using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Golfscript
{
    public class ArrayItem : Item, IResignable<List<Item>>
    {
        public override ItemType Type => ItemType.Array;
        public override object Value => _value;
        public override bool Truthy => _value.Count > 0;
        public override long Size => _value.Count;

        List<Item> _value;

        // Value shown when printing in console
        public string StringValue
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in _value)
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

        public ArrayItem(IResignable<List<Item>> movable)
        {
            _value = movable.Resign();
        }

        public ArrayItem(List<Item> list, bool takeList = true)
        {
            _value = takeList ? list : list.ToList();
        }

        public ArrayItem(IEnumerable<Item> enumerable)
        {
            _value = enumerable.ToList();
        }

        public ArrayItem(params Item[] values)
        {
            _value = values.ToList();
        }

        public override Item Coerce(ItemType type)
        {
            if (type == ItemType.Array)
                return this;

            if (type == ItemType.String)
                return new StringItem(StringValue);

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            if (_value.Count <= 0)
                return "[]";

            if (_value.Count == 1)
                return $"[{_value.First().StackString()}]";

            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var item in _value)
                sb.Append(item.StackString()).Append(' ');

            sb.Length--;
            sb.Append("]");

            return sb.ToString();
        }

        public override string NativeString()
        {
            if (_value.Count <= 0)
                return "";

            var sb = new StringBuilder();

            foreach (var item in _value)
                sb.Append(item.StackString());

            return sb.ToString();
        }

        public List<Item> Resign()
        {
            var temp = _value;
            _value = new List<Item>();
            return temp;
        }
    }
}
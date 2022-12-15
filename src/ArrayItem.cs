using System.Text;

namespace Golfscript
{
    class ArrayItem : Item
    {
        public override ItemType Type => ItemType.Array;
        public override object Value => m_values;
        public override int Truthy => m_values.Count > 0 ? 1 : 0;

        public ArrayItem(IEnumerable<Item> values)
        {
            m_values = new List<Item>(values);
        }

        public ArrayItem(params Item[] values)
        {
            m_values = new List<Item>(values);
        }

        public override Item? Coerce(ItemType type)
        {
            if (type == ItemType.Array)
                return this;

            return null;
        }

        public override string? ToString()
        {
            if (m_values.Count <= 0)
                return "[ ]";

            if (m_values.Count == 1)
                return $"[ {m_values[0]} ]";

            var sb = new StringBuilder();
            sb.Append("[ ");

            foreach (var item in m_values)
                sb.Append(item).Append(' ');

            sb.Length--;
            sb.Append(" ]");

            return sb.ToString();
        }

        protected List<Item> m_values;
    }
}
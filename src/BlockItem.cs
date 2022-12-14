namespace Golfscript
{
    class BlockItem : ArrayItem
    {
        public override ItemType Type => ItemType.Block;

        public BlockItem(IEnumerable<Item> values) : base(values)
        {
        }

        public BlockItem(params Item[] values) : base(values)
        {
        }

        public override void Evaluate(Stack stack)
        {
            foreach (var item in m_values)
            {
                stack.Push(this);
            }
        }

        public override Item? Coerce(ItemType type)
        {
            if (type == ItemType.Block)
                return this;

            return null;
        }
    }
}
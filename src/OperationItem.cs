namespace Golfscript
{
    class OperationItem : Item
    {
        public override ItemType Type => ItemType.Operation;
        public override object Value => m_value;

        public OperationItem(Action<Stack> value)
        {
            m_value = value;
        }

        public override void Evaluate(Stack stack)
        {
            m_value(stack);
        }

        public override Item? Coerce(ItemType type)
        {
            if (type == ItemType.Operation)
                return this;

            return null;
        }

        Action<Stack> m_value;
    }
}
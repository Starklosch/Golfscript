namespace Golfscript
{
    class OperationItem : Item
    {
        public override ItemType Type => ItemType.Operation;
        public override object Value => m_value;
        public bool DelayedExecution => m_delayedExecution;

        public override int Truthy => throw new NotImplementedException();

        public OperationItem(Golfscript.Action value, bool delayedExecution = true)
        {
            m_value = value;
            m_delayedExecution = delayedExecution;
        }

        public override void Evaluate(Stack context)
        {
            m_value(context);
        }

        public override Item? Coerce(ItemType type)
        {
            if (type == ItemType.Operation)
                return this;

            return null;
        }

        Golfscript.Action m_value;
        bool m_delayedExecution;
    }
}
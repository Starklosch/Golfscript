namespace Golfscript
{
    class OperationItem : Item
    {
        public override ItemType Type => ItemType.Operation;
        public override object Value => m_value;
        public bool DelayedExecution => m_delayedExecution;

        public override bool Truthy => throw new NotImplementedException();

        public OperationItem(Golfscript.Action value, bool delayedExecution = true)
        {
            m_value = value;
            m_delayedExecution = delayedExecution;
        }

        public override void Evaluate(Stack context)
        {
            m_value(context);
        }

        public override Item Coerce(ItemType type)
        {
            if (type == ItemType.Operation)
                return this;

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override void Add(Item other)
        {
            // TODO Implement addition
            throw new NotImplementedException();
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

        Golfscript.Action m_value;
        bool m_delayedExecution;
    }
}
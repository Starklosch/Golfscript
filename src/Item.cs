namespace Golfscript
{
    abstract class Item
    {
        public abstract ItemType Type { get; }
        public abstract object Value { get; }
        public abstract bool Truthy { get; }

        public bool IsBlock => Type == ItemType.Block;
        public bool IsOperation => Type == ItemType.Operation;

        #region Operations

        public abstract void Add(Item other);
        public abstract void Subtract(Item other);
        public abstract void Multiply(Item other);
        public abstract void Divide(Item other);

        public virtual void Evaluate(Stack context)
        {
            throw new NotImplementedException();
        }

        public virtual void Sort()
        {
            throw new NotImplementedException();
        }

        public abstract Item Coerce(ItemType type);

        public virtual string Print()
        {
            return ToString() ?? "";
        }

        #endregion


        #region .NET Members

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj == null)
                return false;

            return obj is Item item && item.Type == Type && Value == item.Value;
        }

        public override string? ToString()
        {
            return Value.ToString();
        }

        #endregion
    }
}
namespace Golfscript
{
    abstract class Item
    {
        public virtual ItemType Type { get; }
        public virtual object? Value { get; }

        public bool IsBlock => Type == ItemType.Block;
        public bool IsOperation => Type == ItemType.Operation;

        public virtual void Evaluate(Golfscript stack)
        {
            throw new NotImplementedException();
        }

        public abstract Item? Coerce(ItemType type);

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
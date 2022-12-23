namespace Golfscript
{
    public abstract class Item : IEquatable<Item>
    {
        public abstract ItemType Type { get; }
        public abstract object Value { get; }
        public abstract bool Truthy { get; }
        public abstract long Size { get; }

        #region Operations

        public abstract Item Coerce(ItemType type);

        // Value showed when printing the stack
        public abstract string StackString();

        // Value showed when printing to the console
        public abstract string NativeString();

        #endregion

        #region .NET Members
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }

        public override bool Equals(object? other)
        {
            if (other is Item item)
                return Equals(item);

            return false;
        }

        public bool Equals(Item? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other == null)
                return false;

            return other.Type == Type && Value.Equals(other.Value);
        }

        public override string? ToString()
        {
            return $"{Type}: {Value}";
        }

        #endregion
    }
}
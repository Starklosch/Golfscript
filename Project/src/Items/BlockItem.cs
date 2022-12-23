namespace Golfscript
{
    public class BlockItem : StringItem
    {
        public override ItemType Type => ItemType.Block;

        public BlockItem(string code) : base(code)
        {
        }

        public override Item Coerce(ItemType type)
        {
            if (type == ItemType.Block)
                return this;

            throw new InvalidOperationException("Can't coerce to " + type);
        }

        public override string StackString()
        {
            return $"{{{RealValue}}}";
        }

        public override string NativeString()
        {
            return RealValue;
        }
    }
}
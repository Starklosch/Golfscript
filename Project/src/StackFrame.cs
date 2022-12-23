using System.Text;

namespace Golfscript
{
    public class StackFrame
    {
        List<Item> items;

        public int Size => items.Count;
        public IReadOnlyCollection<Item> Items => items.AsReadOnly();

        public Item this[int index]
        {
            get => Peek(index);
        }

        public StackFrame()
        {
            items = new List<Item>();
        }

        public StackFrame(int capacity)
        {
            items = new List<Item>(capacity);
        }

        public StackFrame(IEnumerable<Item> collection)
        {
            items = new List<Item>(collection);
        }

        public void Push(Item item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Removes and returns the ith element from top of the stack.
        /// </summary>
        /// <param name="index">0-based index from top of the stack.</param>
        /// <returns>The element at <paramref name="index"/></returns>
        public Item Pop(int index = 0)
        {
            if (items.Count <= 0)
                throw new InvalidOperationException("Empty stack");

            var normalIndex = items.Count - 1 - index;
            var item = items[normalIndex];
            items.RemoveAt(normalIndex);
            return item;
        }

        /// <summary>
        /// Returns the ith element from top of the stack.
        /// </summary>
        /// <param name="index">0-based index from top of the stack.</param>
        /// <returns>The element at <paramref name="index"/></returns>
        public Item Peek(int index = 0)
        {
            var normalIndex = items.Count - 1 - index;
            return items[normalIndex];
        }

        public override string ToString()
        {
            if (items.Count == 0)
                return "";

            StringBuilder sb = new();
            foreach (var item in items)
                sb.Append(item.StackString()).Append(' ');

            sb.Length--;
            return sb.ToString();
        }
    }
}
using System.Text;

namespace Golfscript
{
    //class Operation
    //{
    //    public virtual void Perform(Stack stack)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //class SumOperation : Operation
    //{
    //    public override void Perform(Stack stack)
    //    {
    //        Item first = stack.Pop();
    //        Item second = stack.Pop();

    //        if (first == null || second == null)
    //        {
    //            Console.WriteLine(":v");
    //            return;
    //        }

    //        if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
    //        {
    //            var value = (int)first.Value + (int)second.Value;
    //            stack.Push(new IntegerItem(value));
    //        }
    //    }
    //}

    class Stack
    {
        List<Item> items = new List<Item>();

        public void Push(Item item)
        {
            if (item.IsOperation)
            {
                var block = item as OperationItem;
                block.Evaluate(this);
                return;
            }

            items.Add(item);
        }

        public Item? Pop()
        {
            if (items.Count <= 0)
            {
                Console.WriteLine("Empty stack!");
                return null;
            }

            return RemoveAt(0);
        }

        public Item? Peek() => Peek(0);

        /// <summary>
        /// Returns the ith element from top of the stack.
        /// </summary>
        /// <param name="index">0-based index from top of the stack.</param>
        /// <returns>The element at <paramref name="index"/></returns>
        public Item? Peek(int index)
        {
            var normalIndex = items.Count - 1 - index;
            return items[index];
        }

        /// <summary>
        /// Removes and returns the ith element from top of the stack.
        /// </summary>
        /// <param name="index">0-based index from top of the stack.</param>
        /// <returns>The element at <paramref name="index"/></returns>
        public Item? RemoveAt(int index)
        {
            var normalIndex = items.Count - 1 - index;
            var item = items[normalIndex];
            items.RemoveAt(normalIndex);
            return item;
        }

        public override string? ToString()
        {
            if (items.Count <= 0)
                return "[ ]";

            if (items.Count == 1)
                return $"[ {Peek()} ]";

            var sb = new StringBuilder();
            sb.Append("[ ");
            
            foreach (var item in items)
                sb.Append(item).Append(' ');

            sb.Length--;
            sb.Append(" ]");

            return sb.ToString();
        }
    }
}
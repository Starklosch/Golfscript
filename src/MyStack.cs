using System.Collections;

namespace Golfscript
{
    class MyStack<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        List<T> items;

        public int Count => items.Count;

        public T this[int index] {
            get => Peek(index);
        }

        public MyStack()
        {
            items = new List<T>();
        }

        public MyStack(int capacity)
        {
            items = new List<T>(capacity);
        }

        public MyStack(IEnumerable<T> collection) {
            items = new List<T>(collection);
        }

        public void Push(T item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Removes and returns the ith element from top of the stack.
        /// </summary>
        /// <param name="index">0-based index from top of the stack.</param>
        /// <returns>The element at <paramref name="index"/></returns>
        public T Pop(int index = 0)
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
        public T Peek(int index = 0)
        {
            var normalIndex = items.Count - 1 - index;
            return items[index];
        }

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public void Clear() => items.Clear();

        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);
    }
}
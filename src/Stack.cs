using System.Text;

namespace Golfscript
{
    class Stack
    {
        Golfscript m_golfscript;
        List<StackFrame> stackFrames = new();

        public Stack()
        {
            m_golfscript = new Golfscript();
            PushFrame();
        }

        public Stack(Golfscript golfscript)
        {
            m_golfscript = golfscript;
            PushFrame();
        }

        public void PushFrame()
        {
            stackFrames.Add(new StackFrame());
        }

        public StackFrame PopFrame()
        {
            var frame = stackFrames[stackFrames.Count - 1];
            stackFrames.RemoveAt(stackFrames.Count - 1);

            if (stackFrames.Count == 0)
                PushFrame();

            return frame;
        }

        public void Push(Item item)
        {
            if (item is OperationItem operation)
            {
                operation.Evaluate(this);
                return;
            }

            Frame.Push(item);
        }

        //public void PushAll(IEnumerable<Item> items)
        //{
        //    foreach (var item in items)
        //        Push(item);
        //}

        public Item Pop(int index = 0)
        {
            var frame = FindFrame(ref index);
            return frame.Pop(index);
        }

        public Item Peek(int index = 0)
        {
            var frame = FindFrame(ref index);
            return frame.Peek(index);
        }

        StackFrame? FindFrame(ref int index)
        {
            foreach (var frame in stackFrames)
            {
                if (frame.Size > index)
                    return frame;

                index -= frame.Size;
            }

            return null;
        }

        public override string? ToString()
        {
            if (Size == 0)
                return "[]";

            if (Size == 1)
                return $"[{Peek()}]";

            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var frame in stackFrames)
                if (frame.Size > 0)
                    sb.Append(frame.ToString()).Append(' ');

            sb.Length--;
            sb.Append("]");

            return sb.ToString();
        }


        #region Properties

        public int Size
        {
            get
            {
                int size = 0;
                foreach (var frame in stackFrames)
                    size += frame.Size;

                return size;
            }
        }
        public int Frames => stackFrames.Count;
        public Golfscript Golfscript => m_golfscript;
        //public Golfscript Runner => m_golfscript;

        StackFrame Frame => stackFrames.Last();

        #endregion
    }
}
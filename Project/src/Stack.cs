using System.Text;

namespace Golfscript
{
    public class Stack
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

        public void Clear()
        {
            stackFrames.Clear();
            PushFrame();
        }

        public void PushFrame()
        {
            stackFrames.Add(new StackFrame());
        }

        public StackFrame? PopFrame(bool pushArray = false)
        {
            var frame = stackFrames[stackFrames.Count - 1];
            stackFrames.RemoveAt(stackFrames.Count - 1);

            if (stackFrames.Count == 0)
                PushFrame();

            if (pushArray)
            {
                Push(new ArrayItem(frame));
                return null;
            }

            return frame;
        }

        public void Push(Item item)
        {
            Frame.Push(item);
        }

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
                return $"[{Peek().StackString()}]";

            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var frame in stackFrames)
                if (frame.Size > 0)
                    sb.Append(frame.ToString()).Append(' ');

            sb.Length--;
            sb.Append("]");

            return sb.ToString();
        }

        public string? TestString()
        {
            if (Size == 0)
                return "";

            if (Size == 1)
                return $"{Peek().StackString()}";

            var sb = new StringBuilder();

            foreach (var frame in stackFrames)
                if (frame.Size > 0)
                    sb.Append(frame.ToString()).Append(' ');

            sb.Length--;
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
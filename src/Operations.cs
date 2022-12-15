using System.Linq;

namespace Golfscript
{
    static class Operations
    {

        static Dictionary<string, Golfscript.Action> m_operations = new()
        {
            { ";", Pop },
            { "~", Evaluate },

            { "+", Addition },
            { "-", Subtraction },
            { "*", Multiplication },
            { "/", Division },

            { "!", Negate },

            { "print", Print },
        };

        public static void Negate(Stack context)
        {
            var item = context.Pop();
            context.Push(new IntegerItem(item.Truthy));
        }

        public static Dictionary<string, Golfscript.Action> All { get => m_operations; }

        //public static void BinaryOperation(Stack stack, Func<Item, Item, Item> action)
        //{
        //    Item first = stack.Pop();
        //    Item second = stack.Pop();

        //    if (first == null || second == null)
        //    {
        //        Console.WriteLine(":v");
        //        return;
        //    }

        //    if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
        //    {
        //        var newItem = action(first, second);
        //        stack.Push(newItem);
        //    }
        //}

        //public static void Addition(Stack stack)
        //{
        //    BinaryOperation(stack, (a, b) => a.Add(b)));
        //}

        public static void Print(Stack context)
        {
            var item = context.Pop();
            Console.WriteLine(item);
        }

        public static void Pop(Stack context)
        {
            context.Pop();
        }

        public static void Evaluate(Stack context)
        {
            Item item = context.Pop();
            if (item != null)
                item.Evaluate(context);
        }

        public static void Addition(Stack context)
        {
            Item second = context.Peek();
            Item first = context.Pop();

            if (first == null || second == null)
            {
                Console.WriteLine(":v");
                return;
            }

            Coerce(ref second, ref first);

            if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
            {
                var value = (int)first.Value! + (int)second.Value!;
                context.Push(new IntegerItem(value));
                return;
            }


            if (first.Type == ItemType.String && second.Type == ItemType.String)
            {
                var value = (string)first.Value! + (string)second.Value!;
                context.Push(new StringItem(value));
                return;
            }

            Console.WriteLine("Incompatible operation");
        }

        public static void Subtraction(Stack context)
        {
            Item second = context.Pop();
            Item first = context.Pop();

            if (first == null || second == null)
            {
                Console.WriteLine(":v");
                return;
            }

            Coerce(ref second, ref first);

            if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
            {
                var value = (int)first.Value - (int)second.Value;
                context.Push(new IntegerItem(value));
                return;
            }

            Console.WriteLine("Incompatible operation");
        }

        public static void Multiplication(Stack stack)
        {
            Item second = stack.Pop();
            Item first = stack.Pop();

            if (first == null || second == null)
            {
                Console.WriteLine(":v");
                return;
            }

            if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
            {
                var value = (int)first.Value * (int)second.Value;
                stack.Push(new IntegerItem(value));
                return;
            }

            Console.WriteLine("Incompatible operation");
        }

        public static void Division(Stack stack)
        {
            Item second = stack.Pop();
            Item first = stack.Pop();

            if (first == null || second == null)
            {
                Console.WriteLine(":v");
                return;
            }

            if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
            {
                var value = (int)first.Value / (int)second.Value;
                stack.Push(new IntegerItem(value));
                return;
            }

            Console.WriteLine("Incompatible operation");
        }


        static void Coerce(ref Item second, ref Item first)
        {
            if (first.Type < second.Type)
                first = first.Coerce(second.Type);
            else if (first.Type > second.Type)
                second = second.Coerce(first.Type);
        }
    }
}
using System.Linq;

namespace Golfscript
{
    static class Operations
    {
        public static Dictionary<string, Golfscript.Action> All { get => m_operations; }

        static Dictionary<string, Golfscript.Action> m_operations = new()
        {
            { ";", Pop },
            { "$", Peek },
            { "@", Rotate },
            { "~", Evaluate },
            { "`", Inspect },

            { "+", Addition },
            { "-", Subtraction },
            { "*", Multiplication },
            { "/", Division },

            { "!", Negate },

            { "print", Print },
        };

        static void Inspect(Stack context)
        {
            var first = context.Pop();
            context.Push(first.Coerce(ItemType.String));
        }

        static void Rotate(Stack context)
        {
            if (context.Size < 3)
                return;

            var third = context.Pop(2);
            context.Push(third);
        }

        static void Peek(Stack context)
        {
            if (context.Size == 0)
                return;

            var item = context.Pop();

            if (item is IntegerItem integer)
            {
                var position = (int)integer.Value;
                context.Push(context.Peek(position));
            }
            else if (item is ArrayItem array)
                array.Sort();
        }

        static void Negate(Stack context)
        {
            if (context.Size == 0)
                return;

            var item = context.Pop();
            context.Push(new IntegerItem(item.Truthy ? 0 : 1));
        }

        static void BinaryOperation(Stack context, Action<Item, Item> action)
        {
            if (context.Size < 2)
                return;

            Item? first = null, second = null;

            var type = Coerce(context, ref first, ref second);

            action(first, second);
        }

        static void Print(Stack context)
        {
            if (context.Size == 0)
                return;
            var item = context.Pop();
            Console.WriteLine(item.Print());
        }

        static void Pop(Stack context)
        {
            if (context.Size == 0)
            {
                Console.WriteLine("Empty stack");
                return;
            }

            context.Pop();
        }

        static void Evaluate(Stack context)
        {
            if (context.Size == 0)
                return;

            Item item = context.Pop();
            item.Evaluate(context);
        }

        //static void Addition(Stack context)
        //{
        //    if (context.Size < 2)
        //        return;

        //    Item second = context.Pop();
        //    Item first = context.Pop();

        //    Coerce(ref second, ref first);

        //    if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
        //    {
        //        var value = (int)first.Value! + (int)second.Value!;
        //        context.Push(new IntegerItem(value));
        //        return;
        //    }


        //    if (first.Type == ItemType.String && second.Type == ItemType.String)
        //    {
        //        var value = (string)first.Value! + (string)second.Value!;
        //        context.Push(new StringItem(value));
        //        return;
        //    }

        //    Console.WriteLine("Incompatible operation");
        //}

        static void Addition(Stack context)
        {
            BinaryOperation(context, (a, b) => a.Add(b));
        }

        static void Subtraction(Stack context)
        {
            BinaryOperation(context, (a, b) => a.Subtract(b));
        }

        static void Multiplication(Stack context)
        {
            BinaryOperation(context, (a, b) => a.Multiply(b));
        }

        static void Division(Stack context)
        {
            BinaryOperation(context, (a, b) => a.Divide(b));
        }

        static ItemType Coerce(Stack context, ref Item? first, ref Item? second)
        {
            first = context.Peek(1);
            second = context.Peek();

            if (first.Type < second.Type)
            {
                var aux = second;
                second = first.Coerce(second.Type);
                first = aux;
                context.Pop(1);

                return second.Type;
            }

            if (first.Type > second.Type)
            {
                second = second.Coerce(first.Type);
                context.Pop(0);

                return first.Type;
            }

            return first.Type;
        }
    }
}
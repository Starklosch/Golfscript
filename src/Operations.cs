using System.Linq;

namespace Golfscript
{
    static class Operations
    {

        static Dictionary<string, Action<Stack>> m_operations = new Dictionary<string, Action<Stack>>()
        {
            { ";", Pop },
            { "~", Evaluate },
            { "+", Addition },
            { "-", Subtraction },
            { "*", Multiplication },
            { "/", Division },
        };

        static Dictionary<string, Action<Golfscript>> m_operations2 = new()
        {
            { ";", Pop },
            { "~", Evaluate },
            { "+", Addition },
            { "-", Subtraction },
            { "*", Multiplication },
            { "/", Division },
        };

        public static Dictionary<string, Action<Stack>> All { get => m_operations; }

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

        public static void Pop(Golfscript golfscript)
        {
            golfscript.Stack.Pop();
        }

        public static void Evaluate(Golfscript golfscript)
        {
            Item item = golfscript.Stack.Pop();
            if (item != null)
                item.Evaluate(golfscript);
        }

        public static void Addition(Golfscript golfscript)
        {
            Item second = golfscript.Stack.Pop();
            Item first = golfscript.Stack.Pop();

            if (first == null || second == null)
            {
                Console.WriteLine(":v");
                return;
            }

            if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
            {
                var value = (int)first.Value + (int)second.Value;
                golfscript.Stack.Push(new IntegerItem(value));
                return;
            }


            if (first.Type == ItemType.Integer && second.Type == ItemType.String)
            {
                var value = (int)first.Value + (string)second.Value;
                golfscript.Stack.Push(new StringItem(value));
                return;
            }

            if (first.Type == ItemType.String && second.Type == ItemType.Integer)
            {
                var value = (string)first.Value + (int)second.Value;
                golfscript.Stack.Push(new StringItem(value));
                return;
            }

            Console.WriteLine("Incompatible operation");
        }

        public static void Subtraction(Golfscript golfscript)
        {
            Item second = golfscript.Stack.Pop();
            Item first = golfscript.Stack.Pop();

            if (first == null || second == null)
            {
                Console.WriteLine(":v");
                return;
            }

            if (first.Type == ItemType.Integer && second.Type == ItemType.Integer)
            {
                var value = (int)first.Value - (int)second.Value;
                golfscript.Stack.Push(new IntegerItem(value));
                return;
            }

            Console.WriteLine("Incompatible operation");
        }

        public static void Multiplication(Golfscript stack)
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

        public static void Division(Golfscript stack)
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
    }

    static class OperationManager
    {
        // Operator
        static Dictionary<string, List<Overload>> operations = new Dictionary<string, List<Overload>>()
        {
            { "+", new List<Overload>() {
                new Overload(Operations.Addition, ItemType.Integer, ItemType.Integer),
                new Overload(Operations.Addition, ItemType.Array, ItemType.Array),
                new Overload(Operations.Addition, ItemType.String, ItemType.String),
                new Overload(Operations.Addition, ItemType.Block, ItemType.Block)
            } }
        };

        static public bool HasOperator(string op) => operations.ContainsKey(op);

        static public Overload? FindOverload(string op, params ItemType[] argumentTypes)
        {
            if (!operations.ContainsKey(op))
                return null;

            var overloads = operations[op];
            foreach (var overload in overloads)
            {
                if (overload.IsCandidate(argumentTypes))
                    return overload;
            }
            return null;
        }
    }

    struct Overload
    {
        List<ItemType> m_argumentTypes;
        Action<Stack> m_function;

        public List<ItemType> ArgumentTypes { get => m_argumentTypes; }
        public Action<Stack> Function { get => m_function; }

        public Overload(Action<Stack> function, params ItemType[] argumentTypes)
        {
            m_argumentTypes = new List<ItemType>(argumentTypes);
            m_function = function;
        }
        
        public Overload(Action<Stack> function, IEnumerable<ItemType> argumentTypes)
        {
            m_argumentTypes = new List<ItemType>(argumentTypes);
            m_function = function;
        }

        public bool IsCandidate(params ItemType[] argumentTypes)
        {
            if (m_argumentTypes.Count < argumentTypes.Length)
                return false;

            for (int i = 0; i < argumentTypes.Length; i++)
                if (argumentTypes[i] != m_argumentTypes[i])
                    return false;
            
            return true;
        }
    }
}
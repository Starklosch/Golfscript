using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Golfscript
{
    class OperationRunner
    {
        static Dictionary<string, Golfscript.Action> operations = new()
        {
            { ";", Pop },
            { "$", Peek },
            { "\\", Swap },
            { "@", Rotate },
            { "~", Evaluate },
            { "`", Inspect },

            // Coerce
            { "+", Add },
            { "-", Subtract },
            { "|", Or },
            { "&", And },
            { "^", Xor },

            { "(", Decrement },
            { ")", Increment },

            { "*", Multiplicate },
            { "/", Division },

            { "!", Negate },

            { "print", Print },
            { "base", Base },
            { "abs", Abs },
        };

        public static IReadOnlyDictionary<string, Golfscript.Action> Operations
        {
            get => new ReadOnlyDictionary<string, Golfscript.Action>(operations);
        }

        public static void Inspect(Stack context)
        {
            if (context.Size < 1)
                return;

            var item = context.Pop();
            var inspect = new StringItem(item.StackString());
            context.Push(inspect);
        }

        public static void Swap(Stack context)
        {
            if (context.Size < 2)
                return;

            var second = context.Pop(1);
            context.Push(second);
        }

        public static void Rotate(Stack context)
        {
            if (context.Size < 3)
                return;

            var third = context.Pop(2);
            context.Push(third);
        }

        public static void Peek(Stack context)
        {
            if (context.Size == 0)
                return;

            var first = context.Pop();

            context.Golfscript.Overloads.Run("Peek", first);
        }

        public static void Negate(Stack context)
        {
            if (context.Size == 0)
                return;

            var item = context.Pop();
            context.Push(new IntegerItem(item.Truthy ? 0 : 1));
        }

        public static void BinaryCoersion(Stack context, string action)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var type = Coerce(ref first, ref second);

            context.Golfscript.Overloads.Run(action, second, first);
        }

        //void BinaryOrder(Action<Item, Item> action)
        //{
        //    if (context.Size < 2)
        //        return;

        //    Item lower = context.Pop();
        //    Item higher = context.Pop();

        //    Order(ref higher, ref lower);

        //    action(higher, lower);
        //    context.Push(higher);
        //}

        public static void Print(Stack context)
        {
            if (context.Size == 0)
                return;

            var item = context.Pop();
            Console.WriteLine(item.NativeString());
        }

        public static void Pop(Stack context)
        {
            if (context.Size == 0)
            {
                Console.WriteLine("Empty stack");
                return;
            }

            context.Pop();
        }

        public static void Evaluate(Stack context)
        {
            if (context.Size == 0)
                return;

            Item item = context.Pop();
            item.Evaluate(context);
        }

        public static void Add(Stack context)
        {
            BinaryCoersion(context, nameof(Add));
        }

        public static void Subtract(Stack context)
        {
            BinaryCoersion(context, nameof(Subtract));
        }

        public static void Or(Stack context)
        {
            BinaryCoersion(context, nameof(Or));
        }

        public static void And(Stack context)
        {
            BinaryCoersion(context, nameof(And));
        }

        public static void Xor(Stack context)
        {
            BinaryCoersion(context, nameof(Xor));
        }

        public static void Multiplicate(Stack context)
        {
            if (context.Size < 2)
                return;

            Item lower = context.Pop();
            Item higher = context.Pop();

            Order(ref higher, ref lower);
            context.Golfscript.Overloads.Run(nameof(Multiplicate), higher, lower);
        }

        public static void Division(Stack context)
        {
            //    BinaryOrder(context, (a, b) => a.Divide(b));
        }

        public static void Decrement(Stack context)
        {
            if (context.Size < 1)
                return;

            Item first = context.Pop();

            context.Golfscript.Overloads.Run(nameof(Decrement), first);
        }

        public static void Increment(Stack context)
        {
            if (context.Size < 1)
                return;

            Item first = context.Pop();

            context.Golfscript.Overloads.Run(nameof(Increment), first);
        }

        public static void Base(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            context.Golfscript.Overloads.Run(nameof(Increment), first, second);
        }

        public static void Abs(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();

            if (first is IntegerItem integer && integer.RealValue < 0)
                context.Push(new IntegerItem(-integer.RealValue));
        }

        static ItemType Coerce(ref Item item1, ref Item item2)
        {
            if (item1.Type < item2.Type)
            {
                item1 = item1.Coerce(item2.Type);
                return item2.Type;
            }

            if (item1.Type > item2.Type)
            {
                item2 = item2.Coerce(item1.Type);
                return item1.Type;
            }

            return item1.Type;
        }

        static void Order(ref Item higher, ref Item lower)
        {
            if (higher.Type < lower.Type)
            {
                var aux = higher;
                higher = lower;
                lower = aux;
            }
        }
    }

    //class Overload : IEquatable<Overload>
    //{
    //    public string Name { get; }
    //    public IReadOnlyList<Type> Types { get; }

    //    public Overload(string name, IEnumerable<Type> types)
    //    {
    //        Name = name;
    //        Types = types.ToList();
    //    }

    //    public Overload(string name, params Type[] types)
    //    {
    //        Name = name;
    //        Types = types.ToList();
    //    }

    //    public override int GetHashCode()
    //    {
    //        int hash = Name.GetHashCode();
    //        hash = hash * 17 * Types.Count;
    //        foreach (var type in Types)
    //            hash = hash * 17 + type.GetHashCode();

    //        return hash;
    //    }

    //    public bool Equals(Overload other)
    //    {
    //        if (Name != other.Name ||
    //            Types.Count != other.Types.Count)
    //            return false;

    //        var copy = Types.ToList();
    //        foreach (var type in other.Types)
    //            copy.Remove(type);

    //        return copy.Count() == 0;
    //    }
    //}

    public class Function
    {
        public int ParamCount { get; }

        public Function()
        {
            ParamCount = 0;
        }
    }

    public class OperationOverloads
    {
        static BindingFlags s_flags = BindingFlags.NonPublic | BindingFlags.Instance;
        static List<string> s_excluded = new List<string>() { "Run" };
        static HashSet<string>? cachedAvailableOperations;

        HashSet<string> AvailableOperations { get => cachedAvailableOperations ?? new HashSet<string>(); }
        Stack Context { get; }

        public OperationOverloads(Stack context)
        {
            Context = context;

            if (cachedAvailableOperations == null)
                CacheAvailableOperations();
        }

        void Add(IntegerItem left, IntegerItem right)
        {
            var result = left.RealValue + right.RealValue;
            var item = new IntegerItem(result);
            Context.Push(item);
        }

        void Add(StringItem left, StringItem right)
        {
            var result = left.RealValue + right.RealValue;
            var item = new StringItem(result);
            Context.Push(item);
        }

        void Add(BlockItem left, BlockItem right)
        {
            var result = left.RealValue + " " + right.RealValue;
            var item = new BlockItem(result);
            Context.Push(item);
        }

        void Add(ArrayItem left, ArrayItem right)
        {
            var result = left.RealValue.Concat(right.RealValue);
            var item = new ArrayItem(result);
            Context.Push(item);
        }

        void Subtract(IntegerItem left, IntegerItem right)
        {
            var result = left.RealValue - right.RealValue;
            var item = new IntegerItem(result);
            Context.Push(item);
        }

        void Subtract(StringItem left, StringItem right)
        {
            var result = left.RealValue.Difference(right.RealValue);
            var item = new StringItem(result);
            Context.Push(item);
        }

        void Subtract(ArrayItem left, ArrayItem right)
        {
            var result = left.RealValue.Except(right.RealValue);
            var item = new ArrayItem(result);
            Context.Push(item);
        }

        void Or(IntegerItem left, IntegerItem right)
        {
            var result = left.RealValue | right.RealValue;
            var item = new IntegerItem(result);
            Context.Push(item);
        }

        void Or(StringItem left, StringItem right)
        {
            var result = left.RealValue.Union(right.RealValue);
            var item = new StringItem(result);
            Context.Push(item);
        }

        void Or(ArrayItem left, ArrayItem right)
        {
            var result = left.RealValue.Union(right.RealValue);
            var item = new ArrayItem(result);
            Context.Push(item);
        }

        void And(IntegerItem left, IntegerItem right)
        {
            var result = left.RealValue & right.RealValue;
            var item = new IntegerItem(result);
            Context.Push(item);
        }

        void And(StringItem left, StringItem right)
        {
            var result = left.RealValue.Intersect(right.RealValue);
            var item = new StringItem(result);
            Context.Push(item);
        }

        void And(ArrayItem left, ArrayItem right)
        {
            var result = left.RealValue.Intersect(right.RealValue);
            var item = new ArrayItem(result);
            Context.Push(item);
        }

        void Xor(IntegerItem left, IntegerItem right)
        {
            var result = left.RealValue ^ right.RealValue;
            var item = new IntegerItem(result);
            Context.Push(item);
        }

        void Xor(StringItem left, StringItem right)
        {
            var union = Extensions.Union(left.RealValue, right.RealValue);
            var intersection = left.RealValue.Intersect(right.RealValue);
            var result = union.Difference(intersection);
            var item = new StringItem(result);
            Context.Push(item);
        }

        void Xor(ArrayItem left, ArrayItem right)
        {
            var union = left.RealValue.Union(right.RealValue);
            var intersection = left.RealValue.Intersect(right.RealValue);
            var result = union.Except(intersection);
            var item = new ArrayItem(result);
            Context.Push(item);
        }

        void Multiplicate(IntegerItem left, IntegerItem right)
        {
            var result = left.RealValue * right.RealValue;
            var item = new IntegerItem(result);
            Context.Push(item);
        }

        // Repeat
        void Multiplicate(BlockItem left, IntegerItem right)
        {
            for (BigInteger i = 0; i < right.RealValue; i++)
                left.Evaluate(Context);
        }

        void Multiplicate(ArrayItem left, IntegerItem right)
        {
            var result = Enumerable.Empty<Item>();
            for (BigInteger i = 0; i < right.RealValue; i++)
                result = result.Concat(left.RealValue);

            var item = new ArrayItem(result);
            Context.Push(item);
        }

        void Multiplicate(StringItem left, IntegerItem right)
        {
            var result = new StringBuilder();
            for (BigInteger i = 0; i < right.RealValue; i++)
                result.Append(left.RealValue);

            var item = new StringItem(result.ToString());
            Context.Push(item);
        }

        // Join
        void Multiplicate(StringItem left, ArrayItem right)
        {
            var result = new StringBuilder();
            foreach (var item in right.RealValue)
                result.Append(item.Coerce(ItemType.String).NativeString()).Append(left.RealValue);

            if (result.Length > 0)
                result.Length -= left.RealValue.Length;

            Context.Push(new StringItem(result.ToString()));
        }

        void Multiplicate(ArrayItem left, ArrayItem right)
        {
            // TODO: Readable code
            if (left.RealValue.Count == 0)
                return;

            var result = new List<Item>();
            if (left.RealValue[0] is ArrayItem array)
                result.AddRange(array.RealValue);
            else
                result.Add(left.RealValue[0]);

            for (int i = 1; i < left.RealValue.Count; i++)
            {
                result.AddRange(right.RealValue);
                var leftItemArray = left.RealValue[i] as ArrayItem;
                if (leftItemArray != null)
                    result.AddRange(leftItemArray.RealValue);
                else
                    result.Add(left.RealValue[i]);
            }

            Context.Push(new ArrayItem(result));
        }

        #region Unary

        void Peek(IntegerItem integer)
        {
            var position = (int)integer.Value;
            var itemAtPosition = Context.Peek(position);
            Context.Push(itemAtPosition);
        }

        void Peek(ArrayItem array)
        {
            array.Sort();
        }

        void Increment(IntegerItem integer)
        {
            var result = integer.RealValue + 1;
            Context.Push(new IntegerItem(result));
        }

        void Increment(ArrayItem array)
        {
            var result = array.RealValue.SkipLast(1);
            Context.Push(new ArrayItem(result));
            Context.Push(array.RealValue.Last());
        }

        void Decrement(IntegerItem integer)
        {
            var result = integer.RealValue - 1;
            Context.Push(new IntegerItem(result));
        }

        void Decrement(ArrayItem array)
        {
            var result = array.RealValue.Skip(1);
            Context.Push(new ArrayItem(result));
            Context.Push(array.RealValue.First());
        }

        #endregion

        void Base(ArrayItem left, IntegerItem right)
        {
            int i = 0;
            BigInteger result = 0;
            foreach (var item in left.RealValue.Cast<IntegerItem>())
            {
                result *= 10;
                result += item.RealValue;
            }
            Context.Push(new IntegerItem(result));
        }

        void Base(IntegerItem left, IntegerItem right)
        {
            var n = left.RealValue;
            var list = new List<Item>();
            while (n > 0)
            {
                list.Add(new IntegerItem(n % right.RealValue));
                n /= right.RealValue;
            }
            list.Reverse();
            Context.Push(new ArrayItem(list));
        }

        public void Run(string func, params object[] pars)
        {
            if (!AvailableOperations.Contains(func))
                return;

            var types = pars.Select(p => p.GetType());

            var operations = typeof(OperationOverloads);
            var method = operations.GetMethod(func, s_flags, types.ToArray());
            var result = method?.Invoke(this, pars);
        }

        static void CacheAvailableOperations()
        {

            var operations = typeof(OperationOverloads);
            var methods = operations.GetMethods(s_flags);
            var methodsWithParameters = methods.Where(m => m.GetParameters().Length > 0);
            var methodNames = methodsWithParameters.Select(m => m.Name).Except(s_excluded);

            cachedAvailableOperations = methodNames.ToHashSet();
        }
    }
 
}

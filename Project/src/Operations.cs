using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Golfscript
{
    // Ordered naming: First is at the top of the stack
    // and last at the bottom of the stack
    static class Operations
    {
        internal static Dictionary<string, Golfscript.Action> All { get => m_operations; }

        static Dictionary<string, Golfscript.Action> m_operations = new()
        {
            // Unary
            { "~", Evaluate },
            { "`", Inspect },
            { "!", Negate },
            { ".", Duplicate },
            { ";", Pop },
            { "\\", Swap },
            { "@", Rotate },
            { ")", Increment },
            { "(", Decrement },

            // Coerce
            { "+", Addition },
            { "-", Subtraction },
            { "|", Or },
            { "&", And },
            { "^", Xor },

            // Order
            { "*", Multiplication },
            { "/", Division },
            { "%", Modulus },
            { "<", Less },
            { ">", Greater },
            //{ "=", Equal },

            // Other
            { "$", Peek },
            { ",", Size },

            { "abs", Abs },
            { "base", Base },
            { "print", Print },
        };


        static IEnumerable<Item> Sort(IEnumerable<Item> items)
        {
            return items.OrderBy(e => e);
        }

        static string Sort(string item)
        {
            char[] characters = item.ToArray();
            Array.Sort(characters);
            return new string(characters);
        }

        #region Coerce

        static void Addition(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var type = Coerce(ref first, ref second);

            Item result;

            if (type == ItemType.Integer)
            {
                var value = second.GetInt() + first.GetInt();
                result = new IntegerItem(value);
            }
            else if (type == ItemType.Array)
            {
                var firstCasted = second.GetArray();
                var value = firstCasted.Concat(first.GetArray());
                result = new ArrayItem(value);
            }
            else if (type == ItemType.String)
            {
                var value = second.GetString() + first.GetString();
                result = new StringItem(value);

            }
            else if (type == ItemType.Block)
            {
                var value = second.GetString() + " " + first.GetString();
                result = new BlockItem(value);
            }
            else
            {
                return;
            }

            context.Push(result);
        }

        static void Subtraction(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var type = Coerce(ref first, ref second);

            Item result;

            if (type == ItemType.Integer)
            {
                var value = second.GetInt() - first.GetInt();
                result = new IntegerItem(value);
            }
            else if (type == ItemType.Array)
            {
                var value = second.GetArray().Difference(first.GetArray());
                result = new ArrayItem(value);
            }
            else if (type == ItemType.String || type == ItemType.Block)
            {
                var value = second.GetString().Difference(first.GetString());
                result = new StringItem(value);
            }
            else
            {
                return;
            }

            context.Push(result);
        }

        static void Or(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var type = Coerce(ref first, ref second);

            Item result;

            if (type == ItemType.Integer)
            {
                var value = second.GetInt() | first.GetInt();
                result = new IntegerItem(value);
            }
            else if (type == ItemType.Array)
            {
                var value = second.GetArray().Union(first.GetArray());
                result = new ArrayItem(value);
            }
            else if (type == ItemType.String || type == ItemType.Block)
            {
                var value = second.GetString().Union(first.GetString());
                result = new StringItem(value);
            }
            else
            {
                return;
            }

            context.Push(result);
        }

        static void And(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var type = Coerce(ref first, ref second);

            Item result;

            if (type == ItemType.Integer)
            {
                var value = second.GetInt() & first.GetInt();
                result = new IntegerItem(value);
            }
            else if (type == ItemType.Array)
            {
                var value = second.GetArray().Intersect(first.GetArray());
                result = new ArrayItem(value);
            }
            else if (type == ItemType.String || type == ItemType.Block)
            {
                var value = second.GetString().Intersect(first.GetString());
                result = new StringItem(value);
            }
            else
            {
                return;
            }

            context.Push(result);
        }

        static void Xor(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var type = Coerce(ref first, ref second);

            Item result;

            if (type == ItemType.Integer)
            {
                var value = second.GetInt() ^ first.GetInt();
                result = new IntegerItem(value);
            }
            else if (type == ItemType.Array)
            {
                var value = second.GetArray().SymmetricDifference(first.GetArray());
                result = new ArrayItem(value);
            }
            else if (type == ItemType.String || type == ItemType.Block)
            {
                var value = second.GetString().SymmetricDifference(first.GetString());
                result = new StringItem(value);
            }
            else
            {
                return;
            }

            context.Push(result);
        }

        #endregion

        #region Order

        static void Multiplication(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref second, ref first);

            Item result;

            if (tuple == (ItemType.Integer, ItemType.Integer))
            {
                var value = second.GetInt() * first.GetInt();
                result = new IntegerItem(value);
            }
            #region Repeat
            else if (tuple == (ItemType.Block, ItemType.Integer))
            {
                var code = second.GetString();
                for (BigInteger i = 0, times = first.GetInt(); i < times; i++)
                    context.Golfscript.Run(code);

                return;
            }
            else if (tuple == (ItemType.Array, ItemType.Integer))
            {
                var value = Enumerable.Empty<Item>();
                for (BigInteger i = 0, times = first.GetInt(); i < times; i++)
                    value = value.Concat(second.GetArray());

                result = new ArrayItem(value);
            }
            else if (tuple == (ItemType.String, ItemType.Integer))
            {
                var value = new StringBuilder();
                for (BigInteger i = 0, times = first.GetInt(); i < times; i++)
                    value.Append(second.GetString());

                result = new StringItem(value.ToString());
            }
            #endregion
            #region Join
            else if (tuple == (ItemType.String, ItemType.String))
            {
                var separator = first.GetString();
                var str = second.GetString();

                var value = new StringBuilder();
                foreach (var ch in str)
                    value.Append(ch).Append(separator);

                if (value.Length > 0)
                    value.Length -= separator.Length;

                result = new StringItem(value.ToString());
            }
            else if (tuple == (ItemType.String, ItemType.Array))
            {
                var separator = second.GetString();

                var value = new StringBuilder();
                foreach (var item in first.GetArray())
                    value.Append(item.Coerce(ItemType.String).NativeString()).Append(separator);

                if (value.Length > 0)
                    value.Length -= separator.Length;

                result = new StringItem(value.ToString());
            }
            else if (tuple == (ItemType.Array, ItemType.Array))
            {
                var firstArray = first.GetArray();
                var secondArray = second.GetArray();

                if (secondArray.Count == 0)
                    return;

                var value = new List<Item>();

                void AddItem(Item item)
                {
                    if (item is ArrayItem array)
                        value.AddRange(array.GetArray());
                    else
                        value.Add(item);
                }

                AddItem(secondArray[0]);

                for (int i = 1; i < second.GetArray().Count(); i++)
                {
                    value.AddRange(first.GetArray());
                    AddItem(secondArray[i]);
                }

                result = new ArrayItem(value);
            }
            else if (tuple == (ItemType.Block, ItemType.Array))
            {
                var code = second.GetString();
                var array = first.GetArray();

                if (array.Count == 0)
                    return;

                context.Push(array[0]);

                for (int i = 1; i < array.Count; i++)
                {
                    context.Push(array[i]);
                    context.Golfscript.Run(code);
                }

                return;
            }
            else if (tuple == (ItemType.Block, ItemType.String))
            {
                var code = second.GetString();
                var array = first.GetString().ToCharArray();

                if (array.Length == 0)
                    return;

                context.Push(new IntegerItem(array[0]));

                for (int i = 1; i < array.Length; i++)
                {
                    context.Push(new IntegerItem(array[i]));
                    context.Golfscript.Run(code);
                }

                return;
            }
            #endregion
            else
            {
                return;
            }

            context.Push(result);
        }

        static void Division(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref first, ref second);

            Item result;

            if (first.Type == second.Type)
            {
                if (first.Type == ItemType.Integer)
                {
                    var value = second.GetInt() / first.GetInt();
                    result = new IntegerItem(value);
                }
                else if (first.Type == ItemType.String)
                {
                    var separator = second.GetString();
                    var split = first.GetString().Split(separator);
                    var value = split.Where(str => str.Length > 0).Select(str => new StringItem(str));
                    result = new ArrayItem(value);
                }
                // TODO: Array implementation
            }
            else if (tuple == (ItemType.Block, ItemType.Array))
            {
                Each(context, first, second);
                return;
            }
            else
            {
                return;
            }

                context.Push(result);
            }

            static void Each(Stack context, Item first, Item second)
            {
                var code = first.GetString();
                foreach (var item in second.GetArray())
                {
                    context.Push(item);
                    context.Golfscript.Run(code);
                }
            }

            static void Modulus(Stack context)
            {
                if (context.Size < 2)
                    return;

                Item first = context.Pop();
                Item second = context.Pop();

                var tuple = Order(ref first, ref second);

                Item result;

                if (tuple == (ItemType.Integer, ItemType.Integer))
                {
                    var value = second.GetInt() % first.GetInt();
                    result = new IntegerItem(value);
                }
                else if (tuple == (ItemType.String, ItemType.String))
                {
                    var separator = first.GetString();
                    var split = second.GetString().Split(separator);
                    var value = split.Where(str => str.Length > 0).Select(str => new StringItem(str));
                    result = new ArrayItem(value);
                }
                else if (tuple == (ItemType.Block, ItemType.Array))
                {
                    context.PushFrame();
                    Each(context, first, second);
                    context.PopFrame(true);
                    return;
                }
                else
                {
                    return;
                }

                context.Push(result);
            }

            static void Less(Stack context)
            {
                if (context.Size < 2)
                    return;

                Item first = context.Pop();
                Item second = context.Pop();

                var tuple = Order(ref first, ref second);

                if (first.Type == second.Type)
                {
                    if (first.Type == ItemType.Integer)
                    {
                        var value = second.GetInt() < first.GetInt();
                        context.Push(new IntegerItem(value ? 1 : 0));
                    }
                    else if (first.MatchAny(ItemType.String, ItemType.Block))
                    {
                        var value = second.GetString().CompareTo(first.GetString());
                        context.Push(new IntegerItem(value == -1 ? 1 : 0));
                    }
                    // TODO: Compare arrays
                }
                else if (tuple == (ItemType.String, ItemType.Integer))
                {
                    var position = (int)second.GetInt();
                    var str = first.GetString();

                    string value;
                    if (position == 0)
                        value = "";
                    else if (position > 0)
                        value = str.Substring(0, Math.Min(position, str.Length));
                    else
                        value = str.Substring(0, Math.Max(str.Length + position, 0));

                    context.Push(new StringItem(value));
                }
                else if (tuple == (ItemType.Block, ItemType.Integer))
                {
                    var position = (int)second.GetInt();
                    var str = first.GetString();

                    string value;
                    if (position == 0)
                        value = "";
                    else if (position > 0)
                        value = str.Substring(0, Math.Min(position, str.Length));
                    else
                        value = str.Substring(0, Math.Max(str.Length + position, 0));

                    context.Push(new BlockItem(value));
                }
                else if (tuple == (ItemType.Array, ItemType.Integer))
                {
                    var position = (int)second.GetInt();
                    var array = first.GetArray();

                    IEnumerable<Item> value;
                    if (position == 0)
                        value = Enumerable.Empty<Item>();
                    else if (position > 0)
                        value = array.Take(Math.Min(position, array.Count));
                    else
                        value = array.SkipLast(Math.Min(-position, array.Count));

                    context.Push(new ArrayItem(value));
                }
                else
                {
                    return;
                }
            }

            static void Greater(Stack context)
            {
                if (context.Size < 2)
                    return;

                Item first = context.Pop();
                Item second = context.Pop();

                var tuple = Order(ref first, ref second);

                if (first.Type == second.Type)
                {
                    if (first.Type == ItemType.Integer)
                    {
                        var value = second.GetInt() > first.GetInt();
                        context.Push(new IntegerItem(value ? 1 : 0));
                    }
                    else if (first.MatchAny(ItemType.String, ItemType.Block))
                    {
                        var value = second.GetString().CompareTo(first.GetString());
                        context.Push(new IntegerItem(value == 1 ? 1 : 0));
                    }
                    // TODO: Compare arrays
                }
                else if (tuple == (ItemType.String, ItemType.Integer))
                {
                    var position = (int)second.GetInt();
                    var str = first.GetString();

                    string value;
                    if (position == 0)
                        value = "";
                    else if (position > 0)
                        value = str.Substring(Math.Min(position, str.Length - 1));
                    else
                        value = str.Substring(Math.Max(str.Length + position, 0));

                    context.Push(new StringItem(value));
                }
                else if (tuple == (ItemType.Block, ItemType.Integer))
                {
                    var position = (int)second.GetInt();
                    var str = first.GetString();

                    string value;
                    if (position == 0)
                        value = "";
                    else if (position > 0)
                        value = str.Substring(Math.Min(position, str.Length - 1));
                    else
                        value = str.Substring(Math.Max(str.Length + position, 0));

                    context.Push(new BlockItem(value));
                }
                else if (tuple == (ItemType.Array, ItemType.Integer))
                {
                    var position = (int)second.GetInt();
                    var array = first.GetArray();

                    IEnumerable<Item> value;
                    if (position == 0)
                        value = Enumerable.Empty<Item>();
                    else if (position > 0)
                        value = array.Skip(Math.Min(position, array.Count));
                    else
                        value = array.TakeLast(Math.Min(-position, array.Count));

                    context.Push(new ArrayItem(value));
                }
                else
                {
                    return;
                }
            }

            #endregion

            #region Unary

            static void Evaluate(Stack context)
            {
                if (context.Size == 0)
                    return;

                Item first = context.Pop();

                if (first.Type == ItemType.Integer)
                {
                    var value = ~first.GetInt();
                    context.Push(new IntegerItem(value));
                }
                else if (first.Type == ItemType.Array)
                {
                    foreach (var item in first.GetArray())
                        context.Push(item);
                }
                else if (first.Type == ItemType.String || first.Type == ItemType.Block)
                {
                    context.Golfscript.Run(first.GetString());
                }
            }

            static void Inspect(Stack context)
            {
                if (context.Size < 1)
                    return;

                var first = context.Pop();
                var inspect = new StringItem(first.StackString());
                context.Push(inspect);
            }

            static void Duplicate(Stack context)
            {
                if (context.Size == 0)
                    return;

                var first = context.Peek();
                context.Push(first);
            }

            static void Pop(Stack context)
            {
                if (context.Size == 0)
                    return;

                context.Pop();
            }

            static void Swap(Stack context)
            {
                if (context.Size < 2)
                    return;

                var first = context.Pop();
                var second = context.Pop();
                context.Push(first);
                context.Push(second);
            }

            static void Rotate(Stack context)
            {
                if (context.Size < 3)
                    return;

                var first = context.Pop();
                var second = context.Pop();
                var third = context.Pop();
                context.Push(second);
                context.Push(first);
                context.Push(third);
            }

            static void Negate(Stack context)
            {
                if (context.Size == 0)
                    return;

                var item = context.Pop();
                context.Push(new IntegerItem(item.Truthy ? 0 : 1));
            }

            static void Increment(Stack context)
            {
                if (context.Size == 0)
                    return;

                var first = context.Pop();
                if (first.Type == ItemType.Integer)
                {
                    var value = first.GetInt() + 1;
                    context.Push(new IntegerItem(value));
                }
                else if (first.Type == ItemType.Array)
                {
                    var array = first.GetArray();
                    var value = array.SkipLast(1);
                    context.Push(new ArrayItem(value));
                    context.Push(array.Last());
                }
            }

            static void Decrement(Stack context)
            {
                if (context.Size == 0)
                    return;

                var first = context.Pop();
                if (first.Type == ItemType.Integer)
                {
                    var value = first.GetInt() - 1;
                    context.Push(new IntegerItem(value));
                }
                else if (first.Type == ItemType.Array)
                {
                    var array = first.GetArray();
                    var value = array.Skip(1);
                    context.Push(new ArrayItem(value));
                    context.Push(array.First());
                }
            }

            #endregion

            #region Other

            static void Abs(Stack context)
            {
                if (context.Size == 0)
                    return;

                Item first = context.Pop();

                if (first.Type != ItemType.Integer)
                    return;

                var intValue = first.GetInt();
                if (intValue < 0)
                    context.Push(new IntegerItem(-intValue));
            }

            static void Base(Stack context)
            {
                if (context.Size < 2)
                    return;

                var first = context.Pop();
                var second = context.Pop();

                var tuple = MakeTuple(second, first);

                if (tuple == (ItemType.Integer, ItemType.Integer))
                {
                    var n = second.GetInt();
                    var @base = first.GetInt();

                    var list = new List<Item>();
                    while (n > 0)
                    {
                        list.Add(new IntegerItem(n % first.GetInt()));
                        n /= @base;
                    }
                    list.Reverse();
                    context.Push(new ArrayItem(list));
                }
                else if (tuple == (ItemType.Array, ItemType.Integer))
                {
                    int i = 0;
                    var @base = first.GetInt();

                    BigInteger result = 0;
                    foreach (var item in second.GetArray().Cast<IntegerItem>())
                    {
                        result *= @base;
                        result += item.RealValue;
                    }
                    context.Push(new IntegerItem(result));
                }
            }

            static void Peek(Stack context)
            {
                if (context.Size == 0)
                    return;

                var first = context.Pop();

                Item result;

                if (first.Type == ItemType.Integer)
                {
                    var position = (int)first.GetInt();
                    if (context.Size == 0)
                        return;

                    result = context.Peek(position);
                }
                else if (first.Type == ItemType.String)
                {
                    var sorted = Sort(first.GetString());
                    result = new StringItem(string.Concat(sorted));
                }
                else if (first.Type == ItemType.Block)
                {
                    var sorted = Sort(first.GetString());
                    result = new StringItem(string.Concat(sorted));
                }
                else if (first.Type == ItemType.Array)
                {
                    result = new ArrayItem(Sort(first.GetArray()));
                }
                else
                {
                    return;
                }

                context.Push(result);
            }

            static void Print(Stack context)
            {
                if (context.Size == 0)
                    return;

                var first = context.Pop();
                Console.WriteLine(first.NativeString());
            }

            static void Size(Stack context)
            {
                if (context.Size == 0)
                    return;

                var first = context.Pop();

                Item result;

                if (first.Type == ItemType.Integer)
                {
                    var list = new List<Item>();
                    for (BigInteger i = 0, n = first.GetInt(); i < n; i++)
                        list.Add(new IntegerItem(i));

                    result = new ArrayItem(list);
                }
                // Map
                else if (first.Type == ItemType.Block)
                {
                    if (context.Size == 0)
                        return;

                    var second = context.Pop();
                    var array = second.GetArray();

                    context.PushFrame();
                    Each(context, first, second);
                    var frame = context.PopFrame();

                    var tuples = frame.Items.Select((item, index) => (item, array.ElementAt(index)));
                    var value = tuples.Where(tuple => tuple.item.Truthy).Select(tuple => tuple.Item2);
                    result = new ArrayItem(value);
                }
                else// if (first.MatchTypes(ItemType.Array, ItemType.String))
                {
                    result = new IntegerItem(first.Size);
                }

                context.Push(result);
            }

            #endregion

            #region Utils

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

            static (ItemType Higher, ItemType Lower) Order(ref Item higher, ref Item lower)
            {
                if (higher.Type < lower.Type)
                {
                    var aux = higher;
                    higher = lower;
                    lower = aux;
                }

                return MakeTuple(higher, lower);
            }

            static bool MatchAny(this Item item, params ItemType[] types)
            {
                return types.Any(type => type == item.Type);
            }

            static (ItemType First, ItemType Second) MakeTuple(Item first, Item second)
            {
                return (first.Type, second.Type);
            }

            static BigInteger GetInt(this Item item)
            {
                if (item.Type == ItemType.Integer)
                    return (BigInteger)item.Value;

                return 0;
            }
            static string GetString(this Item item)
            {
                if (item.Type == ItemType.String || item.Type == ItemType.Block)
                    return (string)item.Value;

                return "";
            }
            static IReadOnlyList<Item> GetArray(this Item item)
            {
                if (item.Type == ItemType.Array)
                    return (IReadOnlyList<Item>)item.Value;

                return Enumerable.Empty<Item>().ToList().AsReadOnly();
            }

            #endregion
        }

    }
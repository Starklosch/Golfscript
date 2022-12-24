using System.Numerics;
using System.Text;

namespace Golfscript
{
    class ItemComparer : IComparer<Item>
    {
        public int Compare(Item? x, Item? y)
        {
            if (ReferenceEquals(x, y) || x == null || y == null)
                return 0;

            if (x.Type != y.Type)
                return 0;

            if (x.Type == ItemType.Integer)
                return ((BigInteger)x.Value).CompareTo((BigInteger)y.Value);
            else if (x.Type == ItemType.String || x.Type == ItemType.Block)
                return ((string)x.Value).CompareTo((string)y.Value);

            return 0;
        }
    }

    // Ordered naming: First is at the top of the stack
    // and last at the bottom of the stack
    static class Operators
    {
        static readonly IComparer<char> CharComparer = Comparer<char>.Default;
        static readonly IComparer<Item> ItemComparer = new ItemComparer();

        static string Sort(string item)
        {
            char[] characters = item.ToArray();
            Array.Sort(characters, CharComparer);
            return new string(characters);
        }

        #region Coerce

        internal static void Addition(Stack context)
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

        internal static void Subtraction(Stack context)
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

        internal static void Or(Stack context)
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

        internal static void And(Stack context)
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

        internal static void Xor(Stack context)
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

        internal static void Multiplication(Stack context)
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

                result = new StringItem(value);
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

                result = new StringItem(value);
            }
            else if (tuple == (ItemType.String, ItemType.Array))
            {
                var separator = second.GetString();

                var value = new StringBuilder();
                foreach (var item in first.GetArray())
                    value.Append(item.Coerce(ItemType.String).NativeString()).Append(separator);

                if (value.Length > 0)
                    value.Length -= separator.Length;

                result = new StringItem(value);
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

                AddItem(secondArray.ElementAt(0));

                for (int i = 1; i < second.GetArray().Count; i++)
                {
                    value.AddRange(firstArray);
                    AddItem(secondArray.ElementAt(i));
                }

                result = new ArrayItem(value);
            }
            else if (tuple == (ItemType.Block, ItemType.Array))
            {
                var code = second.GetString();
                var array = first.GetArray();

                if (array.Count == 0)
                    return;

                context.Push(array.ElementAt(0));

                for (int i = 1; i < array.Count; i++)
                {
                    context.Push(array.ElementAt(i));
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

        internal static void Division(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref first, ref second);

            Item result;


            if (tuple == (ItemType.Integer, ItemType.Integer))
            {
                var value = second.GetInt() / first.GetInt();
                result = new IntegerItem(value);
            }
            else if (tuple == (ItemType.String, ItemType.String))
            {
                var separator = second.GetString();
                var split = first.GetString().Split(separator);
                var value = split.Where(str => str.Length > 0).Select(str => new StringItem(str));
                result = new ArrayItem(value);
            }
            // TODO: Array implementation
            //else if (tuple == (ItemType.Array, ItemType.Array))
            //{
            //    var separator = second.GetArray();
            //    var split = first.GetArray().Split(separator);
            //    var value = split.Where(str => str.Length > 0).Select(str => new StringItem(str));
            //    result = new ArrayItem(value);
            //}
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

        internal static void Each(Stack context, Item first, Item second)
        {
            var code = first.GetString();
            foreach (var item in second.GetArray())
            {
                context.Push(item);
                context.Golfscript.Run(code);
            }
        }

        internal static void Modulus(Stack context)
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

        internal static void Less(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref first, ref second);

            if (tuple == (ItemType.Integer, ItemType.Integer))
            {
                var value = second.GetInt() < first.GetInt();
                context.Push(new IntegerItem(value ? 1 : 0));
            }
            else if (tuple == (ItemType.String, ItemType.String) || tuple == (ItemType.Block, ItemType.Block))
            {
                var value = second.GetString().CompareTo(first.GetString());
                context.Push(new IntegerItem(value == -1 ? 1 : 0));
            }
            // TODO: Compare arrays
            //else if (tuple == (ItemType.Array, ItemType.Array))
            else if (tuple == (ItemType.String, ItemType.Integer))
            {
                var position = (int)second.GetInt();
                var str = first.GetString();

                string value;
                if (position == 0)
                    value = "";
                else if (position > 0)
                    value = str[..Math.Min(position, str.Length)];
                else
                    value = str[..Math.Max(str.Length + position, 0)];

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
                    value = str[..Math.Min(position, str.Length)];
                else
                    value = str[..Math.Max(str.Length + position, 0)];

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

        internal static void Greater(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref first, ref second);

            if (tuple == (ItemType.Integer, ItemType.Integer))
            {
                var value = second.GetInt() > first.GetInt();
                context.Push(new IntegerItem(value ? 1 : 0));
            }
            else if (tuple == (ItemType.String, ItemType.String) || tuple == (ItemType.Block, ItemType.Block))
            {
                var value = second.GetString().CompareTo(first.GetString());
                context.Push(new IntegerItem(value == 1 ? 1 : 0));
            }
            // TODO: Compare arrays
            //else if (tuple == (ItemType.Array, ItemType.Array)){
            //}
            else if (tuple == (ItemType.String, ItemType.Integer))
            {
                var position = (int)second.GetInt();
                var str = first.GetString();

                string value;
                if (position == 0)
                    value = "";
                else if (position > 0)
                    value = str[Math.Min(position, str.Length - 1)..];
                else
                    value = str[Math.Max(str.Length + position, 0)..];

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
                    value = str[Math.Min(position, str.Length - 1)..];
                else
                    value = str[Math.Max(str.Length + position, 0)..];

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

        internal static void Equal(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref first, ref second);

            if (tuple == (ItemType.Integer, ItemType.Integer))
            {
                var value = second.GetInt() == first.GetInt();
                context.Push(new IntegerItem(value ? 1 : 0));
            }
            else if (tuple == (ItemType.String, ItemType.String) || tuple == (ItemType.Block, ItemType.Block))
            {
                var value = second.GetString().CompareTo(first.GetString());
                context.Push(new IntegerItem(value == 0 ? 1 : 0));
            }
            else if (tuple == (ItemType.Array, ItemType.Array))
            {
                if (second.Size != first.Size)
                {
                    context.Push(new IntegerItem(0));
                    return;
                }

                var firstArray = first.GetArray();
                var secondArray = second.GetArray();

                var value = firstArray.SequenceEqual(secondArray);
                context.Push(new IntegerItem(value ? 1 : 0));
            }
            else if (tuple == (ItemType.String, ItemType.Integer))
            {
                var position = (int)second.GetInt();
                var str = first.GetString();

                char value;
                if (position >= 0)
                    value = str[Math.Min(position, str.Length - 1)];
                else
                    value = str[Math.Max(str.Length + position, 0)];

                context.Push(new IntegerItem(value));
            }
            else if (tuple == (ItemType.Block, ItemType.Integer))
            {
                var position = (int)second.GetInt();
                var str = first.GetString();

                char value;
                if (position >= 0)
                    value = str[Math.Min(position, str.Length - 1)];
                else
                    value = str[Math.Max(str.Length + position, 0)];

                context.Push(new IntegerItem(value));
            }
            else if (tuple == (ItemType.Array, ItemType.Integer))
            {
                var position = (int)second.GetInt();
                var array = first.GetArray();

                int index;
                if (position >= 0)
                    index = Math.Min(position, array.Count - 1);
                else
                    index = Math.Max(array.Count + position, 0);

                Item value = array.ElementAt(index);
                context.Push(value);
            }
            else
            {
                return;
            }
        }

        internal static void Pow(Stack context)
        {
            if (context.Size < 2)
                return;

            Item first = context.Pop();
            Item second = context.Pop();

            var tuple = Order(ref first, ref second);

            if (tuple == (ItemType.Integer, ItemType.Integer))
            {
                var number = second.GetInt();
                var times = first.GetInt();
                var value = number;

                for (BigInteger i = 1; i < times; i++)
                    value *= number;

                context.Push(new IntegerItem(value));
            }
            else if (tuple == (ItemType.String, ItemType.String))
            {
                var str = second.GetString();
                var value = first.GetString();
                var index = str.IndexOf(value);
                context.Push(new IntegerItem(index));
            }
            else if (first.Type == ItemType.Array)
            {
                var array = first.GetArray();
                var index = array.IndexOf(second);
                context.Push(new IntegerItem(index));
            }
            else
            {
                return;
            }
        }

        #endregion

        #region Unary

        internal static void Evaluate(Stack context)
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

        internal static void Inspect(Stack context)
        {
            if (context.Size < 1)
                return;

            var first = context.Pop();
            var inspect = new StringItem(first.StackString());
            context.Push(inspect);
        }

        internal static void Duplicate(Stack context)
        {
            if (context.Size == 0)
                return;

            var first = context.Peek();
            context.Push(first);
        }

        internal static void Pop(Stack context)
        {
            if (context.Size == 0)
                return;

            context.Pop();
        }

        internal static void Swap(Stack context)
        {
            if (context.Size < 2)
                return;

            var first = context.Pop();
            var second = context.Pop();
            context.Push(first);
            context.Push(second);
        }

        internal static void Rotate(Stack context)
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

        internal static void Negate(Stack context)
        {
            if (context.Size == 0)
                return;

            var item = context.Pop();
            context.Push(new IntegerItem(item.Truthy ? 0 : 1));
        }

        internal static void Increment(Stack context)
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

        internal static void Decrement(Stack context)
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

        internal static void Abs(Stack context)
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

        internal static void Base(Stack context)
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
                var @base = first.GetInt();

                BigInteger result = 0;
                foreach (var item in second.GetArray().Cast<IntegerItem>())
                {
                    result *= @base;
                    result += item.GetInt();
                }
                context.Push(new IntegerItem(result));
            }
        }

        internal static void Do(Stack context)
        {
            if (context.Size == 0)
                return;

            var first = context.Pop();

            if (first.Type == ItemType.Block)
            {
                var block = first.GetString();

                do
                {
                    context.Golfscript.Run(block);
                } while (context.Size > 0 && context.Pop().Truthy);
            }
        }

        internal static void If(Stack context)
        {
            if (context.Size < 3)
                return;

            var falseCase = context.Pop();
            var trueCase = context.Pop();
            var condition = context.Pop();

            if (condition.Truthy)
            {
                if (trueCase.Type == ItemType.Block)
                    context.Golfscript.Run(trueCase.GetString());
                else
                    context.Push(trueCase);
            }
            else
            {
                if (falseCase.Type == ItemType.Block)
                    context.Golfscript.Run(falseCase.GetString());
                else
                    context.Push(falseCase);
            }
        }

        internal static void Peek(Stack context)
        {
            if (context.Size == 0)
                return;

            var first = context.Pop();

            Item result;

            if (first.Type == ItemType.Integer)
            {
                var position = (int)first.GetInt();
                if (context.Size <= position)
                    return;

                result = context.Peek(position);
            }
            else if (first.Type == ItemType.String)
            {
                var sorted = Sort(first.GetString());
                result = new StringItem(sorted);
            }
            else if (first.Type == ItemType.Block)
            {
                if (context.Size == 0)
                    return;

                //var second = context.Pop();
                // TODO: Mapping sort
                //if (second.Type == ItemType.String)
                //{
                //}
                //else if (second.Type == ItemType.Array)
                //{
                //}
                //else
                //{
                return;
                //}
            }
            else if (first.Type == ItemType.Array)
            {
                first.GetArray().Sort(ItemComparer);
                result = first;
            }
            else
            {
                return;
            }

            context.Push(result);
        }

        internal static void Print(Stack context)
        {
            if (context.Size == 0)
                return;

            var first = context.Pop();
            Console.Write(first.NativeString());
        }

        internal static void Rand(Stack context)
        {
            if (context.Size == 0)
                return;


            var first = context.Pop();

            if (first.Type != ItemType.Integer)
                return;

            // TODO: Random but for BigInteger
            var random = Random.Shared.Next((int)first.GetInt());
            context.Push(new IntegerItem(random));
        }

        internal static void Size(Stack context)
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

        internal static void Until(Stack context)
        {
            if (context.Size < 2)
                return;

            var first = context.Pop();
            var second = context.Pop();

            if (ItemType.Block.MatchAll(first, second))
            {
                var condition = second.GetString();
                var body = first.GetString();

                while (true)
                {
                    context.Golfscript.Run(condition);
                    if (context.Size == 0 || context.Pop().Truthy)
                        break;

                    context.Golfscript.Run(body);
                }
            }
        }

        internal static void While(Stack context)
        {
            if (context.Size < 2)
                return;

            var first = context.Pop();
            var second = context.Pop();

            if (ItemType.Block.MatchAll(first, second))
            {
                var condition = second.GetString();
                var body = first.GetString();

                while (true)
                {
                    context.Golfscript.Run(condition);
                    if (context.Size == 0 || !context.Pop().Truthy)
                        break;

                    context.Golfscript.Run(body);
                }
            }
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
                (lower, higher) = (higher, lower);

            return MakeTuple(higher, lower);
        }

        static bool MatchAll(this ItemType type, params Item[] items)
        {
            return items.All(item => type == item.Type);
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
        static List<Item> GetArray(this Item item)
        {
            if (item.Type == ItemType.Array)
                return (List<Item>)item.Value;

            return new List<Item>();
        }

        #endregion
    }
}
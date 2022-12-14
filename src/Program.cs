using System.Linq;

namespace Golfscript
{

    class Program
    {
        static Stack stack = new Stack();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            string? line;

            Console.WriteLine(stack);
            stack.Push(new IntegerItem(1));
            Console.WriteLine(stack);
            stack.Push(new IntegerItem(2));
            Console.WriteLine(stack);
            stack.Push(new OperationItem(Operations.Addition));
            Console.WriteLine(stack);

            Golfscript golfscript = new();

#pragma warning disable CS8600  // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

            do
            {
                line = Console.ReadLine();

                var tokenizer = golfscript.ScanTokens(line);
                foreach (var token in tokenizer.Tokens)
                {
                    if (token.Type == TokenType.IdentifierDeclaration)
                    {
                        golfscript[(string)token.Literal] = new IntegerItem(0);
                    }
                    else if (token.Type == TokenType.Identifier &&
                        golfscript.TryGetVariable((string)token.Literal, out Item name))
                    {

                    }
                    else if (token.Type == TokenType.Number)
                    {
                        golfscript.Stack.Push(new);
                    }
                    else if (operations.TryGetValue(token.Text, out var action))
                    {

                    }
                }
                //Console.WriteLine(string.Join(", ", tokenizer.Tokens));

                //var item = Parser.Parse(line);
                //if (item != null)
                //    stack.Push(item);

                //Console.WriteLine(stack);
            } while (line == null || !line.StartsWith("quit"));

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.

        }
    }
}
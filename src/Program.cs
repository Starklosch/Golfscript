using System.Linq;

namespace Golfscript
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Golfscript Interactive Mode");

            string? line;

            Golfscript golfscript = new();

#pragma warning disable CS8600  // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

            do
            {
                line = Console.ReadLine();

                golfscript.Parse(line);

                Console.WriteLine(golfscript.Stack);
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
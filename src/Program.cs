using System.Linq;

namespace Golfscript
{

    class Program
    {
        static void Main(string[] args)
        {
            Golfscript golfscript = new();

            if (args.Length > 0)
            {
                foreach (var file in args)
                {
                    if (File.Exists(file))
                    {
                        string script = File.ReadAllText(file);
                        golfscript.Run(script, true);
                    }
                    else
                        Console.WriteLine($"{file} doesn't exist.");
                }
                Console.Read();
                return;
            }

            Console.WriteLine("Golfscript Interactive Mode");

            string? line;


#pragma warning disable CS8600  // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

            do
            {
                line = Console.ReadLine();

                golfscript.Run(line, true);

                Console.WriteLine(golfscript.Stack);
            } while (line == null || !line.StartsWith("quit"));

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.

        }
    }
}
using BenchmarkDotNet.Running;
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
                Console.Write("> ");
                line = Console.ReadLine();

                golfscript.Run(line, true);

                Console.WriteLine(golfscript.Stack);
            } while (line == null || !line.StartsWith("quit"));

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.

        }

        const string SEPARATOR = "->";

        static void Format()
        {
            Console.WriteLine("File?");
            string file = Console.ReadLine();

            var lines = File.ReadAllLines(file);

            Console.WriteLine("Save to?");
            file = Console.ReadLine();

            using (var stream = new StreamWriter(file))
            {
                stream.WriteLine("[Fact]");
                stream.WriteLine("void Function(){");

                foreach (string line in lines)
                {
                    if (line.StartsWith("args"))
                    {
                        stream.WriteLine("}\n\n");
                        stream.WriteLine("[Fact]");
                        stream.WriteLine("void Function(){");
                    }

                    if (!line.Contains(SEPARATOR))
                        continue;

                    var split = line.Split(SEPARATOR);
                    var code = split[0].Trim().Replace("\"", "\\\"").Replace("\\", "\\\\");
                    var expected = split[1].Trim().Replace("\"", "\\\"").Replace("\\", "\\\\");

                    stream.Write("    Test(\"");
                    stream.Write(code);
                    stream.Write("\", \"");
                    stream.Write(expected);
                    stream.WriteLine("\");");
                }
            }
        }
    }
}
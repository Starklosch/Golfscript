using System;
using System.Collections.Generic;
using System.IO;

namespace Golfscript
{

    class Program
    {
        static Golfscript golfscript = new Golfscript();

        static void Main(string[] args)
        {
            //Format();

            if (args.Length > 0)
            {
                InterpretFile(args);
                Console.WriteLine("\nScript end reached.");
                Console.WriteLine("Press any key to exit.");
                Console.Read();
                return;
            }

            Interactive();
        }

        static void Interactive()
        {
            Console.WriteLine("Golfscript Interactive Mode");

            string? line;
            do
            {
                Console.Write("> ");
                line = Console.ReadLine();

                golfscript.Run(line, true);

                Console.WriteLine(golfscript.Stack);
            } while (line == null || !line.StartsWith("quit"));
        }

        static void InterpretFile(string[] args)
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
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

namespace Golfscript
{

    class Program
    {
        static void Main(string[] args)
        {
            //Format();
            var golfscript = new Golfscript();

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

            do
            {
                Console.Write("> ");
                line = Console.ReadLine();

                golfscript.Run(line, true);

                Console.WriteLine(golfscript.Stack);
            } while (line == null || !line.StartsWith("quit"));

        }
    }
}
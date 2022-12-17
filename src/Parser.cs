namespace Golfscript
{
    static class Parser
    {
        public static void Parse(this Golfscript golfscript, string line)
        {
            var tokenizer = new Tokenizer(golfscript, line);

            foreach (var token in tokenizer.ScanTokens())
            {
                if (token.Type == TokenType.IdentifierDeclaration)
                {
                    golfscript[(string)token.Literal] = golfscript.Stack.Peek();
                }
                else if (token.Type == TokenType.Identifier &&
                    golfscript.TryGetVariable((string)token.Literal, out Item variable))
                {
                    golfscript.Stack.Push(variable);
                }
                else if (token.Type == TokenType.Number)
                {
                    golfscript.Stack.Push(new IntegerItem((int)token.Literal!));
                }
                else if (token.Type == TokenType.String)
                {
                    golfscript.Stack.Push(new StringItem((string)token.Literal!));
                }
                else if (token.Type == TokenType.Operator &&
                    Operations.All.TryGetValue(token.Text, out var action))
                {
                    golfscript.Stack.Push(new OperationItem(action));
                }
            }
        }

        public static void Parse(this Golfscript golfscript, IEnumerable<Token> tokens)
        {
            var enumerator = tokens.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Token token = enumerator.Current;
                if (token.Type == TokenType.IdentifierDeclaration)
                {
                    golfscript[(string)token.Literal] = golfscript.Stack.Peek();
                }
                else if (token.Type == TokenType.Identifier &&
                    golfscript.TryGetVariable((string)token.Literal, out Item variable))
                {
                    golfscript.Stack.Push(variable);
                }
                else if (token.Type == TokenType.Number)
                {
                    golfscript.Stack.Push(new IntegerItem((int)token.Literal!));
                }
                else if (token.Type == TokenType.String)
                {
                    golfscript.Stack.Push(new StringItem((string)token.Literal!));
                }
                else if (token.Type == TokenType.Operator &&
                    Operations.All.TryGetValue(token.Text, out var action))
                {
                    golfscript.Stack.Push(new OperationItem(action));
                }
                else if(token.Type == TokenType.ArrayBeginning)
                {
                    golfscript.Stack.PushFrame();
                }
                else if (token.Type == TokenType.ArrayEnding)
                {
                    var frame = golfscript.Stack.PopFrame();
                    golfscript.Stack.Push(new ArrayItem(frame.Items));
                }
            }
        }

        //public static Item? Parse(string? input)
        //{
        //    if (string.IsNullOrEmpty(input))
        //        return null;

        //    if (input.StartsWith('"') && input.EndsWith('"'))
        //        return new StringItem(input.Trim('"'));

        //    if (input.StartsWith('[') && input.EndsWith(']'))
        //    {
        //        var items = new List<Item>();
        //        foreach (var item in input.Trim('[', ']').Split(' '))
        //        {
        //            var parseAttempt = Parse(item);
        //            if (parseAttempt != null)
        //                items.Add(parseAttempt);
        //        }
        //        return new ArrayItem(items);
        //    }

        //    if (input.StartsWith('{') && input.EndsWith('}'))
        //    {
        //        var items = new List<Item>();
        //        foreach (var item in input.Trim('{', '}').Split(' '))
        //        {
        //            var parseAttempt = Parse(item);
        //            if (parseAttempt != null)
        //                items.Add(parseAttempt);
        //        }
        //        return new BlockItem(items);
        //    }

        //    if (int.TryParse(input, out int number))
        //    {
        //        return new IntegerItem(number);
        //    }

        //    //OperationManager.FindOverload(input);
        //    if (Operations.All.TryGetValue(input, out var operation))
        //    {
        //        return new OperationItem(operation);
        //    }

        //    return null;
        //}

        //public static List<Item> ParseLine(string? line)
        //{
        //    if (string.IsNullOrEmpty(input))
        //        return null;


        //}
    }
}
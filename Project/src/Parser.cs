namespace Golfscript
{
    static class Parser
    {
        public static void Parse(this Golfscript golfscript, string line)
        {
            var tokenizer = new Tokenizer(golfscript, line);
            foreach (var token in tokenizer.ScanTokens())
            {
                ParseToken(golfscript, token);
            }
        }

        public static void Parse(this Golfscript golfscript, IEnumerable<Token> tokens)
        {
            var enumerator = tokens.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParseToken(golfscript, enumerator.Current);
            }
        }

        static void ParseToken(Golfscript golfscript, Token token)
        {
            switch (token.Type)
            {
                case TokenType.ArrayBeginning:
                    golfscript.Stack.PushFrame();
                    return;
                case TokenType.ArrayEnding:
                    golfscript.Stack.PopFrame(true);
                    return;
                case TokenType.IdentifierDeclaration:
                    golfscript[(string)token.Literal] = golfscript.Stack.Peek();
                    return;
                case TokenType.String:
                    golfscript.Stack.Push(new StringItem((string)token.Literal!));
                    return;
                case TokenType.Number:
                    golfscript.Stack.Push(new IntegerItem((int)token.Literal!));
                    return;
                case TokenType.Block:
                    golfscript.Stack.Push(new BlockItem((string)token.Literal!));
                    return;
            }

            if (token.Type == TokenType.Identifier &&
                golfscript.TryGetVariable((string)token.Literal, out Item variable))
            {
                golfscript.Stack.Push(variable);
            }
            else if (token.Type == TokenType.Operator &&
                Operations.All.TryGetValue(token.Text, out var action))
            {
                action(golfscript.Stack);
            }
        }
    }
}
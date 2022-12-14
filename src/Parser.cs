namespace Golfscript
{

    class TokenProcessor
    {
        Tokenizer m_tokenizer;
        List<Item> m_items;

        public TokenProcessor(Tokenizer tokenizer)
        {
            m_tokenizer = tokenizer;
        }

        public TokenProcessor(string source)
        {
            m_tokenizer = new Tokenizer(source);

        }

        void GetItems()
        {
            foreach (var token in m_tokenizer.Tokens)
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
        }

    }

    static class Parser
    {

        public static Item? Parse(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            if (input.StartsWith('"') && input.EndsWith('"'))
                return new StringItem(input.Trim('"'));

            if (input.StartsWith('[') && input.EndsWith(']'))
            {
                var items = new List<Item>();
                foreach (var item in input.Trim('[', ']').Split(' '))
                {
                    var parseAttempt = Parse(item);
                    if (parseAttempt != null)
                        items.Add(parseAttempt);
                }
                return new ArrayItem(items);
            }

            if (input.StartsWith('{') && input.EndsWith('}'))
            {
                var items = new List<Item>();
                foreach (var item in input.Trim('{', '}').Split(' '))
                {
                    var parseAttempt = Parse(item);
                    if (parseAttempt != null)
                        items.Add(parseAttempt);
                }
                return new BlockItem(items);
            }

            if (int.TryParse(input, out int number))
            {
                return new IntegerItem(number);
            }

            //OperationManager.FindOverload(input);
            if (Operations.All.TryGetValue(input, out var operation))
            {
                return new OperationItem(operation);
            }

            return null;
        }

        //public static List<Item> ParseLine(string? line)
        //{
        //    if (string.IsNullOrEmpty(input))
        //        return null;


        //}
    }
}
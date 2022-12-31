namespace Golfscript
{
    class Token
    {
        public string Text { get; }
        public TokenType Type { get; }
        public TokenInfo Info { get; }

        public Token(TokenType type, string text, int line, int column)
        {
            Text = text;
            Type = type;
            Info = new TokenInfo(line, column);
        }

        public override string? ToString()
        {
            if (Type == TokenType.Identifier)
                return $"{Type} \"{Text}\":{Info.Line}:{Info.Column}";

            return $"{Type}:{Info.Line}:{Info.Column}";
        }
    }
}
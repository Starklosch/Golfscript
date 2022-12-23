namespace Golfscript
{
    class Token
    {
        string m_text;
        object? m_literal;
        TokenType m_type;
        TokenInfo m_info;

        public Token(TokenType type, string text, object? literal, int line, int column)
        {
            m_text = text;
            m_literal = literal;
            m_type = type;

            m_info = new TokenInfo(line, column);
        }

        public string Text { get => m_text; }
        public object? Literal { get => m_literal; }
        public int Line { get => m_info.Line; }
        public int Column { get => m_info.Column; }
        public TokenType Type { get => m_type; }

        public override string? ToString()
        {
            return m_literal == null ? $"{Type}:{Line}:{Column}" : $"{Type} ({Literal}):{Line}:{Column}";
        }
    }
}
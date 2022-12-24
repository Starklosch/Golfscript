namespace Golfscript
{
    class Token
    {
        string m_text;
        TokenType m_type;
        TokenInfo m_info;

        public Token(TokenType type, string text, int line, int column)
        {
            m_text = text;
            m_type = type;

            m_info = new TokenInfo(line, column);
        }

        public string Text { get => m_text; }
        public int Line { get => m_info.Line; }
        public int Column { get => m_info.Column; }
        public TokenType Type { get => m_type; }

        public override string? ToString()
        {
            return $"{Type}:{Line}:{Column}";
        }
    }
}
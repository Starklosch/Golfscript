namespace Golfscript
{
    struct TokenInfo
    {
        int m_line;
        int m_column;

        public TokenInfo(int line, int column)
        {
            m_line = line;
            m_column = column;
        }

        public int Line { get => m_line; }
        public int Column { get => m_column; }
    }
}
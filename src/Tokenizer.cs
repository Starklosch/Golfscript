using System.Text;

namespace Golfscript
{
    class Tokenizer
    {
        static readonly Dictionary<char, TokenType> SingleCharTokens = new()
        {
            { '(', TokenType.LeftParen },
            { ')', TokenType.RightParen },
            { '[', TokenType.LeftSquare },
            { ']', TokenType.RightSquare },
            { '{', TokenType.LeftCurly },
            { '}', TokenType.RightCurly },

            { '+', TokenType.Plus },
            { '-', TokenType.Minus },
            { '*', TokenType.Star },
            { '/', TokenType.Slash },
            { '%', TokenType.Percent },

            { '~', TokenType.Tilde },
            { '`', TokenType.GraveAccent },
            { '!', TokenType.Bang },
            { '?', TokenType.Question },
            { '@', TokenType.At },
            { '#', TokenType.Hash },
            { '$', TokenType.Dollar },
            { '\\', TokenType.BackSlash },

            { '|', TokenType.VerticalBar },
            { '&', TokenType.Ampersand },
            { '^', TokenType.CircumflexAccent },
            //{ '\'', TokenType.SingleQuote },
            //{ '"', TokenType.DoubleQuote },
            { '<', TokenType.Less },
            { '>', TokenType.Greater },
            { '=', TokenType.Equal },

            { '.', TokenType.Dot },
            { ',', TokenType.Less },
            { ':', TokenType.Greater },
            { ';', TokenType.Equal },
            { ' ', TokenType.Space },
        };

        static readonly Dictionary<char, char> EscapedChars = new()
        {
            { '\\', '\\' },
            { 'a', '\a' },
            { 'b', '\b' },
            { 'f', '\f'},
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
        };

        List<Token> m_tokens;
        string m_buffer;
        int m_start;
        int m_current;

        int line, lineStart;

        public Tokenizer(string buffer)
        {
            m_buffer = buffer;
            m_current = 0;
            m_tokens = new List<Token>();
        }

        public char Advance(int n = 1)
        {
            var ch = m_buffer[m_current];
            m_current += n;
            return ch;
        }

        public char Peek()
        {
            return m_buffer[m_current];
        }

        public void ScanTokens(IEnumerable<string> variables)
        {
            m_tokens.Clear();
            m_current = 0;
            line = 0;
            lineStart = 0;

            while (Available)
            {
                m_start = m_current;
                
                var token = ScanToken(variables);
                if (token != null)
                    m_tokens.Add(token);
            }

            var eof = new Token(TokenType.EOF, "", null, line, m_buffer.Length);
            m_tokens.Add(eof);
        }

        Token? ScanToken(IEnumerable<string> variables)
        {
            Advance();

            foreach (var variable in variables)
            {
                if (m_current + variable.Length - 1 > m_buffer.Length)
                    continue;

                var span = m_buffer.AsSpan(m_current - 1, variable.Length);
                if (span.Equals(variable, StringComparison.CurrentCulture))
                {
                    return Token(TokenType.Identifier, variable);
                }
            }

            switch (Current)
            {
                case '\n':
                    NewLine();
                    break;
                // Raw
                case '\'':
                    return RawString();
                // Escaped
                case '"':
                    return String();
                case ':':
                    return IdentifierDeclaration();
            }

            if (SingleCharTokens.ContainsKey(Current))
            {
                return Token(SingleCharTokens[Current]);
            }

            if (char.IsDigit(Current))
            {
                return Number();
            }

            Console.WriteLine($"Unknown token at {line}, {Column}");
            return null;
        }

        private void NewLine()
        {
            line++;
            lineStart = m_current;
        }

        Token? RawString()
        {
            bool escape = false;

            StringBuilder sb = new();
            while (Available && (escape || Next != '\''))
            {
                if (escape && Current == '\'')
                    sb.Length--;
                else if (escape && Current != '\\')
                    sb.Append('\\');

                if (Current == '"')
                    sb.Append('\\');

                sb.Append(Next);

                escape = !escape && Next == '\\';
                Advance();

                if (Current == '\n')
                    NewLine();
            }

            if (!Available)
            {
                Console.WriteLine("Unterminated string!");
                return null;
            }

            Advance();


            //var literal = m_buffer.Substring(m_start + 1, m_current - m_start - 2);
            //StringBuilder sb = new(m_current - m_start);
            //for (int i = m_start + 1; i < m_current - 1; i++)
            //{
            //    if (m_buffer[i] == '\\')
            //        sb.Append("\\\\");
            //    else
            //        sb.Append(m_buffer[i]);
            //}
            return Token(TokenType.String, sb.ToString());
        }

        Token? String()
        {
            bool escape = false;
            StringBuilder sb = new();
            while (Available && (escape || Next != '"'))
            {
                Advance();
                escape = !escape && Current == '\\';

                if (Current == '\n')
                    NewLine();

                if (Current != '\\' || Next != '\'')
                    sb.Append(Current);
            }

            if (!Available)
            {
                Console.WriteLine("Unterminated string!");
                return null;
            }

            Advance();
            return Token(TokenType.String, sb.ToString());
        }

        Token Number()
        {
            while (Available && char.IsDigit(Next))
            {
                Advance();
            }

            var span = m_buffer.AsSpan(m_start, m_current - m_start);
            var number = int.Parse(Substring());
            return Token(TokenType.Number, number);
        }

        Token IdentifierDeclaration()
        {
            while (Available && Next != '\n')
            {
                Advance();
            }

            return Token(TokenType.IdentifierDeclaration, Substring(1).ToString());
        }

        Token Token(TokenType type) => Token(type, null);

        Token Token(TokenType type, object? literal)
        {
            var text = Substring().ToString();
            return new Token(type, text, literal, line, Column);
        }

        ReadOnlySpan<char> Substring()
        {
            var length = m_current - m_start;
            return m_buffer.AsSpan(m_start, length);
        }

        ReadOnlySpan<char> Substring(int frontOffset, int backOffset = 0)
        {
            var front = m_start + frontOffset;
            var back = m_current + backOffset;
            var length = back - front;
            return m_buffer.AsSpan(front, length);
        }

        bool Available => m_current < m_buffer.Length;
        char Current => m_buffer[m_current - 1];
        char Next => m_buffer[m_current];
        int Column => m_start - lineStart;

        public IReadOnlyList<Token> Tokens => m_tokens;
        //public LexicUnit? Current => (m_index < m_buffer.Length) ? m_unit : null;
    }
}
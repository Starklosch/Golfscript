using System.Text;

namespace Golfscript
{
    class Tokenizer : ErrorReporter
    {
        static readonly Dictionary<char, TokenType> SingleCharTokens = new()
        {
            { '(', TokenType.LeftParen },
            { ')', TokenType.RightParen },
            { '[', TokenType.ArrayBeginning },
            { ']', TokenType.ArrayEnding },
            { '{', TokenType.BlockBeginning },
            { '}', TokenType.BlockEnding },

            { '#', TokenType.Comment },

            { '+', TokenType.Operator },
            { '-', TokenType.Operator },
            { '*', TokenType.Operator },
            { '/', TokenType.Operator },
            { '%', TokenType.Operator },

            { '~', TokenType.Operator },
            { '`', TokenType.Operator },
            { '!', TokenType.Operator },
            { '?', TokenType.Operator },
            { '@', TokenType.Operator },
            { '$', TokenType.Operator },
            { '\\', TokenType.Operator },

            { '|', TokenType.Operator },
            { '&', TokenType.Operator },
            { '^', TokenType.Operator },
            //{ '\'', TokenType.SingleQuote },
            //{ '"', TokenType.DoubleQuote },
            { '<', TokenType.Operator },
            { '>', TokenType.Operator },
            { '=', TokenType.Operator },

            { '.', TokenType.Operator },
            { ',', TokenType.Operator },
            { ':', TokenType.Operator },
            { ';', TokenType.Operator },

            { ' ', TokenType.Space },
        };

        static readonly List<string> Operators = new()
        {
            "print"
        };

        static readonly Dictionary<char, char> EscapedChars = new()
        {
            { '\\', '\\' },
            { '\'', '\'' },
            { 'a', '\a' },
            { 'b', '\b' },
            { 'f', '\f'},
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
        };

        Golfscript m_context;
        List<Token> m_tokens;
        string m_buffer;
        int m_start;
        int m_current;
        bool m_reportErrors;

        int line, lineStart;

        public Tokenizer(Golfscript context, string code)
        {
            m_context = context;
            m_buffer = code;
            m_current = 0;
            m_tokens = new List<Token>();
        }

        public void Advance(int n = 1)
        {
            m_current += n;
        }

        public IEnumerable<Token> ScanTokens()
        {
            m_current = 0;
            line = 0;
            lineStart = 0;

            while (Available)
            {
                m_start = m_current;

                var token = ScanToken();
                if (token != null)
                    yield return token;
            }

            yield return new Token(TokenType.EOF, "", null, line, m_buffer.Length);
        }

        Token? ScanToken()
        {
            // Identify variables
            foreach (var variable in m_context.Identifiers)
            {
                if (m_current + variable.Length > m_buffer.Length)
                    continue;

                var span = m_buffer.AsSpan(m_current, variable.Length);
                if (span.Equals(variable, StringComparison.CurrentCulture))
                {
                    Advance(variable.Length);
                    return Token(TokenType.Identifier, variable);
                }
            }

            Advance();

            switch (Current)
            {
                case '#':
                    Comment();
                    return null;
                case '\r':
                    return null;
                case '\n':
                    NewLine();
                    return null;
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

            foreach (var op in Operators)
            {
                if (m_current + op.Length - 1 > m_buffer.Length)
                    continue;

                var span = m_buffer.AsSpan(m_current - 1, op.Length);
                if (span.Equals(op, StringComparison.CurrentCulture))
                {
                    Advance(op.Length - 1);
                    return Token(TokenType.Operator, op);
                }
            }

            Report($"Unknown token at {line}, {Column}");
            return null;
        }

        #region Tokens

        Token? RawString()
        {
            bool escape = false;

            StringBuilder sb = new();

            while (Available && (escape || Next != '\''))
            {
                Advance();

                if (Current == '\n')
                    NewLine();

                if (escape)
                {
                    if (EscapedChars.TryGetValue(Current, out char value))
                        sb.Append(value);
                    else
                        Report("Invalid escaped character!");
                }
                else if (Current != '\\')
                    sb.Append(Current);

                escape = !escape && Current == '\\';
            }

            if (!Available)
            {
                Report("Unterminated string!");
                return null;
            }

            Advance();

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
                Report("Unterminated string!");
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

        #endregion

        #region Helpers

        void NewLine()
        {
            line++;
            lineStart = m_current;
        }

        void Comment()
        {
            while (Available && Next != '\n')
                Advance();

            if (Available && Next == '\n')
                Advance();
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

        #endregion

        bool Available => m_current < m_buffer.Length;
        char Current => m_buffer[m_current - 1];
        char Next => m_buffer[m_current];
        int Column => m_start - lineStart;

        public IReadOnlyList<Token> Tokens => m_tokens;
    }
}
using System.Text;

namespace Golfscript
{
    class Tokenizer
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

        //public void ScanTokens(IEnumerable<string> variables)
        //{
        //    m_tokens.Clear();
        //    m_current = 0;
        //    line = 0;
        //    lineStart = 0;

        //    while (Available)
        //    {
        //        m_start = m_current;
                
        //        var token = ScanToken(variables);
        //        if (token != null)
        //            m_tokens.Add(token);
        //    }

        //    var eof = new Token(TokenType.EOF, "", null, line, m_buffer.Length);
        //    m_tokens.Add(eof);
        //}

        public IEnumerable<Token> ScanTokens(Golfscript context)
        {
            m_current = 0;
            line = 0;
            lineStart = 0;

            while (Available)
            {
                m_start = m_current;

                var token = ScanToken(context.VariableNames);
                if (token != null)
                    yield return token;
            }

            yield return new Token(TokenType.EOF, "", null, line, m_buffer.Length);
        }

        Token? ScanToken(IEnumerable<string> variables)
        {
            // Identify variables
            foreach (var variable in variables)
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

            Console.WriteLine($"Unknown token at {line}, {Column}");
            return null;
        }

        void Comment()
        {
            while (Available && Next != '\n')
            {
                Advance();
            }

            Advance();
        }

        void NewLine()
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
                if (Next == '\n')
                    NewLine();

                if (escape && Next == '\'')
                    sb.Length--;
                else if (escape && Next != '\\')
                    sb.Append('\\');

                if (Next == '"')
                    sb.Append('\\');

                sb.Append(Next);

                escape = !escape && Next == '\\';
                Advance();
            }

            if (!Available)
            {
                Console.WriteLine("Unterminated string!");
                return null;
            }
            Console.WriteLine(sb.ToString());

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
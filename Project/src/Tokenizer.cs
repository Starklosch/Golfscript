using System.Text;

namespace Golfscript
{
    class Tokenizer : ErrorReporter
    {
        const string IdentifierCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        const string Operators = "~`!.;\\@()+-|&^*/%<>$,=?";

        static readonly Dictionary<char, TokenType> SingleCharTokens = new()
        {
            { '[', TokenType.ArrayBeginning },
            { ']', TokenType.ArrayEnding },

            { '#', TokenType.Comment },
            { ' ', TokenType.Space },
        };

        static readonly Dictionary<char, char> EscapedChars = new()
        {
            { '\\', '\\' },
            { '\'', '\'' },
            { '\"', '\"' },
            { 'a', '\a' },
            { 'b', '\b' },
            { 't', '\t' },
            { 'n', '\n' },
            { 'v', '\v' },
            { 'f', '\f'},
            { 'r', '\r' },
            { 'e', '\u001B' },
        };

        Golfscript _context;
        List<Token> _tokens;
        string _buffer;
        int _start;
        int _current;

        int line, lineStart;

        public Tokenizer(Golfscript context, string code)
        {
            _context = context;
            _buffer = code;
            _current = 0;
            _tokens = new List<Token>();
        }

        public void Advance(int n = 1)
        {
            _current += n;
        }

        public IEnumerable<Token> ScanTokens()
        {
            _current = 0;
            line = 0;
            lineStart = 0;

            while (Available)
            {
                _start = _current;

                var token = ScanToken();
                if (token != null)
                    yield return token;
            }

            yield return new Token(TokenType.EOF, "", null, line, _buffer.Length);
        }

        Token? ScanToken()
        {
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
                case '{':
                    return Block();
                case '-':
                    return Available && char.IsDigit(Next) ? Number() : Identifier();
            }

            if (SingleCharTokens.ContainsKey(Current))
            {
                return Token(SingleCharTokens[Current]);
            }

            if (char.IsDigit(Current))
            {
                return Number();
            }

            if (Operators.Contains(Current))
            {
                return Token(TokenType.Identifier);
            }

            if (IdentifierCharacters.Contains(Current))
            {
                var id = Identifier();
                if (_context.Identifiers.Contains(id.Literal))
                    return id;
            }

            Report($"Unknown token at {line}, {Column}");
            return null;
        }

        #region Tokens

        int EscapedChar()
        {
            if (EscapedChars.TryGetValue(Current, out char value))
                return value;

            // Hex
            int start = _current;
            int i;

            if (Current == 'x')
            {
                for (i = 1; i <= 2; i++)
                {
                    if (IsHex(Next))
                        Advance();
                }

                if (_current - start <= 2)
                    return Convert.ToInt32(_buffer.Substring(start, i), 16);
            }
            else if (IsOctal(Current))
            {
                for (i = 1; i <= 2; i++)
                {
                    if (IsOctal(Next))
                        Advance();
                }

                if (_current - start <= 3)
                    return Convert.ToInt32(_buffer.Substring(start - 1, i), 8);
            }

            return -1;
        }

        Token Block()
        {
            while (Available && Next != '}')
            {
                Advance();
            }

            if (Available)
                Advance();

            return Token(TokenType.Block, Substring(1, -1).ToString());
        }

        Token? RawString()
        {
            bool escape = false;

            StringBuilder sb = new();

            while (Available && (escape || Next != '\''))
            {
                Advance();

                if (Current == '\n')
                    NewLine();

                if (escape && Current != '\'')
                    sb.Append('\\');

                if (Current != '\\')
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
            while (Available && (escape || Next != '\"'))
            {
                Advance();

                if (Current == '\n')
                    NewLine();

                if (escape)
                {
                    var escapedChar = EscapedChar();
                    if (escapedChar >= 0)
                        sb.Append((char)escapedChar);
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

        Token Number()
        {
            while (Available && char.IsDigit(Next))
            {
                Advance();
            }

            var span = _buffer.AsSpan(_start, _current - _start);
            var number = int.Parse(Substring());
            return Token(TokenType.Number, number);
        }

        Token IdentifierDeclaration()
        {
            while (Available && IdentifierCharacters.Contains(Next))
            {
                Advance();
            }

            return Token(TokenType.IdentifierDeclaration, Substring(1).ToString());
        }

        Token Identifier()
        {
            while (Available && IdentifierCharacters.Contains(Next))
            {
                Advance();
            }

            return Token(TokenType.Identifier, Substring().ToString());
        }

        Token Token(TokenType type) => Token(type, null);

        Token Token(TokenType type, object? literal)
        {
            var text = Substring().ToString();
            return new Token(type, text, literal, line, Column);
        }

        #endregion

        #region Helpers

        bool IsHex(char c)
        {
            return (c >= '0' && c <= '9') || (char.ToUpper(c) >= 'A' && char.ToUpper(c) <= 'F');
        }

        bool IsOctal(char c)
        {
            return c >= '0' && c <= '7';
        }

        void NewLine()
        {
            line++;
            lineStart = _current;
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
            var length = _current - _start;
            return _buffer.AsSpan(_start, length);
        }

        ReadOnlySpan<char> Substring(int frontOffset, int backOffset = 0)
        {
            var front = _start + frontOffset;
            var back = _current + backOffset;
            var length = back - front;
            return _buffer.AsSpan(front, length);
        }

        #endregion

        bool Available => _current < _buffer.Length;
        char Current => _buffer[_current - 1];
        char Next => _buffer[_current];
        int Column => _start - lineStart;

        public IReadOnlyList<Token> Tokens => _tokens;
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Golfscript
{
    class RegexTokenizer : ErrorReporter
    {
        static readonly Regex Expression = new Regex("(?<EOL>\r\n|\n|\r)|(?<Identifier>[a-zA-Z_][a-zA-Z0-9_]*)|(?<RawString>'(?:\\\\.|[^'])*'?)|(?<String>\"(?:\\\\.|[^\"])*\"?)|(?<Number>-?[0-9]+)|(?<Comment>(#[^\n\r]*))|.",
                RegexOptions.Compiled | RegexOptions.Multiline);

        Golfscript Context { get; }

        string _buffer;
        int _start;

        int line = 1, lineStart;

        public RegexTokenizer(Golfscript context, string code)
        {
            Context = context;
            _buffer = code;
        }

        public IEnumerable<Token> ScanTokens()
        {
            foreach (Match match in Expression.Matches(_buffer))
            {
                _start = match.Index;

                var token = ScanToken(match);
                if (token != null)
                    yield return token;
            }
        }

        Token? ScanToken(Match match)
        {
            if (!match.Success)
            {
                Report($"Unknown token at {line}, {Column}");
                return null;
            }

            if (match.Groups["EOL"].Success)
            {
                lineStart = match.Index + match.Groups["EOL"].Length - 1;
                line++;
                return null;
            }

            var value = match.Value;
            if (Context.Identifiers.Contains(value))
                return new Token(TokenType.Identifier, value, line, Column);

            if (match.Groups["Number"].Success)
                return new Token(TokenType.Number, value, line, Column);

            if (match.Groups["String"].Success)
                return new Token(TokenType.String, value, line, Column);

            if (match.Groups["RawString"].Success)
                return new Token(TokenType.RawString, value, line, Column);

            if (match.Groups["Comment"].Success)
                return new Token(TokenType.Comment, value, line, Column);

            return value[0] switch
            {
                ' ' => new Token(TokenType.Space, value, line, Column),
                ':' => new Token(TokenType.IdentifierDeclaration, value, line, Column),
                '[' => new Token(TokenType.ArrayBeginning, value, line, Column),
                ']' => new Token(TokenType.ArrayEnding, value, line, Column),
                '{' => new Token(TokenType.BlockBeginning, value, line, Column),
                '}' => new Token(TokenType.BlockEnding, value, line, Column),
                _ => new Token(TokenType.Identifier, value, line, Column),
            };
        }

        int Column => _start - lineStart;
    }
}
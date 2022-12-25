using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Golfscript
{
    class RegexTokenizer : ErrorReporter
    {
        static readonly Regex Expression = new Regex("(?<EOF>[\n\r])|(?<Identifier>[a-zA-Z_][a-zA-Z0-9_]*)|(?<RawString>'(?:\\\\.|[^'])*'?)|(?<String>\"(?:\\\\.|[^\"])*\"?)|(?<Number>-?[0-9]+)|(?<Comment>(#[^\n\r]*))|.",
                RegexOptions.Compiled | RegexOptions.Multiline);

        Golfscript Context { get; }

        List<Token> _tokens;
        string _buffer;
        int _start;

        int line, lineStart;

        public RegexTokenizer(Golfscript context, string code)
        {
            Context = context;
            _buffer = code;
            _tokens = new List<Token>();
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

            if (match.Groups["EOF"].Success)
            {
                lineStart = match.Index;
                line++;
                return null;
            }

            var value = match.Value;
            if (Context.Identifiers.Contains(value))
                return new Token(TokenType.Identifier, value, Column, match.Index);

            if (match.Groups["Number"].Success)
                return new Token(TokenType.Number, value, Column, match.Index);

            if (match.Groups["String"].Success)
                return new Token(TokenType.String, value, Column, match.Index);

            if (match.Groups["RawString"].Success)
                return new Token(TokenType.RawString, value, Column, match.Index);

            if (match.Groups["Comment"].Success)
                return new Token(TokenType.Comment, value, Column, match.Index);

            return value[0] switch
            {
                ' ' => new Token(TokenType.Space, value, Column, match.Index),
                ':' => new Token(TokenType.IdentifierDeclaration, value, Column, match.Index),
                '[' => new Token(TokenType.ArrayBeginning, value, Column, match.Index),
                ']' => new Token(TokenType.ArrayEnding, value, Column, match.Index),
                '{' => new Token(TokenType.BlockBeginning, value, Column, match.Index),
                '}' => new Token(TokenType.BlockEnding, value, Column, match.Index),
                _ => new Token(TokenType.Identifier, value, Column, match.Index),
            };
        }

        int Column => _start - lineStart;

        public IReadOnlyList<Token> Tokens => _tokens;
    }
}
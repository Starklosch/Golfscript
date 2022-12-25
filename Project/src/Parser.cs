using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Golfscript
{

    class Parser
    {
        static Dictionary<string, string> Escaped = new()
        {
            { @"\\", "\\" },
            { @"\'", "'" },
            { "\\\"", "\"" },
            { @"\a", "\a" },
            { @"\b", "\b" },
            { @"\t", "\t" },
            { @"\n", "\n" },
            { @"\v", "\v" },
            { @"\f", "\f"},
            { @"\r", "\r" },
            { @"\e", "\u001B" },
        };

        Golfscript Context { get; }

        int blockDepth = 0;
        StringBuilder blockContent = new StringBuilder();

        bool pendingAssignment = false;

        public Parser(Golfscript context)
        {
            Context = context;
        }

        public void Parse(IEnumerable<Token> tokens)
        {
            var enumerator = tokens.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParseToken(enumerator.Current);
            }
        }

        void ParseToken(Token token)
        {

            if (token.Type == TokenType.BlockBeginning)
            {
                if (blockDepth++ > 0)
                    blockContent.Append(token.Text);

                return;
            }

            if (token.Type == TokenType.BlockEnding)
            {
                if (--blockDepth > 0)
                {
                    blockContent.Append(token.Text);
                }
                else
                {
                    Context.Stack.Push(new BlockItem(blockContent.ToString()));
                    blockContent.Clear();
                }
                return;
            }

            if (blockDepth > 0)
            {
                blockContent.Append(token.Text);
                return;
            }

            if (pendingAssignment)
            {
                Context.SetVariable(token.Text, Context.Stack.Peek());
                pendingAssignment = false;
                return;
            }

            switch (token.Type)
            {
                case TokenType.ArrayBeginning:
                    Context.Stack.PushFrame();
                    return;
                case TokenType.ArrayEnding:
                    Context.Stack.PopFrame(true);
                    return;
                case TokenType.IdentifierDeclaration:
                    pendingAssignment = true;
                    return;
                case TokenType.RawString:
                    Context.Stack.Push(new StringItem(ParseRawString(token.Text)));
                    return;
                case TokenType.String:
                    Context.Stack.Push(new StringItem(ParseString(token.Text)));
                    return;
                case TokenType.Number:
                    Context.Stack.Push(new IntegerItem(ParseInt(token.Text)));
                    return;
            }

            if (token.Type == TokenType.Identifier && Context.TryGetVariable(token.Text, out object variable))
            {
                if (variable is BlockItem block)
                    Context.Run((string)block.Value);
                else if (variable is Item item)
                    Context.Stack.Push(item);
                else if (variable is Golfscript.Action action)
                    action(Context.Stack);
            }
        }

        static string ParseRawString(string code)
        {
            var sb = new StringBuilder(code, 1, code.Length - 2, code.Length);
            sb.Replace("\\\\", "\\").Replace("\\'", "'");
            return sb.ToString();
        }

        static Regex EscapedRegex = new Regex("\\\\x(?<Hex>[\\da-fA-F]{1,2})|\\\\(?<Oct>[0-7]{1,3})",
            RegexOptions.Compiled | RegexOptions.Multiline);

        static string ParseString(string code)
        {
            var sb = new StringBuilder(code, 1, code.Length - 2, code.Length);
            foreach (var item in Escaped)
                sb.Replace(item.Key, item.Value);

            foreach (Match match in EscapedRegex.Matches(sb.ToString()))
            {
                var hex = match.Groups["Hex"];
                var octal = match.Groups["Oct"];
                if (hex.Success)
                {
                    var _byte = Convert.ToByte(hex.Value, 16);

                    sb.Remove(match.Index, match.Length);
                    sb.Insert(match.Index, (char)_byte);
                }
                else if (octal.Success)
                {
                    var _byte = Convert.ToByte(octal.Value, 8);

                    sb.Remove(match.Index, match.Length);
                    sb.Insert(match.Index, (char)_byte);
                }
            }
            return sb.ToString();
        }

        static BigInteger ParseInt(string code)
        {
            return BigInteger.Parse(code);
        }

        static bool IsHex(char c)
        {
            return (c >= '0' && c <= '9') || (char.ToUpper(c) >= 'A' && char.ToUpper(c) <= 'F');
        }

        static bool IsOctal(char c)
        {
            return c >= '0' && c <= '7';
        }
    }
}
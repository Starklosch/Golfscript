namespace Golfscript
{
    // From https://craftinginterpreters.com/scanning.html
    enum TokenType
    {
        // Single-character tokens.
        // Parens
        LeftParen, RightParen,
        // Blocks
        LeftCurly, RightCurly,
        // Strings
        LeftSquare, RightSquare,

        // Operators
        Operator,

        // Tilde, GraveAccent, Bang, Question, At, Dollar, Plus, Minus, Star, Slash, BackSlash,
        // Percent, VerticalBar, Ampersand, CircumflexAccent, SingleQuote, DoubleQuote, Less, Greater, Equal, Dot, Comma, Colon, Semicolon,
        // ~ ` ! ? @ $ + - * / \
        // % | & ^ ' " < > = . , : ;
        // and or xor print p n puts rand do while until if abs zip base V Q

        // Literals.
        Identifier, IdentifierDeclaration, String, Number, Comments,

        // Keywords.
        Keyword,
        //And, Or, Xor, Print, P, N, Puts, Rand,
        //Do, While, Until, If, Abs, Zip, Base, V, Q,

        Space, EOF
    }
}
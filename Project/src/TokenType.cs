namespace Golfscript
{
    // Based on https://craftinginterpreters.com/scanning.html
    enum TokenType
    {
        // Single-character tokens.
        // Strings
        ArrayBeginning, ArrayEnding,

        // Operators
        Operator,

        // Literals.
        Identifier, IdentifierDeclaration, String, Number, Comment, Block,

        // Keywords.
        Keyword,

        Space, EOF
    }
}

namespace Golfscript
{
    // Based on https://craftinginterpreters.com/scanning.html
    enum TokenType
    {
        // Single-character tokens.
        // Strings
        ArrayBeginning, ArrayEnding,
        BlockBeginning, BlockEnding,

        // Literals.
        Identifier, IdentifierDeclaration, String, RawString, Number, Comment,

        // Keywords.
        Keyword,

        Space, EOF
    }
}

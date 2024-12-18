namespace RhoMicro.CodeAnalysis.DslGenerator.Lexing;

#if DSL_GENERATOR
[IncludeFile]
internal
#endif
enum TokenType
{
    Unknown,
    Name,
    Equal,
    Whitespace,
    Slash,
    SlashEqual,
    ParenLeft,
    ParenRight,
    Star,
    Number,
    BracketLeft,
    BracketRight,
    Semicolon,
    Comment,
    NewLine,
    Terminal,
    Dash,
    Period,
    Eof
}

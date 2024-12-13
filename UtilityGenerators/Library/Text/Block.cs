namespace RhoMicro.CodeAnalysis.Library.Text;

[IncludeFile]
internal readonly record struct Block(
    StringOrChar OpeningDelimiter = default,
    StringOrChar ClosingDelimiter = default,
    Boolean PlaceDelimitersOnNewLine = false,
    StringOrChar? Indentation = null);

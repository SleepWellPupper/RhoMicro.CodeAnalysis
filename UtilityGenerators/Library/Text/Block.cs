namespace RhoMicro.CodeAnalysis.Library.Text;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal readonly record struct Block(
    StringOrChar OpeningDelimiter = default,
    StringOrChar ClosingDelimiter = default,
    Boolean PlaceDelimitersOnNewLine = false,
    StringOrChar? Indentation = null);

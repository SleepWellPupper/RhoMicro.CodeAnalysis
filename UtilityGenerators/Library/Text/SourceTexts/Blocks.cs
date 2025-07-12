// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if SOURCETEXTS_LIBRARY
public
#else
internal
#endif
 static partial class Blocks
{
    public static Block Indent { get; } = new();
    public static Block Braces(StringOrChar newLine) => new($"{{{newLine}", $"}}{newLine}", PlaceDelimitersOnNewLine: true);
    public static Block Parentheses { get; } = new('(', ')', Indentation: StringOrChar.Empty);
    public static Block Brackets { get; } = new('[', ']', Indentation: StringOrChar.Empty);
    public static Block CollectionExpr(StringOrChar newLine) => new($"[{newLine}", $"]{newLine}", PlaceDelimitersOnNewLine: true);
    public static Block Angled { get; } = new('<', '>', Indentation: StringOrChar.Empty);
    public static Block Region(String name, StringOrChar newLine) => new($"#region {name}{newLine}", $"#endregion{newLine}", PlaceDelimitersOnNewLine: true, Indentation: StringOrChar.Empty);
}

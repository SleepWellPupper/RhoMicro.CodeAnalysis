// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.DslGenerator.Lexing;

using System.Text.RegularExpressions;

#if DSL_GENERATOR
[IncludeFile]
#endif
[UnionType<String, Char, StringSlice>]
[UnionTypeSettings(ToStringSetting = ToStringSetting.Simple)]
internal readonly partial struct Lexeme : IEquatable<String>, IEquatable<Char>, IEquatable<StringSlice>
{
    public Int32 Length => Switch(
        onString: s => s.Length,
        onChar: s => 1,
        onStringSlice: s => s.Length);

    public static Lexeme Empty { get; } = String.Empty;

    public Boolean Equals(Lexeme other) => Switch(other.Equals, other.Equals, other.Equals);

    public Boolean Equals(Char c) =>
        Switch(
            onString: s => s.Length == 1 && s[0] == c,
            onChar: thisChar => thisChar == c,
            onStringSlice: s => s.Equals(c));

    public Boolean Equals(String s) =>
        Switch(
            onString: thisString => thisString == s,
            onChar: c => s.Length == 1 && s[0] == c,
            onStringSlice: slice => slice.Equals(s));

    public Boolean Equals(StringSlice s) =>
        Switch(s.Equals, s.Equals, s.Equals);

    public String ToEscapedString() =>
        ToString()?
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
     ?? String.Empty;
}

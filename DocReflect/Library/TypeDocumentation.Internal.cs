namespace RhoMicro.CodeAnalysis.DocReflect;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

using static RhoMicro.CodeAnalysis.Library.Text.SourceTexts.IndentedStringBuilder.Appendables;

public partial class Documentation : IIndentedStringBuilderAppendable
{
    void IIndentedStringBuilderAppendable.AppendTo(IndentedStringBuilder builder) => _ = builder.Operators +
        "new " + GetType().FullName + OpenBlock(Blocks.Parentheses with
        {
            Indentation = builder.Options.DefaultIndentation,
            PlaceDelimitersOnNewLine = true
        }) + AppendJoin(TopLevelComments) + CloseBlock();
}

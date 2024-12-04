namespace RhoMicro.CodeAnalysis.DocReflect;
partial class Documentation : IIndentedStringBuilderAppendable
{
    void IIndentedStringBuilderAppendable.AppendTo(IndentedStringBuilder builder) => _ = builder.Operators +
        "new " + GetType().FullName + OpenBlock(Blocks.Parens with
        {
            Indentation = builder.Options.DefaultIndentation,
            PlaceDelimitersOnNewLine = true
        }) + AppendJoin(TopLevelComments) + CloseBlock();
}

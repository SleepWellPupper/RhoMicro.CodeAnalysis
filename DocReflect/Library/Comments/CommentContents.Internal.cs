namespace RhoMicro.CodeAnalysis.DocReflect.Comments;
partial record struct CommentContents : IIndentedStringBuilderAppendable
{
    void IIndentedStringBuilderAppendable.AppendTo(IndentedStringBuilder builder) => _ = builder.Operators +
        "new " + typeof(CommentContents).FullName + '(' + NewLine + "\"\"\"" + NewLine + Text + NewLine + "\"\"\"" + ')';
}

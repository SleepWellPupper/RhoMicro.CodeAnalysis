namespace RhoMicro.CodeAnalysis.DocReflect.Comments;
partial record DocumentationComment : IIndentedStringBuilderAppendable
{
    void IIndentedStringBuilderAppendable.AppendTo(IndentedStringBuilder builder) => _ = builder.Operators +
       "new " + typeof(DocumentationComment).FullName + '(' + Contents + ')';
}

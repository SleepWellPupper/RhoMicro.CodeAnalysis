// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.DocReflect.Comments;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

public partial record DocumentationComment : IIndentedStringBuilderAppendable
{
    void IIndentedStringBuilderAppendable.AppendTo(IndentedStringBuilder builder) => _ = builder.Operators +
       "new " + typeof(DocumentationComment).FullName + '(' + Contents + ')';
}

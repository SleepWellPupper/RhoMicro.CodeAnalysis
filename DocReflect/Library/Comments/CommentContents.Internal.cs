// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.DocReflect.Comments;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

using static RhoMicro.CodeAnalysis.Library.Text.SourceTexts.IndentedStringBuilder.Appendables;

public partial record struct CommentContents : IIndentedStringBuilderAppendable
{
    void IIndentedStringBuilderAppendable.AppendTo(IndentedStringBuilder builder) => _ = builder.Operators +
        "new " + typeof(CommentContents).FullName + '(' + NewLine + "\"\"\"" + NewLine + Text + NewLine + "\"\"\"" + ')';
}

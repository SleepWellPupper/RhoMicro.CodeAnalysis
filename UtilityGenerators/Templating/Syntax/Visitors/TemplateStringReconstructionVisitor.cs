// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Templating;

internal sealed class TemplateStringReconstructionVisitor(String newline, IndentedStringBuilder builder, CancellationToken ct) : TreeWalkingSyntaxVisitor(ct)
{
    protected override void OnToken(Token token)
    {
        Ct.ThrowIfCancellationRequested();

        if(token.Kind is TokenKind.Newline)
            builder.AppendCore(newline);
        else
            builder.AppendCore(token.Lexeme.ToString());
    }
}

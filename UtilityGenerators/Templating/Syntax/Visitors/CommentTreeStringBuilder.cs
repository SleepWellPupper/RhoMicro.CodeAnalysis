// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal abstract class CommentTreeStringBuilder<TSelf> : TreeStringBuilder<TSelf>
    where TSelf : CommentTreeStringBuilder<TSelf>
{
    public CommentTreeStringBuilder(CancellationToken ct)
        : base(ct) => Builder.OpenBlockCore(CommentBlocks.SingleLine);
    public override String ToString()
    {
        Builder.CloseBlockCore();

        return base.ToString();
    }
}

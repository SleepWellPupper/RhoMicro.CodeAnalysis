// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal interface ISyntax
{
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor;
}
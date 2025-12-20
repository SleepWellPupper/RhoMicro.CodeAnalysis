// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.DslGenerator.Grammar;

using System.Diagnostics;

#if DSL_GENERATOR
[IncludeFile]
#endif
[DebuggerDisplay("{ToDisplayString()}")]
internal abstract partial record RuleDefinition(Name Name, Rule Rule) : SyntaxNode
{
    public override String ToString() => base.ToString();
    protected override void AppendCtorArgs(IndentedStringBuilder builder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _ = AppendCtorArg(AppendCtorArg(builder, nameof(Name), Name, cancellationToken).Append(", "), nameof(Rule), Rule, cancellationToken);
    }
}

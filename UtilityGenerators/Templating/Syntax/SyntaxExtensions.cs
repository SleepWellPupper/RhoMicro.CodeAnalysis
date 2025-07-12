// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using System.Diagnostics;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal static class SyntaxExtensions
{
    public static TokenSpans GetSyntaxSpans<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var visitor = new SyntaxSpansCalculatingVisitor(ct);
        syntax.Accept(visitor);
        var result = visitor.GetSpans();

        return result;
    }
    public static String ToCommentDisplayTreeString<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var builder = new CommentDisplayTreeStringBuilder(ct);
        syntax.Accept(builder);
        var result = builder.ToString();

        return result;
    }
    public static String ToDisplayTreeString<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var builder = new DisplayTreeStringBuilder(ct);
        syntax.Accept(builder);
        var result = builder.ToString();

        return result;
    }
    public static String ToCommentXmlTreeString<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var builder = new CommentXmlTreeStringBuilder(ct);
        syntax.Accept(builder);
        var result = builder.ToString();

        return result;
    }
    public static String ToXmlTreeString<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var builder = new XmlTreeStringBuilder(ct);
        syntax.Accept(builder);
        var result = builder.ToString();

        return result;
    }
    public static Int32 CountTokens<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var counter = new TokenCountingVisitor(ct);
        syntax.Accept(counter);
        var result = counter.Count;

        return result;
    }
    [Conditional("DEBUG")]
    public static void DebugValidate<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var counter = new DebugTokenValidator(ct);
        syntax.Accept(counter);
    }
    public static void Validate<TSyntax>(this TSyntax syntax, CancellationToken ct)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var counter = new TokenValidator(ct);
        syntax.Accept(counter);
    }
}
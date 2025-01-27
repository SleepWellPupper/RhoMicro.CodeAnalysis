namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using System.Diagnostics;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal static class SyntaxExtensions
{
    public static String ToAstString<TSyntax>(this TSyntax syntax, CancellationToken ct = default)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var builder = new TreeStringBuilder(ct);
        syntax.Accept(builder);
        var result = builder.ToString();

        return result;
    }
    public static Int32 CountTokens<TSyntax>(this TSyntax syntax, CancellationToken ct = default)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var counter = new TokenCounter(ct);
        syntax.Accept(counter);
        var result = counter.Count;

        return result;
    }
    [Conditional("DEBUG")]
    public static void DebugValidate<TSyntax>(this TSyntax syntax, CancellationToken ct = default)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var counter = new DebugTokenValidator(ct);
        syntax.Accept(counter);
    }
    public static void Validate<TSyntax>(this TSyntax syntax, CancellationToken ct = default)
        where TSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        var counter = new TokenValidator(ct);
        syntax.Accept(counter);
    }
}
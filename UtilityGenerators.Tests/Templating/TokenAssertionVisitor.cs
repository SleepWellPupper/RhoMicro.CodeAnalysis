#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;

using RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal sealed class TokenAssertionVisitor : TokenValidator
{
    private TokenAssertionVisitor(String message)
        : base(TestContext.Current.CancellationToken) => 
        _message = message;

    private readonly String _message;

    protected override void OnError(String error) => Assert.Fail($"{_message}\n{error}");
    public static void Verify<TSyntax>(TSyntax syntax, String message)
        where TSyntax : ISyntax
        => syntax.Accept(new TokenAssertionVisitor(message));
}

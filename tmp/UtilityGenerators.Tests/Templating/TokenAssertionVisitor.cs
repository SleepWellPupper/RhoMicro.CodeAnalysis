#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;

using RhoMicro.CodeAnalysis.Templating;

internal sealed class TokenAssertionVisitor : TokenVerificationVisitor
{
    private TokenAssertionVisitor() { }
    public static TokenAssertionVisitor Instance { get; } = new();
    protected override void OnError(String message) => Assert.Fail(message);
    public void Verify<TSyntax>(TSyntax syntax)
        where TSyntax : ISyntax
        => syntax.Accept(this);
}

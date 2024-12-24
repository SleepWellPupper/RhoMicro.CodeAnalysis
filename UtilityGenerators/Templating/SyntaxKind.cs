namespace RhoMicro.CodeAnalysis.Templating;

internal enum SyntaxKind
{
    Template,
    Value,
    Code,
    Text
}

internal static class SyntaxKindExtensions
{
    public static String ToStringFast(this SyntaxKind kind) => kind switch
    {
        SyntaxKind.Template => nameof(SyntaxKind.Template),
        SyntaxKind.Value => nameof(SyntaxKind.Value),
        SyntaxKind.Code => nameof(SyntaxKind.Code),
        SyntaxKind.Text => nameof(SyntaxKind.Text),
        _ => throw new ArgumentOutOfRangeException($"Unknown syntax kind: '{kind}'")
    };
}

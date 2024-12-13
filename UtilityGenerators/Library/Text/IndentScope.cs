namespace RhoMicro.CodeAnalysis.Library.Text;

[NonEquatable]
[IncludeFile]
internal readonly partial struct IndentScope(IndentedStringBuilder builder) : IDisposable
{
    private readonly IndentedStringBuilder _builder = builder;
    public void Dispose() => _builder.Detent();
}

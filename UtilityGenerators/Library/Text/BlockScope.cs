namespace RhoMicro.CodeAnalysis.Library.Text;

#if UTILITYGENERATORS
[IncludeFile]
[NonEquatable]
#endif
internal readonly partial struct BlockScope(IndentedStringBuilder builder) : IDisposable
{
    private readonly IndentedStringBuilder _builder = builder;
    public void Dispose() => _builder.CloseBlock();
}
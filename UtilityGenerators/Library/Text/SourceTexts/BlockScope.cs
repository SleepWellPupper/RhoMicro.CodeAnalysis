namespace RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if SOURCETEXTS_LIBRARY
public
#else
internal
#endif
 readonly partial struct BlockScope(IndentedStringBuilder builder) : IDisposable
{
    private readonly IndentedStringBuilder _builder = builder;
    public void Dispose() => _builder.CloseBlock();
}
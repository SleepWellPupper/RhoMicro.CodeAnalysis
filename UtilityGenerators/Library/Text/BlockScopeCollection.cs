namespace RhoMicro.CodeAnalysis.Library.Text;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class BlockScopeCollection : IDisposable
{
    private readonly List<BlockScope> _scopes = [];
    public void AddScope(BlockScope scope) => _scopes.Add(scope);
    public void Dispose()
    {
        foreach(var scope in _scopes)
            scope.Dispose();
    }
}

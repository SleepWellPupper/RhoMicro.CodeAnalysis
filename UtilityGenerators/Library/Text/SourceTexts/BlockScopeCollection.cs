// SPDX-License-Identifier: MPL-2.0

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
 sealed partial class BlockScopeCollection : IDisposable
{
    private readonly List<BlockScope> _scopes = [];
    public void AddScope(BlockScope scope) => _scopes.Add(scope);
    public void Dispose()
    {
        foreach(var scope in _scopes)
        {
            scope.Dispose();
        }
    }
}

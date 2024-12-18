namespace RhoMicro.CodeAnalysis.Library.Models;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal readonly partial struct ModelCreationContext(EquatableCollectionFactory collectionFactory, CancellationToken cancellationToken)
{
    public EquatableCollectionFactory CollectionFactory { get; } = collectionFactory;
    public CancellationToken CancellationToken { get; } = cancellationToken;

    public void ThrowIfCancellationRequested() => CancellationToken.ThrowIfCancellationRequested();
}

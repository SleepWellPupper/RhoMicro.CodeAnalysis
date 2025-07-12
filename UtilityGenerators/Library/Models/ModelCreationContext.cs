// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal readonly partial struct ModelCreationContext(EquatableCollectionFactory collectionFactory, CancellationToken cancellationToken) : IDisposable
{
    public EquatableCollectionFactory CollectionFactory { get; } = collectionFactory;
    public CancellationToken CancellationToken { get; } = cancellationToken;

    public static ModelCreationContext CreateDefault(CancellationToken ct) => new(EquatableCollectionFactory.CreateDefault(), ct);

    public void ThrowIfCancellationRequested() => CancellationToken.ThrowIfCancellationRequested();
    public void Dispose() => CollectionFactory.Dispose();
}

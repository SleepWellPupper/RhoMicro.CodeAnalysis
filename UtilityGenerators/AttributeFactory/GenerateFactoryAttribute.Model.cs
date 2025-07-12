// SPDX-License-Identifier: MPL-2.0

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS || RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
namespace RhoMicro.CodeAnalysis;
using System;

using Microsoft.CodeAnalysis;

internal sealed partial class GenerateFactoryAttribute : Attribute
{
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS && !RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
    [IncludeFile]
#endif
    public partial record Model
    {
        [InitializationMethod]
        private void Init(INamedTypeSymbol target, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ExtensionsTypeName ??= $"{target.Name}Extensions";
        }
    }
}
#endif

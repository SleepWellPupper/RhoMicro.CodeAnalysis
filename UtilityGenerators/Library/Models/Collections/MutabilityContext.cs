// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Runtime.CompilerServices;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if RHOMICRO_EMIT_PUBLIC_COLLECTIONS
public
#else
internal 
#endif
sealed partial class MutabilityContext : IDisposable
{
    private Int32 _mutable;
    public Boolean IsImmutable => _mutable == 1;
    public void SetImmutable() => _mutable = 1;
    public void ThrowIfReadOnly([CallerMemberName] String? callerName = null)
    {
        if(IsImmutable)
            throw new InvalidOperationException($"Unable to mutate using '{callerName}' after its mutability context was set to immutable.");
    }
    public void Dispose() => SetImmutable();
}

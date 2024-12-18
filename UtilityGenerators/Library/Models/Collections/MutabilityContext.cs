namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Runtime.CompilerServices;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class MutabilityContext
{
    private Int32 _mutable;
    public Boolean IsImmutable => _mutable == 1;
    public void SetImmutable() => _mutable = 1;
    public void ThrowIfReadOnly([CallerMemberName]String? callerName = null)
    {
        if(IsImmutable)
            throw new InvalidOperationException($"Unable to mutate using '{callerName}' after its mutability context was set to immutable.");
    }
}
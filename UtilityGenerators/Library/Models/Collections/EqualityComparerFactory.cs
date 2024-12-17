namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System.Collections.Generic;

#if UTILITYGENERATORS
[IncludeFile]
[NonEquatable]
#endif
internal partial class EqualityComparerFactory
{
    public static EqualityComparerFactory Default { get; } = new();

    public virtual IEqualityComparer<T> CreateEqualityComparer<T>() => EqualityComparer<T>.Default;
}

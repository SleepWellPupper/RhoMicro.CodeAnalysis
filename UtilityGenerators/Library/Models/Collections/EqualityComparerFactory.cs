// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System.Collections.Generic;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal partial class EqualityComparerFactory
{
    public static EqualityComparerFactory Default { get; } = new();

    public virtual IEqualityComparer<T> CreateEqualityComparer<T>() => EqualityComparer<T>.Default;
}

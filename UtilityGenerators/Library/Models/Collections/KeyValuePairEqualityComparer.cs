// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections.Generic;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class KeyValuePairEqualityComparer<TKey, TValue>(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer) : IEqualityComparer<KeyValuePair<TKey, TValue>>
{
    public KeyValuePairEqualityComparer(IEqualityComparer<TValue> valueComparer) : this(EqualityComparer<TKey>.Default, valueComparer) { }
    public static KeyValuePairEqualityComparer<TKey, TValue> Default { get; } = new(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default);
    public Boolean Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) =>
        keyComparer.Equals(x.Key, y.Key) && valueComparer.Equals(x.Value, y.Value);
    public Int32 GetHashCode(KeyValuePair<TKey, TValue> obj)
    {
        var hc = new HashCode();
        hc.Add(obj.Key, keyComparer);
        hc.Add(obj.Value, valueComparer);
        return hc.ToHashCode();
    }
}

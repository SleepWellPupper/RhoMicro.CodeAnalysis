// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class DictionaryEqualityComparer<TKey, TValue>(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer) : IEqualityComparer<IDictionary<TKey, TValue>>
{
    public DictionaryEqualityComparer(IEqualityComparer<TValue> valueComparer) : this(EqualityComparer<TKey>.Default, valueComparer) { }
    public DictionaryEqualityComparer(IEqualityComparer<TKey> keyComparer) : this(keyComparer, EqualityComparer<TValue>.Default) { }

    private readonly KeyValuePairEqualityComparer<TKey, TValue> _kvpComparer = new(keyComparer, valueComparer);

    public static DictionaryEqualityComparer<TKey, TValue> Default { get; } = new(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default);

    public Boolean Equals(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y) => Equals(x, y);
    [OverloadResolutionPriority(1)]
    public Boolean Equals<TDictionaryX, TDictionaryY>(TDictionaryX x, TDictionaryY y)
        where TDictionaryX : IDictionary<TKey, TValue>
        where TDictionaryY : IDictionary<TKey, TValue>
    {
        if(x.Count != y.Count)
            return false;

        foreach(var kvp in x)
        {
            if(!y.TryGetValue(kvp.Key, out var value) || !valueComparer.Equals(kvp.Value, value))
                return false;
        }

        return true;
    }
    public Int32 GetHashCode(IDictionary<TKey, TValue> obj) => GetHashCode(obj);
    [OverloadResolutionPriority(1)]
    public Int32 GetHashCode<TDictionary>(TDictionary obj)
        where TDictionary : IDictionary<TKey, TValue>
        => EnumerableEqualityComparer.GetHashCode(obj, _kvpComparer);
}

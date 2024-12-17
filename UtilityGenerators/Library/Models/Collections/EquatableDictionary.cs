namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if UTILITYGENERATORS
[IncludeFile]
#endif
internal sealed record EquatableDictionary<TKey, TValue> : EquatableCollection<KeyValuePair<TKey, TValue>, IDictionary<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
{
    public EquatableDictionary(
        IDictionary<TKey, TValue> collection,
        IEqualityComparer<IDictionary<TKey, TValue>> comparer,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, collectionFactory, mutabilityContext)
    {
        Keys = new MutableCollection<TKey>(collection.Keys, mutabilityContext);
        Values = new MutableCollection<TValue>(collection.Values, mutabilityContext);
    }

    public Boolean Equals(EquatableDictionary<TKey, TValue> other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();

    public void Add(TKey key, TValue value)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.Add(key, value);
    }

    public Boolean ContainsKey(TKey key) => Collection.ContainsKey(key);
    public Boolean Remove(TKey key)
    {
        MutabilityContext.ThrowIfReadOnly();
        return Collection.Remove(key);
    }

    public Boolean TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);

    public TValue this[TKey key]
    {
        get => Collection[key];
        set
        {
            MutabilityContext.ThrowIfReadOnly();
            Collection[key] = value;
        }
    }

    public ICollection<TKey> Keys { get; }
    public ICollection<TValue> Values { get; }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
}

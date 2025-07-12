// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections.Generic;
using System.Diagnostics;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
[DebuggerDisplay("Count: {Count}")]
internal sealed record LazyEquatableDictionary<TKey, TValue> : LazyEquatableDictionary<TKey, TValue, LazyEquatableDictionary<TKey, TValue>>
{
    public LazyEquatableDictionary(
        IDictionary<TKey, TValue> collection,
        IEqualityComparer<IDictionary<TKey, TValue>> comparer,
        Func<TKey, LazyEquatableDictionary<TKey, TValue>, TValue> factory,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, factory, null!, collectionFactory, mutabilityContext)
        => State = this;

    public Boolean Equals(LazyEquatableDictionary<TKey, TValue> other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}
internal record LazyEquatableDictionary<TKey, TValue, TState> : EquatableCollection<KeyValuePair<TKey, TValue>, IDictionary<TKey, TValue>>, IDictionary<TKey, TValue>
{
    public LazyEquatableDictionary(
        IDictionary<TKey, TValue> collection,
        IEqualityComparer<IDictionary<TKey, TValue>> comparer,
        Func<TKey, TState, TValue> factory,
        TState state,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, collectionFactory, mutabilityContext)
    {
        Factory = factory;
        State = state;
        Keys = new MutableCollection<TKey>(collection.Keys, mutabilityContext);
        Values = new MutableCollection<TValue>(collection.Values, mutabilityContext);
    }

    public Func<TKey, TState, TValue> Factory { get; }
    public TState State { get; protected init; }

    public virtual Boolean Equals(LazyEquatableDictionary<TKey, TValue, TState> other) => base.Equals(other);
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
        get
        {
            if(!Collection.TryGetValue(key, out var value))
            {
                MutabilityContext.ThrowIfReadOnly();
                Collection[key] = value = Factory.Invoke(key, State);
            }

            return value;
        }
        set
        {
            MutabilityContext.ThrowIfReadOnly();
            Collection[key] = value;
        }
    }

    public ICollection<TKey> Keys { get; }

    public ICollection<TValue> Values { get; }
}

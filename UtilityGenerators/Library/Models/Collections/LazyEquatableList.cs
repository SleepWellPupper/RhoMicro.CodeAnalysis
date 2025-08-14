// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
[CollectionBuilder(typeof(Builder), "Create")]
[DebuggerDisplay("Count: {Count}")]
#if RHOMICRO_EMIT_PUBLIC_COLLECTIONS
public
#else
internal 
#endif
sealed record LazyEquatableList<T> : LazyEquatableList<T, LazyEquatableList<T>>
{
    public LazyEquatableList(
        IList<T> collection,
        IEqualityComparer<IList<T>> comparer,
        Func<Int32, LazyEquatableList<T, LazyEquatableList<T>>, T> factory,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, factory, null!, collectionFactory, mutabilityContext)
        => State = this;

    public Boolean Equals(LazyEquatableList<T> other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}

#if RHOMICRO_EMIT_PUBLIC_COLLECTIONS
public
#else
internal 
#endif
record LazyEquatableList<T, TState> : EquatableCollection<T, IList<T>>, IList<T>, IReadOnlyList<T>
{
    public LazyEquatableList(
        IList<T> collection,
        IEqualityComparer<IList<T>> comparer,
        Func<Int32, LazyEquatableList<T, TState>, T> factory,
        TState state,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, collectionFactory, mutabilityContext)
    {
        State = state;
        Factory = factory;
    }

    public TState State { get; protected init; }
    private Func<Int32, LazyEquatableList<T, TState>, T> Factory { get; }

    public virtual Boolean Equals(LazyEquatableList<T, TState> other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();

    public Int32 IndexOf(T item) => Collection.IndexOf(item);
    public void Insert(Int32 index, T item)
    {
        MutabilityContext.ThrowIfReadOnly();
        FillUpToIndex(index - 1);
        Collection.Insert(index, item);
    }

    public void RemoveAt(Int32 index)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.RemoveAt(index);
    }

    public T this[Int32 index]
    {
        get
        {
            FillUpToIndex(index);
            return Collection[index];
        }
        set
        {
            MutabilityContext.ThrowIfReadOnly();
            FillUpToIndex(index);
            Collection[index] = value;
        }
    }

    private void FillUpToIndex(Int32 index)
    {
        MutabilityContext.ThrowIfReadOnly();
        while(Count <= index)
            Add(Factory.Invoke(Count, this));
    }
}

file static class Builder
{
    public static LazyEquatableList<T?> Create<T>(ReadOnlySpan<T> elements)
    {
        using var ctx = ModelCreationContext.CreateDefault(CancellationToken.None);
        var result = ctx.CollectionFactory.CreateLazyList<T>();

        foreach(var element in elements)
            result.Add(element);

        return result;
    }
}

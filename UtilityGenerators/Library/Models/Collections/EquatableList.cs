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
sealed record EquatableList<T> : EquatableCollection<T, IList<T>>, IList<T>, IReadOnlyList<T>
{
    public EquatableList(
        IList<T> collection,
        IEqualityComparer<IList<T>> comparer,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, collectionFactory, mutabilityContext)
    { }

    public Boolean Equals(EquatableList<T> other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();

    public Int32 IndexOf(T item) => Collection.IndexOf(item);
    public void Insert(Int32 index, T item)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.Insert(index, item);
    }

    public void RemoveAt(Int32 index)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.RemoveAt(index);
    }

    public T this[Int32 index]
    {
        get => Collection[index];
        set
        {
            MutabilityContext.ThrowIfReadOnly();
            Collection[index] = value;
        }
    }
}

file static class Builder
{
    public static EquatableList<T> Create<T>(ReadOnlySpan<T> elements)
    {
        using var ctx = ModelCreationContext.CreateDefault(CancellationToken.None);
        var result = ctx.CollectionFactory.CreateList<T>();

        foreach(var element in elements)
            result.Add(element);

        return result;
    }
}

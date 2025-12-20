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
internal sealed record EquatableSet<T> : EquatableCollection<T, ISet<T>>, ISet<T>
{
    public EquatableSet(
        ISet<T> collection,
        IEqualityComparer<ISet<T>> comparer,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, collectionFactory, mutabilityContext)
    { }

    public Boolean Equals(EquatableSet<T> other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();

    public void ExceptWith(IEnumerable<T> other)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.ExceptWith(other);
    }
    public void IntersectWith(IEnumerable<T> other)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.IntersectWith(other);
    }
    public Boolean IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
    public Boolean IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
    public Boolean IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
    public Boolean IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
    public Boolean Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);
    public Boolean SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.SymmetricExceptWith(other);
    }
    public void UnionWith(IEnumerable<T> other)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.UnionWith(other);
    }
    public new Boolean Add(T item)
    {
        MutabilityContext.ThrowIfReadOnly();
        return Collection.Add(item);
    }
}

file static class Builder
{
    public static EquatableSet<T> Create<T>(ReadOnlySpan<T> elements)
    {
        using var ctx = ModelCreationContext.CreateDefault(CancellationToken.None);
        var result = ctx.CollectionFactory.CreateSet<T>();

        foreach(var element in elements)
        {
            _ = result.Add(element);
        }

        return result;
    }
}

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Collections.Generic;

[IncludeFile]
internal sealed record EquatableList<T> : EquatableCollection<T, IList<T>>, IList<T>
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
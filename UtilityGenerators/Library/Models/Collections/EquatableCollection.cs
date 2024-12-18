namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System.Collections;
using System.Collections.Generic;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal sealed record EquatableCollection<T> : EquatableCollection<T, ICollection<T>>
{
    public EquatableCollection(
        ICollection<T> collection,
        IEqualityComparer<ICollection<T>> comparer,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(collection, comparer, collectionFactory, mutabilityContext)
    { }
}

internal abstract record EquatableCollection<T, TCollection> : MutableCollection, ICollection<T>
    where TCollection : ICollection<T>
{
    public EquatableCollection(
        TCollection collection,
        IEqualityComparer<TCollection> comparer,
        EquatableCollectionFactory collectionFactory,
        MutabilityContext mutabilityContext)
        : base(mutabilityContext)
    {
        CollectionFactory = collectionFactory;
        Collection = collection;
        Comparer = comparer;
    }

    public IEqualityComparer<TCollection> Comparer { get; }
    public EquatableCollectionFactory CollectionFactory { get; }
    protected TCollection Collection { get; }

    public virtual Boolean Equals(EquatableCollection<T, TCollection> other) => Comparer.Equals(other.Collection, Collection);
    public override Int32 GetHashCode() => Comparer.GetHashCode(Collection);

    public void Add(T item)
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.Add(item);
    }

    public void Clear()
    {
        MutabilityContext.ThrowIfReadOnly();
        Collection.Clear();
    }

    public Boolean Contains(T item) => Collection.Contains(item);
    public void CopyTo(T[] array, Int32 arrayIndex) => Collection.CopyTo(array, arrayIndex);
    public Boolean Remove(T item)
    {
        MutabilityContext.ThrowIfReadOnly();
        return Collection.Remove(item);
    }

    public Int32 Count => Collection.Count;

    public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)Collection ).GetEnumerator();
}
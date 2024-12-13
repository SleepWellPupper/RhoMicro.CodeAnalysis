namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Collections.Generic;

[IncludeFile]
internal sealed record EquatableSet<T> : ISet<T>
{
    public EquatableSet(ISet<T> set, IEqualityComparer<ISet<T>> comparer)
    {
        _set = set;
        _comparer = comparer;
    }

    private readonly ISet<T> _set;
    private readonly IEqualityComparer<ISet<T>> _comparer;

    public Boolean Equals(EquatableSet<T> other) => _comparer.Equals(other._set, _set);
    public override Int32 GetHashCode() => _comparer.GetHashCode(_set);

    public Boolean Add(T item) => _set.Add(item);
    public void ExceptWith(IEnumerable<T> other) => _set.ExceptWith(other);
    public void IntersectWith(IEnumerable<T> other) => _set.IntersectWith(other);
    public Boolean IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);
    public Boolean IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);
    public Boolean IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);
    public Boolean IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);
    public Boolean Overlaps(IEnumerable<T> other) => _set.Overlaps(other);
    public Boolean SetEquals(IEnumerable<T> other) => _set.SetEquals(other);
    public void SymmetricExceptWith(IEnumerable<T> other) => _set.SymmetricExceptWith(other);
    public void UnionWith(IEnumerable<T> other) => _set.UnionWith(other);
    void ICollection<T>.Add(T item) => ( (ICollection<T>)_set ).Add(item);
    public void Clear() => _set.Clear();
    public Boolean Contains(T item) => _set.Contains(item);
    public void CopyTo(T[] array, Int32 arrayIndex) => _set.CopyTo(array, arrayIndex);
    public Boolean Remove(T item) => _set.Remove(item);

    public Int32 Count => _set.Count;

    public Boolean IsReadOnly => _set.IsReadOnly;

    public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_set ).GetEnumerator();
}

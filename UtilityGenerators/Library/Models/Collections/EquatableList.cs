namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Collections.Generic;

[IncludeFile]
internal sealed record EquatableList<T> : IList<T>
{
    public EquatableList(IList<T> list, IEqualityComparer<IList<T>> comparer)
    {
        _list = list;
        _comparer = comparer;
    }

    private readonly IList<T> _list;
    private readonly IEqualityComparer<IList<T>> _comparer;

    public Boolean Equals(EquatableList<T> other) => _comparer.Equals(other._list, _list);
    public override Int32 GetHashCode() => _comparer.GetHashCode(_list);

    public Int32 IndexOf(T item) => _list.IndexOf(item);
    public void Insert(Int32 index, T item) => _list.Insert(index, item);
    public void RemoveAt(Int32 index) => _list.RemoveAt(index);

    public T this[Int32 index] { get => _list[index]; set => _list[index] = value; }

    public void Add(T item) => _list.Add(item);
    public void Clear() => _list.Clear();
    public Boolean Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, Int32 arrayIndex) => _list.CopyTo(array, arrayIndex);
    public Boolean Remove(T item) => _list.Remove(item);

    public Int32 Count => _list.Count;

    public Boolean IsReadOnly => _list.IsReadOnly;

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_list ).GetEnumerator();
}

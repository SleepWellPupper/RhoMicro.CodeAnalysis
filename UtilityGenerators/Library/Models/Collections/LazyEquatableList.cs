namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Collections.Generic;

[IncludeFile]
internal sealed record LazyEquatableList<T, TState> : IList<T>
{
    public LazyEquatableList(
        IList<T> list,
        IEqualityComparer<IList<T>> comparer,
        TState state,
        Func<Int32, TState, T> factory)
    {
        _list = list;
        _comparer = comparer;
        _state = state;
        _factory = factory;
    }

    private readonly TState _state;
    private readonly Func<Int32, TState, T> _factory;
    private readonly IList<T> _list;
    private readonly IEqualityComparer<IList<T>> _comparer;

    public Boolean Equals(LazyEquatableList<T, TState> other) => _comparer.Equals(other._list, _list);
    public override Int32 GetHashCode() => _comparer.GetHashCode(_list);

    public Int32 IndexOf(T item) => _list.IndexOf(item);
    public void Insert(Int32 index, T item)
    {
        FillUpToIndex(index - 1);
        _list.Insert(index, item);
    }

    public void RemoveAt(Int32 index) => _list.RemoveAt(index);

    public T this[Int32 index]
    {
        get
        {
            FillUpToIndex(index);
            return _list[index];
        }
        set
        {
            FillUpToIndex(index);
            _list[index] = value;
        }
    }

    private void FillUpToIndex(Int32 index)
    {
        while(Count <= index)
            Add(_factory.Invoke(Count, _state));
    }

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

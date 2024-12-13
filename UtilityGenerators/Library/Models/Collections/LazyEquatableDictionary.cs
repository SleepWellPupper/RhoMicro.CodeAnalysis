namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Collections.Generic;

[IncludeFile]
internal sealed record LazyEquatableDictionary<TKey, TState, TValue> : IDictionary<TKey, TValue>
{
    public LazyEquatableDictionary(
        IDictionary<TKey, TValue> dictionary,
        IEqualityComparer<IDictionary<TKey, TValue>> comparer,
        TState state,
        Func<TKey, TState, TValue> factory)
    {
        _factory = factory;
        _dictionary = dictionary;
        _comparer = comparer;
        _state = state;
    }

    private readonly Func<TKey, TState, TValue> _factory;
    private readonly TState _state;
    private readonly IDictionary<TKey, TValue> _dictionary;
    private readonly IEqualityComparer<IDictionary<TKey, TValue>> _comparer;

    public Boolean Equals(LazyEquatableDictionary<TKey, TState, TValue> other) => _comparer.Equals(_dictionary, other._dictionary);
    public override Int32 GetHashCode() => _comparer.GetHashCode(_dictionary);

    public void Add(TKey key, TValue value) => _dictionary.Add(key, value);
    public Boolean ContainsKey(TKey key) => true;
    public Boolean Remove(TKey key) => _dictionary.Remove(key);
    public Boolean TryGetValue(TKey key, out TValue value)
    {
        value = this[key];
        return true;
    }

    public TValue this[TKey key]
    {
        get
        {
            if(!_dictionary.TryGetValue(key, out var value))
                _dictionary[key] = value = _factory.Invoke(key, _state);
            return value;
        }
        set => _dictionary[key] = value;
    }

    public ICollection<TKey> Keys => _dictionary.Keys;

    public ICollection<TValue> Values => _dictionary.Values;

    public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item);
    public void Clear() => _dictionary.Clear();
    public Boolean Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
    public Boolean Remove(KeyValuePair<TKey, TValue> item) => _dictionary.Remove(item);

    public Int32 Count => _dictionary.Count;

    public Boolean IsReadOnly => _dictionary.IsReadOnly;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_dictionary ).GetEnumerator();
}

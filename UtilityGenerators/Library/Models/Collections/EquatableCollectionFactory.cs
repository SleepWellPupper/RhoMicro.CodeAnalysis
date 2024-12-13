namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System.Collections.Generic;

[NonEquatable]
[IncludeFile]
internal sealed partial class EquatableCollectionFactory(EqualityComparerFactory comparerFactory)
{
    public static EquatableCollectionFactory Default { get; } = new(EqualityComparerFactory.Default);

    public ISet<T> CreateSet<T>()
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new HashSet<T>(elementComparer);
        var comparer = new SetEqualityComparer<T>(elementComparer);
        var result = new EquatableSet<T>(set, comparer);

        return result;
    }

    public IList<T> CreateList<T>()
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        var result = new EquatableList<T>(set, comparer);

        return result;
    }

    public IList<T> CreateLazyList<T, TState>(Func<Int32, TState, T> factory, TState state)
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        var result = new LazyEquatableList<T, TState>(set, comparer, state, factory);

        return result;
    }
    public IList<T> CreateLazyList<T>(Func<Int32, EquatableCollectionFactory, T> factory) => CreateLazyList(factory, this);
    public IList<T?> CreateLazyList<T>() => CreateLazyList(static (_, _) => default(T), this);

    public IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>()
    {
        var keyComparer = comparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = comparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        var result = new EquatableDictionary<TKey, TValue>(dictionary, comparer);

        return result;
    }

    public IDictionary<TKey, TValue> CreateLazyDictionary<TKey, TState, TValue>(
        Func<TKey, TState, TValue> factory,
        TState state)
    {
        var keyComparer = comparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = comparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        var result = new LazyEquatableDictionary<TKey, TState, TValue>(dictionary, comparer, state, factory);

        return result;
    }
    public IDictionary<TKey, TValue> CreateLazyDictionary<TKey, TValue>(Func<TKey, EquatableCollectionFactory, TValue> factory) => CreateLazyDictionary(factory, this);
    public IDictionary<TKey, TValue?> CreateLazyDictionary<TKey, TValue>() => CreateLazyDictionary<TKey, TValue?>(static (_, _) => default);
}

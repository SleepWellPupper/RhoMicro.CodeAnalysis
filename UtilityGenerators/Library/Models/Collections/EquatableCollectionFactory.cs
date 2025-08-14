// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System.Collections.Generic;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if RHOMICRO_EMIT_PUBLIC_COLLECTIONS
public
#else
internal 
#endif
sealed partial class EquatableCollectionFactory(EqualityComparerFactory comparerFactory, MutabilityContext mutabilityContext) : IDisposable
{
    public static EquatableCollectionFactory CreateDefault() => new(EqualityComparerFactory.Default, new());

    public EquatableSet<T> CreateSet<T>()
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new HashSet<T>(elementComparer);
        var comparer = new SetEqualityComparer<T>(elementComparer);
        var result = new EquatableSet<T>(set, comparer, this, mutabilityContext);

        return result;
    }

    public EquatableList<T> CreateList<T>()
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        var result = new EquatableList<T>(set, comparer, this, mutabilityContext);

        return result;
    }

    public EquatableDictionary<TKey, TValue> CreateDictionary<TKey, TValue>()
    {
        var keyComparer = comparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = comparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        var result = new EquatableDictionary<TKey, TValue>(dictionary, comparer, this, mutabilityContext);

        return result;
    }

    public LazyEquatableList<T, TState> CreateLazyList<T, TState>(Func<Int32, LazyEquatableList<T, TState>, T> factory, TState state)
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        var result = new LazyEquatableList<T, TState>(set, comparer, factory, state, this, mutabilityContext);

        return result;
    }
    public LazyEquatableList<T> CreateLazyList<T>(Func<Int32, LazyEquatableList<T, LazyEquatableList<T>>, T> factory)
    {
        var elementComparer = comparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        var result = new LazyEquatableList<T>(set, comparer, factory, this, mutabilityContext);

        return result;
    }
    public LazyEquatableList<T?> CreateLazyList<T>() => CreateLazyList<T?>(static (_, _) => default);

    public LazyEquatableDictionary<TKey, TValue, TState> CreateLazyDictionary<TKey, TValue, TState>(Func<TKey, TState, TValue> factory, TState state)
    {
        var keyComparer = comparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = comparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        var result = new LazyEquatableDictionary<TKey, TValue, TState>(dictionary, comparer, factory, state, this, mutabilityContext);

        return result;
    }
    public LazyEquatableDictionary<TKey, TValue> CreateLazyDictionary<TKey, TValue>(Func<TKey, LazyEquatableDictionary<TKey, TValue>, TValue> factory)
    {
        var keyComparer = comparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = comparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        var result = new LazyEquatableDictionary<TKey, TValue>(dictionary, comparer, factory, this, mutabilityContext);

        return result;
    }
    public LazyEquatableDictionary<TKey, TValue?> CreateLazyDictionary<TKey, TValue>() => CreateLazyDictionary<TKey, TValue?>(static (_, _) => default);

    public void Dispose() => mutabilityContext.Dispose();
}

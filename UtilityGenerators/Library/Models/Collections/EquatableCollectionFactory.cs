namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System.Collections.Generic;
using System.Net.NetworkInformation;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class EquatableCollectionFactory(EqualityComparerFactory comparerFactory)
{
    public static EquatableCollectionFactory Default { get; } = new(EqualityComparerFactory.Default);

    public EqualityComparerFactory ComparerFactory { get; } = comparerFactory;

    public EquatableSet<T> CreateSet<T>(MutabilityContext? mutabilityContext = null)
    {
        var elementComparer = ComparerFactory.CreateEqualityComparer<T>();
        var set = new HashSet<T>(elementComparer);
        var comparer = new SetEqualityComparer<T>(elementComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new EquatableSet<T>(set, comparer, this, mutabilityContext);

        return result;
    }

    public EquatableList<T> CreateList<T>(MutabilityContext? mutabilityContext = null)
    {
        var elementComparer = ComparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new EquatableList<T>(set, comparer, this, mutabilityContext);

        return result;
    }

    public EquatableDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(MutabilityContext? mutabilityContext = null)
    {
        var keyComparer = ComparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = ComparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new EquatableDictionary<TKey, TValue>(dictionary, comparer, this, mutabilityContext);

        return result;
    }

    public LazyEquatableList<T, TState> CreateLazyList<T, TState>(Func<Int32, LazyEquatableList<T, TState>, T> factory, TState state, MutabilityContext? mutabilityContext = null)
    {
        var elementComparer = ComparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new LazyEquatableList<T, TState>(set, comparer, factory, state, this, mutabilityContext);

        return result;
    }
    public LazyEquatableList<T> CreateLazyList<T>(Func<Int32, LazyEquatableList<T, LazyEquatableList<T>>, T> factory, MutabilityContext? mutabilityContext = null)
    {
        var elementComparer = ComparerFactory.CreateEqualityComparer<T>();
        var set = new List<T>();
        var comparer = new EnumerableEqualityComparer<T>(elementComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new LazyEquatableList<T>(set, comparer, factory, this, mutabilityContext);

        return result;
    }
    public LazyEquatableList<T?> CreateLazyList<T>(MutabilityContext? mutabilityContext = null) => CreateLazyList<T?>(static (_, _) => default, mutabilityContext);

    public LazyEquatableDictionary<TKey, TValue, TState> CreateLazyDictionary<TKey, TValue, TState>(Func<TKey, TState, TValue> factory, TState state, MutabilityContext? mutabilityContext = null)
    {
        var keyComparer = ComparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = ComparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new LazyEquatableDictionary<TKey, TValue, TState>(dictionary, comparer, factory, state, this, mutabilityContext);

        return result;
    }
    public LazyEquatableDictionary<TKey, TValue> CreateLazyDictionary<TKey, TValue>(Func<TKey, LazyEquatableDictionary<TKey, TValue>, TValue> factory, MutabilityContext? mutabilityContext = null)
    {
        var keyComparer = ComparerFactory.CreateEqualityComparer<TKey>();
        var valueComparer = ComparerFactory.CreateEqualityComparer<TValue>();
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        var comparer = new DictionaryEqualityComparer<TKey, TValue>(keyComparer, valueComparer);
        mutabilityContext ??= new MutabilityContext();
        var result = new LazyEquatableDictionary<TKey, TValue>(dictionary, comparer, factory, this, mutabilityContext);

        return result;
    }
    public LazyEquatableDictionary<TKey, TValue?> CreateLazyDictionary<TKey, TValue>(MutabilityContext? mutabilityContext = null) => CreateLazyDictionary<TKey, TValue?>(static (_, _) => default, mutabilityContext);
}

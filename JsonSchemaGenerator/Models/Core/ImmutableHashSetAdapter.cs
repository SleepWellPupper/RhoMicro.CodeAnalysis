// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Collections;
using System.Collections.Immutable;

internal sealed class ImmutableHashSetAdapter<T>(HashSet<T> decorated) : IImmutableSet<T>
{
    public Int32 Count { get; }

    public IImmutableSet<T> Clear()
    {
        decorated.Clear();
        return this;
    }
    public Boolean Contains(T value) => decorated.Contains(value);
    public IImmutableSet<T> Add(T value)
    {
        _ = decorated.Add(value);
        return this;
    }

    public IImmutableSet<T> Remove(T value)
    {
        _ = decorated.Remove(value);
        return this;
    }

    public Boolean TryGetValue(T equalValue, out T actualValue)
    {
        actualValue = equalValue;
        var result = decorated.Contains(equalValue);

        return result;
    }
    public IImmutableSet<T> Intersect(IEnumerable<T> other)
    {
        decorated.IntersectWith(other);
        return this;
    }
    public IImmutableSet<T> Except(IEnumerable<T> other)
    {
        decorated.ExceptWith(other);
        return this;
    }
    public IImmutableSet<T> SymmetricExcept(IEnumerable<T> other)
    {
        decorated.SymmetricExceptWith(other);
        return this;
    }
    public IImmutableSet<T> Union(IEnumerable<T> other)
    {
        decorated.UnionWith(other);
        return this;
    }
    public Boolean SetEquals(IEnumerable<T> other) => decorated.SetEquals(other);
    public Boolean IsProperSubsetOf(IEnumerable<T> other) => decorated.IsProperSubsetOf(other);
    public Boolean IsProperSupersetOf(IEnumerable<T> other) => decorated.IsProperSupersetOf(other);
    public Boolean IsSubsetOf(IEnumerable<T> other) => decorated.IsSubsetOf(other);
    public Boolean IsSupersetOf(IEnumerable<T> other) => decorated.IsSupersetOf(other);
    public Boolean Overlaps(IEnumerable<T> other) => decorated.Overlaps(other);
    public IEnumerator<T> GetEnumerator() => decorated.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)decorated ).GetEnumerator();
}
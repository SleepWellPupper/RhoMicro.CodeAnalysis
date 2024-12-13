namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[IncludeFile]
internal static class EnumerableEqualityComparer
{
    public static Int32 GetHashCode<TEnumerable, T>(TEnumerable obj, IEqualityComparer<T> elementComparer)
        where TEnumerable : IEnumerable<T>
    {
        var hc = new HashCode();

        foreach(var element in obj)
            hc.Add(element, elementComparer);

        return hc.ToHashCode();
    }
}

[NonEquatable]
internal sealed partial class EnumerableEqualityComparer<T>(IEqualityComparer<T> elementComparer) : IEqualityComparer<IEnumerable<T>>
{
    public static EnumerableEqualityComparer<T> Default { get; } = new(EqualityComparer<T>.Default);

    public Boolean Equals(IEnumerable<T> x, IEnumerable<T> y) => Equals(x, y);
    [OverloadResolutionPriority(1)]
    public Boolean Equals<TEnumerableX, TEnumerableY>(TEnumerableX x, TEnumerableY y)
        where TEnumerableX : IEnumerable<T>
        where TEnumerableY : IEnumerable<T>
    {
        if(x.GetType() != y.GetType())
            return false;

        var result = x.SequenceEqual(y, elementComparer);

        return result;
    }
    public Int32 GetHashCode(IEnumerable<T> obj) => GetHashCode(obj);
    [OverloadResolutionPriority(1)]
    public Int32 GetHashCode<TEnumerable>(TEnumerable obj)
        where TEnumerable : IEnumerable<T>
        => EnumerableEqualityComparer.GetHashCode(obj, elementComparer);
}
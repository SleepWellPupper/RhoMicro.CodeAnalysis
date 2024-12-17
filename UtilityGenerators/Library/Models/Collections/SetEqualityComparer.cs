namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

#if UTILITYGENERATORS
[IncludeFile]
[NonEquatable]
#endif
internal sealed partial class SetEqualityComparer<T>(IEqualityComparer<T> elementComparer) : IEqualityComparer<ISet<T>>
{
    public static SetEqualityComparer<T> Default { get; } = new(EqualityComparer<T>.Default);

    public Boolean Equals(ISet<T> x, ISet<T> y) => Equals(x, y);
    [OverloadResolutionPriority(1)]
    public Boolean Equals<TSetX, TSetY>(TSetX x, TSetY y)
        where TSetX : ISet<T>
        where TSetY : ISet<T>
    {
        if(x.Count != y.Count)
            return false;

        foreach(var element in x)
        {
            if(!y.Contains(element, elementComparer))
                return false;
        }

        return true;
    }
    public Int32 GetHashCode(ISet<T> obj) => GetHashCode(obj);
    [OverloadResolutionPriority(1)]
    public Int32 GetHashCode<TSet>(TSet obj)
        where TSet : ISet<T>
        => EnumerableEqualityComparer.GetHashCode(obj, elementComparer);
}

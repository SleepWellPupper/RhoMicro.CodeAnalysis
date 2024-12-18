namespace RhoMicro.CodeAnalysis.Library.Models.Collections;

using System;
using System.Collections;
using System.Runtime.CompilerServices;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal abstract partial record MutableCollection(MutabilityContext MutabilityContext)
{
    public MutableCollection() : this(new MutabilityContext()) { }
    public Boolean IsReadOnly => MutabilityContext.IsImmutable;

    public override Int32 GetHashCode() => throw new NotSupportedException($"{typeof(MutableCollection)}.GetHashCode() is not supported.");
    public virtual Boolean Equals(MutableCollection other) => throw new NotSupportedException($"{typeof(MutableCollection)}.Equals({typeof(MutableCollection)}) is not supported.");
}

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[NonEquatable]
#endif
internal partial class MutableCollection<T>(ICollection<T> wrapped, MutabilityContext mutabilityContext) : ICollection<T>
{
    public void Add(T item)
    {
        mutabilityContext.ThrowIfReadOnly();
        wrapped.Add(item);
    }

    public void Clear()
    {
        mutabilityContext.ThrowIfReadOnly();
        wrapped.Clear();
    }

    public Boolean Contains(T item) => wrapped.Contains(item);
    public void CopyTo(T[] array, Int32 arrayIndex) => wrapped.CopyTo(array, arrayIndex);
    public Boolean Remove(T item)
    {
        mutabilityContext.ThrowIfReadOnly();
        return wrapped.Remove(item);
    }

    public Int32 Count => wrapped.Count;

    public Boolean IsReadOnly => mutabilityContext.IsImmutable;

    public IEnumerator<T> GetEnumerator() => wrapped.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)wrapped ).GetEnumerator();
}
namespace RhoMicro.CodeAnalysis.UnionsGenerator.Utils;
using System;
using System.Collections.Generic;

static class EnumerableExtensions
{
    public static IEnumerable<T> DistinctBy<T, TDiscriminator>(this IEnumerable<T> values, Func<T, TDiscriminator> discriminatorSelector)
    {
        var yieldedValueDiscriminators = new HashSet<TDiscriminator>();
        foreach(var value in values)
        {
            var discriminator = discriminatorSelector.Invoke(value);
            if(yieldedValueDiscriminators.Add(discriminator))
                yield return value;
        }
    }
}

namespace RhoMicro.CodeAnalysis.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal static class KeyValuePairExtensions
{
    [OverloadResolutionPriority(1)]
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }
    public static void Deconstruct(this KeyValuePair<String, TypedConstant> kvp, out String key, out TypedConstant value) =>
        kvp.Deconstruct(out key, out value);
}

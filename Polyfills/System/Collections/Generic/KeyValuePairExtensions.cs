// SPDX-License-Identifier: MPL-2.0

namespace System.Collections.Generic;

/// <summary>
/// Provides extensions for instances of <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
#if RHOMICRO_CODEANALYSIS_POLYFILLS
[RhoMicro.CodeAnalysis.IncludeFile]
#endif
internal static class KeyValuePairExtensions
{
    extension<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
    {
        /// <summary>
        /// Deconstructs the <see cref="KeyValuePair{TKey, TValue}"/> into its key and value.
        /// </summary>
        /// <param name="key">
        /// The key contained in the <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </param>
        /// <param name="value">
        /// The value contained in the <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </param>
        public void Deconstruct(out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}

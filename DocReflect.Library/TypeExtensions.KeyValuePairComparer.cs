// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.DocReflect;
public static partial class Extensions
{
    private sealed class KeyValuePairComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        public KeyValuePairComparer() { }

        private static readonly EqualityComparer<TKey> _keyComparer = EqualityComparer<TKey>.Default;

        public Boolean Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) =>
            _keyComparer.Equals(x.Key, y.Key);

        public Int32 GetHashCode(KeyValuePair<TKey, TValue> obj) =>
            _keyComparer.GetHashCode(obj.Key);
    }
}

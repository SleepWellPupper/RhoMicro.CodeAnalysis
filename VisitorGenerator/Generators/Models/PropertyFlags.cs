// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Flags]
internal enum PropertyFlags
{
    None = 0,
    IsEnumerable = 1 << 0,
    IsNullable = 1 << 1,
}

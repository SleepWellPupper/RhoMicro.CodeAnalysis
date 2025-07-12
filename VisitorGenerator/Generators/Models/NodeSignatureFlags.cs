// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Flags]
internal enum NodeSignatureFlags
{
    None = 0,
    IsRecord = 1 << 0,
    IsPublic = 1 << 1,
    IsSealed = 1 << 2
}

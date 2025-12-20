// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Tests.E2E;

[GenerateFactory]
sealed partial class TestAttribute
{
    public Type[] TypeArrayProperty { get; set; } = [];
    public Type?[] NullableTypeArrayProperty { get; set; } = [];
    public Type[]? TypeNullableArrayProperty { get; set; } = [];
    public Type?[]? NullableTypeNullableArrayProperty { get; set; } = [];
}

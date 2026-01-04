// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

public partial class UnionTypeVariantTests
{
    [UnionType<String>]
    partial struct Text;

    [UnionType<Text, Int32>]
    partial struct Union;

    [Fact]
    public void ManagedUnionTypeStructVariantDoesNotThrowTle()
    {
        Text text = "foo";
        Union union = text;
        Assert.Equal(text, union.CastToText);
    }
}

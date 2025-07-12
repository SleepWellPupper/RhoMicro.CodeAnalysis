// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.UnionsGenerator.EndToEnd.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class ToStringTests
{
    [UnionType<String>]
    [UnionTypeSettings(ToStringSetting = ToStringSetting.None)]
    private partial class NoToStringUnionType;

    [Fact]
    public void UsesDefaultToString()
    {
        NoToStringUnionType u = "Foo";
        var expected = typeof(NoToStringUnionType).FullName;
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }

    [UnionType<String>]
    private partial class CustomToStringUnionType
    {
        public override String ToString() => "Foo";
    }

    [Fact]
    public void UsesCustomToString()
    {
        CustomToStringUnionType u = "Bar";
        var expected = "Foo";
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }
}

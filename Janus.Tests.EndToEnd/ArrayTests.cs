// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

#pragma warning disable CS1591
public partial class ArrayTests
{
    [UnionType<int[]>]
    sealed partial class Union;

    [Fact]
    public void HasReadableName()
    {
        Union u = new int[] { 0 };
        
        Assert.True(u.IsInt32Array);
    }
}

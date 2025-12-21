// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;
using System;

public partial class FactoryTests
{
    [UnionType<Int32>]
    private sealed partial class UnnamedFactoryUnion;

    [Fact]
    public void UsesDefaultFactoryName()
    {
        _ = UnnamedFactoryUnion.Create(0);
    }
}

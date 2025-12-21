// SPDX-License-Identifier: MPL-2.0

namespace Janus.Tests;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using RhoMicro.CodeAnalysis.Janus;

public class JanusCodeFixProviderTests
{
    [Fact]
    public Task ClassUnionsShouldBeSealed() =>
        JanusTest.TestCodeFix(
            source:
            """
            using RhoMicro.CodeAnalysis;

            [UnionType<int>]
            partial class {|RMJ0018:Union|};
            """,
            fixedSource:
            """
            using RhoMicro.CodeAnalysis;

            [UnionType<int>]
            sealed partial class Union;
            """);
}

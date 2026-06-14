namespace NoBang.Tests;

using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NoBang;

public class AnalyzerTests
{
    private sealed class NoBangAnalyzerTest : CSharpAnalyzerTest<Analyzer, DefaultVerifier>
    {
        public NoBangAnalyzerTest(String source)
        {
            TestState.Sources.Add(source);
            TestState.ReferenceAssemblies =
                Microsoft.CodeAnalysis.Testing.ReferenceAssemblies.NetStandard.NetStandard20;
        }
    }

    [Fact]
    public Task DoesNotReportWithoutBangOperator() =>
        new NoBangAnalyzerTest(
                """
                #nullable enable

                public static class C
                {
                    public static string M(string? value) => value ?? "";
                }
                """)
            .RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public Task ReportsBangOperatorInDifferentExpressions() =>
        new NoBangAnalyzerTest(
                """
                #nullable enable

                public static class C
                {
                    public static string M(string? value)
                    {
                        var first = {|RMNB0001:value!|};
                        var second = {|RMNB0001:(value?.Trim())!|};
                        return {|RMNB0001:value!|}.Trim();
                    }
                }
                """)
            .RunAsync(TestContext.Current.CancellationToken);
}

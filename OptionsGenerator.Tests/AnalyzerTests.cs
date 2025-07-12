// SPDX-License-Identifier: MPL-2.0

namespace OptionsGenerator.Tests;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

using RhoMicro.CodeAnalysis.OptionsGenerator.Analyzers;

public class AnalyzerTests
{
    private sealed class OptionsGeneratorAnalyzerTest
        : CSharpAnalyzerTest<OptionsAnalyzer, DefaultVerifier>
    {
        public OptionsGeneratorAnalyzerTest(
            [StringSyntax("c#-test")] String source,
            params String[] expectedDiagnostics)
        {
            TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck;
            TestState.Sources.Add(source);
            TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages(
                [
                    new PackageIdentity("Microsoft.Extensions.Options", "8.0.2"),
                    new PackageIdentity("Microsoft.Extensions.Options.ConfigurationExtensions", "8.0.0")
                ]);
            ExpectedDiagnostics.AddRange(expectedDiagnostics.Select(DiagnosticResult.CompilerError));
        }

        protected override IEnumerable<Type> GetSourceGenerators() =>
            [typeof(RhoMicro.CodeAnalysis.OptionsGenerator.Generators.OptionsGenerator)];
    }

    [Fact]
    public Task NonTargetWritablePropertyDoesNotRaise_ROG0002() =>
        new OptionsGeneratorAnalyzerTest(
            $$"""
              public partial interface IFoo
              {
                  string Property { get; set; }
              }
              """).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public Task ReadOnlyPropertyDoesNotRaise_ROG0002() =>
        new OptionsGeneratorAnalyzerTest(
            $$"""
              using RhoMicro.CodeAnalysis;

              [Options]
              public partial interface IFoo
              {
                  string Property { get; }
              }
              """).RunAsync(TestContext.Current.CancellationToken);
    [Fact]
    public Task WritablePropertyRaises_ROG0002() =>
        new OptionsGeneratorAnalyzerTest(
            $$"""
              using RhoMicro.CodeAnalysis;

              [Options]
              public partial interface IFoo
              {
                  string {|ROG0002:Property|} { get; set; }
              }
              """).RunAsync(TestContext.Current.CancellationToken);


    [Theory]
    [InlineData("T")]
    [InlineData("T, S")]
    [InlineData("TName")]
    [InlineData("TElement, TName")]
    public Task NonTargetGenericInterfaceDoesNotRaise_ROG0001(String typeParameters) =>
        new OptionsGeneratorAnalyzerTest(
                $"public partial interface IFoo<{typeParameters}>;")
            .RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public Task NestedTargetInterfaceRaises_ROG0003() =>
        new OptionsGeneratorAnalyzerTest(
            """
            using RhoMicro.CodeAnalysis;

            public class Bar
            {
                [Options]
                public partial interface {|ROG0003:IFoo|};
            }
            """).RunAsync(TestContext.Current.CancellationToken);

    [Theory]
    [InlineData("T")]
    [InlineData("T, S")]
    [InlineData("TName")]
    [InlineData("TElement, TName")]
    public Task GenericTargetInterfaceRaises_ROG0001(String typeParameters) =>
        new OptionsGeneratorAnalyzerTest(
            $$"""
              using RhoMicro.CodeAnalysis;

              [Options]
              public partial interface {|ROG0001:IFoo|}<{{typeParameters}}>;
              """).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public Task NonPartialTargetInterfaceRaises_CS0260() =>
        new OptionsGeneratorAnalyzerTest(
            $$"""
              using RhoMicro.CodeAnalysis;

              [Options]
              public interface {|CS0260:IFoo|}
              {
                  string Property { get; }
              }
              """)
            .RunAsync(TestContext.Current.CancellationToken);
}

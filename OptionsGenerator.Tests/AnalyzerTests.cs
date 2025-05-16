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
            params String[] ignoredDiagnostics)
        {
            TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck;
            TestState.Sources.Add(source);
            TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages(
                [
                    new PackageIdentity("Microsoft.Extensions.Options", "8.0.2"),
                    new PackageIdentity("Microsoft.Extensions.Options.ConfigurationExtensions", "8.0.0")
                ]);
            ExpectedDiagnostics.AddRange(ignoredDiagnostics.Select(DiagnosticResult.CompilerError));
        }

        protected override IEnumerable<Type> GetSourceGenerators() =>
            [typeof(RhoMicro.CodeAnalysis.OptionsGenerator.Generators.OptionsGenerator)];
    }

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
              """/*,"CS0535", "CS0535", "CS0535", "CS0535"*/).RunAsync(TestContext.Current.CancellationToken);

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
}
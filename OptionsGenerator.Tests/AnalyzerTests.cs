namespace OptionsGenerator.Tests;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

using RhoMicro.CodeAnalysis.OptionsGenerator.Analyzers;

public class AnalyzerTests
{
    private sealed class OptionsGeneratorAnalyzerTest
        : CSharpAnalyzerTest<OptionsAnalyzer, DefaultVerifier>
    {
        public OptionsGeneratorAnalyzerTest([StringSyntax("c#-test")] String source)
        {
            TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck;
            TestState.Sources.Add(source);
            TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        }

        protected override IEnumerable<Type> GetSourceGenerators() =>
            [typeof(RhoMicro.CodeAnalysis.OptionsGenerator.Generators.OptionsGenerator)];
    }

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
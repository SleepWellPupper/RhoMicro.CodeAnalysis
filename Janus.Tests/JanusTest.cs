// SPDX-License-Identifier: MPL-2.0

namespace Janus.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Janus;

class JanusTest : CSharpCodeFixTest<JanusAnalyzer, JanusCodeFixProvider, DefaultVerifier>
{
    public JanusTest()
    {
        TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck;
        ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
        TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(IUnion).Assembly.Location));
    }

    public static Task TestCodeFix(String source, String fixedSource)
    {
        return CodeFixVerifier<JanusAnalyzer, JanusCodeFixProvider, JanusTest, DefaultVerifier>
            .VerifyCodeFixAsync(source, fixedSource)
            .WaitAsync(TestContext.Current.CancellationToken);
    }

    public static Task TestAnalyzer(String source)
    {
        return CodeFixVerifier<JanusAnalyzer, JanusCodeFixProvider, JanusTest, DefaultVerifier>
            .VerifyAnalyzerAsync(source)
            .WaitAsync(TestContext.Current.CancellationToken);
    }

    protected override IEnumerable<Type> GetSourceGenerators() =>
    [
        typeof(JanusGenerator),
        typeof(OverloadResolutionPriorityAttributeGenerator) // TODO: remove when net10 is supported by MS.CA.Testing.ReferenceAssemblies
    ];
}

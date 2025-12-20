// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Basic.Reference.Assemblies;
using System.Text;
using System.Collections.Immutable;
using System.Numerics;
using System.ComponentModel;
using System.Globalization;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class UtilityGeneratorTestAssertion : Attribute;

public abstract class TestBase<TGenerator>
    where TGenerator : IIncrementalGenerator, new()
{
    protected TestBase() : this(Net80.References.All.ToArray()) { }
    protected TestBase(IEnumerable<MetadataReference> references) =>
        _references =
        [
            .. references,
            MetadataReference.CreateFromFile(typeof(SyntaxNode).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(CSharpSyntaxNode).Assembly.Location)
        ];

    private readonly MetadataReference[] _references;

    //requiring >= C#11 due to file scoped modifiers
    private const LanguageVersion _targetLanguageVersion = LanguageVersion.Preview;
    private static readonly CSharpParseOptions _parseOptions =
        new(languageVersion: _targetLanguageVersion,
            documentationMode: DocumentationMode.Diagnose,
            kind: SourceCodeKind.Regular);

    protected void TestFactory(String source, String assertion, CancellationToken ct)
    {
        Compilation compilation = CreateCompilation(ct, source, assertion);
        var runResult = RunGenerator(ref compilation, ct);

        if(runResult.Diagnostics.Where(d => d.IsWarningAsError || d.Severity is DiagnosticSeverity.Error).Any())
        {
            FailDiagnostics("Generator run produced diagnostics:", runResult.Diagnostics);
        }
    }
    protected GeneratorDriverRunResult RunGenerator(ref Compilation compilation, CancellationToken ct)
    {
        var driver = CSharpGeneratorDriver.Create(
                new FileInclusionGenerator(),
                new TGenerator())
            .WithUpdatedParseOptions(_parseOptions);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out compilation,
            out var diagnostics,
            cancellationToken: ct);

        if(!diagnostics.IsEmpty)
        {
            FailDiagnostics("Generator produced diagnostics:", diagnostics);
        }

        var aggregateDiagnostics = compilation
            .GetDiagnostics(ct)
            .Where(d => d is { IsWarningAsError: true } or { Severity: DiagnosticSeverity.Error })
            .GroupBy(d => d.Location.SourceTree?.FilePath ?? String.Empty)
            .Select(g => (key: g.Key, diagnostics: g.ToImmutableArray()))
            .ToImmutableArray();

        var result = driver.GetRunResult();

        if(aggregateDiagnostics.Length > 0)
        {
            var messageBuilder = new StringBuilder();
            var trees = compilation.SyntaxTrees.ToDictionary(t => t.FilePath);

            foreach(var diagnosticGroup in aggregateDiagnostics)
            {
                _ = messageBuilder.Append(diagnosticGroup.diagnostics.Length).Append(" diagnostics in ").AppendLine(diagnosticGroup.key);

                foreach(var diagnostic in diagnosticGroup.diagnostics)
                {
                    _ = messageBuilder.AppendLine(diagnostic.ToString().Split(".g.cs").Last());
                }

                if(trees.TryGetValue(diagnosticGroup.key, out var tree))
                {
                    _ = messageBuilder.AppendLine("Source:");

                    var lines = tree.ToString().Split(["\n", "\r\n"], StringSplitOptions.None);

                    var padding = lines.Length.ToString(CultureInfo.InvariantCulture).Length;

                    for(var i = 0; i < lines.Length; i++)
                    {
                        _ = messageBuilder
                            .Append(( i + 1 ).ToString(CultureInfo.InvariantCulture).PadLeft(padding, ' '))
                            .Append("  ")
                            .AppendLine(lines[i]);
                    }
                }
            }

            Assert.Fail(messageBuilder.ToString());
        }

        return result;
    }

    private static void FailDiagnostics(String message, IEnumerable<Diagnostic> diagnostics)
    {
        Assert.Fail($"{message}\n{String.Join("\n", diagnostics
                        .OrderBy(d => d.Location.GetLineSpan().StartLinePosition.Line)
                        .ThenBy(d => d.Location.GetLineSpan().StartLinePosition.Character))}");
    }
    private static Int32 _sourceIndex;
    protected CSharpCompilation CreateCompilation(CancellationToken ct, params String[] sources)
    {
        var options = CreateCompilationOptions();
        var syntaxTrees = sources
            .Append(
            """
            #pragma warning disable
            global using global::System;
            global using global::System.Collections.Generic;
            global using global::System.IO;
            global using global::System.Linq;
            global using global::System.Net.Http;
            global using global::System.Threading;
            global using global::System.Threading.Tasks;
            global using global::System.Runtime.CompilerServices;
            """)
            .Append(
            """
            #pragma warning disable
            namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests;

            [System.AttributeUsage(System.AttributeTargets.Class)]
            internal sealed class UtilityGeneratorTestAssertion : System.Attribute;
            """
            )
            .Append(
            """
            // <auto-generated/>
            #pragma warning disable
            #nullable enable annotations

            // Licensed to the .NET Foundation under one or more agreements.
            // The .NET Foundation licenses this file to you under the MIT license.

            namespace System.Diagnostics.CodeAnalysis
            {
                /// <summary>
                /// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter will not be null even if the corresponding type allows it.
                /// </summary>
                [global::System.AttributeUsage(global::System.AttributeTargets.Parameter, Inherited = false)]
                [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                internal sealed class NotNullWhenAttribute : global::System.Attribute
                {
                    /// <summary>
                    /// Initializes the attribute with the specified return value condition.
                    /// </summary>
                    /// <param name="returnValue">The return value condition. If the method returns this value, the associated parameter will not be null.</param>
                    public NotNullWhenAttribute(bool returnValue)
                    {
                        ReturnValue = returnValue;
                    }

                    /// <summary>Gets the return value condition.</summary>
                    public bool ReturnValue { get; }
                }
                [global::System.AttributeUsage(
                    global::System.AttributeTargets.Method |
                    global::System.AttributeTargets.Property,
                    Inherited = false, AllowMultiple = true)]
                [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                internal sealed class MemberNotNullWhenAttribute : global::System.Attribute
                {
                    /// <summary>
                    /// Initializes the attribute with the specified return value condition and a field or property member.
                    /// </summary>
                    /// <param name="returnValue">The return value condition. If the method returns this value, the associated parameter will not be null.</param>
                    /// <param name="member">The field or property member that is promised to be not-null.</param>
                    public MemberNotNullWhenAttribute(bool returnValue, string member)
                    {
                        ReturnValue = returnValue;
                        Members = new[] { member };
                    }

                    /// <summary>
                    /// Initializes the attribute with the specified return value condition and list of field and property members.
                    /// </summary>
                    /// <param name="returnValue">The return value condition. If the method returns this value, the associated parameter will not be null.</param>
                    /// <param name="members">The list of field and property members that are promised to be not-null.</param>
                    public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
                    {
                        ReturnValue = returnValue;
                        Members = members;
                    }

                    /// <summary>
                    /// Gets the return value condition.
                    /// </summary>
                    public bool ReturnValue { get; }

                    /// <summary>
                    /// Gets field or property member names.
                    /// </summary>
                    public string[] Members { get; }
                }
            }
            """)
            .Append(
            """
            // <auto-generated/>
            #pragma warning disable
            #nullable enable annotations

            // Licensed to the .NET Foundation under one or more agreements.
            // The .NET Foundation licenses this file to you under the MIT license.

            namespace System.Runtime.CompilerServices
            {
                /// <summary>
                /// Specifies the priority of a member in overload resolution. When unspecified, the default priority is 0.
                /// </summary>
                [global::System.AttributeUsage(
                    global::System.AttributeTargets.Method |
                    global::System.AttributeTargets.Constructor |
                    global::System.AttributeTargets.Property,
                    AllowMultiple = false,
                    Inherited = false)]
                [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                internal sealed class OverloadResolutionPriorityAttribute : global::System.Attribute
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="global::System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute"/> class.
                    /// </summary>
                    /// <param name="priority">The priority of the attributed member. Higher numbers are prioritized, lower numbers are deprioritized. 0 is the default if no attribute is present.</param>
                    public OverloadResolutionPriorityAttribute(int priority)
                    {
                        Priority = priority;
                    }

                    /// <summary>
                    /// The priority of the member.
                    /// </summary>
                    public int Priority { get; }
                }
            }
            """)
            .Select(s => CSharpSyntaxTree.ParseText(
                s,
                _parseOptions,
                path: $"SourceText_{Interlocked.Increment(ref _sourceIndex).ToString(CultureInfo.InvariantCulture)}.g.cs",
                cancellationToken: ct));

        var result = CSharpCompilation.Create(
            assemblyName: $"TestAssembly_{Interlocked.Increment(ref _testAssemblyCount)}",
            syntaxTrees: syntaxTrees,
            references: [.. _references],
            options: options);

        return result;
    }
    private static Int32 _testAssemblyCount;
    private static CSharpCompilationOptions CreateCompilationOptions()
    {
        String[] args = ["/warnaserror"];
#pragma warning disable RS1035 // Do not use APIs banned for analyzers (not an analyzer)
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
        var result = commandLineArguments.CompilationOptions
            .WithOutputKind(OutputKind.DynamicallyLinkedLibrary)
            .WithAllowUnsafe(enabled: true);

        return result;
    }
}

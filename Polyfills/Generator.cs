namespace RhoMicro.CodeAnalysis.Polyfills;

using Generated;
using Microsoft.CodeAnalysis;

/// <summary>
/// Generates polyfills for netstandard2 libraries.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
        => IncludedFileSources.RegisterToContext(context);
}

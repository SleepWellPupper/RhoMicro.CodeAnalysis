// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using Generated;
using Microsoft.CodeAnalysis;

/// <summary>
/// Generates sources for using the <see cref="CSharpSourceBuilder"/> and related types.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
        => IncludedFileSources.RegisterToContext(context);
}

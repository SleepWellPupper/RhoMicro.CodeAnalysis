// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Cli;
using System;

/// <summary>
/// Provides settings for the <see cref="MainService"/>.
/// </summary>
public sealed class Settings
{
    /// <summary>
    /// Gets or sets the path of the assembly to emit generated json schemata for.
    /// </summary>
    public required String AssemblyPath { get; set; }
    /// <summary>
    /// Gets or sets the path to output generated schemata into.
    /// </summary>
    public required String SchemataPath { get; set; }
}
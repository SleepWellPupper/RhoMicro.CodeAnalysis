#pragma warning disable
namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Marks the annotated property to be excluded from json schema generation.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[GenerateFactory]
[IncludeFile]
#endif
internal sealed partial class JsonSchemaExcludeAttribute : Attribute { }
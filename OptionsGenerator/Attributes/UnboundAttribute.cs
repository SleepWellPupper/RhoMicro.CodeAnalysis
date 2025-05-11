namespace RhoMicro.CodeAnalysis;

using System;

/// <summary>
/// Marks the target property to be excluded from generated classes.
/// </summary>
#if RHOMICRO_CODEANALYSIS_OPTIONSGENERATOR
[IncludeFile]
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
#if GENERATOR
[NonEquatable]
#endif
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed partial class UnboundAttribute : Attribute;

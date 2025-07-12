// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

/// <summary>
/// Marks the target interface for options generation.
/// </summary>
#if RHOMICRO_CODEANALYSIS_OPTIONSGENERATOR
[IncludeFile]
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
#if GENERATOR
[NonEquatable]
#endif
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
internal sealed partial class OptionsAttribute : Attribute;

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

#if DSL_GENERATOR
[GenerateFactory]
#endif
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed partial class GeneratedGrammarAttribute : Attribute;

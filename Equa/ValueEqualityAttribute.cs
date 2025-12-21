// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

#if RHOMICRO_CODEANALYSIS_EQUA
[RhoMicro.CodeAnalysis.IncludeFile]
[RhoMicro.CodeAnalysis.GenerateFactory]
#endif
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed partial class ValueEqualityAttribute : Attribute;

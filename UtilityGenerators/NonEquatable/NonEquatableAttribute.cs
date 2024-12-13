namespace RhoMicro.CodeAnalysis;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[NonEquatable]
[IncludeFile]
#endif
internal sealed partial class NonEquatableAttribute : Attribute;

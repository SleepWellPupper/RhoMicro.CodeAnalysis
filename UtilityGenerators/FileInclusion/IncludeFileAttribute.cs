namespace RhoMicro.CodeAnalysis;

using System;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
#if GENERATOR
[NonEquatable]
[IncludeFile]
#endif
internal sealed partial class IncludeFileAttribute : Attribute
{
    public String? Hint { get; set; }
}

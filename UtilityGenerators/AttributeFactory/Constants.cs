namespace RhoMicro.CodeAnalysis;

using System;

internal static class Constants
{
    public const String AggressiveInliningAttributeSyntax = "[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]";
    public const String AttributeDataDisplayString = "global::Microsoft.CodeAnalysis.AttributeData";
    public const String GenerateFactoryAttributeMetadataName = "RhoMicro.CodeAnalysis.GenerateFactoryAttribute";
    public const String InitializationMethodAttributeMetadataName = "RhoMicro.CodeAnalysis.InitializationMethodAttribute";
}

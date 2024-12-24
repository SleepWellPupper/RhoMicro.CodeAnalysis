namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;

/// <summary>
/// Provides helpers for writing data to a buffer of chars.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal static class TemplateRuntimeHelpers
{
    public static void Render(in String value, ref DynamicallyAllocatedBuffer<Char> buffer) => buffer.Add(value.AsSpan());
    public static void Render<T>(in T value, ref DynamicallyAllocatedBuffer<Char> buffer)
        where T : ITemplate
        => value.Render(ref buffer);
}
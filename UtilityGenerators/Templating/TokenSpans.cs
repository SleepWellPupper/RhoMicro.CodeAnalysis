// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;
/// <summary>
/// Contains the spans describing a tokens span of characters in a source text
/// and template string.
/// </summary>
/// <param name="TemplateSpan">
/// The tokens span relative to the original template string. This span may be
/// used to retrieve verbatim substrings from the user-defined template string,
/// e.g. in <c>render-block-head</c> productions.
/// </param>
/// <param name="NewlineAwareTemplateSpan">
/// The tokens spans relative to the newline aware translation of the template
/// string (according to the requested newline to be used). This span may be
/// used to retrieve <see cref="Span{T}"/>s from the generated template string
/// to be passed to the renderer.
/// </param>
/// <param name="SourceSpan">
/// The tokens span relative to the source text containing the template string.
/// </param>
internal sealed record TokenSpans(
    TemplateSpan TemplateSpan,
    TemplateSpan NewlineAwareTemplateSpan,
    SourceSpan SourceSpan)
{
    /// <summary>
    /// Gets empty token spans.
    /// </summary>
    public static TokenSpans Empty { get; } = new(
        TemplateSpan.Empty,
        TemplateSpan.Empty,
        SourceSpan.Empty);
    public override String ToString() => $"{SourceSpan} {TemplateSpan} {NewlineAwareTemplateSpan}";
}

namespace RhoMicro.CodeAnalysis.Templating;
/// <summary>
/// Contains the spans describing a tokens span of characters in a source text
/// and template string.
/// </summary>
/// <param name="TextSpan">
/// The tokens span relative to the template string.
/// </param>
/// <param name="SourceSpan">
/// The tokens span relative to the source text containing the template string.
/// </param>
internal sealed record TokenSpans(TemplateSpan TextSpan, SourceSpan SourceSpan);

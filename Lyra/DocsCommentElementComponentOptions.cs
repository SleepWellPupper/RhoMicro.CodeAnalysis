// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System.Runtime.CompilerServices;

/// <summary>
/// Provides options for rendering docs comment elements.
/// </summary>
/// <param name="Element">
/// The name of the element.
/// </param>
/// <param name="IndentSelfWithComment">
/// Indicates whether to indent using <c>/// </c> regardless of whether
/// <see cref="DocsCommentElementComponent.Body"/> is <see langword="null"/> or not.
/// </param>
/// <param name="Multiline">
/// Indicates whether to render docs comment elements on multiple lines.
/// This should be set to <see langword="true"/> for elements whose contents
/// span multiple lines. 
/// </param>
/// <param name="IndentBody">
/// Indicates whether to indent the body using the builders default indentation.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct DocsCommentElementComponentOptions(
    String Element,
    Boolean IndentSelfWithComment,
    Boolean Multiline,
    Boolean IndentBody)
{
    private static DocsCommentElementComponentOptions CreateRoot([CallerMemberName] String element = "") =>
        new(Element: element.ToLowerInvariant(),
            IndentSelfWithComment: true,
            Multiline: true,
            IndentBody: false);

    private static DocsCommentElementComponentOptions CreateMultilineChild([CallerMemberName] String element = "") =>
        new(Element: element.ToLowerInvariant(),
            IndentSelfWithComment: false,
            Multiline: true,
            IndentBody: false);

    private static DocsCommentElementComponentOptions CreateInlineChild([CallerMemberName] String element = "") =>
        new(Element: element.ToLowerInvariant(),
            IndentSelfWithComment: false,
            Multiline: false,
            IndentBody: false);

    private static DocsCommentElementComponentOptions
        CreateIndentingMultilineChild([CallerMemberName] String element = "") =>
        new(Element: element.ToLowerInvariant(),
            IndentSelfWithComment: false,
            Multiline: true,
            IndentBody: true);

    /// <summary>
    /// Gets options for the <c>summary</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Summary { get; } = CreateRoot();

    /// <summary>
    /// Gets options for the <c>remarks</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Remarks { get; } = CreateRoot();

    /// <summary>
    /// Gets options for the <c>returns</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Returns { get; } = CreateRoot();

    /// <summary>
    /// Gets options for the <c>param</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Param { get; } = CreateRoot();

    /// <summary>
    /// Gets options for the <c>typeparam</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions TypeParam { get; } = CreateRoot();

    /// <summary>
    /// Gets options for the <c>inheritdoc</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Inheritdoc { get; } = CreateRoot();

    /// <summary>
    /// Gets options for the <c>c</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions C { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>b</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions B { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>br</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Br { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>i</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions I { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>em</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Em { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>paramref</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions ParamRef { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>typeparamref</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions TypeParamRef { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>see</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions See { get; } = CreateInlineChild();

    /// <summary>
    /// Gets options for the <c>code</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Code { get; } = CreateMultilineChild();

    /// <summary>
    /// Gets options for the <c>br</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Para { get; } = CreateMultilineChild();

    /// <summary>
    /// Gets options for the <c>list</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions List { get; } = CreateIndentingMultilineChild();

    /// <summary>
    /// Gets options for the <c>term</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Term { get; } = CreateIndentingMultilineChild();

    /// <summary>
    /// Gets options for the <c>description</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Description { get; } = CreateIndentingMultilineChild();

    /// <summary>
    /// Gets options for the <c>listheader</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions ListHeader { get; } = CreateIndentingMultilineChild();

    /// <summary>
    /// Gets options for the <c>item</c> element.
    /// </summary>
    public static DocsCommentElementComponentOptions Item { get; } = CreateIndentingMultilineChild();
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

[Template(
    """
    {:
        if(_summary is null or [])
            return;
    :}
    /// <summary>
    (:DocumentationComment, _summary:)
    /// </summary>
    
    """)]

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal readonly partial struct DocsCommentTemplate
{
    private DocsCommentTemplate(String summary) => _summary = summary;
    private readonly String _summary;

    public static DocsCommentTemplate Create(String summary) => new(summary);
}

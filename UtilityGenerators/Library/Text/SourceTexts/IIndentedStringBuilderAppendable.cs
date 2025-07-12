// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if SOURCETEXTS_LIBRARY
public
#else
internal
#endif
 interface IIndentedStringBuilderAppendable
{
    public void AppendTo(IndentedStringBuilder builder);
}

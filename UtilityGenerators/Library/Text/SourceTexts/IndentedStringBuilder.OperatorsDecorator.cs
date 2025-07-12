// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if SOURCETEXTS_LIBRARY
public
#else
internal
#endif
 partial class IndentedStringBuilder
{
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
    [IncludeFile]
#endif
#if GENERATOR
    [NonEquatable]
#endif
    public sealed partial class OperatorsDecorator(IndentedStringBuilder builder)
    {
        public IndentedStringBuilder Builder => builder;
        public static OperatorsDecorator operator +(OperatorsDecorator operators, String value)
        {
            _ = operators ?? throw new ArgumentNullException(nameof(operators));

            operators.Builder.AppendCore(value);
            return operators;
        }
        public static OperatorsDecorator operator +(OperatorsDecorator operators, Char value)
        {
            _ = operators ?? throw new ArgumentNullException(nameof(operators));

            operators.Builder.AppendCore(value);
            return operators;
        }
        public static OperatorsDecorator operator +(OperatorsDecorator operators, IIndentedStringBuilderAppendable value)
        {
            _ = operators ?? throw new ArgumentNullException(nameof(operators));

            operators.Builder.AppendCore(value);
            return operators;
        }
        public override String ToString() => Builder.ToString();
    }
}

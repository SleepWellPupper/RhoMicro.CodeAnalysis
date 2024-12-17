namespace RhoMicro.CodeAnalysis.Library.Text;

#pragma warning disable IDE0040 // Add accessibility modifiers
partial class IndentedStringBuilder
#pragma warning restore IDE0040 // Add accessibility modifiers
{
#if UTILITYGENERATORS
    [IncludeFile]
    [NonEquatable]
#endif
    public sealed partial class OperatorsDecorator(IndentedStringBuilder builder)
    {
        public IndentedStringBuilder Builder => builder;
        public static OperatorsDecorator operator +(OperatorsDecorator operators, String value)
        {
            operators.Builder.AppendCore(value);
            return operators;
        }
        public static OperatorsDecorator operator +(OperatorsDecorator operators, Char value)
        {
            operators.Builder.AppendCore(value);
            return operators;
        }
        public static OperatorsDecorator operator +(OperatorsDecorator operators, IIndentedStringBuilderAppendable value)
        {
            operators.Builder.AppendCore(value);
            return operators;
        }
        public override String ToString() => Builder.ToString();
    }
}

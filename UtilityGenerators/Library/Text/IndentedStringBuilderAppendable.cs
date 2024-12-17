namespace RhoMicro.CodeAnalysis.Library.Text;

[NonEquatable]
[IncludeFile]
#pragma warning disable IDE0040 // Add accessibility modifiers
sealed partial class IndentedStringBuilderAppendable(Action<IndentedStringBuilder> strategy) : IIndentedStringBuilderAppendable
#pragma warning restore IDE0040 // Add accessibility modifiers
{
    public void AppendTo(IndentedStringBuilder builder) => strategy.Invoke(builder);
}
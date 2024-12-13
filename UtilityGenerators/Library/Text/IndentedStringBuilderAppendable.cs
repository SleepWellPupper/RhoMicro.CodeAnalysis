namespace RhoMicro.CodeAnalysis.Library.Text;

[IncludeFile]
#pragma warning disable IDE0040 // Add accessibility modifiers
sealed class IndentedStringBuilderAppendable(Action<IndentedStringBuilder> strategy) : IIndentedStringBuilderAppendable
#pragma warning restore IDE0040 // Add accessibility modifiers
{
    public void AppendTo(IndentedStringBuilder builder) => strategy.Invoke(builder);
}
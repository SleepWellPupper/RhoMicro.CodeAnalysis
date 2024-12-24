namespace RhoMicro.CodeAnalysis.Templating;

internal readonly record struct SourceSpanModel
{
    public SourceSpanModel(Int32 start, Int32 length, String text)
    {
        if(start < 0 || start > text.Length)
            throw new ArgumentOutOfRangeException(nameof(start), start, $"{nameof(start)} ('{start}') was outside the source text (length '{text.Length}').");

        if(length < 0 || start + length > text.Length)
            throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} ('{length}') was outside the source text (length '{text.Length}'), given the start index '{start}'.");

        Start = start;
        Length = length;
        Text = text;
    }

    public Int32 Start { get; }
    public Int32 Length { get; }
    public String Text { get; }

    public ReadOnlySpan<Char> AsSpan => Text.AsSpan(Start, Length);

    public unsafe override String ToString()
    {
        fixed(Char* chars = AsSpan)
        {
            return new(chars, 0, Length);
        }
    }

    public static implicit operator SourceSpanModel(String text) => new(0, text.Length, text);

    public override Int32 GetHashCode()
    {
        var hc = new HashCode();

        foreach(var c in AsSpan)
            hc.Add(c);

        return hc.ToHashCode();
    }

    public Boolean Equals(SourceSpanModel other) => AsSpan.SequenceEqual(other.AsSpan);
}

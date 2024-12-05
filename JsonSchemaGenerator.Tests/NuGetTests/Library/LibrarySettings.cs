namespace Library.Configuration;

using RhoMicro.CodeAnalysis;

[JsonSchema]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class LibrarySettings
{
    public Int32 Prop { get; set; }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

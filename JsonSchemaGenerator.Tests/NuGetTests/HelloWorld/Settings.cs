namespace HelloWorldConfiguration;

using Library;
using Library.Configuration;

using RhoMicro.CodeAnalysis;

[JsonSchema]
class Settings
{
    public LibrarySettings? Prop { get; set; }
}
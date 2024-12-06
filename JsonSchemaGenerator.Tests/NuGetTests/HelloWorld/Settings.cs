namespace HelloWorldConfiguration;

using Library;
using Library.Configuration;

using RhoMicro.CodeAnalysis;

[Flags]
enum MyEnum
{
    None = 0,                         //0000
    North = 1,                        //0001
    East = 2,                         //0010
    South = 4,                        //0100
    West = 8,                         //1000
    NorthEast = North | East,         //0011
    SouthEast = South | East,         //0101
    All = North | East | South | West //1111
}

[JsonSchema]
class Settings
{
    public LibrarySettings? Prop { get; set; }
    public MyEnum? Prop2 { get; set; }
    [JsonSchemaProperty(Description="This is a flags property.")]
    public MyEnum Prop3 { get; set; }
    public required Dictionary<String, MyEnum> Values { get; set; }
}
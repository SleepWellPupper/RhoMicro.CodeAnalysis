using RhoMicro.CodeAnalysis;

using TestApp;

internal class Program
{
    private static void Main(String[] args)
    {
    }
}

enum MyEnum
{
    A, B, C
}

[JsonSchema]
sealed partial class Settings
{
    public required NamespaceSettings NamespaceSettingsProp { get; set; }
    public MyEnum EnumProp { get; set; }
    public Settings[] ArrayProp { get; set; } = [];
    public List<SubSettings> ListProp { get; set; } = [];
}

[JsonSchema]
sealed partial class SubSettings
{
    public required Int32 IntProp { get; set; }
}
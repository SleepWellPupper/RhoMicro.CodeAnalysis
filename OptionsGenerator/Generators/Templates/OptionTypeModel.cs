namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

internal readonly record struct OptionTypeModel(String FullyQualifiedName, String ValueAccessorName)
{
    public static OptionTypeModel Default { get; } = new(TypeNames.IOptions, "Value");
    public static OptionTypeModel Snapshot { get; } = new(TypeNames.IOptionsSnapshot, "Value");
    public static OptionTypeModel Monitor { get; } = new(TypeNames.IOptionsMonitor, "CurrentValue");
}
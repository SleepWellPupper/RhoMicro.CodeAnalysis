namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        if(model.Namespace is [{},..])
        {
    :}
    namespace (:model.Namespace:);

    {:
        }
    :}
    partial interface (:model.Name:)
    {
        static (:model.FullyQualifiedNamespacePrefix:)(:model.Name:) Default { get; } = new (:model.FullyQualifiedNamespacePrefix:)Mutable(:model.NormalizedName:)();
    }

    public sealed record (:model.NormalizedName:) : (:model.FullyQualifiedNamespacePrefix:)(:model.Name:)
    {
        public static (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:) Default { get; } = new();
    {:
        foreach(var property in model.Properties)
        {
            (:Space4, new PropertyTemplate(property):)
        }
    :}}
    
    internal sealed class Mutable(:model.NormalizedName:) : (:model.FullyQualifiedNamespacePrefix:)(:model.Name:)
    {
    {:
        foreach(var property in model.Properties)
        {
            (:Space4, new MutablePropertyTemplate(property):)
        }
    :}}
    
    internal sealed class (:model.NormalizedName:)Monitor(global::Microsoft.Extensions.Options.IOptionsMonitor<Mutable(:model.NormalizedName:)> monitor) : (:model.FullyQualifiedNamespacePrefix:)(:model.Name:)
    {
    {:
        foreach(var property in model.Properties)
        {
            (:Space4, new MonitorPropertyTemplate(property):)
        }
    :}}

    (:new RegistrationStrategyTemplate(model):)
    (:new ConfigurationTemplate(model):)
    (:new ServiceCollectionExtensionsTemplate(model):)
    """)]
[NonEquatable]
internal readonly partial struct OptionsTemplate(OptionsInterfaceModel model);

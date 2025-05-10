namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    public static partial class ServiceCollectionExtensions
    {
        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection Add(:model.NormalizedName:)(
            this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services, 
            global::System.Action<(:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration>? configure = null)
        {
            global::System.ArgumentNullException.ThrowIfNull(services);

            var config = new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration();
            configure?.Invoke(config);

            config.RegistrationStrategy.Execute(services, config);

            return services;
        }
    }
    """
    )]
    
[NonEquatable]
internal readonly partial struct ServiceCollectionExtensionsTemplate(OptionsInterfaceModel model);
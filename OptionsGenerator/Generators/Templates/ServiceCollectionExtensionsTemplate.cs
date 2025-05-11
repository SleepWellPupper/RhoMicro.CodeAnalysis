namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <summary>
    /// Provides extension methods for <see cref="(:TypeNames.IServiceCollection:)"/>.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> to the service collection.
        /// </summary>
        public static (:TypeNames.IServiceCollection:) Add(:model.NormalizedName:)(
            this (:TypeNames.IServiceCollection:) services, 
            (:TypeNames.Action:)<(:model.Templates().FullyQualifiedTypeNames.Configuration:)>? configure = null)
        {
            (:TypeNames.ArgumentNullException:).ThrowIfNull(services);

            var config = new (:model.Templates().FullyQualifiedTypeNames.Configuration:)();
            configure?.Invoke(config);

            config.RegistrationStrategy.Execute(services, config);

            return services;
        }
    }
    """
    )]
    
[NonEquatable]
internal readonly partial struct ServiceCollectionExtensionsTemplate(OptionsModel model);
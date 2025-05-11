namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <summary>
    /// Provides configuration for the registration of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> instances to <see cref="(:TypeNames.IServiceCollection:)"/> instances.
    /// </summary>
    public partial class (:model.Templates().TypeNames.Configuration:)
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public (:model.Templates().TypeNames.Configuration:)() => UseDefaultOptions();

        internal (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:) RegistrationStrategy { get; private set; }

        private (:model.Templates().FullyQualifiedTypeNames.Configuration:) UseRegistrationStrategy((:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy registrationStrategy)
        {
            RegistrationStrategy = registrationStrategy;
            return this;
        }
    
        /// <summary>
        /// Registers an implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> based on <see cref="(:TypeNames.IOptions:){T}"/>.
        /// </summary>
        /// <param name="configurationSection">
        /// The configuration section to bind the options instance against.
        /// </param>
        /// <returns>
        /// A reference to the configuration, for chaining of further method calls.
        /// </returns>
        public (:model.Templates().FullyQualifiedTypeNames.Configuration:) UseDefaultOptions(
            string configurationSection = "(:model.NormalizedName:)")
            => UseRegistrationStrategy(
                new (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:).Pattern<(:model.Templates().FullyQualifiedTypeNames.Default:)>(
                    configurationSection,
                    (:TypeNames.ServiceLifetime:).Singleton));
        
        /// <summary>
        /// Registers an implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> based on <see cref="(:TypeNames.IOptionsSnapshot:){T}"/>.
        /// </summary>
        /// <param name="configurationSection">
        /// The configuration section to bind the options instance against.
        /// </param>
        /// <returns>
        /// A reference to the configuration, for chaining of further method calls.
        /// </returns>
        public (:model.Templates().FullyQualifiedTypeNames.Configuration:) UseSnapshotOptions(
            string configurationSection = "(:model.NormalizedName:)")
            => UseRegistrationStrategy(
                new (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:).Pattern<(:model.Templates().FullyQualifiedTypeNames.Snapshot:)>(
                    configurationSection,
                    (:TypeNames.ServiceLifetime:).Scoped));
    
        /// <summary>
        /// Registers an implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> based on <see cref="(:TypeNames.IOptionsMonitor:){T}"/>.
        /// </summary>
        /// <param name="configurationSection">
        /// The configuration section to bind the options instance against.
        /// </param>
        /// <returns>
        /// A reference to the configuration, for chaining of further method calls.
        /// </returns>
        public (:model.Templates().FullyQualifiedTypeNames.Configuration:) UseMonitorOptions(
            string configurationSection = "(:model.NormalizedName:)")
            => UseRegistrationStrategy(
                new (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:).Pattern<(:model.Templates().FullyQualifiedTypeNames.Monitor:)>(
                    configurationSection,
                    (:TypeNames.ServiceLifetime:).Singleton));
    
        /// <summary>
        /// Registers a custom implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/>.
        /// </summary>
        /// <param name="factory">
        /// The delegate to register as the factory for instances of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/>.
        /// </param>
        /// <param name="">
        /// The lifetime of the custom <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> registration.
        /// </param>
        /// <returns>
        /// A reference to the configuration, for chaining of further method calls.
        /// </returns>
        public (:model.Templates().FullyQualifiedTypeNames.Configuration:) UseCustomOptions(
            (:TypeNames.Func:)<(:TypeNames.IServiceProvider:), (:model.Templates().FullyQualifiedTypeNames.Interface:)> factory, 
            (:TypeNames.ServiceLifetime:) lifetime = (:TypeNames.ServiceLifetime:).Singleton)
            => UseRegistrationStrategy(
                new (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:).Custom(
                    factory, 
                    lifetime));
    }
    """)]
[NonEquatable]
internal readonly partial struct ConfigurationTemplate(OptionsModel model);

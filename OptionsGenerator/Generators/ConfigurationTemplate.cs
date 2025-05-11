namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    public partial class (:model.NormalizedName:)Configuration
    {
        public (:model.NormalizedName:)Configuration()
        {
            UseDefaultOptions();
        }

        internal (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy RegistrationStrategy { get; set; }

        private (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration UseRegistrationStrategy((:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy registrationStrategy)
        {
            RegistrationStrategy = registrationStrategy;
            return this;
        }

        public (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration UseDefaultOptions(
            string configurationSection = "(:model.NormalizedName:)")
            => UseRegistrationStrategy(
                new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Pattern.Default(configurationSection));
        
        public (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration UseSnapshotOptions(
            string configurationSection = "(:model.NormalizedName:)")
            => UseRegistrationStrategy(
                new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Pattern.Snapshot(configurationSection));

        public (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration UseMonitorOptions(
            string configurationSection = "(:model.NormalizedName:)")
            => UseRegistrationStrategy(
                new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Pattern.Monitor(configurationSection));

        public (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration UseCustomOptions(
            global::System.Func<global::System.IServiceProvider, (:model.FullyQualifiedNamespacePrefix:)(:model.Name:)> factory, 
            global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)
            => UseRegistrationStrategy(
                new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Custom(
                    factory, 
                    lifetime));

        public (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration UseCustomOptions(
            global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)
            => UseRegistrationStrategy(
                new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Custom(
                    static _ => (:model.FullyQualifiedNamespacePrefix:)(:model.Name:).Default,
                    lifetime));
    }

    """)]
[NonEquatable]
internal readonly partial struct ConfigurationTemplate(OptionsInterfaceModel model);

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    internal abstract partial class (:model.NormalizedName:)RegistrationStrategy
    {
        public abstract partial class Pattern
            : (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy
        {
            public sealed class Default(
                string configurationSection)
                : (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Pattern(
                    configurationSection, 
                    global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)
            {
                protected override (:model.FullyQualifiedNamespacePrefix:)(:model.Name:) ResolveOptions(
                    global::System.IServiceProvider sp) 
                    => global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::Microsoft.Extensions.Options.IOptions<(:model.FullyQualifiedNamespacePrefix:)Mutable(:model.NormalizedName:)>>(sp).Value;
            }
    
            public sealed class Snapshot(
                string configurationSection)
                : (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Pattern(
                    configurationSection, 
                    global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)
            {
                protected override (:model.FullyQualifiedNamespacePrefix:)(:model.Name:) ResolveOptions(
                    global::System.IServiceProvider sp) 
                    => global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::Microsoft.Extensions.Options.IOptionsSnapshot<(:model.FullyQualifiedNamespacePrefix:)Mutable(:model.NormalizedName:)>>(sp).Value;
            }
    
            public sealed class Monitor(
                string configurationSection)
                : (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy.Pattern(
                    configurationSection, 
                    global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)
            {
                protected override (:model.FullyQualifiedNamespacePrefix:)(:model.Name:) ResolveOptions(
                    global::System.IServiceProvider sp) 
                    => new (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Monitor(global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::Microsoft.Extensions.Options.IOptionsMonitor<(:model.FullyQualifiedNamespacePrefix:)Mutable(:model.NormalizedName:)>>(sp));
            }

            private Pattern(
                string configurationSection,
                global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime)
            {
                _configurationSection = configurationSection;
                _lifetime = lifetime;
            }

            private readonly string _configurationSection;
            private readonly global::Microsoft.Extensions.DependencyInjection.ServiceLifetime _lifetime;
        
            internal sealed override void Execute(
                global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,
                (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration config)
            {
                global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAdd(
                    services,
                    new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
                        serviceType: typeof((:model.FullyQualifiedNamespacePrefix:)(:model.Name:)),
                        factory: this.ResolveOptions,
                        lifetime: _lifetime));

                var builder = global::Microsoft.Extensions.DependencyInjection.OptionsBuilderConfigurationExtensions.BindConfiguration(
                    global::Microsoft.Extensions.DependencyInjection.OptionsServiceCollectionExtensions.AddOptions<(:model.FullyQualifiedNamespacePrefix:)Mutable(:model.NormalizedName:)>(services),
                    _configurationSection);

                ConfigureOptionsBuilder(builder, config);
            }

            static partial void ConfigureOptionsBuilder(
                global::Microsoft.Extensions.Options.OptionsBuilder<(:model.FullyQualifiedNamespacePrefix:)Mutable(:model.NormalizedName:)> builder,
                (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration config);

            protected abstract (:model.FullyQualifiedNamespacePrefix:)(:model.Name:) ResolveOptions(
                global::System.IServiceProvider sp);
        }

        public sealed class Custom(
            global::System.Func<global::System.IServiceProvider, (:model.FullyQualifiedNamespacePrefix:)(:model.Name:)> factory, 
            global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime)
            : (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)RegistrationStrategy
        {
            internal override void Execute(
                global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,
                (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration _)
                => global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAdd(
                    services,
                    new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
                        serviceType: typeof((:model.FullyQualifiedNamespacePrefix:)(:model.Name:)),
                        factory: factory,
                        lifetime: lifetime));
        }

        private (:model.NormalizedName:)RegistrationStrategy() { }

        internal abstract void Execute(
            global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,
            (:model.FullyQualifiedNamespacePrefix:)(:model.NormalizedName:)Configuration config);
    }

    """
    )]

[NonEquatable]
internal readonly partial struct RegistrationStrategyTemplate(OptionsInterfaceModel model);
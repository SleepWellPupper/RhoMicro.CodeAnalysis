// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <summary>
    /// Provides strategies for registering various implementations of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> to an <see cref="(:TypeNames.IServiceCollection:)"/>.
    /// </summary>
    internal abstract partial class (:model.Templates().TypeNames.RegistrationStrategy:)
    {
        /// <summary>
        /// Provides an implementation for registering an options pattern for <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/>, as well as an adapter implementation.
        /// </summary>
        /// <param name="configurationSection">
        /// The configuration section to bind the options instance against.
        /// </param>
        /// <param name="lifetime">
        /// The lifetime of the registered adapter type.
        /// </param>
        /// <typeparam name="TAdapter">
        /// The adapter type to register.
        /// </typeparam>
        public sealed partial class Pattern<TAdapter>(
            string configurationSection,
            (:TypeNames.ServiceLifetime:) lifetime,
            bool tryAdd)
            : (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:)
            where TAdapter : class, (:model.Templates().FullyQualifiedTypeNames.Interface:)
        {
            /// <inheritdoc/>
            internal sealed override void Execute(
                (:TypeNames.IServiceCollection:) services,
                (:model.Templates().FullyQualifiedTypeNames.Configuration:) configuration)
            {
                var descriptor = new (:TypeNames.ServiceDescriptor:)(
                    serviceType: typeof((:model.Templates().FullyQualifiedTypeNames.Interface:)),
                    implementationType: typeof(TAdapter),
                    lifetime: lifetime);
                    
                if(tryAdd)
                {
                    (:TypeNames.ServiceCollectionDescriptorExtensions:).TryAdd(
                        services,
                        descriptor);
                }
                else
                {
                    (:TypeNames.ServiceCollectionDescriptorExtensions:).Add(
                        services,
                        descriptor);
                }
    
                var builder = (:TypeNames.OptionsBuilderConfigurationExtensions:).BindConfiguration(
                    (:TypeNames.OptionsServiceCollectionExtensions:).AddOptions<(:model.Templates().FullyQualifiedTypeNames.Mutable:)>(services),
                    configurationSection);
              
                ConfigureOptionsBuilder(builder, configuration);
            }
    
            /// <summary>
            /// Hook method for intercepting the options configuration in <see cref="Execute((:TypeNames.IServiceCollection:), (:model.Templates().FullyQualifiedTypeNames.Configuration:))"/>.
            /// </summary>
            /// <param name="builder">
            /// The builder used to set up the options pattern.
            /// </param>
            /// <param name="configuration">
            /// The configuration used to configure registration.
            /// </param>
            static partial void ConfigureOptionsBuilder(
                (:TypeNames.OptionsBuilder:)<(:model.Templates().FullyQualifiedTypeNames.Mutable:)> builder,
                (:model.Templates().FullyQualifiedTypeNames.Configuration:) configuration);
        }
    
        /// <summary>
        /// Provides an implementation for registering a custom implementation type or factory for <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/>.
        /// </summary>
        public sealed partial class Custom(
            (:TypeNames.Func:)<(:TypeNames.IServiceProvider:), (:model.Templates().FullyQualifiedTypeNames.Interface:)> factory, 
            (:TypeNames.ServiceLifetime:) lifetime,
            bool tryAdd)
            : (:model.Templates().FullyQualifiedTypeNames.RegistrationStrategy:)
        {
            /// <inheritdoc/>
            internal override void Execute(
                (:TypeNames.IServiceCollection:) services,
                (:model.Templates().FullyQualifiedTypeNames.Configuration:) _)
            {
                var descriptor = new (:TypeNames.ServiceDescriptor:)(
                    serviceType: typeof((:model.Templates().FullyQualifiedTypeNames.Interface:)),
                    factory: factory,
                    lifetime: lifetime);
    
                if(tryAdd)
                {
                    (:TypeNames.ServiceCollectionDescriptorExtensions:).TryAdd(
                        services,
                        descriptor);
                }
                else
                {
                    (:TypeNames.ServiceCollectionDescriptorExtensions:).Add(
                        services,
                        descriptor);
                }
            }
        }
    
        private (:model.Templates().TypeNames.RegistrationStrategy:)() { }
    
        /// <summary>
        /// Executes the strategy, registering configured services to a service collection.
        /// </summary>
        /// <param name="services">
        /// The service collection to register services to.
        /// </param>
        /// <param name="configuration">
        /// The configuration used to configure registration.
        /// </param>
        internal abstract void Execute(
            (:TypeNames.IServiceCollection:) services,
            (:model.Templates().FullyQualifiedTypeNames.Configuration:) configuration);
    }
    """
)]
[NonEquatable]
internal readonly partial struct RegistrationStrategyTemplate(OptionsModel model);

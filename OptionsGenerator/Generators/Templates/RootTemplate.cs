// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    (:new RootNamespaceTemplate(model):)
    (:new InterfaceOptionsTemplate(model):)
    (:new PassthroughOptionsImplementationTemplate<DefaultOptionsNameTemplate>(model, new(model), OptionTypeModel.Default):)
    (:new PassthroughOptionsImplementationTemplate<SnapshotOptionsNameTemplate>(model, new(model), OptionTypeModel.Snapshot):)
    (:new PassthroughOptionsImplementationTemplate<MonitorOptionsNameTemplate>(model, new(model), OptionTypeModel.Monitor):)
    (:new MutableOptionsTemplate(model):)
    (:new ImmutableOptionsTemplate(model):)
    (:new RegistrationStrategyTemplate(model):)
    (:new ConfigurationTemplate(model):)
    (:new ServiceCollectionExtensionsTemplate(model):)
    """), NonEquatable]
internal readonly partial struct RootTemplate(OptionsModel model);

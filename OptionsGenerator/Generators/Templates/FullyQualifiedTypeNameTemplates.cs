// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;
internal readonly struct FullyQualifiedTypeNameTemplates(OptionsModel model)
{
    public InterfaceOptionsFullyQualifiedNameTemplate Interface => new(model);
    public DefaultOptionsFullyQualifiedNameTemplate Default => new(model);
    public SnapshotOptionsFullyQualifiedNameTemplate Snapshot => new(model);
    public MonitorOptionsFullyQualifiedNameTemplate Monitor => new(model);
    public MutableOptionsFullyQualifiedNameTemplate Mutable => new(model);
    public ImmutableOptionsFullyQualifiedNameTemplate Immutable => new(model);
    public RegistrationStrategyFullyQualifiedNameTemplate RegistrationStrategy => new(model);
    public ConfigurationFullyQualifiedNameTemplate Configuration => new(model);
}

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;
internal readonly struct TypeNameTemplates(OptionsModel model)
{
    public InterfaceOptionsNameTemplate Interface => new(model);
    public DefaultOptionsNameTemplate Default => new(model);
    public SnapshotOptionsNameTemplate Snapshot => new(model);
    public MonitorOptionsNameTemplate Monitor => new(model);
    public MutableOptionsNameTemplate Mutable => new(model);
    public ImmutableOptionsNameTemplate Immutable => new(model);
    public RegistrationStrategyNameTemplate RegistrationStrategy => new(model);
    public ConfigurationNameTemplate Configuration => new(model);
}

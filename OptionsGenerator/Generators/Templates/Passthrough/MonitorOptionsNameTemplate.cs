namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Monitor(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct MonitorOptionsNameTemplate(OptionsModel model);

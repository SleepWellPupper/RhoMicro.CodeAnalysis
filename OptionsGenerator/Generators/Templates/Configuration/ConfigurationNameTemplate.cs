namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.NormalizedName:)Configuration"), NonEquatable]
internal readonly partial struct ConfigurationNameTemplate(OptionsModel model);

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Default(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct DefaultOptionsNameTemplate(OptionsModel model);

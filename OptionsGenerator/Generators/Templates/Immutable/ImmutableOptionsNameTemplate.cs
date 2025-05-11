namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct ImmutableOptionsNameTemplate(OptionsModel model);

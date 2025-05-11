namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Mutable(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct MutableOptionsNameTemplate(OptionsModel model);

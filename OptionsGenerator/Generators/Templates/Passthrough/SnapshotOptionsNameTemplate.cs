namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Snapshot(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct SnapshotOptionsNameTemplate(OptionsModel model);

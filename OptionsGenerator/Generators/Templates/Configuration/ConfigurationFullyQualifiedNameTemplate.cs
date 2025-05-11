namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.FullyQualifiedNamespacePrefix:)(:model.Templates().TypeNames.Configuration:)"), NonEquatable]
internal readonly partial struct ConfigurationFullyQualifiedNameTemplate(OptionsModel model);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("(:model.FullyQualifiedNamespacePrefix:)(:model.Templates().TypeNames.Immutable:)"), NonEquatable]
internal readonly partial struct ImmutableOptionsFullyQualifiedNameTemplate(OptionsModel model);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.FullyQualifiedNamespacePrefix:)(:model.Templates().TypeNames.Immutable:)"), NonEquatable]
internal readonly partial struct ImmutableOptionsFullyQualifiedNameTemplate(OptionsModel model);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.FullyQualifiedNamespacePrefix:)(:model.Templates().TypeNames.Default:)"), NonEquatable]
internal readonly partial struct DefaultOptionsFullyQualifiedNameTemplate(OptionsModel model);

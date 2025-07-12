// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.FullyQualifiedNamespacePrefix:)(:model.Templates().TypeNames.Interface:)"), NonEquatable]
internal readonly partial struct InterfaceOptionsFullyQualifiedNameTemplate(OptionsModel model);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("(:model.FullyQualifiedNamespacePrefix:)(:model.Templates().TypeNames.RegistrationStrategy:)"), NonEquatable]
internal readonly partial struct RegistrationStrategyFullyQualifiedNameTemplate(OptionsModel model);

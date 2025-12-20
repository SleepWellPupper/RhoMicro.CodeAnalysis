// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.NormalizedName:)RegistrationStrategy"), NonEquatable]
internal readonly partial struct RegistrationStrategyNameTemplate(OptionsModel model);

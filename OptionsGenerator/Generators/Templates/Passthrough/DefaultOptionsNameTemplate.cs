// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Default(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct DefaultOptionsNameTemplate(OptionsModel model);

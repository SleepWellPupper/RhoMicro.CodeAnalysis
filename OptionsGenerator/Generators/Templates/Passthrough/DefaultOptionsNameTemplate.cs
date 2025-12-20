// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("Default(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct DefaultOptionsNameTemplate(OptionsModel model);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct ImmutableOptionsNameTemplate(OptionsModel model);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("(:model.NormalizedName:)Configuration"), NonEquatable]
internal readonly partial struct ConfigurationNameTemplate(OptionsModel model);

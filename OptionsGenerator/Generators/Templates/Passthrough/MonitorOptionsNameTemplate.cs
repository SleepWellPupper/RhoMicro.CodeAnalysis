// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("Monitor(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct MonitorOptionsNameTemplate(OptionsModel model);

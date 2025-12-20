// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Monitor(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct MonitorOptionsNameTemplate(OptionsModel model);

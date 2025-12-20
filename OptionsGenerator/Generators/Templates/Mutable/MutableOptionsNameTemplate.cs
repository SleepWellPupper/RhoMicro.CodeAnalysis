// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Mutable(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct MutableOptionsNameTemplate(OptionsModel model);

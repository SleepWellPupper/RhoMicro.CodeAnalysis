// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template("Snapshot(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct SnapshotOptionsNameTemplate(OptionsModel model);

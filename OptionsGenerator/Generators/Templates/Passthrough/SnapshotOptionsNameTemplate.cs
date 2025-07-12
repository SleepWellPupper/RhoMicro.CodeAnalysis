// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("Snapshot(:model.NormalizedName:)"), NonEquatable]
internal readonly partial struct SnapshotOptionsNameTemplate(OptionsModel model);

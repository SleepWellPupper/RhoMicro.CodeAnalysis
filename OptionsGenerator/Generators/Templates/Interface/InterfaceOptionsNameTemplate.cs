// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template("(:model.Name:)"), NonEquatable]
internal readonly partial struct InterfaceOptionsNameTemplate(OptionsModel model);

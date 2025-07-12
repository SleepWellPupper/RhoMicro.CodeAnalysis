// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.CopyToGenerator.MacroExpansions;

using RhoMicro.CodeAnalysis.CopyToGenerator;
using RhoMicro.CodeAnalysis.Library;

internal abstract class MacroExpansionBase(Model model, Macro macro)
    : MacroExpansion<Macro>(macro)
{
    protected Model Model { get; } = model;
}

namespace RhoMicro.CodeAnalysis.CopyToGenerator.MacroExpansions;

using RhoMicro.CodeAnalysis.CopyToGenerator;
using RhoMicro.CodeAnalysis.Library;

internal abstract class MacroExpansionBase(Model model, Macro macro)
    : MacroExpansion<Macro>(macro)
{
    protected Model Model { get; } = model;
}

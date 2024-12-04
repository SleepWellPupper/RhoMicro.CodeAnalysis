namespace RhoMicro.CodeAnalysis.CopyToGenerator.ExpansionProviders;
using RhoMicro.CodeAnalysis.CopyToGenerator;
using RhoMicro.CodeAnalysis.CopyToGenerator.MacroExpansions;

sealed class TailExpansion(Model model) : MacroExpansionBase(model, Macro.Tail)
{
    public override void Expand(IExpandingMacroStringBuilder<Macro> builder, CancellationToken cancellationToken)
    {
        if(!Model.IsInGlobalNamespace)
            _ = builder.Append('}');

        _ = builder.Append('}');
    }
}

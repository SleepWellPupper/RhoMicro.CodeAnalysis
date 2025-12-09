// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Equa;

using Lyra;

internal readonly record struct RecordNameComponent(RecordModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        builder.Append(Model.Name)
            .SetCondition(Model.TypeParameters is not [])
            .Append($"<{ComponentFactory.List(Model.TypeParameters, separator: ", ")}>")
            .UnsetCondition();
    }
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;

internal readonly record struct UnionTypeMetadataNameComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Model.Namespace is not [])
        {
            builder.Append($"{Model.Namespace}.");
        }

        foreach (var containingType in Model.ContainingTypes)
        {
            builder.Append(containingType.Name);

            if (containingType.TypeParameters is { Count: > 0 and var containingTypeParameterCount })
            {
                builder.Append($"`{containingTypeParameterCount}");
            }

            builder.Append('.');
        }

        builder.Append(Model.Name);

        if (Model.TypeParameters is { Count: > 0 and var count })
        {
            builder.Append($"`{count}");
        }
    }
}

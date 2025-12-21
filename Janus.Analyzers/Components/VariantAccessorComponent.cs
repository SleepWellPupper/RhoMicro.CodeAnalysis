// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;

internal readonly record struct VariantAccessorComponent(
    UnionTypeAttribute.Model Model,
    String InstanceExpression = "this",
    Boolean NullableCast = false)
    : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (Model.Type.Kind)
        {
            case VariantTypeKind.Value or VariantTypeKind.Unknown:
                builder.Append($"{InstanceExpression}._{Model.Name}");
                break;
            case VariantTypeKind.Unmanaged:
                builder.Append($"{InstanceExpression}._unmanagedVariantsContainer.{Model.Name}");
                break;
            case VariantTypeKind.Reference:
                if (NullableCast && !Model.Type.IsNullable)
                {
                    builder.Append($"({Model.Type.Name}?){InstanceExpression}._referenceVariantsContainer");
                }
                else
                {
                    builder.Append($"(({Model.Type.NullableName}){InstanceExpression}._referenceVariantsContainer!)");
                }

                break;
        }
    }
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;

internal readonly record struct VariantsSwitchComponent(
    UnionModel Model,
    Action<UnionTypeAttribute.Model, UnionModel, CSharpSourceBuilder, CancellationToken> Append,
    Action<UnionModel, CSharpSourceBuilder, CancellationToken>? Default = null,
    String VariantExpression = "Variant.Kind") : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        builder.AppendLine($"switch({VariantExpression})")
            .AppendLine('{')
            .Indent();

        foreach (var variant in Model.Variants)
        {
            builder.AppendLine($"case VariantKind.{variant.Name}:")
                .AppendLine('{')
                .Indent();

            Append.Invoke(variant, Model, builder, cancellationToken);

            builder.AppendLine()
                .Detent()
                .AppendLine('}');
        }

        builder.AppendLine("default:")
            .AppendLine('{')
            .Indent();

        if (Default is not null)
        {
            Default.Invoke(Model, builder, cancellationToken);
        }
        else
        {
            builder.Append("throw CreateUnknownVariantException();");
        }

        builder.AppendLine()
            .Detent()
            .AppendLine('}');

        builder.Detent()
            .Append('}');
    }

    public Boolean Equals(VariantsSwitchComponent other) =>
        throw new NotSupportedException("Equals is not supported on this type.");

    public override Int32 GetHashCode() =>
        throw new NotSupportedException("GetHashCode is not supported on this type.");
}

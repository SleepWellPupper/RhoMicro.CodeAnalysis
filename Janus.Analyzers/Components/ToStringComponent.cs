// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct ToStringComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Model.IsToStringUserProvided || Model.Settings.ToStringSetting is ToStringSetting.None)
        {
            return;
        }

        switch (Model.Settings.ToStringSetting)
        {
            case ToStringSetting.Simple:
                AppendSimple(builder);
                break;
            case ToStringSetting.Detailed:
                AppendDetailed(builder);
                break;
        }
    }

    private void AppendDetailed(CSharpSourceBuilder builder) =>
        builder.Append(
            $$"""
              {{Inheritdoc()}}
              public override string ToString()
              {
                  {{new VariantsSwitchComponent(Model, static (v, m, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append(
                          $$$"""
                             return $"{{{ComponentFactory.Create(m, static (m, b, ct) =>
                             {
                                 ct.ThrowIfCancellationRequested();

                                 b.Append(m.Name);

                                 if (m.TypeParameters is [])
                                 {
                                     return;
                                 }

                                 b.Append($"<{ComponentFactory.List<String>(m.TypeParameters, static (p, _, _, b, ct) =>
                                 {
                                     ct.ThrowIfCancellationRequested();

                                     b.Append(
                                         $$"""
                                           {{p}}{(typeof({{p}}) is { IsGenericParameter: true, DeclaringMethod: null } ? string.Empty : $":{typeof({{p}})}" )}
                                           """
                                     );
                                 }, separator: ", ")}>");
                             })}}} {{ Variants: [{{{ComponentFactory.Create((v, m), static (t, b, ct) =>
                         {
                             ct.ThrowIfCancellationRequested();

                             var (variant, model) = t;

                             for (var i = 0; i < model.Variants.Count; i++)
                             {
                                 ct.ThrowIfCancellationRequested();

                                 var isCaseVariant = ReferenceEquals(variant.Name, model.Variants[i].Name);

                                 b.SetCondition(i is not 0)
                                     .Append(", ")
                                     .UnsetCondition()
                                     .SetCondition(isCaseVariant)
                                     .Append('<')
                                     .UnsetCondition()
                                     .Append(model.Variants[i].Name)
                                     .SetCondition(isCaseVariant)
                                     .Append('>')
                                     .UnsetCondition();
                             }
                         })}}}], Value: {Value} }}";
                             """
                      );
                  })}}
              }
              """
        );

    private void AppendSimple(CSharpSourceBuilder builder) =>
        builder.Append(
            $$"""
              {{Inheritdoc()}}
              public override string ToString()
              {
                  {{new VariantsSwitchComponent(Model, static (v, _, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"return {new VariantAccessorComponent(v)}.ToString();");
                  })}}
              }
              """
        );
}

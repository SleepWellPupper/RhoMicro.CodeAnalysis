// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct ValidationComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methods = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.Append(
                $$"""
                  {{Summary("Creates an exception used when converting to a given variant.")}}
                  {{Param("variant", "The variant a conversion was attempted to.")}}
                  {{Returns("The created exception.")}}
                  private {{typeof(InvalidCastException)}} CreateInvalidCastException(VariantKind variant)
                      => new {{typeof(InvalidCastException)}}(
                              $"Unable to convert union to '{new VariantModel(variant).Name}', as it is currently representing " +
                              $"the '{Variant}' variant.");

                  {{Summary("Creates an exception used when encountering an unknown variant state.")}}
                  {{Returns("The created exception.")}}
                  private {{typeof(InvalidOperationException)}} CreateUnknownVariantException()
                      => new {{typeof(InvalidOperationException)}}(
                          $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not " +
                          $"representing a valid variant of this union: '{Variant}'. This could be either " + 
                          "because the union itself was not initialized correctly, or due to a bug in the " + 
                          "'JanusJanus' source generator that generated this union type. Please report an issue to the " + 
                          "maintainer.");
                          
                  {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append(
                          $"""
                           {Summary("Validates a variant value for creating a new instance of the union.")}
                           {Remarks(static (b, ct) =>
                           {
                               ct.ThrowIfCancellationRequested();

                               b.Append(
                                   $"""
                                    If {ParamRef("throwIfInvalid")} is {Langword("true")}, the implementation should throw an 
                                    exception outlining why {ParamRef("value")} is invalid.
                                    Otherwise, {ParamRef("isValid")} should be assigned {Langword("true")} if 
                                    {ParamRef("value")} is valid and {Langword("false")} if it is not.
                                    """);
                           })}
                           {Param("value", "The value to validate.")}
                           {Param("throwIfInvalid", "Indicates whether to throw an exception if the value is invalid.")}
                           {Param("isValid", "Indicates whether the value is valid.")}
                           static partial void Validate{v.Name}({v.Type.NullableName} value, bool throwIfInvalid, ref bool isValid);
                           """
                      );
                  }, separator: "\n\n")}}
                  """
            );
        });

        var region = Region("Validation", methods);

        builder.Append(region);
    }
}

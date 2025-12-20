// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Runtime.CompilerServices;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct ConstructorComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var body = Create(this, static (@this, b, _) =>
        {
            var model = @this.Model;

            foreach (var variant in model.Variants)
            {
                b.AppendLine(
                    $$"""
                      {{Summary("Initializes a new instance.")}}
                      {{Param("value", "The variant value to initialize the new instance with.")}}
                      [{{typeof(OverloadResolutionPriorityAttribute)}}(1)]
                      public {{model.Name}}({{variant.Type.NullableName}} value) : this(value, validate: true) { }
                      
                      {{Summary("Initializes a new instance.")}}
                      {{Param("value", "The variant value to initialize the new instance with.")}}
                      {{Param("validate", "Indicates whether to validate the value.")}}
                      private {{model.Name}}({{variant.Type.NullableName}} value, bool validate)
                      {
                          if (validate)
                          {
                              var isValid = true;
                              Validate{{variant.Name}}(value, throwIfInvalid: true, ref isValid);
                          }

                          {{Create(variant, static (v, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              switch (v.Type.Kind)
                              {
                                  case VariantTypeKind.Unmanaged:
                                      b.Append($"this._unmanagedVariantsContainer = new UnmanagedVariantsContainer(value);");
                                      break;
                                  case VariantTypeKind.Value or VariantTypeKind.Unknown:
                                      b.Append($"this._{v.Name} = value;");
                                      break;
                                  case VariantTypeKind.Reference:
                                      b.Append("this._referenceVariantsContainer = value;");
                                      break;
                              }
                          })}}
                          this.Variant = VariantModel.{{variant.Name}};
                      }
                      """
                );
            }

            b.Append(
                $$"""
                  {{Summary("Initializes a new instance.")}}
                  {{Param("value", "The instance whose variant value to use when initializing the new instance.")}}
                  [{{typeof(OverloadResolutionPriorityAttribute)}}(0)]
                  public {{model.Name}}({{new UnionTypeNameComponent(model)}} value)
                  {
                      {{Create(model, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          var unmanagedAssigned = false;
                          var referenceAssigned = false;

                          for (var i = 0; i < m.Variants.Count; i++)
                          {
                              ct.ThrowIfCancellationRequested();

                              var variant = m.Variants[i];
                              switch (variant.Type.Kind)
                              {
                                  case VariantTypeKind.Unmanaged:
                                      if (unmanagedAssigned)
                                      {
                                          continue;
                                      }

                                      if (i is not 0)
                                      {
                                          b.AppendLine();
                                      }

                                      b.Append("this._unmanagedVariantsContainer = value._unmanagedVariantsContainer;");
                                      unmanagedAssigned = true;
                                      break;
                                  case VariantTypeKind.Reference:
                                      if (referenceAssigned)
                                      {
                                          continue;
                                      }

                                      if (i is not 0)
                                      {
                                          b.AppendLine();
                                      }

                                      referenceAssigned = true;
                                      b.Append("this._referenceVariantsContainer = value._referenceVariantsContainer;");
                                      break;
                                  case VariantTypeKind.Value or VariantTypeKind.Unknown:
                                      if (i is not 0)
                                      {
                                          b.AppendLine();
                                      }

                                      b.Append($"this._{variant.Name} = value._{variant.Name};");
                                      break;
                              }
                          }
                      })}}
                      this.Variant = value.Variant;
                  }
                  """
            );
        });

        var region = Region("Constructors", body);

        builder.Append(region);
    }
}

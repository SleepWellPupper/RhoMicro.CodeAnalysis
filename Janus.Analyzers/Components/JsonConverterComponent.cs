// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct JsonConverterComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = Create(Model, static (m, b, _) =>
        {
            b.Append(
                $$"""
                  private static readonly global::System.Collections.Generic.HashSet<global::System.Text.Json.JsonNamingPolicy> _knownNamingPolicies =
                  [
                         global::System.Text.Json.JsonNamingPolicy.CamelCase,
                         global::System.Text.Json.JsonNamingPolicy.KebabCaseLower,
                         global::System.Text.Json.JsonNamingPolicy.KebabCaseUpper,
                         global::System.Text.Json.JsonNamingPolicy.SnakeCaseLower,
                         global::System.Text.Json.JsonNamingPolicy.SnakeCaseUpper
                  ];

                  private static readonly byte[] _variantDefaultCase = global::System.Text.Encoding.UTF8.GetBytes("Variant");
                  private static readonly byte[] _variantLowerCase = global::System.Text.Encoding.UTF8.GetBytes("variant");
                  private static readonly byte[] _valueDefaultCase = global::System.Text.Encoding.UTF8.GetBytes("Value");
                  private static readonly byte[] _valueLowerCase = global::System.Text.Encoding.UTF8.GetBytes("value");

                  {{Inheritdoc()}}
                  public override {{new UnionTypeNameComponent(m, RenderNullable: true)}} Read(
                          ref global::System.Text.Json.Utf8JsonReader reader,
                          global::System.Type typeToConvert,
                          global::System.Text.Json.JsonSerializerOptions options)
                  {
                      if (!global::System.Text.Json.JsonElement.TryParseValue(ref reader, out var e) ||
                          e is not { ValueKind: global::System.Text.Json.JsonValueKind.Object } unionObject)
                      {
                          throw new global::System.Text.Json.JsonException("Unable to read a union object value from the reader.");
                      }

                      if (!TryReadVariantPropertyValue(unionObject, options, out var variantElement))
                      {
                          throw new global::System.Text.Json.JsonException(
                                  $"Unable to read a value for the '{nameof(Variant)}' property of the union object.");
                      }
                      
                      var variant = global::System.Text.Json.JsonSerializer.Deserialize<VariantKind>(variantElement, options); 

                      if (!TryReadValuePropertyValue(unionObject, options, out var valueElement))
                      {
                          throw new global::System.Text.Json.JsonException(
                                  $"Unable to read a value for the '{nameof(Value)}' property of the union object.");
                      }

                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"return global::System.Text.Json.JsonSerializer.Deserialize<{v.Type.NullableName}>(valueElement, options)");

                          if (v.Type is { IsNullable: false, Kind: VariantTypeKind.Reference })
                          {
                              b.Append($$"""
                                          ?? throw new global::System.Text.Json.JsonException($"Unable to deserialize a non-null value for the represented non-nullable variant '{nameof(VariantKind.{{v.Name}})}' of type '{typeof({{v.Type.NullableName}})}'.")
                                         """
                              );
                          }

                          b.Append(';');
                      }, static (_, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append(
                              """
                              var exception = new global::System.Text.Json.JsonException(
                                      $"Unable to read a value for the '{nameof(Value)}' property of the union object, as the " +
                                      $"'{nameof(Variant)}' property is not representing a valid variant of this union: " +
                                      $"'{variant}'. This could be either because the union itself was not serialized " +
                                      "correctly, or due to a bug in the 'Janus' Janussource generator that generated this union " +
                                      "type. Please verify that the serialized data is in the correct format, and, if " +
                                      "necessary, report an issue to the maintainer. The json used for deserialization " +
                                      "has been attached to this exception.");

                              exception.Data[$"{GetType()}.Data"] =
                                      global::System.Text.Json.JsonSerializer.Serialize(unionObject, options);

                              throw exception;
                              """
                          );
                      }, "variant")}}
                  }

                  private bool TryReadVariantPropertyValue(
                          global::System.Text.Json.JsonElement unionObject,
                          global::System.Text.Json.JsonSerializerOptions options,
                          out global::System.Text.Json.JsonElement value)
                      => TryReadPropertyValue(
                              unionObject,
                              options,
                              defaultName: nameof(Variant),
                              defaultUtf8Name: _variantDefaultCase,
                              lowerCaseUtf8Name: _variantLowerCase,
                              out value);

                  private bool TryReadValuePropertyValue(
                          global::System.Text.Json.JsonElement unionObject,
                          global::System.Text.Json.JsonSerializerOptions options,
                          out global::System.Text.Json.JsonElement value)
                      => TryReadPropertyValue(
                              unionObject,
                              options,
                              defaultName: nameof(Value),
                              defaultUtf8Name: _valueDefaultCase,
                              lowerCaseUtf8Name: _valueLowerCase,
                              out value);

                  private bool TryReadPropertyValue(
                          global::System.Text.Json.JsonElement unionObject,
                          global::System.Text.Json.JsonSerializerOptions options,
                          string defaultName,
                          global::System.ReadOnlySpan<byte> defaultUtf8Name,
                          global::System.ReadOnlySpan<byte> lowerCaseUtf8Name,
                          out global::System.Text.Json.JsonElement value)
                  {
                      if (options.PropertyNamingPolicy == null || // Pascal case (for our properties)
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.SnakeCaseUpper ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.KebabCaseUpper)
                      {
                          return unionObject.TryGetProperty(defaultUtf8Name, out value) ||
                                 options.PropertyNameCaseInsensitive &&
                                 unionObject.TryGetProperty(lowerCaseUtf8Name, out value);
                      }

                      if (options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.CamelCase ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.SnakeCaseLower ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.KebabCaseLower)
                      {
                          return unionObject.TryGetProperty(lowerCaseUtf8Name, out value) ||
                                 options.PropertyNameCaseInsensitive &&
                                 unionObject.TryGetProperty(defaultUtf8Name, out value);
                      }

                      var convertedName = options.PropertyNamingPolicy.ConvertName(defaultName);
                      var comparison = options.PropertyNameCaseInsensitive
                              ? global::System.StringComparison.OrdinalIgnoreCase
                              : global::System.StringComparison.Ordinal;
                      foreach (var property in unionObject.EnumerateObject())
                      {
                          if (!property.Name.Equals(convertedName, comparison))
                          {
                              continue;
                          }

                          value = property.Value;
                          return true;
                      }

                      value = default;
                      return false;
                  }

                  private void WriteVariantPropertyName(global::System.Text.Json.Utf8JsonWriter writer, global::System.Text.Json.JsonSerializerOptions options)
                      => WritePropertyName(
                              writer,
                              options,
                              defaultName: nameof(Variant),
                              defaultUtf8Name: _variantDefaultCase,
                              lowerCaseUtf8Name: _variantLowerCase);

                  private void WriteValuePropertyName(global::System.Text.Json.Utf8JsonWriter writer, global::System.Text.Json.JsonSerializerOptions options)
                      => WritePropertyName(
                              writer,
                              options,
                              defaultName: nameof(Value),
                              defaultUtf8Name: _valueDefaultCase,
                              lowerCaseUtf8Name: _valueLowerCase);

                  private void WritePropertyName(
                          global::System.Text.Json.Utf8JsonWriter writer,
                          global::System.Text.Json.JsonSerializerOptions options,
                          string defaultName,
                          global::System.ReadOnlySpan<byte> defaultUtf8Name,
                          global::System.ReadOnlySpan<byte> lowerCaseUtf8Name)
                  {
                      if (options.PropertyNamingPolicy == null ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.SnakeCaseUpper ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.KebabCaseUpper)
                      {
                          writer.WritePropertyName(defaultUtf8Name);
                          return;
                      }

                      if (options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.CamelCase ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.SnakeCaseLower ||
                          options.PropertyNamingPolicy == global::System.Text.Json.JsonNamingPolicy.KebabCaseLower)
                      {
                          writer.WritePropertyName(lowerCaseUtf8Name);
                          return;
                      }

                      var convertedName = options.PropertyNamingPolicy.ConvertName(defaultName);
                      writer.WritePropertyName(convertedName);
                  }

                  {{Inheritdoc()}}
                  public override void Write(
                          global::System.Text.Json.Utf8JsonWriter writer,
                          {{new UnionTypeNameComponent(m)}} value,
                          global::System.Text.Json.JsonSerializerOptions options)
                  {
                      writer.WriteStartObject();
                      WriteVariantPropertyName(writer, options);
                      global::System.Text.Json.JsonSerializer.Serialize(writer, value.Variant.Kind, options);
                      WriteValuePropertyName(writer, options);
                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append(
                              $$"""
                                global::System.Text.Json.JsonSerializer.Serialize(writer, value.CastTo{{v.Name}}, options);
                                break;
                                """
                          );
                      }, static (_, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();
                          b.Append("throw value.CreateUnknownVariantException();");
                      }, "value.Variant.Kind")}}

                      writer.WriteEndObject();
                  }
                  """
            );
        });

        var type = Create((Model, members), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, members) = t;

            b.Append(
                $"""
                 {Summary(model, static (m, b, ct) =>
                 {
                     ct.ThrowIfCancellationRequested();

                     b.Append($"Implements JSON conversion logic for serializing and deserializing instances of {Cref(m.DocsCommentId)}.");
                 })}
                 {Type(
                     "private sealed class",
                     "JsonConverter",
                     members,
                     baseTypeList:
                     [
                         TypeName(
                             $"global::System.Text.Json.Serialization.JsonConverter<{new UnionTypeNameComponent(model)}>")
                     ])}
                 """
            );
        });

        var region = Region("Json Converter", type);

        builder.Append(region);
    }
}

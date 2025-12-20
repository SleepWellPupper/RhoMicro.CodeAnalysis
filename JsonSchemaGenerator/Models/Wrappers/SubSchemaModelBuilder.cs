// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

internal readonly struct SubSchemaModelBuilder
{
    public SubSchemaModelBuilder()
    {
        Annotations = new(JsonValueModel.CreateObject());
        Simple = new(() => new(JsonValueModel.CreateSimpleSchema()));
        Ref = new(() => new(JsonValueModel.CreateRefSchema()));
        Enum = new(() => new(JsonValueModel.CreateEnumSchema()));
    }
    private AnnotationsBuilder Annotations { get; }
    private Lazy<SimpleSchemaModelBuilder> Simple { get; }
    private Lazy<RefSchemaModelBuilder> Ref { get; }
    private Lazy<EnumSchemaModelBuilder> Enum { get; }

    public void Build(List<JsonValueModel> result, Boolean includeId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(Simple is { IsValueCreated: true, Value: { } simple })
        {
            var simpleSchema = new Lazy<SimpleSchemaModel>(JsonValueModel.CreateSimpleSchema);
            if(simple is
                {
                    Type.Model: { Value.Count: > 0 } onlyTypes,
                    Additional.ModelOrSetDefault: JsonBooleanModel { Value: false },
                    Items.IsValueCreated: false,
                    Properties.Model.Value.Count: 0,
                    Required.Model.Value.Count: 0,
                })
            {
                simpleSchema.Value.SetProperty("type", onlyTypes);
            } else
            {
                if(simple.Items.IsValueCreated && simple.Items.Value.Build(includeId: false, ct) is { Value.Count: > 0 } items)
                {
                    simpleSchema.Value.SetProperty("items", items);
                }

                if(simple.Type.Model is { Value.Count: > 0 } types)
                {
                    simpleSchema.Value.SetProperty("type", types);
                }

                if(simple.Properties.Model is { Value.Count: > 0 } properties)
                {
                    simpleSchema.Value.SetProperty("properties", properties);
                }

                if(simple.Required.Model is { Value.Count: > 0 } required)
                {
                    simpleSchema.Value.SetProperty("required", required);
                }
            }

            if(simple.Type.Model.Value.Any(t => t is JsonTypeModel { Value: JsonType.Object }))
            {
                simpleSchema.Value.SetProperty("additionalProperties", simple.Additional.ModelOrSetDefault);
            }

            if(includeId && simple.GetId() is { Value.Length: > 0 } id)
            {
                simpleSchema.Value.SetProperty("$id", id);
            }

            if(simpleSchema is { IsValueCreated: true, Value: { } schema })
            {
                Annotations.CopyTo(schema);
                result.Add(schema);
            }
        }

        if(Ref is { IsValueCreated: true, Value.Model: { } @ref })
        {
            Annotations.CopyTo(@ref);
            result.Add(@ref);
        }

        if(Enum is { IsValueCreated: true, Value.Model: { } @enum })
        {
            Annotations.CopyTo(@enum);
            result.Add(@enum);
        }
    }

    public void Populate(GeneratorAttributeSyntaxContext ctx, CancellationToken ct) =>
        Populate(rootId: null, ctx.TargetSymbol, ctx.Attributes, [], populateNonProperties: false, ct);
    private void Populate(Id? rootId, ISymbol symbol, HashSet<String> idCache, CancellationToken ct) =>
        Populate(rootId, symbol, symbol.GetAttributes(), idCache, populateNonProperties: true, ct);
    private void Populate(
        Id? rootId,
        ISymbol symbol,
        ImmutableArray<AttributeData> attributes,
        HashSet<String> idCache,
        Boolean populateNonProperties,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(symbol is not INamedTypeSymbol target)
        {
            if(symbol is ITypeSymbol typeSymbol && populateNonProperties)
            {
                PopulateNonProperties(rootId, typeSymbol, idCache, isKnownToBeRef: false, ct);
            }

            return;
        }

        ct.ThrowIfCancellationRequested();
        var id = Id.Create(attributes, target, out var attribute, ct);

        if(rootId.HasValue && idCache.Contains(id.Value))
        {
            Ref.Value.Ref(id, rootId.Value);

            if(populateNonProperties)
            {
                PopulateNonProperties(rootId, target, idCache, isKnownToBeRef: true, ct);
            }

            return;
        }

        rootId ??= id;

        ct.ThrowIfCancellationRequested();
        if(populateNonProperties)
        {
            PopulateNonProperties(rootId, target, idCache, isKnownToBeRef: false, ct);
        }

        ct.ThrowIfCancellationRequested();
        if(Ref.IsValueCreated)
        {
            return;
        }

        ct.ThrowIfCancellationRequested();
        // is schema?
        if(attribute is not null)
        {
            Simple.Value.SetId(id);
            _ = idCache.Add(id.Value);
        }

        // annotations
        if(attribute is not null)
        {
            Annotations.Description.Value = attribute.Description;
            Annotations.Title.Value = attribute.Title;
        }

        // properties
        var members = target.GetMembers();
        for(var i = 0; i < members.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            if(members[i] is not IPropertySymbol
                {
                    SetMethod: { },
                    GetMethod: { },
                    DeclaredAccessibility: Accessibility.Public,
                    Name: { } propName,
                    IsIndexer: false
                } propSymbol ||
                propSymbol.TryGetFirstJsonSchemaExcludeAttribute(out _) ||
                IsKnownExcludedProperty(propSymbol))
            {
                continue;
            }

            var propType = propSymbol.Type;
            var propSchema = Simple.Value.Properties.Add(propName).SubSchema();
            propSchema.Populate(rootId, propType, idCache, ct);

            if(propSymbol.IsRequired)
            {
                Simple.Value.Required.Add(propName);
            }

            if(propSymbol.TryGetFirstJsonSchemaPropertyAttribute(out var a))
            {
                propSchema.Annotations.Description.Value = a.Description;
                propSchema.Annotations.Title.Value = a.Title;
            }
        }
    }

    private void PopulateNonProperties(
        Id? rootId,
        ITypeSymbol target,
        HashSet<String> idCache,
        Boolean isKnownToBeRef,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        // Nullable<>
        if(target is INamedTypeSymbol
            {
                IsGenericType: true,
                TypeArguments: [{ } typeArg],
                OriginalDefinition.SpecialType: SpecialType.System_Nullable_T
            })
        {
            ct.ThrowIfCancellationRequested();
            Simple.Value.Type.Add(JsonType.Null);
            if(typeArg is INamedTypeSymbol namedTypeArg)
            {
                PopulateNonProperties(rootId, namedTypeArg, idCache, isKnownToBeRef, ct);
            }

            return;
        }
        // enum
        else if(target is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: { } underlyingType })
        {
            ct.ThrowIfCancellationRequested();
            var definedConstants = target.GetMembers()
                .OfType<IFieldSymbol>()
                .Select(f => (f.Name, f.ConstantValue));

            foreach(var (name, value) in definedConstants)
            {
                ct.ThrowIfCancellationRequested();
                Enum.Value.Add(name);
                Number? strongValue = value switch
                {
                    SByte b => b,
                    Int16 s => s,
                    Int32 i => i,
                    Int64 l => l,
                    Byte ub => (UInt64)ub,
                    UInt16 us => (UInt64)us,
                    UInt32 ui => (UInt64)ui,
                    UInt64 ul => ul,
                    _ => null
                };
                if(strongValue.HasValue)
                {
                    Enum.Value.Add(strongValue.Value);
                }
            }

            Populate(rootId, underlyingType, idCache, ct);
            return;
        }
        // reftype?
        else if(target.NullableAnnotation == NullableAnnotation.Annotated)
        {
            ct.ThrowIfCancellationRequested();
            Simple.Value.Type.Add(JsonType.Null);
        }

        if(isKnownToBeRef)
        {
            return;
        }

        ct.ThrowIfCancellationRequested();
        var typeString = target.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNoGlobalNamespaceFormat);
        // known type
        if(_builtInTypes.TryGetValue(typeString, out var builtinTypes))
        {
            // type is supported simple type that does not have special schema props
            foreach(var builtinType in builtinTypes)
            {
                ct.ThrowIfCancellationRequested();
                Simple.Value.Type.Add(builtinType);
            }
        }
        // ref
        else if(rootId.HasValue && target.TryGetFirstJsonSchemaAttribute(out var a))
        {
            ct.ThrowIfCancellationRequested();
            // type is complex type w/ schema
            Ref.Value.Ref(Id.Create(a, target, ct), rootId.Value);
            return;
        }
        // list-like
        else if(target switch
        {
            IArrayTypeSymbol { ElementType: { } arrayElement } => arrayElement,
            INamedTypeSymbol { OriginalDefinition: { } originalDefinition, TypeArguments: [{ } collectionElement] } => originalDefinition is
            {
                SpecialType:
                    SpecialType.System_Collections_Generic_IEnumerable_T
                    or SpecialType.System_Collections_Generic_IList_T
                    or SpecialType.System_Collections_Generic_IReadOnlyList_T
                    or SpecialType.System_Collections_Generic_ICollection_T
                    or SpecialType.System_Collections_Generic_IReadOnlyCollection_T
            } || _listLikeTypes.Contains(originalDefinition.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNoGlobalNamespaceFormat))
                ? collectionElement
                : null,
            _ => null
        } is { } elementType)
        {
            ct.ThrowIfCancellationRequested();
            // type is list-like type, items type is obtained recursively
            Simple.Value.Items.Value.SubSchema().Populate(rootId, elementType, idCache, ct);
            Simple.Value.Type.Add(JsonType.Array);
        }
        // map-like
        else if(target is INamedTypeSymbol
        {
            OriginalDefinition: { } originalDefinition,
            TypeArguments: [{ } _, { } valueType]
        } && _mapLikeTypes.Contains(originalDefinition.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNoGlobalNamespaceFormat)))
        {
            ct.ThrowIfCancellationRequested();
            // type is map-like type, values type is obtained recursively
            Simple.Value.Additional.Schema.SubSchema().Populate(rootId, valueType, idCache, ct);
            Simple.Value.Type.Add(JsonType.Object);
        } else
        {
            Simple.Value.Type.Add(JsonType.Object);
        }

        // type is not supported or of unknown schema
    }

    private static Boolean IsKnownExcludedProperty(IPropertySymbol propSymbol) =>
        _knownExcludedProperties.TryGetValue(
            propSymbol.ContainingType.OriginalDefinition.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNoGlobalNamespaceFormat),
            out var excludedProperties)
        && excludedProperties.Contains(propSymbol.Name);
    private static readonly Dictionary<String, HashSet<String>> _knownExcludedProperties = new()
    {
        ["System.Collections.Generic.List<T>"] = ["Capacity"]
    };
    private static readonly HashSet<String> _listLikeTypes =
    [
        "System.Collections.Generic.ISet<T>",
        "System.Collections.Generic.IReadOnlySet<T>",
        "System.Collections.Generic.HashSet<T>",
        "System.Collections.Generic.List<T>"
    ];
    private static readonly HashSet<String> _mapLikeTypes =
    [
        "System.Collections.Generic.IDictionary<TKey, TValue>",
        "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>",
        "System.Collections.Generic.Dictionary<TKey, TValue>"
    ];
    private static readonly Dictionary<String, JsonType[]> _builtInTypes = new()
    {
        ["float"] = [JsonType.Number],
        ["double"] = [JsonType.Number],
        ["decimal"] = [JsonType.Number],

        ["sbyte"] = [JsonType.Integer],
        ["short"] = [JsonType.Integer],
        ["int"] = [JsonType.Integer],
        ["long"] = [JsonType.Integer],

        ["byte"] = [JsonType.Integer],
        ["ushort"] = [JsonType.Integer],
        ["uint"] = [JsonType.Integer],
        ["ulong"] = [JsonType.Integer],

        ["bool"] = [JsonType.Boolean],

        ["string"] = [JsonType.String],

        ["object"] = [JsonType.Object],

        ["System.DateTime"] = [JsonType.String],
        ["System.TimeSpan"] = [JsonType.String],
        ["System.TimeOnly"] = [JsonType.String],
        ["System.DateOnly"] = [JsonType.String],
        ["System.DateTimeOffset"] = [JsonType.String]
    };
    public override Int32 GetHashCode() => throw new NotSupportedException("GetHashCode is not supported on this type. This indicates a bug or error, as instances of this typoe are not intended to be cached.");
    public override Boolean Equals(Object? other) => throw new NotSupportedException("GetHashCode is not supported on this type. This indicates a bug or error, as instances of this typoe are not intended to be cached.");
}

internal static class SymbolDisplayFormats
{
    public static readonly SymbolDisplayFormat FullyQualifiedNoGlobalNamespaceFormat = new(
         SymbolDisplayGlobalNamespaceStyle.Omitted,
         SymbolDisplayFormat.FullyQualifiedFormat.TypeQualificationStyle,
         SymbolDisplayFormat.FullyQualifiedFormat.GenericsOptions,
         SymbolDisplayFormat.FullyQualifiedFormat.MemberOptions,
         SymbolDisplayFormat.FullyQualifiedFormat.DelegateStyle,
         SymbolDisplayFormat.FullyQualifiedFormat.ExtensionMethodStyle,
         SymbolDisplayFormat.FullyQualifiedFormat.ParameterOptions,
         SymbolDisplayFormat.FullyQualifiedFormat.PropertyStyle,
         SymbolDisplayFormat.FullyQualifiedFormat.LocalOptions,
         SymbolDisplayFormat.FullyQualifiedFormat.KindOptions,
         SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions);
}

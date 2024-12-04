namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

readonly struct SubSchemaModelBuilder
{
    public SubSchemaModelBuilder()
    {
        Simple = new(() => new(JsonValueModel.CreateSimpleSchema()));
        Ref = new(() => new(JsonValueModel.CreateRefSchema()));
        Enum = new(() => new(JsonValueModel.CreateEnumSchema()));
    }
    public Lazy<SimpleSchemaModelBuilder> Simple { get; }
    public Lazy<RefSchemaModelBuilder> Ref { get; }
    public Lazy<EnumSchemaModelBuilder> Enum { get; }

    public void Build(List<JsonValueModel> result, Boolean includeId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        //if(Simple is
        //    {
        //        IsValueCreated: true,
        //        Value: not
        //        {
        //            Additional.ModelOrSetDefault: JsonBooleanModel { Value: false },
        //            Items.IsValueCreated: false,
        //            Properties.Model.Value.Count: 0,
        //            Required.Model.Value.Count: 0,
        //            Type.Model.Value.Count: 0
        //        }
        //    })
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
                    simpleSchema.Value.SetProperty("items", items);
                if(simple.Type.Model is { Value.Count: > 0 } types)
                    simpleSchema.Value.SetProperty("type", types);
                if(simple.Properties.Model is { Value.Count: > 0 } properties)
                    simpleSchema.Value.SetProperty("properties", properties);
                if(simple.Required.Model is { Value.Count: > 0 } required)
                    simpleSchema.Value.SetProperty("required", required);
            }

            if(simple.Type.Model.Value.Any(t => t is JsonTypeModel { Value: JsonType.Object }))
                simpleSchema.Value.SetProperty("additionalProperties", simple.Additional.ModelOrSetDefault);
            if(simple.Annotations.Title.Value is { Length: > 0 } title)
                simpleSchema.Value.String("title").Value = title;
            if(simple.Annotations.Description.Value is { Length: > 0 } description)
                simpleSchema.Value.String("description").Value = description;

            if(includeId && simple.Id is { Value.Length: > 0 } id)
                simpleSchema.Value.SetProperty("$id", id);

            if(simpleSchema.IsValueCreated)
                result.Add(simpleSchema.Value);
        }

        if(Ref.IsValueCreated)
            result.Add(Ref.Value.Model);
        if(Enum.IsValueCreated)
            result.Add(Enum.Value.Model);
    }

    public void Populate(GeneratorAttributeSyntaxContext ctx, CancellationToken ct) =>
        Populate(ctx.TargetSymbol, ctx.Attributes, [], populateNonProperties: false, ct);
    private void Populate(ISymbol symbol, HashSet<String> idCache, CancellationToken ct) =>
        Populate(symbol, symbol.GetAttributes(), idCache, populateNonProperties: true, ct);
    private void Populate(
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
                PopulateNonProperties(typeSymbol, idCache, isKnownToBeRef: false, ct);

            return;
        }

        ct.ThrowIfCancellationRequested();
        var id = GetId(attributes, target, out var attribute, ct);

        if(idCache.Contains(id))
        {
            Ref.Value.Ref(id);

            if(populateNonProperties)
                PopulateNonProperties(target, idCache, isKnownToBeRef: true, ct);

            return;
        }

        ct.ThrowIfCancellationRequested();
        if(populateNonProperties)
            PopulateNonProperties(target, idCache, isKnownToBeRef: false, ct);

        ct.ThrowIfCancellationRequested();
        if(Ref.IsValueCreated)
            return;

        ct.ThrowIfCancellationRequested();
        // is schema?
        if(attribute is not null)
        {
            Simple.Value.Id.Value = id;
            _ = idCache.Add(id);
        }

        // annotations
        if(attribute is not null)
        {
            Simple.Value.Annotations.Description.Value = attribute.Description;
            Simple.Value.Annotations.Title.Value = attribute.Title;
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
            propSchema.Populate(propType, idCache, ct);

            if(propSymbol.IsRequired)
                Simple.Value.Required.Add(propName);

            if(propSymbol.TryGetFirstJsonSchemaPropertyAttribute(out var a))
            {
                propSchema.Simple.Value.Annotations.Description.Value = a.Description;
                propSchema.Simple.Value.Annotations.Title.Value = a.Title;
            }
        }
    }

    private void PopulateNonProperties(
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
                PopulateNonProperties(namedTypeArg, idCache, isKnownToBeRef, ct);
            return;
        }
        // enum
        else if(target is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: { } underlyingType })
        {
            ct.ThrowIfCancellationRequested();
            var definedConstantNames = target.GetMembers()
                .OfType<IFieldSymbol>()
                .Select(f => f.Name);

            foreach(var definedConstantName in definedConstantNames)
            {
                ct.ThrowIfCancellationRequested();
                Enum.Value.Add(definedConstantName);
            }

            Populate(underlyingType, idCache, ct);
            return;
        }
        // reftype?
        else if(target.NullableAnnotation == NullableAnnotation.Annotated)
        {
            ct.ThrowIfCancellationRequested();
            Simple.Value.Type.Add(JsonType.Null);
        }

        if(isKnownToBeRef)
            return;

        ct.ThrowIfCancellationRequested();
        var typeString = target.ToDisplayString(_fullyQualifiedNoGlobalNamespaceFormat);
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
        else if(target.TryGetFirstJsonSchemaAttribute(out var a))
        {
            ct.ThrowIfCancellationRequested();
            // type is complex type w/ schema
            Ref.Value.Ref(GetId(a, target, ct));
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
            } || _listLikeTypes.Contains(originalDefinition.ToDisplayString(_fullyQualifiedNoGlobalNamespaceFormat))
                ? collectionElement
                : null,
            _ => null
        } is { } elementType)
        {
            ct.ThrowIfCancellationRequested();
            // type is list-like type, items type is obtained recursively
            Simple.Value.Items.Value.SubSchema().Populate(elementType, idCache, ct);
            Simple.Value.Type.Add(JsonType.Array);
        }
        // map-like
        else if(target is INamedTypeSymbol
        {
            OriginalDefinition: { } originalDefinition,
            TypeArguments: [{ } _, { } valueType]
        } && _mapLikeTypes.Contains(originalDefinition.ToDisplayString(_fullyQualifiedNoGlobalNamespaceFormat)))
        {
            ct.ThrowIfCancellationRequested();
            // type is map-like type, values type is obtained recursively
            Simple.Value.Additional.Schema.SubSchema().Populate(valueType, idCache, ct);
            Simple.Value.Type.Add(JsonType.Object);
        } else
        {
            Simple.Value.Type.Add(JsonType.Object);
        }

        // type is not supported or of unknown schema
    }

    private static String GetId(ImmutableArray<AttributeData> attributes, ISymbol target, out JsonSchemaAttribute? attribute, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        for(var i = 0; i < attributes.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            if(JsonSchemaAttribute.TryCreate(attributes[i], out var a))
            {
                attribute = a;
                return GetId(a, target, ct);
            }
        }

        attribute = null;
        return GetId(target, ct);
    }
    private static String GetId(JsonSchemaAttribute? attribute, ISymbol target, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var id = attribute?.Id?.Trim();

        if(String.IsNullOrWhiteSpace(id))
            id = GetId(target, ct);

        return id!;
    }
    private static String GetId(ISymbol target, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var result = target.ToDisplayString(_fullyQualifiedNoGlobalNamespaceFormat).Replace('.', '/');
        return result;
    }
    private static Boolean IsKnownExcludedProperty(IPropertySymbol propSymbol) =>
        _knownExcludedProperties.TryGetValue(
            propSymbol.ContainingType.OriginalDefinition.ToDisplayString(_fullyQualifiedNoGlobalNamespaceFormat),
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
    private static readonly SymbolDisplayFormat _fullyQualifiedNoGlobalNamespaceFormat = new(
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
    public override Int32 GetHashCode() => throw new NotSupportedException("GetHashCode is not supported on this type. This indicates a bug or error, as instances of this typoe are not intended to be cached.");
    public override Boolean Equals(Object? other) => throw new NotSupportedException("GetHashCode is not supported on this type. This indicates a bug or error, as instances of this typoe are not intended to be cached.");
}
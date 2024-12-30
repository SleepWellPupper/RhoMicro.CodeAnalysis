namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

using static Constants;
using RhoMicro.CodeAnalysis.Library.Models;

/// <summary>
/// Generates factories for parsing attributes from <see cref="AttributeData"/> instances.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AttributeFactoryGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var initMethodProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            InitializationMethodAttributeMetadataName,
            static (_, _) => true,
            static (ctx, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if(ctx.TargetSymbol is not IMethodSymbol
                    {
                        ContainingType:
                        {
                            ContainingType: { } containingType
                        }
                    } target)
                {
                    return null;
                }

                InitializationMethodAttribute.Model? attributeModel = null;
                foreach(var attribute in ctx.Attributes)
                {
                    ct.ThrowIfCancellationRequested();

                    if(attribute.TryGetInitializationMethodAttributeModel(out var m, cancellationToken: ct))
                    {
                        attributeModel = m;
                        break;
                    }
                }

                if(attributeModel is null)
                    return null;

                using var modelCreationContext = ModelCreationContext.CreateDefault(ct);
                var model = InitializationMethodModel.Create(target, attributeModel.Value, in modelCreationContext);
                var result = new InitializationMethodPipelineData(model, NamedTypeModel.Create(containingType, in modelCreationContext).GetDisplayString(ct));

                return result;
            })
            .Where(static m => m is not null)
            .Collect()
            .Select(static (data, ct) =>
            {
                using var ctx = ModelCreationContext.CreateDefault(ct);

                ctx.ThrowIfCancellationRequested();

                var result = ctx.CollectionFactory.CreateLazyDictionary<String, EquatableList<InitializationMethodModel>>(
                    (_, d) => d.CollectionFactory.CreateList<InitializationMethodModel>());

                foreach(var datum in data)
                {
                    ctx.ThrowIfCancellationRequested();

                    var (model, key) = datum!;

                    result[key].Add(model);
                }

                return result;
            });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            GenerateFactoryAttributeMetadataName,
            static (_, _) => true,
            static (ctx, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if(ctx is not { Attributes: [{ } attribute], TargetSymbol: INamedTypeSymbol target })
                    return null;

                using var modelCreationContext = ModelCreationContext.CreateDefault(ct);
                var model = AttributeFactoryModel.Create(target, attribute, in modelCreationContext);

                return model;
            }).Where(static m => m is not null)
            .Combine(initMethodProvider)
            .Select(static (t, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var (model, initMethodMap) = t;

                var result = initMethodMap.TryGetValue(
                    model!.Signature.GetDisplayString(ct),
                    out var initMethods)
                ? model with { InitializationMethods = initMethods }
                : model;

                return result;
            })
            .Select(static (m, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var sourceBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
                {
                    AmbientCancellationToken = ct,
                    GeneratorName = typeof(AttributeFactoryGenerator).FullName
                });

                AppendUsings(sourceBuilder, ct);

                m.Signature.BuildStrings(sourceBuilder, out var hintName, out var displayString, ct);
                var ctx = new SourceBuildingContext(sourceBuilder, m, displayString, ct);
                AppendPropertyIdType(in ctx);
                AppendConstructorAccessorType(in ctx);
                AppendPropertyAccessorType(in ctx);
                AppendModelType(in ctx);
                sourceBuilder.CloseAllBlocksCore();

                AppendExtensionsType(in ctx);

                var source = sourceBuilder.ToString();

                return (hintName, source);
            });

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }

    #region Usings
    private static void AppendUsings(IndentedStringBuilder sourceBuilder, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        sourceBuilder
            .Append("using RhoMicro.CodeAnalysis.Library.Extensions;")
            .AppendLineCore();
    }
    #endregion
    #region PropertyId
    private static void AppendPropertyIdType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder
            .OpenRegionBlock("Property Id").Comment
            .OpenSummary()
            .Append("Provides strongly typed access to property ids of ").Comment.SeeCRef(ctx.DisplayString).Append(" properties.")
            .CloseBlock()
            .Append("public enum ").Append(ctx.Model.AttributeModel.PropertyIdTypeName)
            .OpenBracesBlock()
            //ordered by name and then by whether or not a ctor parameter mapping exists for the property
            //mapped first is crucial as we depend on the backing values being valid indices into ctor arg maps
            .OpenRegionBlock("Mapped Properties");

        for(var i = 0; i < ctx.Model.MappedProperties.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var mappedProperty = ctx.Model.MappedProperties[i];

            _ = ctx.SourceBuilder.Comment
                .OpenSummary()
                    .Append("The id representing the ").Comment.SeeCRef($"{ctx.DisplayString}.{mappedProperty.Name}").AppendLine(" property.")
                .CloseBlock()

                .Comment.OpenRemarks()
                    .Comment.OpenParagraph()
                        .Append("This property has the following constructor parameter mappings:<br/>")
                        .Comment.OpenList("table");

            foreach(var mapping in mappedProperty.Mappings)
            {
                ctx.ThrowIfCancellationRequested();

                _ = ctx.SourceBuilder.Comment
                    .OpenItem();

                ctx.Model.Constructors[mapping.ConstructorIndex].AppendConstructorExpressionComment(in ctx, mapping.ParameterIndex);

                ctx.SourceBuilder.CloseBlockCore();
            }

            ctx.SourceBuilder
                        .CloseBlock()
                    .CloseBlock()
                .CloseBlock()
                .AppendCore(mappedProperty.Name);

            if(i < ctx.Model.MappedProperties.Count - 1 || ctx.Model.UnmappedProperties.Count > 0)
                ctx.SourceBuilder.Append(',').AppendLineCore();
        }

        _ = ctx.SourceBuilder.CloseBlock()
            .OpenRegionBlock("Unmapped Properties");

        for(var i = 0; i < ctx.Model.UnmappedProperties.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var unmappedProperty = ctx.Model.UnmappedProperties[i];

            ctx.SourceBuilder.Comment
                .OpenSummary()
                    .Append("The id representing the ").Comment.SeeCRef($"{ctx.DisplayString}.{unmappedProperty.Name}").AppendLine(" property.")
                .CloseBlock()
                .Comment.OpenRemarks()
                    .Append("This property is not mapped to any constructor parameters.")
                .CloseBlock()
                .AppendCore(unmappedProperty.Name);

            if(i < ctx.Model.UnmappedProperties.Count - 1)
                ctx.SourceBuilder.Append(',').AppendLineCore();
        }

        ctx.SourceBuilder
            .CloseBlock()
            .CloseBlock()
            .CloseBlockCore();
    }
    #endregion
    #region ConstructorAccessor
    private static void AppendConstructorAccessorType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.OpenRegionBlock("Constructor Accessor");
        OpenConstructorAccessorType(in ctx);
        AppendConstructorAccessorFieldsAndProperties(in ctx);
        AppendConstructorAccessorGeneralTryGet(in ctx);
        AppendConstructorAccessorSpecificTryGet(in ctx);
        AppendEquality(in ctx, ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName);
        ctx.SourceBuilder
            .CloseBlock()
            .CloseBlockCore();
    }
    private static void OpenConstructorAccessorType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Provides strongly typed access to individual properties of ").Comment
            .SeeCRef(ctx.DisplayString).Append(" provided by named arguments an instance of ").Comment
            .SeeCRef(AttributeDataDisplayString).Append('.')
            .CloseBlock().Comment
            .OpenParam("data")
            .Append("The ").Comment.SeeCRef(AttributeDataDisplayString).Append(" instance to wrap.")
            .CloseBlock().Comment
            .OpenParam("isTypeMatch")
            .Append("A value indicating whether or not ").Comment.ParamRef("data")
            .Append(" is representing an instance of ").Comment.SeeCRef(ctx.DisplayString).Append('.')
            .CloseBlock()
            .Append("public readonly struct ")
            .Append(ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName).Append('(')
            .Append(AttributeDataDisplayString).Append(" data, ")
            .Append("bool isTypeMatch")
            .Append(") : global::System.IEquatable<").Append(ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName).Append('>')
            .OpenBracesBlock();
    }
    private static void AppendConstructorAccessorFieldsAndProperties(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder
            .OpenRegionBlock("Fields And Properties").Comment
            .OpenSummary()
            .Append("Gets the wrapped ").Comment.SeeCRef(AttributeDataDisplayString).Append(" instance.")
            .CloseBlock()
            .Append("public ").Append(AttributeDataDisplayString).AppendLine(" Data => data;")
            .Append("private const int _maxMappedPropId = ").Append(( ctx.Model.MappedProperties.Count - 1 ).ToString()).AppendLine(';').Comment
            .OpenSummary()
            .Append("The mapping of constructor indices onto their constructor. Parameters mapped to properties are highlighted.")
            .CloseBlock().Comment
            .OpenRemarks().Comment
            .OpenParagraph().Comment
            .OpenList("table").Comment
                .OpenItem().Comment
                    .OpenTerm()
                        .Append('0')
                    .CloseBlock().Comment
                    .OpenDescription()
                        .Append("default fallback constructor")
                    .CloseBlock()
                .CloseBlockCore();

        for(var i = 0; i < ctx.Model.Constructors.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            _ = ctx.SourceBuilder.Comment
                .OpenItem().Comment
                    .OpenTerm()
                        .Append(( i + 1 ).ToString())
                    .CloseBlock().Comment
                    .OpenDescription();

            ctx.Model.Constructors[i].AppendConstructorExpressionComment(in ctx);

            ctx.SourceBuilder.CloseBlock()
                .CloseBlockCore();
        }

        ctx.SourceBuilder.CloseBlock()
            .CloseBlock()
            .CloseBlock()
            .AppendLine("private static readonly global::System.Collections.Immutable.ImmutableArray<global::System.Collections.Immutable.ImmutableArray<int>> _argIndices =")
            .OpenCollectionExprBlock()
                .AppendLine("// ctorIndex->propIndex->paramIndex")
                .AppendCore("//0: default fallback constructor");

        if(ctx.Model.MappedProperties.Count == 0)
        {
            ctx.SourceBuilder
                .AppendLine()
                .AppendCore("global::System.Collections.Immutable.ImmutableArray<int>.Empty");
        } else
        {
            _ = ctx.SourceBuilder.OpenCollectionExprBlock();

            for(var i = 0; i < ctx.Model.MappedProperties.Count; i++)
            {
                ctx.ThrowIfCancellationRequested();

                ctx.SourceBuilder
                    .Append("//").Append(i.ToString()).Append(": ").Append(ctx.Model.MappedProperties[i].Name).AppendLine(" -> ?")
                    .Append("-1,").AppendLineCore();
            }

            ctx.SourceBuilder.CloseBlockCore();
        }

        for(var i = 0; i < ctx.Model.Constructors.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var ctor = ctx.Model.Constructors[i];

            ctx.SourceBuilder
                .AppendLine(',')
                .Append("//").Append(( i + 1 ).ToString()).AppendCore(": ");

            ctor.AppendConstructorExpression(in ctx);

            if(ctx.Model.MappedProperties.Count == 0)
            {
                ctx.SourceBuilder
                    .AppendLine()
                    .AppendCore("global::System.Collections.Immutable.ImmutableArray<int>.Empty");
            } else
            {
                _ = ctx.SourceBuilder.OpenCollectionExprBlock();

                for(var j = 0; j < ctx.Model.MappedProperties.Count; j++)
                {
                    ctx.ThrowIfCancellationRequested();

                    if(j > 0)
                        ctx.SourceBuilder.Append(',').AppendLineCore();

                    var parameterIndex = ctor.Mappings[ctx.Model.MappedProperties[j].Name]?.ParameterIndex ?? -1;
                    var parameterIndexString = parameterIndex.ToString();
                    ctx.SourceBuilder
                        .Append("//").Append(j.ToString()).Append(": ").Append(ctx.Model.MappedProperties[j].Name).Append(" -> ").AppendLine(parameterIndex == -1 ? "?" : $"args[{parameterIndexString}]")
                        .AppendCore(parameterIndexString);
                }

                ctx.SourceBuilder.CloseBlockCore();
            }
        }

        _ = ctx.SourceBuilder.CloseBlock()
            .Append(';')
            .AppendLine().Comment
            .OpenSummary()
            .Append("The mapping of propIndex->argIndex for the ctorIndex of the ctor used in ").Comment.SeeCRef("Data").Append('.')
            .CloseBlock().Comment
            .OpenRemarks()
            .Append("Properties are mapped up to ").Comment.SeeCRef("_maxMappedPropId").Append('.')
            .CloseBlock()
            .AppendLine("private readonly global::System.Collections.Immutable.ImmutableArray<Int32> _specificArgIndices = data.AttributeConstructor?.Parameters switch")
            .OpenBracesBlock();

        for(var i = 0; i < ctx.Model.Constructors.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var ctor = ctx.Model.Constructors[i];

            if(ctor.Parameters.Count == 0)
                continue;

            ctx.SourceBuilder.AppendCore('[');

            for(var j = 0; j < ctor.Parameters.Count; j++)
            {
                ctx.ThrowIfCancellationRequested();

                if(j > 0)
                    ctx.SourceBuilder.AppendCore(", ");

                ctx.SourceBuilder.AppendCore(ctor.Parameters[j].Pattern);
            }

            ctx.SourceBuilder.Append("] => _argIndices[").Append(( i + 1 ).ToString()).Append("],").AppendLineCore();
        }

        ctx.SourceBuilder
            .Append("_ => _argIndices[0]")
            .CloseBlock()
            .Append(';')
            .AppendLineCore();

        ctx.SourceBuilder.CloseBlockCore();
    }
    private static void AppendConstructorAccessorGeneralTryGet(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder
            .Append(
"""
#region General TryGet
        /// <summary>
        /// Attempts to get the value of a property (reference type) mapped from
        /// its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetReferenceTypeValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value)
            where T : class
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetReferenceTypeValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable reference type)
        /// mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetNullableReferenceTypeValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out T? value)
            where T : class
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetNullableReferenceTypeValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of reference types)
        /// mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetReferenceTypeArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetReferenceTypeArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of nullable reference
        /// types) mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetNullableReferenceTypeArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetNullableReferenceTypeArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of nullable
        /// reference types) mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetNullableReferenceTypeNullableArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetNullableReferenceTypeNullableArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of reference
        /// types) mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetReferenceTypeNullableArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetReferenceTypeNullableArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (Type) mapped from its
        /// parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetTypeValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Microsoft.CodeAnalysis.ITypeSymbol? value)
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetTypeValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable Type) mapped from
        /// its parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetNullableTypeValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::Microsoft.CodeAnalysis.ITypeSymbol? value)
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetNullableTypeValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of Types) mapped from
        /// its parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetTypeArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol>? value)
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetTypeArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of nullable Types)
        /// mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetNullableTypeArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol?>? value)
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetNullableTypeArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of nullable
        /// Types) mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetNullableTypeNullableArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol?>? value)
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetNullableTypeNullableArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of Types)
        /// mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetTypeNullableArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol>? value)
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetTypeNullableArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (value type) mapped from its
        /// parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetValueTypeValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value)
            where T : struct
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetValueTypeValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of value types)
        /// mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetValueTypeArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetValueTypeArrayValue(out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of value
        /// types) mapped from its parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined from a mapped
        /// constructor parameter in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        public bool TryGetValueTypeNullableArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).AppendCore(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch || (int)id > _maxMappedPropId || (int)id < 0)
            {
                value = null;
                return false;
            }

            var argIndex = _specificArgIndices[(int)id];
            if(argIndex < 0)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[argIndex].TryGetValueTypeNullableArrayValue(out value);
        }
        #endregion
""");
    }
    private static void AppendConstructorAccessorSpecificTryGet(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder
            .OpenRegionBlock("Specific TryGet")
            .OpenRegionBlock("Mapped Properties");

        for(var i = 0; i < ctx.Model.MappedProperties.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var mappedProperty = ctx.Model.MappedProperties[i];

            ctx.SourceBuilder.Comment
                .OpenSummary()
                .Append("Attempts to retrieve the value for ").Comment.SeeCRef(mappedProperty.Name).Append(" mapped from constructor arguments in <see cref=\"Data\"/>.")
                .CloseBlock().Comment
                .OpenParam("value")
                .Append("The value obtained for ").Comment
                .SeeCRef(mappedProperty.Name)
                .Append(", if one could be found in the constructor arguments in <see cref=\"Data\"/>; otherwise, <see langword=\"null\"/>.")
                .CloseBlock().Comment
                .OpenReturns()
                .Append("<see langword=\"true\"/> if a value for ").Comment.SeeCRef(mappedProperty.Name).Append(" could be found in the constructor arguments in <see cref=\"Data\"/>; otherwise, <see langword=\"false\"/>.")
                .CloseBlock()
                .AppendLine(AggressiveInliningAttributeSyntax)
                .Append("public bool TryGet").Append(mappedProperty.Name)
                .Append("([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ")
                .Append(mappedProperty.Type.NullableDisplayString).Append(" value)")
                .OpenBracesBlock()
                .Append("var argIndex = _specificArgIndices[")
                .Append(i.ToString())
                .AppendLine("];")
                .Append("if(argIndex < 0)")
                .OpenBracesBlock()
                .AppendLine("value = null;")
                .AppendLine("return false;")
                .CloseBlock()
                .Append("return data.ConstructorArguments[argIndex].TryGet").Append(mappedProperty.Type.KindString).Append("Value(out value);")
                .CloseBlockCore();
        }

        _ = ctx.SourceBuilder
            .CloseBlock()
            .OpenRegionBlock("Unmapped Properties");

        foreach(var unmappedProperty in ctx.Model.UnmappedProperties)
        {
            ctx.ThrowIfCancellationRequested();

            ctx.SourceBuilder.Comment
                .OpenSummary()
                .Append("Attempts to retrieve the value for ").Comment.SeeCRef(unmappedProperty.Name).Append(" mapped from constructor arguments in <see cref=\"Data\"/>.")
                .CloseBlock().Comment
                .OpenRemarks()
                .Append("Because no constructor parameter mapping could be determined for ").Comment
                .SeeCRef(unmappedProperty.Name)
                .Append(", this method will always return <see langword=\"false\"/>. It is generated nonetheless to allow for new parameter mappings to be non-breaking.")
                .CloseBlock().Comment
                .OpenParam("value")
                .Append("The value obtained for ").Comment
                .SeeCRef(unmappedProperty.Name)
                .Append(", if one could be found in the constructor arguments in <see cref=\"Data\"/>; otherwise, <see langword=\"null\"/>.")
                .CloseBlock().Comment
                .OpenReturns()
                .Append("<see langword=\"true\"/> if a value for ").Comment.SeeCRef(unmappedProperty.Name).Append(" could be found in the constructor arguments in <see cref=\"Data\"/>; otherwise, <see langword=\"false\"/>.")
                .CloseBlock()
                .AppendLine(AggressiveInliningAttributeSyntax)
                .Append("public bool TryGet").Append(unmappedProperty.Name)
                .Append("([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ")
                .Append(unmappedProperty.Type.NullableDisplayString).Append(" value)")
                .OpenBracesBlock()
                .AppendLine("value = null;")
                .AppendLine("return false;")
                .CloseBlockCore();
        }

        ctx.SourceBuilder
                .CloseBlock()
                .CloseBlockCore();
    }
    #endregion
    #region PropertyAccessor
    private static void AppendPropertyAccessorType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        using var _ = ctx.SourceBuilder.OpenRegionBlockScope("Property Accessor");
        OpenPropertyAccessor(in ctx);
        AppendPropertyAccessorFieldsAndProperties(in ctx);
        AppendPropertyAccessorGeneralTryGet(in ctx);
        AppendPropertyAccessorSpecificTryGet(in ctx);
        AppendPropertyAccessorGet(in ctx);
        AppendEquality(in ctx, $"{ctx.DisplayString}.{ctx.Model.AttributeModel.PropertyAccessorTypeName}");
        ctx.SourceBuilder.CloseBlockCore();
    }
    private static void OpenPropertyAccessor(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Provides strongly typed access to individual properties of ").Comment
            .SeeCRef(ctx.DisplayString).Append(" provided by named and constructor arguments an instance of ").Comment
            .SeeCRef(AttributeDataDisplayString).Append('.')
            .CloseBlock().Comment
            .OpenParam("data")
            .Append("The ").Comment.SeeCRef(AttributeDataDisplayString).Append(" instance to wrap.")
            .CloseBlock().Comment
            .OpenParam("isTypeMatch")
            .Append("A value indicating whether or not ").Comment.ParamRef("data")
            .Append(" is representing an instance of ").Comment.SeeCRef(ctx.DisplayString).Append('.')
            .CloseBlock()
            .Append("public readonly struct ")
            .Append(ctx.Model.AttributeModel.PropertyAccessorTypeName).Append('(')
            .Append(AttributeDataDisplayString).Append(" data, ")
            .Append("bool isTypeMatch")
            .Append(") : global::System.IEquatable<").Append(ctx.Model.AttributeModel.PropertyAccessorTypeName).Append('>')
            .OpenBracesBlock();
    }
    private static void AppendPropertyAccessorFieldsAndProperties(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder
            .OpenRegionBlock("Fields And Properties").Comment
            .OpenSummary()
            .Append("Gets the wrapped ").Comment.SeeCRef(AttributeDataDisplayString).Append(" instance.")
            .CloseBlock()
            .Append("public ").Append(AttributeDataDisplayString).AppendLine(" Data => data;")
            .Append("private readonly ").Append(ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName).AppendLine(" _constructor = new(data, isTypeMatch);")
            .Append("private static readonly global::System.Collections.Immutable.ImmutableArray<string> _propertyNames =")
            .OpenCollectionExprBlock();

        if(ctx.Model.MappedProperties.Count > 0)
        {
            _ = ctx.SourceBuilder.OpenRegionBlock("Mapped Properties");

            for(var i = 0; i < ctx.Model.MappedProperties.Count; i++)
            {
                ctx.ThrowIfCancellationRequested();

                var property = ctx.Model.MappedProperties[i];

                if(i > 0)
                    ctx.SourceBuilder.Append(',').AppendLineCore();

                ctx.SourceBuilder.Append("nameof(").Append(property.Name).AppendCore(')');
            }

            if(ctx.Model.UnmappedProperties.Count > 0)
                ctx.SourceBuilder.Append(',').AppendLineCore();

            ctx.SourceBuilder.CloseBlockCore();
        }

        if(ctx.Model.UnmappedProperties.Count > 0)
        {
            _ = ctx.SourceBuilder.OpenRegionBlock("Unmapped Properties");

            for(var i = 0; i < ctx.Model.UnmappedProperties.Count; i++)
            {
                ctx.ThrowIfCancellationRequested();

                var property = ctx.Model.UnmappedProperties[i];

                if(i > 0)
                    ctx.SourceBuilder.Append(',').AppendLineCore();

                ctx.SourceBuilder.Append("nameof(").Append(property.Name).AppendCore(')');
            }

            ctx.SourceBuilder.CloseBlockCore();
        }

        _ = ctx.SourceBuilder
            .CloseBlock()
            .AppendLine(';')
            .Append("private static readonly global::System.Collections.Immutable.ImmutableHashSet<").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append("> _settableProperties =").OpenCollectionExprBlock();

        var settablePropertiesCount = 0;
        appendSettablePropertyNames(in ctx, ctx.Model.MappedProperties);
        appendSettablePropertyNames(in ctx, ctx.Model.UnmappedProperties);

        ctx.SourceBuilder
            .CloseBlock()
            .Append(';')
            .CloseBlockCore();

        void appendSettablePropertyNames(in SourceBuildingContext ctx, IList<PropertyModel> properties)
        {
            ctx.ThrowIfCancellationRequested();

            foreach(var property in properties)
            {
                ctx.ThrowIfCancellationRequested();

                if(property.HasSetter)
                {
                    if(settablePropertiesCount > 0)
                        ctx.SourceBuilder.Append(',').AppendLineCore();

                    ctx.SourceBuilder.Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(".").AppendCore(property.Name);

                    settablePropertiesCount++;
                }
            }
        }
    }
    private static void AppendPropertyAccessorGeneralTryGet(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder
            .OpenRegionBlock("General TryGet")
.Append(
"""
/// <summary>
        /// Attempts to get the value of a property (reference type) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetReferenceTypeValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetReferenceTypeValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetReferenceTypeValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable reference type) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetNullableReferenceTypeValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out T? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetNullableReferenceTypeValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetNullableReferenceTypeValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of reference types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetReferenceTypeArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetReferenceTypeArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetReferenceTypeArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of nullable reference
        /// types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetNullableReferenceTypeArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetNullableReferenceTypeArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetNullableReferenceTypeArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of nullable
        /// reference types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetNullableReferenceTypeNullableArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetNullableReferenceTypeNullableArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetNullableReferenceTypeNullableArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of reference
        /// types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetReferenceTypeNullableArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetReferenceTypeNullableArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetReferenceTypeNullableArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (Type) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetTypeValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Microsoft.CodeAnalysis.ITypeSymbol? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetTypeValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetTypeValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable Type) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetNullableTypeValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::Microsoft.CodeAnalysis.ITypeSymbol? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetNullableTypeValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetNullableTypeValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of Types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetTypeArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetTypeArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetTypeArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of nullable Types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetNullableTypeArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol?>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetNullableTypeArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetNullableTypeArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of nullable
        /// Types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetNullableTypeNullableArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol?>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetNullableTypeNullableArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetNullableTypeNullableArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of Types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetTypeNullableArrayValue(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.ITypeSymbol>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetTypeNullableArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetTypeNullableArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (value type) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetValueTypeValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value)
            where T : struct
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetValueTypeValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetValueTypeValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (array of value types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetValueTypeArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetValueTypeArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetValueTypeArrayValue(id, out value);
        }
        /// <summary>
        /// Attempts to get the value of a property (nullable array of value
        /// types) as set in
        /// named arguments or mapped from a constructor parameter in <see cref="Data"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property to retrieve.
        /// </typeparam>
        /// <param name="id">
        /// The id of the property to retrieve.
        /// </param>
        /// <param name="value">
        /// The value of the property, if one could be determined; otherwise,
        /// <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value could be determined; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetValueTypeNullableArrayValue<T>(
""").Append(ctx.Model.AttributeModel.PropertyIdTypeName).Append(
"""
 id, out global::System.Collections.Immutable.ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            if(_settableProperties.Contains(id))
            {
                var name = _propertyNames[(int)id];

                foreach(var kvp in data.NamedArguments)
                {
                    if(kvp.Key == name && kvp.Value.TryGetValueTypeNullableArrayValue(out value))
                        return true;
                }
            }

            return _constructor.TryGetValueTypeNullableArrayValue(id, out value);
        }
""")
            .CloseBlockCore();
    }
    private static void AppendPropertyAccessorSpecificTryGet(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.OpenRegionBlock("Specific TryGet");

        appendTryGet(in ctx, ctx.Model.MappedProperties);
        appendTryGet(in ctx, ctx.Model.UnmappedProperties);

        ctx.SourceBuilder.CloseBlockCore();

        static void appendTryGet(in SourceBuildingContext ctx, IList<PropertyModel> properties)
        {
            ctx.ThrowIfCancellationRequested();

            foreach(var property in properties)
            {
                ctx.ThrowIfCancellationRequested();

                var hasSetter = property.HasSetter;
                var hasMappings = property.Mappings.Count > 0;

                ctx.SourceBuilder.Comment
                    .OpenSummary()
                    .Append("Attempts to retrieve the value for ").Comment.SeeCRef(property.Name).Append(" from <see cref=\"Data\"/>.")
                    .CloseBlockCore();

                if(!hasSetter && !hasMappings)
                {
                    ctx.SourceBuilder.Comment
                        .OpenRemarks()
                        .Append("Because setter or no constructor parameter mapping could be determined for ").Comment
                        .SeeCRef(property.Name)
                        .Append(", this method will always return <see langword=\"false\"/>. It is generated nonetheless to allow for new parameter mappings and setters to be non-breaking.")
                        .CloseBlockCore();
                }

                _ = ctx.SourceBuilder.Comment
                    .OpenParam("value")
                    .Append("The value obtained for ").Comment.SeeCRef(property.Name).Append(", if one could be found in <see cref=\"Data\"/>; otherwise, <see langword=\"null\"/>.")
                    .CloseBlock().Comment
                    .OpenReturns()
                    .Append("<see langword=\"true\"/> if a value for ").Comment.SeeCRef(property.Name).Append(" could be found in <see cref=\"Data\"/>; otherwise, <see langword=\"false\"/>.")
                    .CloseBlock()
                    .Append("public bool TryGet").Append(property.Name)
                    .Append("([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ")
                    .Append(property.Type.NullableDisplayString)
                    .Append(" value)")
                    .OpenBracesBlock();

                if(!hasSetter && !hasMappings)
                {
                    ctx.SourceBuilder
                        .AppendLine("value = null;")
                        .Append("return false;").AppendLineCore();
                } else
                {

                    ctx.SourceBuilder
                        .Append("if(!isTypeMatch)")
                        .OpenBracesBlock()
                        .AppendLine("value = null;")
                        .AppendLine("return false;")
                        .CloseBlockCore();

                    if(hasSetter)
                    {
                        ctx.SourceBuilder
                            .Append("foreach(var kvp in data.NamedArguments)")
                            .OpenBracesBlock()
                                .Append("if(kvp.Key == nameof(").Append(property.Name)
                                .Append(") && kvp.Value.TryGet").Append(property.Type.KindString)
                                .AppendLine("Value(out value))")
                                .Indent()
                                    .Append("return true;")
                                .Detent()
                            .CloseBlockCore();
                    }

                    if(hasMappings)
                    {
                        ctx.SourceBuilder.Append("return _constructor.TryGet").Append(property.Name).Append("(out value);").AppendLineCore();
                    } else
                    {
                        ctx.SourceBuilder
                            .AppendLine("value = null;")
                            .Append("return false;").AppendLineCore();
                    }
                }

                ctx.SourceBuilder.CloseBlockCore();
            }
        }
    }
    private static void AppendPropertyAccessorGet(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.OpenRegionBlock("Get");

        append(in ctx, ctx.Model.MappedProperties);
        append(in ctx, ctx.Model.UnmappedProperties);

        ctx.SourceBuilder.CloseBlockCore();

        static void append(in SourceBuildingContext ctx, IList<PropertyModel> properties)
        {
            ctx.ThrowIfCancellationRequested();

            foreach(var property in properties)
            {
                ctx.ThrowIfCancellationRequested();

                var isMapped = property.Mappings.Count > 0;
                var hasSetter = property.HasSetter;

                if(!hasSetter && !isMapped)
                {
                    // stub
                } else if(!hasSetter)
                {
                    // ctor only
                } else
                {
                    // pro args then ctor
                }
            }
        }
    }
    #endregion
    #region Model
    private static void AppendModelType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        using var _ = ctx.SourceBuilder.OpenRegionBlockScope("Model");
        OpenModel(in ctx);

        AppendFactoryStateTypes(in ctx);

        using(ctx.SourceBuilder.OpenRegionBlockScope("Factories"))
        {
            AppendModelFactories(in ctx);
            AppendModelApplyConstructorArguments(in ctx);
            AppendModelApplyNamedArguments(in ctx);
        }

        AppendModelProperties(in ctx);

        if(!ctx.Model.IsEquatable)
            AppendEquality(in ctx, $"{ctx.DisplayString}.{ctx.Model.AttributeModel.ModelTypeName}");

        ctx.SourceBuilder.CloseBlockCore();
    }
    private static void OpenModel(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Provides strongly typed access to properties of ").Comment
            .SeeCRef(ctx.DisplayString).Append(" as represented by an <see cref=\"global::Microsoft.CodeAnalysis.AttributeData\"/> instance.")
            .CloseBlock()
            .AppendCore("public ");

        var (modifiers, accessibility) = ctx.Model.AttributeModel.GenerateModelTypeAsStruct
            ? (ctx.Model.IsEquatable ? "partial record struct " : "partial struct ", "public ")
            : (ctx.Model.IsEquatable ? "sealed partial record " : "sealed partial class ", "private ");

        ctx.SourceBuilder
            .Append(modifiers)
            .AppendCore(ctx.Model.AttributeModel.ModelTypeName);

        // If not equatable, we implement NonEquatable, it provides the impl for IEquatable<T>.
        if(!ctx.Model.IsEquatable)
            ctx.SourceBuilder.Append(" : IEquatable<").Append(ctx.Model.AttributeModel.ModelTypeName).AppendCore('>');

        ctx.SourceBuilder.OpenBracesBlock()
            .Append(accessibility).Append(ctx.Model.AttributeModel.ModelTypeName).Append("()")
            .OpenBracesBlock()
            .CloseBlockCore();
    }
    private static void AppendModelFactories(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(ctx.Model.InitializationMethods.Count == 0)
        {
            appendFactory(in ctx, initializationMethod: null);
        } else
        {
            foreach(var hook in ctx.Model.InitializationMethods)
            {
                appendFactory(in ctx, hook);
            }
        }

        static void appendFactory(in SourceBuildingContext ctx, InitializationMethodModel? initializationMethod)
        {
            ctx.ThrowIfCancellationRequested();

            ctx.SourceBuilder.Comment
                .OpenSummary()
                .Append("Creates a new model of a ").Comment.SeeCRef(ctx.DisplayString).Append(" represented by <paramref name=\"data\"/>. ")
                .CloseBlock().Comment
                .OpenRemarks()
                .Append("No validation is performed to ensure all properties are determinable from <paramref name=\"data\"/>. Therefore, if <paramref name=\"data\"/> is not correctly representing a ").Comment
                .SeeCRef(ctx.DisplayString).Append(", properties may unexpectedly be <see langword=\"null\"/>.")
                .CloseBlock().Comment
                .OpenParam("data")
                .Append("The ").Comment.SeeCRef(AttributeDataDisplayString).Append(" representing an instance of ").Comment
                .SeeCRef(ctx.DisplayString)
                .CloseBlockCore();

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Comment
                    .OpenParam("state")
                    .Append("The state object to pass to the initialization function of the model created.")
                    .CloseBlockCore();
            }

            ctx.SourceBuilder.Comment.OpenParam("cancellationToken")
                .Append("The cancellation token used to request model creation to be cancelled.")
                .CloseBlock().Comment
                .OpenReturns()
                .Append("A new model of ").Comment.SeeCRef(ctx.DisplayString).Append('.')
                .CloseBlock()
                .Append("public static ").Append(ctx.Model.AttributeModel.ModelTypeName).Append(" Create(").Append(AttributeDataDisplayString)
                .AppendCore(" data");

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Append(", in ").Append(initializationMethod.StateTypeDisplayString).AppendCore(" state");
            }

            ctx.SourceBuilder.Append(", global::System.Threading.CancellationToken cancellationToken = default)")
                .OpenBracesBlock()
                .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                .Append("var result = new ").Append(ctx.Model.AttributeModel.ModelTypeName).AppendLine("();")
                .AppendLine("result.ApplyConstructorArguments(data, cancellationToken);")
                .Append("result.ApplyNamedArguments(data, cancellationToken);").AppendLineCore();

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder
                    .Append("result.").Append(initializationMethod.Name).AppendCore('(');

                var i = 0;
                for(; i < initializationMethod.Parameters.Count; i++)
                {
                    ctx.ThrowIfCancellationRequested();

                    if(i != 0)
                        ctx.SourceBuilder.AppendCore(", ");

                    var parameter = initializationMethod.Parameters[i];
                    ctx.SourceBuilder.Append(parameter.Name).Append(": state.").AppendCore(parameter.PropertyName);
                }

                if(initializationMethod.CancellationTokenParameterName is { } name)
                {
                    if(i != 0)
                        ctx.SourceBuilder.AppendCore(", ");

                    ctx.SourceBuilder.Append(name).AppendCore(": cancellationToken");
                }

                ctx.SourceBuilder.Append(");").AppendLineCore();
            }

            ctx.SourceBuilder
                .AppendLine("return result;")
                .CloseBlockCore();
        }
    }
    private static void AppendModelApplyConstructorArguments(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder.Append("private void ApplyConstructorArguments(").Append(AttributeDataDisplayString).Append(" data, global::System.Threading.CancellationToken cancellationToken = default)")
            .OpenBracesBlock()
            .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
            .Append("var ctor = new ").Append(ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName).Append("(data, true);").AppendLineCore();

        for(var i = 0; i < ctx.Model.MappedProperties.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var property = ctx.Model.MappedProperties[i];

            var index = i.ToString();
            ctx.SourceBuilder
                .Append("if(ctor.TryGet").Append(property.Name).Append("(out var value").Append(index).AppendLine("))")
                .Indent().Append(property.Name).Append(" = value").Append(index).DetentCore();

            var kind = property.Type.Kind;

            if(kind.HasFlagsFast(AttributeParameterTypeKind.ValueType, AttributeParameterTypeKind.Array))
                ctx.SourceBuilder.AppendCore(".Value");

            ctx.SourceBuilder.Append(';').AppendLineCore();
        }

        ctx.SourceBuilder.CloseBlockCore();
    }
    private static void AppendModelApplyNamedArguments(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.Append("private void ApplyNamedArguments(").Append(AttributeDataDisplayString).Append(" data, global::System.Threading.CancellationToken cancellationToken = default)")
            .OpenBracesBlock()
            .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
            .Append("foreach(var kvp in data.NamedArguments)")
            .OpenBracesBlock()
            .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
            .Append("switch(kvp.Key)")
            .OpenBracesBlock();

        append(in ctx, ctx.Model.MappedProperties);
        append(in ctx, ctx.Model.UnmappedProperties);

        ctx.SourceBuilder
            .CloseBlock()
            .CloseBlock()
            .CloseBlockCore();

        static void append(in SourceBuildingContext ctx, IList<PropertyModel> properties)
        {
            ctx.ThrowIfCancellationRequested();

            foreach(var property in properties)
            {
                ctx.ThrowIfCancellationRequested();

                if(!property.HasSetter)
                    continue;

                ctx.SourceBuilder
                    .Append("case nameof(").Append(ctx.DisplayString).Append('.').Append(property.Name).Append("):")
                    .OpenBracesBlock()
                    .Append("if(kvp.Value.TryGet").Append(property.Type.KindString).Append("Value(out ").Append(property.Type.NullableDisplayString).AppendLine(" value))")
                    .Indent().Append(property.Name).AppendCore(" = value");

                if(property.Type.Kind.HasFlagsFast(AttributeParameterTypeKind.ValueType))
                    ctx.SourceBuilder.AppendCore(".Value");

                ctx.SourceBuilder.AppendLine(';').Detent()
                    .AppendLine("break;")
                    .CloseBlockCore();
            }
        }
    }
    private static void AppendModelProperties(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        using(ctx.SourceBuilder.OpenRegionBlockScope("Properties"))
        {
            append(in ctx, ctx.Model.MappedProperties);
            append(in ctx, ctx.Model.UnmappedProperties);
        }

        static void append(in SourceBuildingContext ctx, IList<PropertyModel> properties)
        {
            ctx.ThrowIfCancellationRequested();

            foreach(var property in properties)
            {
                ctx.ThrowIfCancellationRequested();

                if(property is { HasSetter: false, Mappings: [] })
                    continue;
                
                ctx.SourceBuilder.Comment
                    .OpenSummary()
                    .Append("Gets the value for the ").Comment.SeeCRef($"{ctx.DisplayString}.{property.Name}").Append(" property.")
                    .CloseBlock()
                    .AppendCore("public ");

                //if(ctx.Model.AttributeModel.GenerateModelTypeAsStruct)
                //    ctx.SourceBuilder.AppendCore("readonly ");

                ctx.SourceBuilder.Append(property.Type.DisplayString).Append(' ').Append(property.Name).Append("{ get; private set; } = ")
                    .Append(property.DefaultValueExpression ?? "default!").Append(';').AppendLineCore();
            }
        }
    }
    private static void AppendFactoryStateTypes(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.OpenRegionBlock("State Types");

        foreach(var initMethod in ctx.Model.InitializationMethods.Where(m => m.Parameters.Count > 0))
        {
            ctx.ThrowIfCancellationRequested();

            _ = ctx.SourceBuilder
                .Append("public readonly record struct ").Append(initMethod.StateTypeName)
                .OpenParensBlock();

            for(var i = 0; i < initMethod.Parameters.Count; i++)
            {
                ctx.ThrowIfCancellationRequested();

                if(i != 0)
                    ctx.SourceBuilder.AppendCore(", ");

                var parameter = initMethod.Parameters[i];
                ctx.SourceBuilder.Append(parameter.Type).Append(' ').AppendCore(parameter.PropertyName);
            }

            ctx.SourceBuilder.CloseBlock().Append(';').AppendLineCore();
        }

        ctx.SourceBuilder.CloseBlockCore();
    }
    #endregion
    #region Extensions
    private static void AppendExtensionsType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Provides extension methods for working with ").Comment.SeeCRef(ctx.DisplayString).AppendLine('.')
            .CloseBlock()
            .Append("internal static partial class ")
            .Append(ctx.Model.AttributeModel.ExtensionsTypeName)
            .OpenBracesBlock();

        AppendTypeCheckExtension(in ctx);
        AppendConstructorAccessorExtension(in ctx);
        AppendPropertyAccessorExtension(in ctx);
        AppendGetModelExtension(in ctx);
        AppendOfModelExtensions(in ctx);

        ctx.SourceBuilder.CloseAllBlocksCore();
    }
    private static void AppendTypeCheckExtension(in SourceBuildingContext ctx)
    {
        // TODO: generic and nested attributes
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Determines whether an instance of ").Comment.SeeCRef(AttributeDataDisplayString).Append(" represents an attribute of type ").Comment.SeeCRef(ctx.DisplayString).Append('.')
            .CloseBlock()
            .Comment
            .OpenParam("data")
            .Append("The ").Comment.SeeCRef(AttributeDataDisplayString).Append(" to check.")
            .CloseBlock()
            .Comment
            .OpenReturns()
            .Comment.Langword("true").Append(" if ").Comment.ParamRef("data").Append(" is representing an attribute of type").Comment.SeeCRef(ctx.DisplayString).Append("; otherwise, ").Comment.Langword("false").Append('.')
            .CloseBlock()
            .AppendLine(AggressiveInliningAttributeSyntax)
            .Append("public static bool Is").Append(ctx.Model.Signature.Name).Append("(this ").Append(AttributeDataDisplayString).Append(" data)")
            .OpenBracesBlock()
            .AppendLine("var result = data is")
            .OpenBracesBlock()
            .AppendLine("// checks if class is available")
            .Append("AttributeClass:")
            .OpenBracesBlock()
            .AppendLine("// checks type name match")
            .Append("Name: \"").Append(ctx.Model.Signature.Name).AppendLine("\",")
            .Append("// recursively check containing namespace, avoid TDS").AppendLineCore();

        for(var i = ctx.Model.Signature.NamespaceParts.Count; i > 0; i--)
        {
            ctx.ThrowIfCancellationRequested();

            ctx.SourceBuilder
                .Append("ContainingNamespace:")
                .OpenBracesBlock()
                .Append("Name: \"").Append(ctx.Model.Signature.NamespaceParts[i - 1]).Append("\",").AppendLineCore();
        }

        ctx.SourceBuilder
            .Append("ContainingNamespace:")
            .OpenBracesBlock()
            .Append("IsGlobalNamespace: true")
            .CloseBlockCore();

        for(var i = 0; i < ctx.Model.Signature.NamespaceParts.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            ctx.SourceBuilder.CloseBlockCore();
        }

        ctx.SourceBuilder.CloseBlock()
            .CloseBlock()
            .AppendLine(';')
            .AppendLine("return result;")
            .CloseBlockCore();
    }
    private static void AppendConstructorAccessorExtension(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Gets an object providing strongly typed access to individual ").Comment
            .SeeCRef(ctx.DisplayString)
            .Append(" constructor arguments represented in an ").Comment
            .SeeCRef(AttributeDataDisplayString)
            .Append(" instance.")
            .CloseBlock().Comment
            .OpenParam("data")
            .Append("The data to wrap.")
            .CloseBlock().Comment
            .OpenParam("checkType")
            .Append("A value indicating whether to check that <paramref name=\"data\"/> is representing an instance of ").Comment
            .SeeCRef(ctx.DisplayString)
            .Append('.')
            .CloseBlock().Comment
            .OpenReturns()
            .Append("A new instance of ").Comment
            .SeeCRef(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName)
            .Append(" wrapping <paramref name=\"data\"/>.")
            .CloseBlock()
            .AppendLine(AggressiveInliningAttributeSyntax)
            .Append("public static ")
            .Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ConstructorArgumentAccessorTypeName)
            .Append(" Get").Append(ctx.Model.Signature.Name).Append("ConstructorArgumentAccessor(this ")
            .Append(AttributeDataDisplayString).AppendLine(" data, bool checkType = true) =>")
            .Append("new(data, isTypeMatch: !checkType || data.Is").Append(ctx.Model.Signature.Name).Append("());").AppendLineCore();
    }
    private static void AppendPropertyAccessorExtension(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Gets an object providing strongly typed access to individual ").Comment
            .SeeCRef(ctx.DisplayString)
            .Append(" properties represented in an ").Comment
            .SeeCRef(AttributeDataDisplayString)
            .Append(" instance.")
            .CloseBlock().Comment
            .OpenParam("data")
            .Append("The data to wrap.")
            .CloseBlock().Comment
            .OpenParam("checkType")
            .Append("A value indicating whether to check that <paramref name=\"data\"/> is representing an instance of ").Comment
            .SeeCRef(ctx.DisplayString)
            .Append('.')
            .CloseBlock().Comment
            .OpenReturns()
            .Append("A new instance of ").Comment
            .SeeCRef(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.PropertyAccessorTypeName)
            .Append(" wrapping <paramref name=\"data\"/>.")
            .CloseBlock()
            .AppendLine(AggressiveInliningAttributeSyntax)
            .Append("public static ")
            .Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.PropertyAccessorTypeName)
            .Append(" Get").Append(ctx.Model.Signature.Name).Append("PropertyAccessor(this ")
            .Append(AttributeDataDisplayString).AppendLine(" data, bool checkType = true) =>")
            .Append("new(data, isTypeMatch: !checkType || data.Is").Append(ctx.Model.Signature.Name).Append("());").AppendLineCore();
    }
    private static void AppendGetModelExtension(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(ctx.Model.InitializationMethods.Count == 0)
        {
            appendExtension(in ctx, initializationMethod: null);
        } else
        {
            foreach(var hook in ctx.Model.InitializationMethods)
            {
                appendExtension(in ctx, hook);
            }
        }

        static void appendExtension(in SourceBuildingContext ctx, InitializationMethodModel? initializationMethod)
        {
            ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Attempts to get an object providing strongly typed access to ").Comment
            .SeeCRef(ctx.DisplayString)
            .Append(" properties represented by an ").Comment.SeeCRef(AttributeDataDisplayString)
            .Append(" instance.")
            .CloseBlock().Comment
            .OpenParam("data")
            .Append("The data to create a model with.")
            .CloseBlock().Comment
            .OpenParam("checkType")
            .Append("A value indicating whether to check that <paramref name=\"data\"/> is representing an instance of ").Comment
            .SeeCRef(ctx.DisplayString).Append('.')
            .CloseBlock().Comment
            .OpenParam("model")
            .Append("The model created, if one could be created; otherwise, <see langword=\"null\"/>.")
            .CloseBlockCore();

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Comment
                    .OpenParam("state")
                    .Append("The state object to pass to the initialization function of the model created.")
                    .CloseBlockCore();
            }

            ctx.SourceBuilder.Comment.OpenParam("cancellationToken")
            .Append("The cancellation token used to request model creation to be cancelled.")
            .CloseBlock().Comment
            .OpenReturns()
            .Append("<see langword=\"true\"/> if a model could be created; otherwise, <see langword=\"false\"/>.")
            .CloseBlock()
            .AppendLine(AggressiveInliningAttributeSyntax)
            .Append("public static bool TryGet").Append(ctx.Model.Signature.Name).Append("Model(this ").Append(AttributeDataDisplayString)
            .AppendCore(" data");

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Append(", in ").Append(initializationMethod.StateTypeDisplayString).AppendCore(" state");
            }

            ctx.SourceBuilder.Append(", [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ")
            .Append(ctx.DisplayString).Append('.').AppendCore(ctx.Model.AttributeModel.ModelTypeName);

            if(!ctx.Model.AttributeModel.GenerateModelTypeAsStruct)
                ctx.SourceBuilder.AppendCore('?');

            ctx.SourceBuilder
                .Append(" model, bool checkType = true, global::System.Threading.CancellationToken cancellationToken = default)")
                .OpenBracesBlock()
                .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                .Append("if(!checkType || data.Is").Append(ctx.Model.Signature.Name).Append("())")
                .OpenBracesBlock()
                .Append("model = ").Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ModelTypeName).AppendCore(".Create(data");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.AppendCore(", in state");

            ctx.SourceBuilder
                .Append(", cancellationToken);")
                .AppendLine("return true;")
                .CloseBlock()
                .AppendLine("model = default;")
                .AppendLine("return false;")
                .CloseBlockCore();

            ctx.SourceBuilder.Comment
                .OpenSummary()
                .Append("Gets an object providing strongly typed access to ").Comment
                .SeeCRef(ctx.DisplayString)
                .Append(" properties represented by an ").Comment.SeeCRef(AttributeDataDisplayString)
                .Append(" instance. <paramref name=\"data\"/> will not be verified to actually represent an instance of ")
                .Comment.SeeCRef(ctx.DisplayString).Append(", so properties might unexpectedly be <see langword=\"null\"/>.")
                .CloseBlock().Comment
                .OpenParam("data")
                .Append("The data to create a model with.")
                .CloseBlock().Comment
                .OpenParam("model")
                .CloseBlockCore();

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Comment
                    .OpenParam("state")
                    .Append("The state object to pass to the initialization function of the model created.")
                    .CloseBlockCore();
            }

            ctx.SourceBuilder.Comment
                .OpenParam("cancellationToken")
                .Append("The cancellation token used to request model creation to be cancelled.")
                .CloseBlock().Comment
                .OpenReturns()
                .Append("A new instance of ").Comment
                .SeeCRef($"{ctx.DisplayString}.{ctx.Model.AttributeModel.ModelTypeName}")
                .CloseBlock()
                .AppendLine(AggressiveInliningAttributeSyntax)
                .Append("public static ").Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ModelTypeName)
                .Append(" Get").Append(ctx.Model.Signature.Name).Append("Model(this ").Append(AttributeDataDisplayString)
                .AppendCore(" data");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.Append(", in ").Append(initializationMethod.StateTypeDisplayString).AppendCore(" state");

            ctx.SourceBuilder.Append(", global::System.Threading.CancellationToken cancellationToken = default)")
                .OpenBracesBlock()
                .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                .Append("return ").Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ModelTypeName).AppendCore(".Create(data");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.AppendCore(", in state");

            ctx.SourceBuilder
                .Append(", cancellationToken);")
                .CloseBlockCore();
        }
    }
    private static void AppendOfModelExtensions(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(ctx.Model.InitializationMethods.Count == 0)
        {
            appendExtension(in ctx, initializationMethod: null);
        } else
        {
            foreach(var hook in ctx.Model.InitializationMethods)
            {
                appendExtension(in ctx, hook);
            }
        }

        static void appendExtension(in SourceBuildingContext ctx, InitializationMethodModel? initializationMethod)
        {
            ctx.SourceBuilder.Comment
                .OpenSummary()
                .Append("Filters an array of <see cref=\"global::Microsoft.CodeAnalysis.AttributeData\"/> for instances representing a ").Comment
                .SeeCRef(ctx.DisplayString).Append(" instance.")
                .CloseBlock().Comment
                .OpenParam("data")
                .Append("The array to filter.")
                .CloseBlockCore();

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Comment
                    .OpenParam("state")
                    .Append("The state object to pass to the initialization function of the model created.")
                    .CloseBlockCore();
            }

            ctx.SourceBuilder.Comment.OpenParam("cancellationToken")
                .Append("The cancellation token used to request model creation to be cancelled.")
                .CloseBlock().Comment
                .OpenReturns()
                .Append("An <see cref=\"global::System.Collections.Generic.IEnumerable{T}\"/> that contains elements of the input sequence parsed as ").Comment
                .SeeCRef(ctx.DisplayString).Append(" instances.")
                .CloseBlock()
                .Append("public static global::System.Collections.Generic.IEnumerable<")
                .Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ModelTypeName).Append("> Of").Append(ctx.Model.Signature.Name)
                .AppendCore("(this global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.AttributeData> data");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.Append(", ").Append(initializationMethod.StateTypeDisplayString).AppendCore(" state");

            ctx.SourceBuilder.Append(", global::System.Threading.CancellationToken cancellationToken = default)")
                .OpenBracesBlock()
                .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                .AppendLine("foreach(var datum in data)")
                .OpenBracesBlock()
                    .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                    .Append("if(datum.Is").Append(ctx.Model.Signature.Name).AppendLine("())")
                        .Indent().Append("yield return Get").Append(ctx.Model.Signature.Name).AppendCore("Model(datum");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.AppendCore(", in state");

            ctx.SourceBuilder.Append(", cancellationToken);").Detent()
                .CloseBlock()
                .CloseBlock().Comment
                .OpenSummary()
                .Append("Filters an enumeration of <see cref=\"global::Microsoft.CodeAnalysis.AttributeData\"/> for instances representing a ").Comment
                .SeeCRef(ctx.DisplayString).Append(" instance.")
                .CloseBlock().Comment
                .OpenParam("data")
                .Append("The enumeration to filter.")
                .CloseBlockCore();

            if(initializationMethod is { Parameters.Count: > 0 })
            {
                ctx.SourceBuilder.Comment
                    .OpenParam("state")
                    .Append("The state object to pass to the initialization function of the model created.")
                    .CloseBlockCore();
            }

            ctx.SourceBuilder.Comment.OpenParam("cancellationToken")
                .Append("The cancellation token used to request model creation to be cancelled.")
                .CloseBlock().Comment
                .OpenReturns()
                .Append("An <see cref=\"global::System.Collections.Generic.IEnumerable{T}\"/> that contains elements of the input sequence parsed as ").Comment
                .SeeCRef(ctx.DisplayString).Append(" instances.")
                .CloseBlock()
                .Append("public static global::System.Collections.Generic.IEnumerable<")
                .Append(ctx.DisplayString).Append('.').Append(ctx.Model.AttributeModel.ModelTypeName).Append("> Of").Append(ctx.Model.Signature.Name)
                .AppendCore("(this global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.AttributeData> data");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.Append(", ").Append(initializationMethod.StateTypeDisplayString).AppendCore(" state");

            ctx.SourceBuilder.Append(", global::System.Threading.CancellationToken cancellationToken = default)")
                .OpenBracesBlock()
                .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                .AppendLine("foreach(var datum in data)")
                .OpenBracesBlock()
                    .AppendLine("cancellationToken.ThrowIfCancellationRequested();")
                    .Append("if(datum.Is").Append(ctx.Model.Signature.Name).AppendLine("())")
                        .Indent().Append("yield return Get").Append(ctx.Model.Signature.Name).AppendCore("Model(datum");

            if(initializationMethod is { Parameters.Count: > 0 })
                ctx.SourceBuilder.AppendCore(", in state");

            ctx.SourceBuilder.Append(", cancellationToken);").Detent()
                .CloseBlock()
                .CloseBlockCore();
        }
    }
    #endregion
    #region Shared
    private static void AppendEquality(in SourceBuildingContext ctx, String? displayString = null)
    {
        ctx.ThrowIfCancellationRequested();

        var nonNullDisplayString = displayString ?? ctx.DisplayString;

        ctx.SourceBuilder
            .OpenRegionBlock("Equality").Comment
            .InheritDoc()
            .Append("public override bool Equals(object? obj) => throw new NotSupportedException($\"")
            .Append(nonNullDisplayString)
            .AppendLine(".Equals() is not supported.\");").Comment
            .InheritDoc()
            .Append("public bool Equals(").Append(nonNullDisplayString).Append(" other) => throw new NotSupportedException($\"")
            .Append(nonNullDisplayString)
            .Append(".Equals(").Append(nonNullDisplayString).AppendLine(") is not supported.\");").Comment
            .InheritDoc()
            .Append("public override int GetHashCode() => throw new NotSupportedException($\"")
            .Append(nonNullDisplayString)
            .AppendLine(".GetHashCode() is not supported.\");")
            .CloseBlockCore();
    }
    #endregion
}

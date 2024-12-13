namespace RhoMicro.CodeAnalysis;

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text;
using RhoMicro.CodeAnalysis.Library.Extensions;
using System.Runtime.InteropServices.ComTypes;

using static Constants;

/// <summary>
/// Generates factories for parsing attributes from <see cref="AttributeData"/> instances.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AttributeFactoryGenerator : IIncrementalGenerator
{
    private static readonly EquatableCollectionFactory _collectionFactory = EquatableCollectionFactory.Default;

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            GenerateFactoryAttributeMetadataName,
            (_, _) => true,
            (ctx, ct) =>
            {
                if(ctx is not { Attributes: [{ } attribute], TargetSymbol: INamedTypeSymbol target })
                    return null;

                /*
                TODO:
                generic attributes

                procedure:
                - map properties onto indices, ordered by name, begin at 0
                - map ctors onto indices, ordered by params count, then by params names, begin at 1, prepended by default at 0
                - examine every ctor for param-prop mapping
                */
                var model = AttributeFactoryModel.Create(target, attribute, new(_collectionFactory, ct));

                return model;
            }).Where(m => m is not null)
            .Select((m, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return m!;
            })
            .Select((m, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var sourceBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
                {
                    AmbientCancellationToken = ct,
                    GeneratorName = typeof(AttributeFactoryGenerator).FullName
                });

                // append attribute members
                m.Signature.BuildStrings(sourceBuilder, out var hintName, out var displayString, ct);
                var buildContext = new SourceBuildingContext(sourceBuilder, m, displayString, ct);
                AppendPropertyIdType(in buildContext);
                AppendConstructorAccessorType(in buildContext);
                AppendPropertyAccessorType(in buildContext);
                sourceBuilder.CloseAllBlocksCore();

                // append extensions
                OpenExtensionsType(in buildContext);
                AppendTypeCheckExtension(in buildContext);
                sourceBuilder.CloseAllBlocksCore();

                var source = sourceBuilder.ToString();

                return (hintName, source);
            });

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }

    #region PropertyId
    private static void AppendPropertyIdType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Provides strongly typed access to property ids of ").Comment.SeeCRef(ctx.DisplayString).Append(" properties.")
            .CloseBlock()
            .Append("public enum ").Append(ctx.Model.PropertyIdTypeName)
            .OpenBracesBlock()
            //ordered by name and then by whether or not a ctor parameter mapping exists for the property
            //mapped first is crucial as we depend on the backing values being valid indices into ctor arg maps
            .OpenRegionBlock("Mapped Properties");

        for(var i = 0; i < ctx.Model.MappedProperties.Count; i++)
        {
            var mappedProperty = ctx.Model.MappedProperties[i];
            ctx.ThrowIfCancellationRequested();

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
            var unmappedProperty = ctx.Model.UnmappedProperties[i];
            ctx.ThrowIfCancellationRequested();

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

        ctx.SourceBuilder.CloseBlock()
            .CloseBlockCore();
    }
    #endregion
    #region ConstructorAccessor
    private static void AppendConstructorAccessorType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();
        OpenConstructorAccessorType(in ctx);
        AppendConstructorAccessorFieldsAndProperties(in ctx);
        //TODO: General TryGet Region
        //TODO: Specific TryGet Region
        AppendEqualityRegion(in ctx, ctx.Model.ConstructorArgumentAccessorTypeName);
        ctx.SourceBuilder.CloseBlockCore();
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
            .Append(ctx.Model.ConstructorArgumentAccessorTypeName).Append('(')
            .Append(AttributeDataDisplayString).Append(" data, ")
            .Append("bool isTypeMatch")
            .Append(") : IEquatable<").Append(ctx.Model.ConstructorArgumentAccessorTypeName).Append('>')
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
            .Append("private const ").Append(ctx.Model.PropertyIdTypeName).AppendCore(" _maxMappedPropId = ");

        if(ctx.Model.MappedProperties is [..,{ } lastMappedProp])
            ctx.SourceBuilder.Append(ctx.Model.PropertyIdTypeName).Append('.').AppendCore(lastMappedProp.Name);
        else
            ctx.SourceBuilder.Append('(').Append(ctx.Model.PropertyIdTypeName).Append(')').AppendCore("(-1)");

            
        ctx.SourceBuilder.AppendLine(';').Comment
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
            .Append("private static readonly global::System.Collections.Immutable.ImmutableArray<global::System.Collections.Immutable.ImmutableArray<int>> _argIndices = [];")
            .CloseBlockCore();
        /*
        private static readonly ImmutableArray<ImmutableArray<Int32>> _argIndices =
            [
                //0: fallback/default
                [
                    //0: __GetSetInt32Property__ -> ?
                    -1,
                    //1: __GetSetStringProperty__ -> ?
                    -1,
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //1: __TestAttribute__Attribute(Int32)
                [
                    //0: __GetSetInt32Property__ -> args[0]
                    0,
                    //1: __GetSetStringProperty__ -> ?
                    -1,                    
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //2: __TestAttribute__Attribute(String)
                [
                    //0: __GetSetInt32Property__ -> ?
                    -1,
                    //1: __GetSetStringProperty__ -> args[0]
                    0,
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //3: __TestAttribute__Attribute(Type[])
                [
                    //0: __GetSetInt32Property__ -> ?
                    -1,
                    //1: __GetSetStringProperty__ -> ?
                    -1,                    
                    //2: __GetSetTypeArrayProperty__ -> args[0]
                    0
                ],
                //4: __TestAttribute__Attribute(Int32, String)
                [
                    //0: __GetSetInt32Property__ -> args[0]
                    0,
                    //1: __GetSetStringProperty__ -> args[1]
                    1,                    
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //5: __TestAttribute__Attribute(String, Int32)
                [
                    //0: __GetSetInt32Property__ -> args[1]
                    1,
                    //1: __GetSetStringProperty__ -> args[0]
                    0,                    
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ]
            ];
        /// <summary>
        /// The mapping of propIndex->argIndex for the ctorIndex of the ctor used in  <see cref="Data"/>.
        /// </summary>
        /// <remarks>
        /// Properties are mapped up to <see cref="_maxMappedPropId"/>.
        /// </remarks>
        private readonly ImmutableArray<Int32> _specificArgIndices =
            data.AttributeConstructor?.Parameters switch
            {
            [{ Type.SpecialType: SpecialType.System_String }] => _argIndices[1],
            [{ Type.SpecialType: SpecialType.System_Int32 }] => _argIndices[2],
            [{ Type: IArrayTypeSymbol { ElementType: { Name: "Type", ContainingType: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } } }] => _argIndices[3],
            [{ Type.SpecialType: SpecialType.System_Int32 }, { Type.SpecialType: SpecialType.System_String }] => _argIndices[3],
            [{ Type.SpecialType: SpecialType.System_String }, { Type.SpecialType: SpecialType.System_Int32 }] => _argIndices[4],
                _ => _argIndices[0],
            };*/
    }
    #endregion
    #region PropertyAccessor
    private static void AppendPropertyAccessorType(in SourceBuildingContext ctx)
    {
        //use NNW + NRT for reference prop types
        //use NRT for NRT prop types 
        //use value type for value prop types
        //use NNW + NRT + symbol for type prop types
        //use NRT + symbol for NRT prop types 
        //(use NNW + NVT for NVT prop types) => attribute disallows NVT

        //get-only props require some modeling of available constructors, map
        //ctor params to props via attribute? if such a mapping is provided,
        //named arg extraction methods should also inspect ctor args on data
        //TODO: add comment to remarks that explains this fallback mechanism
        //(fallback) => always check ctor args first, then named args; as value
        //should reflect reassign via property after ctor invocation
        //example: [Foo(prop = "bar", Prop = "foobar")] => Prop is "foobar"

        //TODO: only generate named args iteration if prop is settable
        //TODO: only generate ctor fallback if prop is mapped

        ctx.ThrowIfCancellationRequested();

        using var _ = ctx.SourceBuilder.OpenRegionBlockScope("Property Accessor");
        OpenPropertyAccessor(in ctx);
        //TODO: General TryGet Region
        //TODO: Specific TryGet Region
        //TODO: Get Region
        AppendEqualityRegion(in ctx, $"{ctx.DisplayString}.{ctx.Model.PropertyAccessorTypeName}");
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
            .Append(ctx.Model.PropertyAccessorTypeName).Append('(')
            .Append(AttributeDataDisplayString).Append(" data, ")
            .Append("bool isTypeMatch")
            .Append(") : IEquatable<").Append(ctx.Model.PropertyAccessorTypeName).Append('>')
            .OpenBracesBlock();
    }
    #endregion
    #region Extensions
    private static void OpenExtensionsType(in SourceBuildingContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        _ = ctx.SourceBuilder.Comment
            .OpenSummary()
            .Append("Provides extension methods for working with ").Comment.SeeCRef(ctx.DisplayString).AppendLine('.')
            .CloseBlock()
            .Append("internal static partial class ")
            .Append(ctx.Model.ExtensionsTypeName)
            .OpenBracesBlock();
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
    #endregion
    #region Shared
    private static void AppendEqualityRegion(in SourceBuildingContext ctx, String? displayString = null)
    {
        ctx.ThrowIfCancellationRequested();

        var nonNullDisplayString = displayString ?? ctx.DisplayString;

        ctx.SourceBuilder
            .OpenRegionBlock("Equality").Comment
            .InheritDoc()
            .Append("public override bool Equals(object? obj) => throw new NotSupportedException($\"{typeof(")
            .Append(nonNullDisplayString)
            .AppendLine(")}.Equals() is not supported.\");").Comment
            .InheritDoc()
            .Append("public bool Equals(").Append(nonNullDisplayString).Append(" other) => throw new NotSupportedException($\"{typeof(")
            .Append(nonNullDisplayString)
            .Append(")}.Equals({typeof(").Append(nonNullDisplayString).AppendLine(")}) is not supported.\");").Comment
            .InheritDoc()
            .Append("public override int GetHashCode() => throw new NotSupportedException($\"{typeof(")
            .Append(nonNullDisplayString)
            .AppendLine(")}.GetHashCode() is not supported.\");")
            .CloseBlockCore();
    }
    #endregion
}

//##############################################

#pragma warning disable IDE1006 // Naming Styles
[GenerateFactory(ExtensionsTypeName = "__TestExtensions__")]
internal sealed partial class __TestAttribute__Attribute : Attribute
{
    #region ctors
    public __TestAttribute__Attribute([MapToProperty(nameof(__GetSetStringProperty__))] String __getSetStringProperty__) => __GetSetStringProperty__ = __getSetStringProperty__;
    public __TestAttribute__Attribute([MapToProperty(nameof(__GetSetInt32Property__))] Int32 __getSetInt32Property__) => __GetSetInt32Property__ = __getSetInt32Property__;
    public __TestAttribute__Attribute([MapToProperty(nameof(__GetSetStringProperty__))] String __getSetStringProperty__, [MapToProperty(nameof(__GetSetInt32Property__))] Int32 __getSetInt32Property__)
        : this(__getSetStringProperty__) =>
        __GetSetInt32Property__ = __getSetInt32Property__;
    public __TestAttribute__Attribute([MapToProperty(nameof(__GetSetInt32Property__))] Int32 __getSetInt32Property__, [MapToProperty(nameof(__GetSetStringProperty__))] String __getSetStringProperty__)
    {
        __GetSetInt32Property__ = __getSetInt32Property__;
        __GetSetStringProperty__ = __getSetStringProperty__;
    }
    public __TestAttribute__Attribute([MapToProperty(nameof(__GetSetTypeArrayProperty__))] Type[] getSetTypeArrayProperty__) => __GetSetTypeArrayProperty__ = getSetTypeArrayProperty__;
    #endregion
    #region properties
    [DefaultValue("Hello, Default!")]
    public String __GetSetStringProperty__ { get; set; } = null!;
    [DefaultValue(null)]
    public String? __GetSetNrtStringProperty__ { get; set; }

    [DefaultValue(42)]
    public Int32 __GetSetInt32Property__ { get; set; }

    public Type __GetSetTypeProperty__ { get; set; } = null!;
    public Type? __GetSetNrtTypeProperty__ { get; set; }

    public String[] __GetSetStringArrayProperty__ { get; set; } = null!;
    public String[]? __GetSetStringNrtArrayProperty__ { get; set; }
    public String?[] __GetSetNrtStringArrayProperty__ { get; set; } = null!;
    public String?[]? __GetSetNrtStringNrtArrayProperty__ { get; set; }

    public Type[] __GetSetTypeArrayProperty__ { get; set; } = null!;

    public Type[]? __GetSetTypeNrtArrayProperty__ { get; set; }
    public Type?[] __GetSetNrtTypeArrayProperty__ { get; set; } = null!;
    public Type?[]? __GetSetNrtTypeNrtArrayProperty__ { get; set; }

    public Int32[] __GetSetInt32ArrayProperty__ { get; set; } = null!;
    public Int32[]? __GetSetInt32NrtArrayProperty__ { get; set; }
    #endregion
}

#if true

//define model class and other helper members here; omit modifiers
internal partial class __TestAttribute__Attribute
{
    /// <summary>
    /// Provides strongly typed access to all target attribute properties mapped
    /// from parameters in an <see cref="AttributeData"/> instance.
    /// </summary>
    //TODO: support optional readonly struct generation
    public sealed class __Model__ : IEquatable<__Model__?>
    {
#pragma warning disable CS8618
        private __Model__() { }
#pragma warning restore CS8618

        /// <summary>
        /// Creates a new model of <see cref="__TestAttribute__Attribute"/>,
        /// based off of <paramref name="data"/>.
        /// </summary>
        /// <remarks>
        /// No validation is done to ensure all properties are determinable from
        /// <paramref name="data"/>. Therefore, if <paramref name="data"/> is
        /// malformed, exceptions of type <see cref="NullReferenceException"/>
        /// may be thrown in this method, or upon accessing non-nullable
        /// properties on the model result.
        /// </remarks>
        /// <param name="data">
        /// The <see cref="AttributeData"/> to derive a model of <see
        /// cref="__TestAttribute__Attribute"/> from.
        /// </param>
        /// <returns></returns>
        public static __Model__ Create(AttributeData data)
        {
            var result = new __Model__();

            result.ApplyConstructorArguments(data);
            result.ApplyNamedArguments(data);

            return result;
        }
        private void ApplyConstructorArguments(AttributeData data)
        {
            var ctor = new __ConstructorArgumentAccessor__(data, isTypeMatch: true);

            // TODO: generate for all mapped properties
            if(ctor.TryGet__GetSetInt32Property__(out var value0))
                __GetSetInt32Property__ = value0.Value;
            if(ctor.TryGet__GetSetStringProperty__(out var value1))
                __GetSetStringProperty__ = value1;
            if(ctor.TryGet__GetSetTypeArrayProperty__(out var value2))
                __GetSetTypeArrayProperty__ = value2.Value;
        }
        private void ApplyNamedArguments(AttributeData data)
        {
            foreach(var kvp in data.NamedArguments)
            {
                switch(kvp.Key)
                {
                    // TODO: generate for all settable properties
                    case nameof(__GetSetStringProperty__):
                    {
                        if(kvp.Value.TryGetReferenceTypeValue<String>(out var value))
                            __GetSetStringProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetNrtStringProperty__):
                    {
                        if(kvp.Value.TryGetReferenceTypeValue<String>(out var value))
                            __GetSetNrtStringProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetInt32Property__):
                    {
                        if(kvp.Value.TryGetValueTypeValue<Int32>(out var value))
                            __GetSetInt32Property__ = value.Value;
                        break;
                    }
                    case nameof(__GetSetTypeProperty__):
                    {
                        if(kvp.Value.TryGetTypeValue(out var value))
                            __GetSetTypeProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetNrtTypeProperty__):
                    {
                        if(kvp.Value.TryGetNullableTypeValue(out var value))
                            __GetSetNrtTypeProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetStringArrayProperty__):
                    {
                        if(kvp.Value.TryGetReferenceTypeArrayValue<String>(out var value))
                            __GetSetStringArrayProperty__ = value.Value;
                        break;
                    }
                    case nameof(__GetSetStringNrtArrayProperty__):
                    {
                        if(kvp.Value.TryGetReferenceTypeNullableArrayValue<String>(out var value))
                            __GetSetStringNrtArrayProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetNrtStringArrayProperty__):
                    {
                        if(kvp.Value.TryGetNullableReferenceTypeArrayValue<String>(out var value))
                            __GetSetNrtStringArrayProperty__ = value.Value;
                        break;
                    }
                    case nameof(__GetSetNrtStringNrtArrayProperty__):
                    {
                        if(kvp.Value.TryGetNullableReferenceTypeNullableArrayValue<String>(out var value))
                            __GetSetNrtStringNrtArrayProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetTypeArrayProperty__):
                    {
                        if(kvp.Value.TryGetTypeArrayValue(out var value))
                            __GetSetTypeArrayProperty__ = value.Value;
                        break;
                    }
                    case nameof(__GetSetTypeNrtArrayProperty__):
                    {
                        if(kvp.Value.TryGetTypeNullableArrayValue(out var value))
                            __GetSetTypeNrtArrayProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetNrtTypeArrayProperty__):
                    {
                        if(kvp.Value.TryGetNullableTypeArrayValue(out var value))
                            __GetSetNrtTypeArrayProperty__ = value.Value;
                        break;
                    }
                    case nameof(__GetSetNrtTypeNrtArrayProperty__):
                    {
                        if(kvp.Value.TryGetNullableTypeNullableArrayValue(out var value))
                            __GetSetNrtTypeNrtArrayProperty__ = value;
                        break;
                    }
                    case nameof(__GetSetInt32ArrayProperty__):
                    {
                        if(kvp.Value.TryGetValueTypeArrayValue<Int32>(out var value))
                            __GetSetInt32ArrayProperty__ = value.Value;
                        break;
                    }
                    case nameof(__GetSetInt32NrtArrayProperty__):
                    {
                        if(kvp.Value.TryGetValueTypeNullableArrayValue<Int32>(out var value))
                            __GetSetInt32NrtArrayProperty__ = value;
                        break;
                    }
                }
            }
        }

        public String __GetSetStringProperty__ { get; private set; }
        public String? __GetSetNrtStringProperty__ { get; private set; }

        public Int32 __GetSetInt32Property__ { get; private set; }

        public ITypeSymbol __GetSetTypeProperty__ { get; private set; }
        public ITypeSymbol? __GetSetNrtTypeProperty__ { get; private set; }

        public ImmutableArray<String> __GetSetStringArrayProperty__ { get; private set; }
        public ImmutableArray<String>? __GetSetStringNrtArrayProperty__ { get; private set; }
        public ImmutableArray<String?> __GetSetNrtStringArrayProperty__ { get; private set; }
        public ImmutableArray<String?>? __GetSetNrtStringNrtArrayProperty__ { get; private set; }

        public ImmutableArray<ITypeSymbol> __GetSetTypeArrayProperty__ { get; private set; }

        public ImmutableArray<ITypeSymbol>? __GetSetTypeNrtArrayProperty__ { get; private set; }
        public ImmutableArray<ITypeSymbol?> __GetSetNrtTypeArrayProperty__ { get; private set; }
        public ImmutableArray<ITypeSymbol?>? __GetSetNrtTypeNrtArrayProperty__ { get; private set; }

        public ImmutableArray<Int32> __GetSetInt32ArrayProperty__ { get; private set; }
        public ImmutableArray<Int32>? __GetSetInt32NrtArrayProperty__ { get; private set; }

        #region Equality
        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) =>
            throw new NotSupportedException("__TestAttribute__Attribute.__Model__.Equals is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        /// <inheritdoc/>
        public Boolean Equals(__Model__? other) =>
            throw new NotSupportedException("__TestAttribute__Attribute.__Model__.Equals is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            throw new NotSupportedException("__TestAttribute__Attribute.__Model__.GetHashCode is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        #endregion
    }

    /// <summary>
    /// Provides strongly typed access to property ids of target attribute properties.
    /// </summary>
    public enum __PropertyId__
    {
        //ordered by name and then by whether or not a ctor parameter mapping exists for the property
        //mapped first is crucial as we depend on the backing values being valid indices into ctor arg maps
        #region Mapped Properties
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetInt32Property__"/>.
        /// This property has constructor parameter mappings.
        /// </summary>
        __GetSetInt32Property__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetStringProperty__"/>.
        /// This property has constructor parameter mappings.
        /// </summary>
        __GetSetStringProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetTypeArrayProperty__"/>.
        /// This property has constructor parameter mappings.
        /// </summary>
        __GetSetTypeArrayProperty__,
        #endregion
        #region Non-Mapped Properties
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetNrtStringProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetNrtStringProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetNrtTypeProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetNrtTypeProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetStringArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetStringArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetInt32ArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetInt32ArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetStringNrtArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetStringNrtArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetInt32NrtArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetInt32NrtArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetNrtStringArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetNrtStringArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetNrtStringNrtArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetNrtStringNrtArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetTypeNrtArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetTypeNrtArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetNrtTypeArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetNrtTypeArrayProperty__,
        /// <summary>
        /// The id representing the <see cref="__TestAttribute__Attribute.__GetSetNrtTypeNrtArrayProperty__"/>.
        /// This property does not have constructor mappings.
        /// </summary>
        __GetSetNrtTypeNrtArrayProperty__,
        #endregion
    }
    /// <summary>
    /// Provides strongly typed access to individual target attribute properties mapped
    /// from parameters in an <see cref="AttributeData"/> instance.
    /// </summary>
    /// <remarks>
    /// The accessor does not take into account default parameters. In order to
    /// guarantee correct behavior, do not utilize default parameters in target
    /// attribute constructors. Default values for properties are taken into
    /// account as long as they are implemented via constant expressions.
    /// </remarks>
    /// <param name="data">
    /// The wrapped <see cref="AttributeData"/> instance.
    /// </param>
    /// <param name="isTypeMatch">
    /// A value indicating whether <paramref name="data"/> is assumed to
    /// represent an instance of <see cref="__TestAttribute__Attribute"/>.
    /// </param>
    public readonly struct __ConstructorArgumentAccessor__(AttributeData data, Boolean isTypeMatch) : IEquatable<__ConstructorArgumentAccessor__>
    {
        #region Fields And Properties
        /// <summary>
        /// Gets the wrapped <see cref="AttributeData"/> instance.
        /// </summary>
        public AttributeData Data => data;
        private const __PropertyId__ _maxMappedPropId = __PropertyId__.__GetSetTypeArrayProperty__;
        /// <summary>
        /// The mapping of ctorIndex->propIndex->argIndex.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constructors (ordered by params count, then by params names, prepended by default):<br/>
        /// <list type="table">
        /// <item><term>0</term><description>fallback/default</description></item>
        /// <item><term>1</term><description><see cref="__TestAttribute__Attribute(Int32)"/></description></item>
        /// <item><term>2</term><description><see cref="__TestAttribute__Attribute(String)"/></description></item>
        /// <item><term>3</term><description><see cref="__TestAttribute__Attribute(Type[])"/></description></item>
        /// <item><term>4</term><description><see cref="__TestAttribute__Attribute(Int32, String)"/></description></item>
        /// <item><term>5</term><description><see cref="__TestAttribute__Attribute(String, Int32)"/></description></item>
        /// </list>
        /// </para>
        /// </remarks>
        private static readonly ImmutableArray<ImmutableArray<Int32>> _argIndices =
            [
                //0: fallback/default
                [
                    //0: __GetSetInt32Property__ -> ?
                    -1,
                    //1: __GetSetStringProperty__ -> ?
                    -1,
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //1: __TestAttribute__Attribute(Int32)
                [
                    //0: __GetSetInt32Property__ -> args[0]
                    0,
                    //1: __GetSetStringProperty__ -> ?
                    -1,                    
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //2: __TestAttribute__Attribute(String)
                [
                    //0: __GetSetInt32Property__ -> ?
                    -1,
                    //1: __GetSetStringProperty__ -> args[0]
                    0,
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //3: __TestAttribute__Attribute(Type[])
                [
                    //0: __GetSetInt32Property__ -> ?
                    -1,
                    //1: __GetSetStringProperty__ -> ?
                    -1,                    
                    //2: __GetSetTypeArrayProperty__ -> args[0]
                    0
                ],
                //4: __TestAttribute__Attribute(Int32, String)
                [
                    //0: __GetSetInt32Property__ -> args[0]
                    0,
                    //1: __GetSetStringProperty__ -> args[1]
                    1,                    
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ],
                //5: __TestAttribute__Attribute(String, Int32)
                [
                    //0: __GetSetInt32Property__ -> args[1]
                    1,
                    //1: __GetSetStringProperty__ -> args[0]
                    0,                    
                    //2: __GetSetTypeArrayProperty__ -> ?
                    -1
                ]
            ];
        /// <summary>
        /// The mapping of propIndex->argIndex for the ctorIndex of the ctor used in  <see cref="Data"/>.
        /// </summary>
        /// <remarks>
        /// Properties are mapped up to <see cref="_maxMappedPropId"/>.
        /// </remarks>
        private readonly ImmutableArray<Int32> _specificArgIndices =
            data.AttributeConstructor?.Parameters switch
            {
            [{ Type.SpecialType: SpecialType.System_String }] => _argIndices[1],
            [{ Type.SpecialType: SpecialType.System_Int32 }] => _argIndices[2],
            [{ Type: IArrayTypeSymbol { ElementType: { Name: "Type", ContainingType: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } } }] => _argIndices[3],
            [{ Type.SpecialType: SpecialType.System_Int32 }, { Type.SpecialType: SpecialType.System_String }] => _argIndices[3],
            [{ Type.SpecialType: SpecialType.System_String }, { Type.SpecialType: SpecialType.System_Int32 }] => _argIndices[4],
                _ => _argIndices[0],
            };
        #endregion
        #region Specific TryGet
        #region Mapped Properties
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringProperty__"/> mapped from constructor arguments
        /// in <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetStringProperty__"/>, if
        /// one could be found in the constructor arguments in <see
        /// cref="Data"/>; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetStringProperty__"/> could be found in the constructor
        /// arguments in <see cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGet__GetSetStringProperty__([NotNullWhen(true)] out String? value) => TryGetReferenceTypeValue(__PropertyId__.__GetSetStringProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32Property__"/> mapped from constructor arguments
        /// in <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetInt32Property__"/>, if one
        /// could be found in the constructor arguments in <see cref="Data"/>;
        /// otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetInt32Property__"/> could be found in the constructor
        /// arguments in <see cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGet__GetSetInt32Property__([NotNullWhen(true)] out Int32? value) => TryGetValueTypeValue(__PropertyId__.__GetSetInt32Property__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeArrayProperty__"/> mapped from constructor
        /// arguments in <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetTypeArrayProperty__"/>, if
        /// one could be found in the constructor arguments in <see
        /// cref="Data"/>; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetTypeArrayProperty__"/> could be found in the
        /// constructor arguments in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGet__GetSetTypeArrayProperty__([NotNullWhen(true)] out ImmutableArray<ITypeSymbol>? value) => TryGetTypeArrayValue(__PropertyId__.__GetSetTypeArrayProperty__, out value);
        #endregion
        #region Non-Mapped Properties
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringProperty__"/> mapped from constructor
        /// arguments in <see cref="Data"/>.
        /// </summary>
        /// <remarks>
        /// Because no constructor parameter mapping could be determined for
        /// <see cref="__GetSetNrtStringProperty__"/>, this method will always
        /// return <see langword="false"/>.
        /// </remarks>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtStringProperty__"/>, if
        /// one could be found in the constructor arguments in <see
        /// cref="Data"/>; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtStringProperty__"/> could be found in the
        /// constructor arguments in <see cref="Data"/>; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGet__GetSetNrtStringProperty__(out String? value)
        {
            value = null;
            return false;
        }
        // TODO: generate rest of non-mapped properties with same template.
        // generate as instance methods to enable non-breaking iteration of
        // target attributes
        #endregion
        #endregion
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
        public Boolean TryGetReferenceTypeValue<T>(__PropertyId__ id, [NotNullWhen(true)] out T? value)
            where T : class
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetReferenceTypeValue(out value);
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
        public Boolean TryGetNullableReferenceTypeValue<T>(__PropertyId__ id, out T? value)
            where T : class
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetNullableReferenceTypeValue(out value);
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
        public Boolean TryGetReferenceTypeArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetReferenceTypeArrayValue(out value);
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
        public Boolean TryGetNullableReferenceTypeArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetNullableReferenceTypeArrayValue(out value);
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
        public Boolean TryGetNullableReferenceTypeNullableArrayValue<T>(__PropertyId__ id, out ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetNullableReferenceTypeNullableArrayValue(out value);
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
        public Boolean TryGetReferenceTypeNullableArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetReferenceTypeNullableArrayValue(out value);
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
        public Boolean TryGetTypeValue(__PropertyId__ id, [NotNullWhen(true)] out ITypeSymbol? value)
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetTypeValue(out value);
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
        public Boolean TryGetNullableTypeValue(__PropertyId__ id, out ITypeSymbol? value)
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetNullableTypeValue(out value);
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
        public Boolean TryGetTypeArrayValue(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol>? value)
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetTypeArrayValue(out value);
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
        public Boolean TryGetNullableTypeArrayValue(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol?>? value)
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetNullableTypeArrayValue(out value);
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
        public Boolean TryGetNullableTypeNullableArrayValue(__PropertyId__ id, out ImmutableArray<ITypeSymbol?>? value)
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetNullableTypeNullableArrayValue(out value);
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
        public Boolean TryGetTypeNullableArrayValue(__PropertyId__ id, out ImmutableArray<ITypeSymbol>? value)
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetTypeNullableArrayValue(out value);
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
        public Boolean TryGetValueTypeValue<T>(__PropertyId__ id, [NotNullWhen(true)] out T? value)
            where T : struct
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetValueTypeValue(out value);
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
        public Boolean TryGetValueTypeArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetValueTypeArrayValue(out value);
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
        public Boolean TryGetValueTypeNullableArrayValue<T>(__PropertyId__ id, out ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch || id > _maxMappedPropId)
            {
                value = null;
                return false;
            }

            return data.ConstructorArguments[_specificArgIndices[(Int32)id]].TryGetValueTypeNullableArrayValue(out value);
        }
        #endregion
        #region Equality
        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) =>
            throw new NotSupportedException("__TestAttribute__Attribute.__ConstructorArgumentAccessor__.Equals is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        /// <inheritdoc/>
        public Boolean Equals(__ConstructorArgumentAccessor__ other) =>
            throw new NotSupportedException("__TestAttribute__Attribute.__ConstructorArgumentAccessor__.Equals is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            throw new NotSupportedException("__TestAttribute__Attribute.__ConstructorArgumentAccessor__.GetHashCode is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        #endregion
    }
    /// <summary>
    /// Provides strongly typed access to individual properties of <see
    /// cref="__TestAttribute__Attribute"/> provided by an <see
    /// cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="data">
    /// The wrapped <see cref="AttributeData"/> instance.
    /// </param>
    /// <param name="isTypeMatch">
    /// A value indicating whether or not <paramref name="data"/> is
    /// representing an instance of <see cref="__TestAttribute__Attribute"/>.
    /// </param>
    public readonly struct __PropertyAccessor__(AttributeData data, Boolean isTypeMatch) : IEquatable<__PropertyAccessor__>
    {
        /// <summary>
        /// Gets the wrapped <see cref="AttributeData"/> instance.
        /// </summary>
        public AttributeData Data => data;

        private readonly __ConstructorArgumentAccessor__ _constructor = new(data, isTypeMatch);

        #region Specific TryGet
        // TODO: generate stubs for get-only unmapped props
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetStringProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetStringProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetStringProperty__([NotNullWhen(true)] out String? value) => TryGetReferenceTypeValue(__PropertyId__.__GetSetStringProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtStringProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtStringProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetNrtStringProperty__(out String? value) => TryGetNullableReferenceTypeValue(__PropertyId__.__GetSetNrtStringProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32Property__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetInt32Property__"/>, if one
        /// could be found in the wrapped <see cref="AttributeData"/> instance;
        /// otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetInt32Property__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetInt32Property__([NotNullWhen(true)] out Int32? value) => TryGetValueTypeValue(__PropertyId__.__GetSetInt32Property__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetTypeProperty__"/>, if one
        /// could be found in the wrapped <see cref="AttributeData"/> instance;
        /// otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetTypeProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetTypeProperty__([NotNullWhen(true)] out ITypeSymbol? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp is
                    {
                        // check for name == prop name
                        Key: nameof(__GetSetTypeProperty__),
                        Value:
                        {
                            // check arg is type
                            Kind: TypedConstantKind.Type,
                            // prop type is not NRT, so we check for not-null
                            Value: ITypeSymbol arg
                        }
                    })
                {
                    value = arg;
                    return true;
                }
            }

            value = null;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtTypeProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtTypeProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetNrtTypeProperty__(out ITypeSymbol? value) => TryGetNullableTypeValue(__PropertyId__.__GetSetNrtTypeProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetStringArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetStringArrayProperty__([NotNullWhen(true)] out ImmutableArray<String>? value) => TryGetReferenceTypeArrayValue(__PropertyId__.__GetSetStringArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32ArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetInt32ArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetInt32ArrayProperty__([NotNullWhen(true)] out ImmutableArray<Int32>? value) => TryGetValueTypeArrayValue(__PropertyId__.__GetSetInt32ArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringNrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetStringNrtArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetStringNrtArrayProperty__(out ImmutableArray<String>? value) => TryGetReferenceTypeNullableArrayValue(__PropertyId__.__GetSetStringNrtArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32NrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetInt32NrtArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetInt32NrtArrayProperty__(out ImmutableArray<Int32>? value) => TryGetValueTypeNullableArrayValue(__PropertyId__.__GetSetInt32NrtArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtStringArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetNrtStringArrayProperty__([NotNullWhen(true)] out ImmutableArray<String?>? value) => TryGetNullableReferenceTypeArrayValue(__PropertyId__.__GetSetNrtStringArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringNrtArrayProperty__"/> from <see
        /// cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtStringNrtArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetNrtStringNrtArrayProperty__(out ImmutableArray<String?>? value) => TryGetNullableReferenceTypeNullableArrayValue(__PropertyId__.__GetSetNrtStringNrtArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetTypeArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetTypeArrayProperty__([NotNullWhen(true)] out ImmutableArray<ITypeSymbol>? value) => TryGetTypeArrayValue(__PropertyId__.__GetSetTypeArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeNrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetTypeNrtArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetTypeNrtArrayProperty__(out ImmutableArray<ITypeSymbol>? value) => TryGetTypeNullableArrayValue(__PropertyId__.__GetSetTypeNrtArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtTypeArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtTypeArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetNrtTypeArrayProperty__([NotNullWhen(true)] out ImmutableArray<ITypeSymbol?>? value) => TryGetNullableTypeArrayValue(__PropertyId__.__GetSetNrtTypeArrayProperty__, out value);
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtTypeNrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <param name="value">
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value for <see
        /// cref="__GetSetNrtTypeNrtArrayProperty__"/> could be found in <see
        /// cref="Data"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public Boolean TryGet__GetSetNrtTypeNrtArrayProperty__(out ImmutableArray<ITypeSymbol?>? value) => TryGetNullableTypeNullableArrayValue(__PropertyId__.__GetSetNrtTypeNrtArrayProperty__, out value);
        #endregion
        #region General TryGet
        // TODO: generate short circuits for get-only unmapped props ids
        private static readonly ImmutableArray<String> _propertyNames =
            [
                //mapped properties
                nameof(__GetSetInt32Property__),
                nameof(__GetSetStringProperty__),
                nameof(__GetSetTypeArrayProperty__),
                //unmapped properties
                // TODO: generate for all properties
            ];
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
        public Boolean TryGetReferenceTypeValue<T>(__PropertyId__ id, [NotNullWhen(true)] out T? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetReferenceTypeValue(out value))
                    return true;
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
        public Boolean TryGetNullableReferenceTypeValue<T>(__PropertyId__ id, out T? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetNullableReferenceTypeValue(out value))
                    return true;
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
        public Boolean TryGetReferenceTypeArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetReferenceTypeArrayValue(out value))
                    return true;
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
        public Boolean TryGetNullableReferenceTypeArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetNullableReferenceTypeArrayValue(out value))
                    return true;
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
        public Boolean TryGetNullableReferenceTypeNullableArrayValue<T>(__PropertyId__ id, out ImmutableArray<T?>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetNullableReferenceTypeNullableArrayValue(out value))
                    return true;
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
        public Boolean TryGetReferenceTypeNullableArrayValue<T>(__PropertyId__ id, out ImmutableArray<T>? value)
            where T : class
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetReferenceTypeNullableArrayValue(out value))
                    return true;
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
        public Boolean TryGetTypeValue(__PropertyId__ id, [NotNullWhen(true)] out ITypeSymbol? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetTypeValue(out value))
                    return true;
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
        public Boolean TryGetNullableTypeValue(__PropertyId__ id, out ITypeSymbol? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetNullableTypeValue(out value))
                    return true;
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
        public Boolean TryGetTypeArrayValue(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetTypeArrayValue(out value))
                    return true;
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
        public Boolean TryGetNullableTypeArrayValue(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol?>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetNullableTypeArrayValue(out value))
                    return true;
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
        public Boolean TryGetNullableTypeNullableArrayValue(__PropertyId__ id, out ImmutableArray<ITypeSymbol?>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetNullableTypeNullableArrayValue(out value))
                    return true;
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
        public Boolean TryGetTypeNullableArrayValue(__PropertyId__ id, out ImmutableArray<ITypeSymbol>? value)
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetTypeNullableArrayValue(out value))
                    return true;
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
        public Boolean TryGetValueTypeValue<T>(__PropertyId__ id, [NotNullWhen(true)] out T? value)
            where T : struct
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetValueTypeValue(out value))
                    return true;
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
        public Boolean TryGetValueTypeArrayValue<T>(__PropertyId__ id, [NotNullWhen(true)] out ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetValueTypeArrayValue(out value))
                    return true;
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
        public Boolean TryGetValueTypeNullableArrayValue<T>(__PropertyId__ id, out ImmutableArray<T>? value)
            where T : struct
        {
            if(!isTypeMatch)
            {
                value = null;
                return false;
            }

            var name = _propertyNames[(Int32)id];

            foreach(var kvp in data.NamedArguments)
            {
                if(kvp.Key == name && kvp.Value.TryGetValueTypeNullableArrayValue(out value))
                    return true;
            }

            return _constructor.TryGetValueTypeNullableArrayValue(id, out value);
        }
        #endregion
        #region Get
        // TODO: generate stubs for get-only unmapped props
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetStringProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public String? Get__GetSetStringProperty__() => TryGet__GetSetStringProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtStringProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public String? Get__GetSetNrtStringProperty__() => TryGet__GetSetNrtStringProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32Property__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetInt32Property__"/>, if one
        /// could be found in the wrapped <see cref="AttributeData"/> instance;
        /// otherwise, <see langword="null"/>.
        /// </returns>
        public Int32? Get__GetSetInt32Property__() => TryGet__GetSetInt32Property__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetTypeProperty__"/>, if one
        /// could be found in the wrapped <see cref="AttributeData"/> instance;
        /// otherwise, <see langword="null"/>.
        /// </returns>
        public ITypeSymbol? Get__GetSetTypeProperty__() => TryGet__GetSetTypeProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtTypeProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ITypeSymbol? Get__GetSetNrtTypeProperty__() => TryGet__GetSetNrtTypeProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<String>? Get__GetSetStringArrayProperty__() => TryGet__GetSetStringArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32ArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<Int32>? Get__GetSetInt32ArrayProperty__() => TryGet__GetSetInt32ArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetStringNrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<String>? Get__GetSetStringNrtArrayProperty__() => TryGet__GetSetStringNrtArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetInt32NrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<Int32>? Get__GetSetInt32NrtArrayProperty__() => TryGet__GetSetInt32NrtArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<String?>? Get__GetSetNrtStringArrayProperty__() => TryGet__GetSetNrtStringArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtStringNrtArrayProperty__"/> from <see
        /// cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<String?>? Get__GetSetNrtStringNrtArrayProperty__() => TryGet__GetSetNrtStringNrtArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<ITypeSymbol>? Get__GetSetTypeArrayProperty__() => TryGet__GetSetTypeArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetTypeNrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<ITypeSymbol>? Get__GetSetTypeNrtArrayProperty__() => TryGet__GetSetTypeNrtArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtTypeArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<ITypeSymbol?>? Get__GetSetNrtTypeArrayProperty__() => TryGet__GetSetNrtTypeArrayProperty__(out var value) ? value : null;
        /// <summary>
        /// Attempts to retrieve the value for <see
        /// cref="__GetSetNrtTypeNrtArrayProperty__"/> from <see cref="Data"/>.
        /// </summary>
        /// <returns>
        /// The value obtained for <see cref="__GetSetNrtTypeProperty__"/>, if
        /// one could be found in <see cref="Data"/>; otherwise, <see
        /// langword="null"/>.
        /// </returns>
        public ImmutableArray<ITypeSymbol?>? Get__GetSetNrtTypeNrtArrayProperty__() => TryGet__GetSetNrtTypeNrtArrayProperty__(out var value) ? value : null;
        #endregion
        #region Equality
        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) =>
            throw new NotSupportedException("__TestAttribute__Attribute.__PropertyAccessor__.Equals is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        /// <inheritdoc/>
        public Boolean Equals(__PropertyAccessor__ other) =>
            throw new NotSupportedException("__TestAttribute__Attribute.__PropertyAccessor__.Equals is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            throw new NotSupportedException("__TestAttribute__Attribute.__PropertyAccessor__.GetHashCode is not supported. This exception might indicate an accidental leakage into the source generator pipeline cache.");
        #endregion
    }
}

/// <summary>
/// Provides extension methods for working with <see
/// cref="__TestAttribute__Attribute"/>.
/// </summary>
// TODO: extensions class should be internal
internal static class __TestAttribute__AttributeExtensions
{
    /// <summary>
    /// Determines whether an instance of <see cref="AttributeData"/> represents
    /// an attribute of type <see cref="__TestAttribute__Attribute"/>.
    /// </summary>
    /// <param name="data">
    /// The <see cref="AttributeData"/> to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> is <paramref name="data"/> is representing an
    /// instance of <see cref="__TestAttribute__Attribute"/>; otherwise, <see
    /// langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean Is__TestAttribute__(this AttributeData data) =>
        data is
        {
            // checks if class is available
            AttributeClass:
            {
                // TODO: generic and nested attributes
                // checks type name match
                Name: nameof(__TestAttribute__Attribute),
                // recursively check containing namespace, avoid TDS
                ContainingNamespace:
                {
                    Name: "CodeAnalysis",
                    ContainingNamespace:
                    {
                        Name: "RhoMicro",
                        // at root: check for global NS
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };
    /// <summary>
    /// Gets an object providing strongly typed access to all <see
    /// cref="__TestAttribute__Attribute"/> properties represented in an <see
    /// cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="data">
    /// The data to wrap.
    /// </param>
    /// <param name="checkType">
    /// A value indicating whether to check that <paramref name="data"/> is
    /// representing an instance of <see cref="__TestAttribute__Attribute"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see
    /// cref="__TestAttribute__Attribute.__PropertyAccessor__"/> wrapping <paramref
    /// name="data"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static __TestAttribute__Attribute.__PropertyAccessor__ Get__TestAttribute__PropertyAccessor(this AttributeData data, Boolean checkType = true) =>
        new(data, isTypeMatch: !checkType || data.Is__TestAttribute__());
    /// <summary>
    /// Gets an object providing strongly typed access to individual <see
    /// cref="__TestAttribute__Attribute"/> constructor arguments represented
    /// in an <see cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="data">
    /// The data to wrap.
    /// </param>
    /// <param name="checkType">
    /// A value indicating whether to check that <paramref name="data"/> is
    /// representing an instance of <see cref="__TestAttribute__Attribute"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see
    /// cref="__TestAttribute__Attribute.__ConstructorArgumentAccessor__"/> wrapping
    /// <paramref name="data"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static __TestAttribute__Attribute.__ConstructorArgumentAccessor__ Get__TestAttribute__ConstructorArgumentAccessor(this AttributeData data, Boolean checkType = true) =>
        new(data, isTypeMatch: !checkType || data.Is__TestAttribute__());
    /// <summary>
    /// Attempts to get an object providing strongly typed access to all <see
    /// cref="__TestAttribute__Attribute"/> properties represented in an <see
    /// cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="data">
    /// The data to create a model with.
    /// </param>
    /// <param name="checkType">
    /// A value indicating whether to check that <paramref name="data"/> is
    /// representing an instance of <see cref="__TestAttribute__Attribute"/>.
    /// </param>
    /// <param name="model">
    /// The model created, if one could be created; otherwise, <see
    /// langword="null"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="__TestAttribute__Attribute.__Model__"/>
    /// representing <paramref name="data"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGet__TestAttribute__Model(this AttributeData data, Boolean checkType, [NotNullWhen(true)] out __TestAttribute__Attribute.__Model__? model)
    {
        if(!checkType || data.Is__TestAttribute__())
        {
            model = __TestAttribute__Attribute.__Model__.Create(data);
            return true;
        }

        model = null;
        return false;
    }
    /// <summary>
    /// Attempts to get an object providing strongly typed access to all <see
    /// cref="__TestAttribute__Attribute"/> properties represented in an <see
    /// cref="AttributeData"/> instance. A type check ensuring <paramref
    /// name="data"/> to represent an instance of <see
    /// cref="__TestAttribute__Attribute"/> is omitted.
    /// </summary>
    /// <param name="data">
    /// The data to create a model with.
    /// </param>
    /// <param name="model">
    /// The model created, if one could be created; otherwise, <see
    /// langword="null"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="__TestAttribute__Attribute.__Model__"/>
    /// representing <paramref name="data"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGet__TestAttribute__Model(this AttributeData data, [NotNullWhen(true)] out __TestAttribute__Attribute.__Model__? model) =>
        data.TryGet__TestAttribute__Model(checkType: false, out model);
}
#endif
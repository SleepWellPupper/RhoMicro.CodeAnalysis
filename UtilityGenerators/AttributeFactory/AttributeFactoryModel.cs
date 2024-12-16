namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Extensions;

internal sealed record AttributeFactoryModel(
    TypeSignatureModel Signature,

    IList<PropertyModel> MappedProperties,
    IList<PropertyModel> UnmappedProperties,
    IList<ConstructorModel> Constructors,

    String PropertyAccessorTypeName,
    String ConstructorArgumentAccessorTypeName,
    String PropertyIdTypeName,
    String ModelTypeName,
    String ExtensionsTypeName,
    Boolean GenerateModelTypeAsStruct)
{
    public static AttributeFactoryModel Create(INamedTypeSymbol target, AttributeData attribute, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var signature = TypeSignatureModel.Create(target, in ctx);

        var ctorModels = ctx.CollectionFactory.CreateList<ConstructorModel>();

        var mappedProperties = ctx.CollectionFactory.CreateList<PropertyModel>();
        var unmappedProperties = ctx.CollectionFactory.CreateList<PropertyModel>();

        var propertyMappings = ctx.CollectionFactory.CreateLazyDictionary<String, EquatableCollectionFactory, IList<ParameterMapping>>(
            static (_, f) => f.CreateList<ParameterMapping>(),
            ctx.CollectionFactory);

        GetConstructorModels(target, ctorModels, propertyMappings, in ctx);
        GetPropertyModels(target, mappedProperties, unmappedProperties, propertyMappings, in ctx);
        GetAttributeProperties(
            target,
            attribute,
            out var propertyAccessorTypeName,
            out var constructorArgumentAccessorTypeName,
            out var propertyIdTypeName,
            out var modelTypeName,
            out var extensionsTypeName,
            out var generateModelTypeAsStruct,
            in ctx);

        var result = new AttributeFactoryModel(
            signature,
            MappedProperties: mappedProperties,
            UnmappedProperties: unmappedProperties,
            ctorModels,
            PropertyAccessorTypeName: propertyAccessorTypeName,
            ConstructorArgumentAccessorTypeName: constructorArgumentAccessorTypeName,
            PropertyIdTypeName: propertyIdTypeName,
            ModelTypeName: modelTypeName,
            ExtensionsTypeName: extensionsTypeName,
            GenerateModelTypeAsStruct: generateModelTypeAsStruct);

        return result;
    }

    private static void GetAttributeProperties(
        INamedTypeSymbol target,
        AttributeData attribute,
        out String propertyAccessorTypeName,
        out String constructorArgumentAccessorTypeName,
        out String propertyIdTypeName,
        out String modelTypeName,
        out String extensionsTypeName,
        out Boolean generateModelTypeAsStruct,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        propertyAccessorTypeName = GenerateFactoryAttribute.DefaultPropertyAccessorTypeName;
        constructorArgumentAccessorTypeName = GenerateFactoryAttribute.DefaultConstructorArgumentAccessorTypeName;
        propertyIdTypeName = GenerateFactoryAttribute.DefaultPropertyIdTypeName;
        modelTypeName = GenerateFactoryAttribute.DefaultModelTypeName;
        generateModelTypeAsStruct = GenerateFactoryAttribute.DefaultGenerateModelTypeAsStruct;
        extensionsTypeName = $"{target.Name}Extensions";

        var model = attribute.GetGenerateFactoryAttributeModel();

        foreach(var (name, value) in attribute.NamedArguments)
        {
            ctx.ThrowIfCancellationRequested();

            switch(name)
            {
                case nameof(GenerateFactoryAttribute.PropertyAccessorTypeName):
                {
                    if(value.Value is String s)
                        propertyAccessorTypeName = s;
                    break;
                }
                case nameof(GenerateFactoryAttribute.ConstructorArgumentAccessorTypeName):
                {
                    if(value.Value is String s)
                        constructorArgumentAccessorTypeName = s;
                    break;
                }
                case nameof(GenerateFactoryAttribute.PropertyIdTypeName):
                {
                    if(value.Value is String s)
                        propertyIdTypeName = s;
                    break;
                }
                case nameof(GenerateFactoryAttribute.ModelTypeName):
                {
                    if(value.Value is String s)
                        modelTypeName = s;
                    break;
                }
                case nameof(GenerateFactoryAttribute.ExtensionsTypeName):
                {
                    if(value.Value is String s)
                        extensionsTypeName = s;
                    break;
                }
                case nameof(GenerateFactoryAttribute.GenerateModelTypeAsStruct):
                {
                    if(value.Value is Boolean b)
                        generateModelTypeAsStruct = b;
                    break;
                }
            }
        }
    }

    private static void GetPropertyModels(INamedTypeSymbol target, IList<PropertyModel> mappedProperties, IList<PropertyModel> unmappedProperties, IDictionary<String, IList<ParameterMapping>> propertyMappings, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        foreach(var property in target.GetMembers().OfType<IPropertySymbol>())
        {
            ctx.ThrowIfCancellationRequested();

            var model = PropertyModel.Create(property, propertyMappings, in ctx);

            if(model.Mappings.Count > 0)
            {
                mappedProperties.Add(model);
            } else
            {
                unmappedProperties.Add(model);
            }
        }
    }

    private static void GetConstructorModels(INamedTypeSymbol target, IList<ConstructorModel> ctorModels, IDictionary<String, IList<ParameterMapping>> propertyMappings, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        for(var ctorIndex = 0; ctorIndex < target.Constructors.Length; ctorIndex++)
        {
            ctx.ThrowIfCancellationRequested();

            var ctor = target.Constructors[ctorIndex];

            var ctorModel = ConstructorModel.Create(ctor, ctorIndex, in ctx);

            ctorModels.Add(ctorModel);

            foreach(var mapping in ctorModel.Mappings.Values)
            {
                ctx.ThrowIfCancellationRequested();

                if(mapping is { } m)
                    propertyMappings[m.PropertyName].Add(m);
            }
        }
    }
}

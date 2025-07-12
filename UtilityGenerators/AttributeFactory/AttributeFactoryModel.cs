// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record AttributeFactoryModel(
    NamedTypeModel Signature,

    EquatableList<PropertyModel> MappedProperties,
    EquatableList<PropertyModel> UnmappedProperties,
    EquatableList<ConstructorModel> Constructors,
    EquatableList<InitializationMethodModel> InitializationMethods,

    GenerateFactoryAttribute.Model AttributeModel,

    Boolean IsEquatable)
{
    public static AttributeFactoryModel Create(INamedTypeSymbol target, AttributeData attribute, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var signature = NamedTypeModel.Create(target, in ctx);

        var constructors = ctx.CollectionFactory.CreateList<ConstructorModel>();
        var mappedProperties = ctx.CollectionFactory.CreateList<PropertyModel>();
        var unmappedProperties = ctx.CollectionFactory.CreateList<PropertyModel>();
        var initializationMethods = ctx.CollectionFactory.CreateList<InitializationMethodModel>();
        var propertyMappings = ctx.CollectionFactory.CreateLazyDictionary<String, EquatableList<ParameterMapping>>(
            static (_, f) => f.CollectionFactory.CreateList<ParameterMapping>());

        GetConstructorModels(
            target,
            constructors,
            propertyMappings,
            in ctx);
        GetPropertyModels(
            target,
            mappedProperties,
            unmappedProperties,
            propertyMappings,
            out var isEquatable,
            in ctx);

        var attributeModel = attribute.GetGenerateFactoryAttributeModel(new(target), ctx.CancellationToken);

        var result = new AttributeFactoryModel(
            signature,

            MappedProperties: mappedProperties,
            UnmappedProperties: unmappedProperties,

            constructors,
            initializationMethods,

            attributeModel,

            IsEquatable: isEquatable);

        return result;
    }

    private static void GetPropertyModels(
        INamedTypeSymbol target,
        EquatableList<PropertyModel> mappedProperties,
        EquatableList<PropertyModel> unmappedProperties,
        LazyEquatableDictionary<String, EquatableList<ParameterMapping>> propertyMappings,
        out Boolean isEquatable,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        isEquatable = true;

        foreach(var member in target.GetMembers())
        {
            ctx.ThrowIfCancellationRequested();

            if(member is IPropertySymbol property)
            {
                AddProperty(
                    mappedProperties,
                    unmappedProperties,
                    propertyMappings,
                    property,
                    ref isEquatable,
                    in ctx);
            }
        }
    }

    private static void AddProperty(
        EquatableList<PropertyModel> mappedProperties,
        EquatableList<PropertyModel> unmappedProperties,
        LazyEquatableDictionary<String, EquatableList<ParameterMapping>> propertyMappings,
        IPropertySymbol property,
        ref Boolean isEquatable,
        in ModelCreationContext ctx)
    {
        var model = PropertyModel.Create(property, propertyMappings, in ctx);

        if(model.Mappings.Count > 0)
        {
            mappedProperties.Add(model);
        } else
        {
            unmappedProperties.Add(model);
        }

        if(property.Type is
        { SpecialType: SpecialType.System_Object }
            or IArrayTypeSymbol { ElementType: { SpecialType: SpecialType.System_Object } or { Name: "Type", ContainingNamespace: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } } }
            or { Name: "Type", ContainingNamespace: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } })
        {
            isEquatable = false;
        }
    }

    private static void GetConstructorModels(INamedTypeSymbol target, EquatableList<ConstructorModel> ctorModels, LazyEquatableDictionary<String, EquatableList<ParameterMapping>> propertyMappings, in ModelCreationContext ctx)
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

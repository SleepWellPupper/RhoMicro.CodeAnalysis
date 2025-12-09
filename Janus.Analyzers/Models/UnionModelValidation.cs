// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Microsoft.CodeAnalysis;

internal readonly struct UnionModelValidation(UnionModel model, CancellationToken ct)
{
    public Boolean HasUnsupportedSettingsUnawareStatePostCondition(INamedTypeSymbol target)
    {
        ct.ThrowIfCancellationRequested();

        if (HasUnsupportedSettingsUnawareState)
        {
            return true;
        }

        var unionName = target.ToDisplayString(UnionTypeAttribute.Model.TypeDisplayFormat);
        foreach (var variant in model.Variants)
        {
            ct.ThrowIfCancellationRequested();
            
            if (variant.Type.Name == unionName)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool HasUnsupportedSettingsUnawareState
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            if (ExceedsVariantGroupsLimit)
            {
                return true;
            }

            if (ContainsDuplicateVariantNames)
            {
                return true;
            }

            if (ContainsObjectVariant)
            {
                return true;
            }

            if (ContainsValueTypeVariantAsStructUnion)
            {
                return true;
            }

            if (ContainsDuplicateNullableValueTypeVariants)
            {
                return true;
            }

            return false;
        }
    }

    public Boolean ContainsDuplicateNullableValueTypeVariants
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            var handledTypes = new HashSet<String>();
            
            foreach (var variant in model.Variants)
            {
                ct.ThrowIfCancellationRequested();

                if (!handledTypes.Add(variant.Type.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
    
    public Boolean ContainsValueTypeVariantAsStructUnion
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            if (model.TypeKind is UnionTypeKind.Class)
            {
                return false;
            }
            
            foreach (var variant in model.Variants)
            {
                ct.ThrowIfCancellationRequested();

                if (variant.Type.Name is "global::System.ValueType")
                {
                    return true;
                }
            }

            return false;
        }
    }

    public Boolean ContainsObjectVariant
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            foreach (var variant in model.Variants)
            {
                ct.ThrowIfCancellationRequested();

                if (variant.Type.Name is "object")
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool HasUnsupportedSettingsAwareState
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            if (IsGenericJsonSerializable)
            {
                return true;
            }

            return false;
        }
    }

    public bool ExceedsVariantGroupsLimit
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            if (model.VariantGroups.Count > 31)
            {
                return true;
            }

            return false;
        }
    }

    private bool IsGenericJsonSerializable
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            if (model.Settings.JsonConverterSetting is JsonConverterSetting.OmitJsonConverter)
            {
                return false;
            }

            var isExplicitlyGeneric = model.TypeParameters is not [];
            if (isExplicitlyGeneric)
            {
                return true;
            }

            foreach (var containingType in model.ContainingTypes)
            {
                ct.ThrowIfCancellationRequested();

                if (containingType.TypeParameters is not [])
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool ContainsDuplicateVariantNames
    {
        get
        {
            ct.ThrowIfCancellationRequested();

            var variantNames = new HashSet<String>();
            foreach (var variant in model.Variants)
            {
                ct.ThrowIfCancellationRequested();

                if (!variantNames.Add(variant.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

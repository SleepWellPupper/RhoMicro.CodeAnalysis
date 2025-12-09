// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Equa;

using System.Collections.Immutable;

internal readonly record struct ContainingTypeModel(
    String Modifier,
    String Name,
    ImmutableArray<String> TypeParameters)
{
    public Boolean Equals(ContainingTypeModel other)
    {
        if (other.Modifier != Modifier)
        {
            return false;
        }

        if (other.Name != Name)
        {
            return false;
        }

        if (!other.TypeParameters.SequenceEqual(TypeParameters))
        {
            return false;
        }

        return true;
    }

    public override Int32 GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(Modifier);
        hc.Add(Name);

        foreach (var typeParameter in TypeParameters)
        {
            hc.Add(typeParameter);
        }

        var result = hc.ToHashCode();

        return result;
    }
}

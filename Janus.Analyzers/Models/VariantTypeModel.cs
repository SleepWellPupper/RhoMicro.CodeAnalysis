// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

internal readonly record struct VariantTypeModel(
    VariantTypeKind Kind,
    Boolean IsNullable,
    Boolean IsInterface,
    String Name,
    String DocsId)
{
    public String NullableName { get; } = IsNullable || Kind is VariantTypeKind.Unknown ? Name + "?" : Name;

    public String NullableValueTypeName =>
        Kind is VariantTypeKind.Reference or VariantTypeKind.Unknown ? Name : NullableName;
    
    public Boolean Equals(VariantTypeModel other) =>
        other.Kind == Kind
     && other.IsNullable == IsNullable
     && other.Name == Name;

    public override Int32 GetHashCode() => 
        HashCode.Combine(Kind, IsNullable, Name);

    public override String ToString() => throw new NotSupportedException();
}

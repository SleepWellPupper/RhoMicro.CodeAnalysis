namespace RhoMicro.CodeAnalysis.Library.Models;
using System;

using Microsoft.CodeAnalysis;

#if UTILITYGENERATORS
[IncludeFile]
#endif
internal readonly record struct PartialTypeKindModel(String Value)
{
    public static PartialTypeKindModel Class => new("class");
    public static PartialTypeKindModel Record => new("record");
    public static PartialTypeKindModel RecordStruct => new("record struct");
    public static PartialTypeKindModel Interface => new("interface");
    public static PartialTypeKindModel Struct => new("struct");

    public static PartialTypeKindModel Create(ITypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var result = type switch
        {
            { IsRecord: true, IsReferenceType: true } => Record,
            { IsRecord: true, IsValueType: true } => RecordStruct,
            { IsRecord: false, IsValueType: true } => Struct,
            { TypeKind: TypeKind.Interface } => Interface,
            _ => Class
        };

        return result;
    }
}

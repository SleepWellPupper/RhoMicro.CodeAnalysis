namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library;

readonly record struct Id(String Value, Int32 Depth)
{
    public String Absolute => $"./{Value}.json";
    public String Relative(Id to) => $"{String.Concat(Enumerable.Repeat("../", to.Depth))}{Value}.json";

    public static Id Create(
        ImmutableArray<AttributeData> attributes,
        ISymbol target,
        out JsonSchemaAttribute? attribute,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        for(var i = 0; i < attributes.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            if(JsonSchemaAttribute.TryCreate(attributes[i], out var a))
            {
                attribute = a;
                return Create(a, target, ct);
            }
        }

        attribute = null;
        return Create(target, ct);
    }
    public static Id Create(JsonSchemaAttribute? attribute, ISymbol target, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var value = attribute?.Id?.Trim();

        if(String.IsNullOrWhiteSpace(value))
            return Create(target, ct);

        return Create(value!);
    }
    private static Id Create(ISymbol target, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var result = $"{target.ContainingAssembly.Name}/{target.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNoGlobalNamespaceFormat).Replace('.', '/')}";

        return Create(result);
    }
    private static Id Create(String value) => new(value, value.Count(c => c == '/'));
}

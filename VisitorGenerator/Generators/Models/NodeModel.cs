namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record NodeModel(
    EquatableList<PropertyModel> Properties,
    NodeSignatureModel Signature,
    NodeSignatureModel BaseSignature)
    : NodeModelBase(Signature)
{
    public static void AddModels(
        ITypeSymbol nodeTypeCandidate,
        INamedTypeSymbol baseNodeType,
        NodeSignatureModel baseSignature,
        EquatableList<NodeModel> models,
        HashSet<ITypeSymbol> handledTypes,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if (SymbolEqualityComparer.Default.Equals(nodeTypeCandidate, baseNodeType))
            return;

        if (!handledTypes.Add(nodeTypeCandidate))
            return;

        if (nodeTypeCandidate is not INamedTypeSymbol namedNodeTypeCandidate)
            return;

        if (namedNodeTypeCandidate.ContainingType is not null)
            return;

        if (!namedNodeTypeCandidate.Inherits(baseNodeType))
            return;

        if (!NodeSignatureModel.TryCreate(namedNodeTypeCandidate, out var signature, in ctx))
            return;

        var properties = ctx.CollectionFactory.CreateList<PropertyModel>();
        foreach (var member in namedNodeTypeCandidate.GetMembers())
        {
            ctx.ThrowIfCancellationRequested();

            if (PropertyModel.TryCreate(member, baseNodeType, out var property, in ctx))
                properties.Add(property);
        }

        var model = new NodeModel(properties, signature, baseSignature);
        models.Add(model);

        if (nodeTypeCandidate.BaseType is { } baseNodeTypeCandidate)
            AddModels(baseNodeTypeCandidate, baseNodeType, baseSignature, models, handledTypes, in ctx);
    }

    public Boolean IsLeaf() => Signature.Flags.HasFlag(NodeSignatureFlags.IsSealed);
    public Boolean IsPublic() => Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic);
    public Boolean IsRecord() => Signature.Flags.HasFlag(NodeSignatureFlags.IsRecord);

    public MemberNames MemberNames() => new(Signature);
    public TypeNames TypeNames() => new(BaseSignature);
    public TypeNameTemplate FullName() => new(String.Empty, Signature, String.Empty, true);
}

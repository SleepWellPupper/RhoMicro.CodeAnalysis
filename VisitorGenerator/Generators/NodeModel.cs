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
    public static Boolean TryCreate(
        ITypeSymbol nodeTypeCandidate,
        INamedTypeSymbol baseNodeType,
        [NotNullWhen(true)] out NodeModel? result,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        result = null;

        if(nodeTypeCandidate is not INamedTypeSymbol namedNodeTypeCandidate)
            return false;

        if(namedNodeTypeCandidate.ContainingType is not null)
            return false;

        if(!namedNodeTypeCandidate.Inherits(baseNodeType))
            return false;

        var properties = ctx.CollectionFactory.CreateList<PropertyModel>();
        foreach(var member in namedNodeTypeCandidate.GetMembers())
        {
            ctx.ThrowIfCancellationRequested();

            if(PropertyModel.TryCreate(member, baseNodeType, out var property, in ctx))
                properties.Add(property);
        }

        var signature = NodeSignatureModel.Create(namedNodeTypeCandidate, in ctx);
        var baseSignature = NodeSignatureModel.Create(baseNodeType, in ctx);

        result = new NodeModel(properties, signature, baseSignature);

        return true;
    }

    public MemberNames MemberNames() => new(Signature);
    public TypeNames TypeNames() => new(BaseSignature);
    public TypeNameTemplate FullName() => new(String.Empty, Signature, String.Empty, true);
}

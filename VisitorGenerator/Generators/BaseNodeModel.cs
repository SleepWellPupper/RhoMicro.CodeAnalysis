namespace RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record BaseNodeModel(
    EquatableList<NodeModel> Nodes,
    NodeSignatureModel Signature)
    : NodeModelBase(Signature)
{
    public TypeNames TypeNames() => new(Signature);
    public TypeNameTemplate FullName() => new(String.Empty, Signature, String.Empty, true);
}

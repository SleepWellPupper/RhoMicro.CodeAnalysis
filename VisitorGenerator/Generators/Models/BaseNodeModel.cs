// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record BaseNodeModel(
    EquatableList<NodeModel> Nodes,
    NodeSignatureModel Signature)
    : NodeModelBase(Signature)
{
    public IEnumerable<NodeModel> LeafNodes() => Nodes.Where(static n => n.IsLeaf());
    public TypeNames TypeNames() => new(Signature);
    public TypeNameTemplate FullName() => new(String.Empty, Signature, String.Empty, true);
    public CrefTemplate Cref() => new(Signature, String.Empty);
}

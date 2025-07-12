// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[NonEquatable]
internal readonly partial struct MemberNames(NodeSignatureModel signature)
{
    public MethodNameTemplate VisitMethod { get; } = new("Visit", signature);
    public MethodNameTemplate TraverseMethod { get; } = new("Traverse", signature);
    public MethodNameTemplate OnBeforeVisitMethod { get; } = new("OnBeforeVisit", signature);
    public MethodNameTemplate OnAfterVisitMethod { get; } = new("OnAfterVisit", signature);

    public MethodNameTemplate GenericVisitMethod { get; } = new("Visit", signature);
    public MethodNameTemplate GenericTraverseMethod { get; } = new("Traverse", signature);
    public MethodNameTemplate GenericOnBeforeVisitMethod { get; } = new("OnBeforeVisit", signature);
    public MethodNameTemplate GenericOnAfterVisitMethod { get; } = new("OnAfterVisit", signature);
}

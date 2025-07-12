// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
#if VISITOR_GENERATOR
[IncludeFile(Hint = "RhoMicro.CodeAnalysis.GenerateVisitorAttribute.g.cs")]
#endif
internal sealed class GenerateVisitorAttribute(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } = nodeTypes;
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode">The type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode3">The third type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2, TNode3>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), typeof(TNode3), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode3">The third type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode4">The fourth type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2, TNode3, TNode4>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), typeof(TNode3), typeof(TNode4), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode3">The third type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode4">The fourth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode5">The fifth type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2, TNode3, TNode4, TNode5>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), typeof(TNode3), typeof(TNode4), typeof(TNode5), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode3">The third type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode4">The fourth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode5">The fifth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode6">The sixth type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2, TNode3, TNode4, TNode5, TNode6>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), typeof(TNode3), typeof(TNode4), typeof(TNode5), typeof(TNode6), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode3">The third type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode4">The fourth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode5">The fifth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode6">The sixth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode7">The seventh type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2, TNode3, TNode4, TNode5, TNode6, TNode7>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), typeof(TNode3), typeof(TNode4), typeof(TNode5), typeof(TNode6), typeof(TNode7), .. nodeTypes];
}
/// <summary>
/// Defines the types of node to generate visitor methods for.
/// </summary>
/// <param name="nodeTypes">The node types to generate visitor methods for.</param>
/// <typeparam name="TNode1">The first type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode2">The second type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode3">The third type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode4">The fourth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode5">The fifth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode6">The sixth type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode7">The seventh type of node to generate visitor methods for.</typeparam>
/// <typeparam name="TNode8">The eighth type of node to generate visitor methods for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GenerateVisitorAttribute<TNode1, TNode2, TNode3, TNode4, TNode5, TNode6, TNode7, TNode8>(params Type[] nodeTypes) : Attribute
{
    public IReadOnlyList<Type> NodeTypes { get; } =
        [typeof(TNode1), typeof(TNode2), typeof(TNode3), typeof(TNode4), typeof(TNode5), typeof(TNode6), typeof(TNode7), typeof(TNode8), .. nodeTypes];
}
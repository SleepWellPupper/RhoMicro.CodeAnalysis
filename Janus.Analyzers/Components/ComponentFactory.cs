// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Janus;
using Library.Models.Collections;

partial class ComponentFactory
{
    partial class Docs
    {
        public static DocsCommentElementComponent<String> SwitchReturns() =>
            Returns("The result produced by the invoked handler.");
        
        public static DocsCommentElementComponent<String> SwitchSummary() =>
            Summary("Invokes a handler based on the represented variant.");
        
        public static DocsCommentElementComponent<(String, String)> SwitchStateParam() =>
            Param("state", "The state to pass to handlers.");
        
        public static DocsCommentElementComponent<(String, String)> SwitchStateTypeparam() =>
            TypeParam("TState", "The type of state to pass to handlers.");

        public static DocsCommentElementComponent<(String, String)> SwitchResultTypeparam() =>
            TypeParam("TResult", "The type of result returned by handlers.");
        
        public static DocsCommentElementComponent<(String, String)> SwitchDefaultHandlerParam() =>
            Param("defaultHandler", "The handler to invoke if no handler for the represented variant was passed.");

        public static DocsCommentElementComponent<(String, String)> SwitchDefaultResultParam() =>
            Param("defaultResult", "The result to return if no handler for the represented variant was passed.");

        public static StrategyComponent<UnionModel> SwitchHandlerParams(UnionModel model) =>
            Create(model, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(ComponentFactory.List(m.Variants, static (v, i, l, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append(SwitchHandlerParam(v));
                }, separator: "\n"));
            });

        public static
            DocsCommentElementComponent<(
                String,
                UnionTypeAttribute.Model,
                Action<UnionTypeAttribute.Model, CSharpSourceBuilder, CancellationToken>)>
            SwitchHandlerParam(UnionTypeAttribute.Model variant) =>
            Param($"on{variant.Name}", variant, static (v, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append($"The handler to invoke if the union is of the {C(v.Name)} variant.");
            });
    }

    public static ListComponent<EquatableList<TElement>, TElement, String> List<TElement>(
        EquatableList<TElement> list,
        Action<TElement, Int32, Int32, CSharpSourceBuilder, CancellationToken> append,
        String separator = "",
        String terminator = "")
        => List<EquatableList<TElement>, TElement>(
            list,
            append,
            separator,
            terminator);

    public static ListComponent<EquatableList<String>, String, String> List(
        EquatableList<String> list,
        Action<String, Int32, Int32, CSharpSourceBuilder, CancellationToken> append,
        String separator = "",
        String terminator = "")
        => List<EquatableList<String>, String>(
            list,
            append,
            separator,
            terminator);

    public static ListComponent<EquatableList<UnionTypeAttribute.Model>, UnionTypeAttribute.Model, String> List(
        EquatableList<UnionTypeAttribute.Model> list,
        Action<UnionTypeAttribute.Model, Int32, Int32, CSharpSourceBuilder, CancellationToken> append,
        String separator = "",
        String terminator = "")
        => List<EquatableList<UnionTypeAttribute.Model>, UnionTypeAttribute.Model>(
            list,
            append,
            separator,
            terminator);

    public static ListComponent<String[], String, String> List(
        String[] list,
        Action<String, Int32, Int32, CSharpSourceBuilder, CancellationToken> append,
        String separator = "",
        String terminator = "")
        => List<String[], String>(
            list,
            append,
            separator,
            terminator);
}

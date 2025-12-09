// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System.Collections.Immutable;
using Options = DocsCommentElementComponentOptions;

#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
partial class ComponentFactory
{
    /// <summary>
    /// Creates docs comment component instances.
    /// </summary>
    public static partial class Docs
    {
        /// <summary>
        /// Creates a new docs comment element.
        /// </summary>
        /// <param name="options">
        /// The options to be used by the component.
        /// </param>
        /// <returns>
        /// A new docs comment element component.
        /// </returns>
        public static DocsCommentElementComponent Element(Options options)
            => new(Attributes: null, Body: null, options);
        /// <summary>
        /// Creates a new docs comment element.
        /// </summary>
        /// <param name="state">
        /// The state used by the component.
        /// </param>
        /// <param name="options">
        /// The options to be used by the component.
        /// </param>
        /// <param name="attributes">
        /// The callback to invoke when appending attributes.
        /// </param>
        /// <param name="body">
        /// The callback to invoke when appending the body.
        /// </param>
        /// <typeparam name="TState">
        /// The type of state used by the component.
        /// </typeparam>
        /// <returns>
        /// A new docs comment element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> Element<TState>(
            TState state,
            Options options,
            Action<TState, CSharpSourceBuilder, CancellationToken>? body,
            Action<TState, CSharpSourceBuilder, CancellationToken>? attributes)
            => new(state, Attributes: attributes, Body: body, options);

        /// <summary>
        /// Creates a new docs comment element.
        /// </summary>
        /// <param name="options">
        /// The options to be used by the component.
        /// </param>
        /// <param name="attributes">
        /// The callback to invoke when appending attributes.
        /// </param>
        /// <param name="body">
        /// The callback to invoke when appending the body.
        /// </param>
        /// <returns>
        /// A new docs comment element component.
        /// </returns>
        public static DocsCommentElementComponent Element(
            Options options,
            Action<CSharpSourceBuilder, CancellationToken>? body,
            Action<CSharpSourceBuilder, CancellationToken>? attributes)
            => new(Attributes: attributes, Body: body, options);

        /// <summary>
        /// Creates a new docs comment element.
        /// </summary>
        /// <param name="options">
        /// The options to be used by the component.
        /// </param>
        /// <param name="text">
        /// The text to render in the element.
        /// </param>
        /// <returns>
        /// A new docs comment element component.
        /// </returns>
        public static DocsCommentElementComponent<String> Element(
            Options options,
            String text)
            => Element(
                text,
                options,
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append(t);
                },
                attributes: null);

        /// <summary>
        /// Creates a new <c>summary</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>summary</c> element.
        /// </param>
        /// <returns>
        /// A new <c>summary</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> Summary(String text) =>
            Element(Options.Summary, text);

        /// <summary>
        /// Creates a new <c>summary</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>summary</c> element component.
        /// </returns>
        public static DocsCommentElementComponent Summary(Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(Options.Summary, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>summary</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>summary</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> Summary<TState>(
            TState state,
            Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(state, Options.Summary, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>returns</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>returns</c> element.
        /// </param>
        /// <returns>
        /// A new <c>returns</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> Returns(String text) =>
            Element(Options.Returns, text);

        /// <summary>
        /// Creates a new <c>returns</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>returns</c> element component.
        /// </returns>
        public static DocsCommentElementComponent Returns(Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(Options.Returns, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>returns</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>returns</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> Returns<TState>(
            TState state,
            Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(state, Options.Returns, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>remarks</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>remarks</c> element.
        /// </param>
        /// <returns>
        /// A new <c>remarks</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> Remarks(String text) =>
            Element(Options.Remarks, text);

        /// <summary>
        /// Creates a new <c>remarks</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>remarks</c> element component.
        /// </returns>
        public static DocsCommentElementComponent Remarks(Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(Options.Remarks, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>remarks</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>remarks</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> Remarks<TState>(
            TState state,
            Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(state, Options.Remarks, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>param</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>param</c> element.
        /// </param>
        /// <param name="name">
        /// The name of the parameter documented by the <c>param</c> element.
        /// </param>
        /// <returns>
        /// A new <c>param</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<(String, String)> Param(String name, String text) =>
            Element(
                state: (name, text),
                Options.Param,
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (name, _) = t;

                    b.Append($"name=\"{name}\"");
                },
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (_, text) = t;
                    b.Append(text);
                });

        /// <summary>
        /// Creates a new <c>param</c> element component.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter documented by the <c>param</c> element.
        /// </param>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>param</c> element component.
        /// </returns>
        public static
            DocsCommentElementComponent<(String, TState, Action<TState, CSharpSourceBuilder, CancellationToken>)>
            Param<TState>(
                String name,
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(
                state: (name, state, body),
                Options.Param,
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (name, _, _) = t;

                    b.Append($"name=\"{name}\"");
                },
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (_, state, body) = t;

                    body.Invoke(state, b, ct);
                });

        /// <summary>
        /// Creates a new <c>param</c> element component.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter documented by the <c>param</c> element.
        /// </param>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>param</c> element component.
        /// </returns>
        public static
            DocsCommentElementComponent<(String, Action<CSharpSourceBuilder, CancellationToken>)>
            Param(String name, Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(
                state: (name, body),
                Options.Param,
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (name, _) = t;

                    b.Append($"name=\"{name}\"");
                },
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (_, body) = t;

                    body.Invoke(b, ct);
                });

        /// <summary>
        /// Creates a new <c>typeparam</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>typeparam</c> element.
        /// </param>
        /// <param name="name">
        /// The name of the type parameter documented by the <c>typeparam</c> element.
        /// </param>
        /// <returns>
        /// A new <c>typeparam</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<(String, String)> TypeParam(String name, String text) =>
            Element(
                state: (name, text),
                Options.TypeParam,
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (name, _) = t;

                    b.Append($"name=\"{name}\"");
                },
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (_, text) = t;
                    b.Append(text);
                });

        /// <summary>
        /// Creates a new <c>typeparam</c> element component.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter documented by the <c>typeparam</c> element.
        /// </param>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>typeparam</c> element component.
        /// </returns>
        public static
            DocsCommentElementComponent<(String, TState, Action<TState, CSharpSourceBuilder, CancellationToken>)>
            TypeParam<TState>(
                String name,
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(
                state: (name, state, body),
                Options.TypeParam,
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (name, _, _) = t;

                    b.Append($"name=\"{name}\"");
                },
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (_, state, body) = t;

                    body.Invoke(state, b, ct);
                });

        /// <summary>
        /// Creates a new <c>see</c> langword element component.
        /// </summary>
        /// <param name="word">
        /// The name of the language word referenced by the component.
        /// </param>
        /// <returns>
        /// A new <c>see</c> element component referencing the language word.
        /// </returns>
        public static DocsCommentElementComponent<String> Langword(String word) =>
            Element(
                state: word,
                Options.See,
                body: null,
                attributes: static (w, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append($"langword=\"{w}\"");
                });

        /// <summary>
        /// Creates a new <c>see</c> cref element component.
        /// </summary>
        /// <param name="memberReference">
        /// The member referenced by the <c>see</c> element.
        /// </param>
        /// <returns>
        /// A new <c>see</c> element component referencing the member.
        /// </returns>
        public static DocsCommentElementComponent<String> Cref(String memberReference) =>
            Element(
                state: memberReference,
                Options.See,
                body: null,
                attributes: static (m, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append($"cref=\"{m}\"");
                });

        /// <summary>
        /// Creates a new <c>see</c> cref element component.
        /// </summary>
        /// <param name="state">
        /// The state used by <paramref name="memberReference"/>.
        /// </param>
        /// <param name="memberReference">
        /// The callback invoked when appending the member referenced by the <c>see</c> element.
        /// </param>
        /// <typeparam name="TState">
        /// The type of state used by <paramref name="memberReference"/>.
        /// </typeparam>
        /// <returns>
        /// A new <c>see</c> element component referencing the member.
        /// </returns>
        public static DocsCommentElementComponent<(TState, Action<TState, CSharpSourceBuilder, CancellationToken>)>
            Cref<TState>(
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken> memberReference) =>
            Element(
                state: (state, memberReference),
                Options.See,
                body: null,
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (state, memberReference) = t;

                    b.Append("cref=\"");
                    memberReference.Invoke(state, b, ct);
                    b.Append("\"");
                });

        /// <summary>
        /// Creates a new <c>paramref</c> element component.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter referenced by the <c>paramref</c> element.
        /// </param>
        /// <returns>
        /// A new <c>paramref</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> ParamRef(String name) =>
            Element(
                state: name,
                Options.ParamRef,
                body: null,
                attributes: static (n, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append($"name=\"{n}\"");
                });

        /// <summary>
        /// Creates a new <c>typeparamref</c> element component.
        /// </summary>
        /// <param name="name">
        /// The name of the type parameter referenced by the <c>typeparamref</c> element.
        /// </param>
        /// <returns>
        /// A new <c>typeparamref</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> TypeParamRef(String name) =>
            Element(
                state: name,
                Options.TypeParamRef,
                body: null,
                attributes: static (n, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append($"name=\"{n}\"");
                });

        /// <summary>
        /// Creates a new <c>inheritdoc</c> element component.
        /// </summary>
        /// <param name="reference">
        /// The member referenced by the <c>inheritdoc</c> element.
        /// </param>
        /// <returns>
        /// A new <c>inheritdoc</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String?> Inheritdoc(String? reference = null) =>
            Element(
                state: reference,
                Options.Inheritdoc,
                body: null,
                attributes: static (r, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    if (r is null)
                    {
                        return;
                    }

                    b.Append($"cref=\"{r}\"");
                });

        /// <summary>
        /// Creates a new <c>c</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>c</c> element.
        /// </param>
        /// <returns>
        /// A new <c>c</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> C(String text) =>
            Element(Options.C, text);

        /// <summary>
        /// Creates a new <c>c</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>c</c> element component.
        /// </returns>
        public static DocsCommentElementComponent C(Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(Options.C, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>c</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>c</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> C<TState>(
            TState state,
            Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(state, Options.C, body: body, attributes: null);


        /// <summary>
        /// Creates a new <c>term</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>term</c> element.
        /// </param>
        /// <returns>
        /// A new <c>term</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> Term(String text) =>
            Element(Options.Term, text);

        /// <summary>
        /// Creates a new <c>term</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>term</c> element component.
        /// </returns>
        public static DocsCommentElementComponent Term(Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(Options.Term, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>term</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>term</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> Term<TState>(
            TState state,
            Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(state, Options.Term, body: body, attributes: null);


        /// <summary>
        /// Creates a new <c>description</c> element component.
        /// </summary>
        /// <param name="text">
        /// The text to render in the <c>description</c> element.
        /// </param>
        /// <returns>
        /// A new <c>description</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<String> Description(String text) =>
            Element(Options.Description, text);

        /// <summary>
        /// Creates a new <c>description</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>description</c> element component.
        /// </returns>
        public static DocsCommentElementComponent Description(Action<CSharpSourceBuilder, CancellationToken> body) =>
            Element(Options.Description, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>description</c> element component.
        /// </summary>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>description</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<TState> Description<TState>(
            TState state,
            Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(state, Options.Description, body: body, attributes: null);

        /// <summary>
        /// Creates a new <c>list</c> element component.
        /// </summary>
        /// <param name="type">
        /// The type of the list component.
        /// </param>
        /// <param name="body">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <returns>
        /// A new <c>list</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<
                (String type,
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken> body)>
            List<TState>(
                String type,
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken> body) =>
            Element(
                (type, state, body),
                Options.List,
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (_, state, body) = t;

                    body.Invoke(state, b, ct);
                },
                attributes: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (type, _, _) = t;

                    b.Append($"type=\"{type}\"");
                });

        /// <summary>
        /// Creates a new <c>item</c> element component.
        /// </summary>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="term">
        /// The callback invoked when appending the <c>term</c> child of the <c>item</c> element.
        /// </param>
        /// <param name="description">
        /// The callback invoked when appending the <c>description</c> child of the <c>item</c> element.
        /// </param>
        /// <returns>
        /// A new <c>item</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<
                (TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken>? term,
                Action<TState, CSharpSourceBuilder, CancellationToken>? description)>
            Item<TState>(
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken>? term,
                Action<TState, CSharpSourceBuilder, CancellationToken>? description) =>
            Element(
                (state, term, description),
                Options.Item,
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (state, term, description) = t;

                    if (term is not null)
                    {
                        b.Append(Term(state, term));
                    }

                    if (description is null)
                    {
                        return;
                    }

                    if (term is not null)
                    {
                        b.AppendLine();
                    }

                    b.Append(Description(state, description));
                }, attributes: null);

        /// <summary>
        /// Creates a new <c>listheader</c> element component.
        /// </summary>
        /// <param name="state">
        /// <inheritdoc cref="Element{TState}"/>
        /// </param>
        /// <param name="term">
        /// The callback invoked when appending the <c>term</c> child of the <c>listheader</c> element.
        /// </param>
        /// <param name="description">
        /// The callback invoked when appending the <c>description</c> child of the <c>listheader</c> element.
        /// </param>
        /// <returns>
        /// A new <c>listheader</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<
                (TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken>? term,
                Action<TState, CSharpSourceBuilder, CancellationToken>? description)>
            ListHeader<TState>(
                TState state,
                Action<TState, CSharpSourceBuilder, CancellationToken>? term,
                Action<TState, CSharpSourceBuilder, CancellationToken>? description) =>
            Element(
                (state, term, description),
                Options.ListHeader,
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (state, term, description) = t;

                    if (term is not null)
                    {
                        b.Append(Term(state, term));
                    }

                    if (description is null)
                    {
                        return;
                    }

                    if (term is not null)
                    {
                        b.AppendLine();
                    }

                    b.Append(Description(state, description));
                }, attributes: null);

        /// <summary>
        /// Creates a new <c>listheader</c> element component.
        /// </summary>
        /// <param name="term">
        /// The text to append as the body of the <c>term</c> child of the <c>listheader</c> element.
        /// </param>
        /// <param name="description">
        /// The text to append as the body of the <c>description</c> child of the <c>listheader</c> element.
        /// </param>
        /// <returns>
        /// A new <c>listheader</c> element component.
        /// </returns>
        public static DocsCommentElementComponent<(String? term, String? description)> ListHeader(
                String? term,
                String? description) =>
            Element(
                (term, description),
                Options.ListHeader,
                body: static (t, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var (term, description) = t;

                    if (term is not null)
                    {
                        b.Append(Term(term));
                    }

                    if (description is null)
                    {
                        return;
                    }

                    if (term is not null)
                    {
                        b.AppendLine();
                    }

                    b.Append(Description(description));
                }, attributes: null);

        /// <summary>
        /// Creates a new <c>br</c> element.
        /// </summary>
        /// <returns>
        /// A new <c>br</c> element.
        /// </returns>
        public static DocsCommentElementComponent Br() => Element(Options.Br);
    }
}

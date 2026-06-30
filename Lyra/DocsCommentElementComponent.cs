// SPDX-License-Identifier: MPL-2.0

using System;
using System.Threading;

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Implements a docs comment XML element.
/// </summary>
/// <remarks>
/// This type does not support value equality.
/// </remarks>
/// <param name="State">
/// The state used by the component.
/// </param>
/// <param name="Attributes">
/// The callback to invoke when appending attributes.
/// </param>
/// <param name="Body">
/// The callback to invoke when appending the body.
/// </param>
/// <param name="Options">
/// The options used by the component.
/// </param>
/// <typeparam name="TState">
/// The type of state used by the component.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct DocsCommentElementComponent<TState>(
    TState State,
    Action<TState, CSharpSourceBuilder, CancellationToken>? Attributes,
    Action<TState, CSharpSourceBuilder, CancellationToken>? Body,
    DocsCommentElementComponentOptions Options)
    : ICSharpSourceComponent
{
    /// <inheritdoc/>
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Options.IndentSelfWithComment)
        {
            builder.Indent("/// ");
        }

        builder.Append('<');

        builder.Append(Options.Element);

        if (Attributes is not null)
        {
            builder.Append(' ');
            Attributes.Invoke(State, builder, cancellationToken);
        }

        if (Body is not null)
        {
            builder.Append('>');

            if (Options.Multiline)
            {
                builder.AppendLine();
            }

            if (Options.IndentBody)
            {
                builder.Indent();
            }

            Body.Invoke(State, builder, cancellationToken);

            if (Options.IndentBody)
            {
                builder.Detent();
            }

            if (Options.Multiline)
            {
                builder.AppendLine();
            }

            builder.Append($"</{Options.Element}");
        }
        else
        {
            builder.Append('/');
        }

        builder.Append('>');

        if (Options.IndentSelfWithComment)
        {
            builder.Detent();
        }
    }

    /// <inheritdoc />
    public Boolean Equals(DocsCommentElementComponent<TState> other) =>
        throw new NotSupportedException("Equals is not supported on this type.");

    /// <inheritdoc />
    public override Int32 GetHashCode() =>
        throw new NotSupportedException("GetHashCode is not supported on this type.");
}

/// <summary>
/// Implements a docs comment XML element.
/// </summary>
/// <param name="Attributes">
/// The callback to invoke when appending attributes.
/// </param>
/// <param name="Body">
/// The callback to invoke when appending the body.
/// </param>
/// <param name="Options">
/// The options used by the component.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct DocsCommentElementComponent(
    Action<CSharpSourceBuilder, CancellationToken>? Attributes,
    Action<CSharpSourceBuilder, CancellationToken>? Body,
    DocsCommentElementComponentOptions Options)
    : ICSharpSourceComponent
{
    /// <inheritdoc/>
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Options.IndentSelfWithComment)
        {
            builder.Indent("/// ");
        }

        builder.Append('<');

        builder.Append(Options.Element);

        if (Attributes is not null)
        {
            builder.Append(' ');
            Attributes.Invoke(builder, cancellationToken);
        }

        if (Body is not null)
        {
            builder.Append('>');

            if (Options.Multiline)
            {
                builder.AppendLine();
            }

            if (Options.IndentBody)
            {
                builder.Indent();
            }

            Body.Invoke(builder, cancellationToken);

            if (Options.IndentBody)
            {
                builder.Detent();
            }

            if (Options.Multiline)
            {
                builder.AppendLine();
            }

            builder.Append($"</{Options.Element}");
        }
        else
        {
            builder.Append('/');
        }

        builder.Append('>');

        if (Options.IndentSelfWithComment)
        {
            builder.Detent();
        }
    }

    /// <inheritdoc />
    public Boolean Equals(DocsCommentElementComponent other) =>
        throw new NotSupportedException("Equals is not supported on this type.");

    /// <inheritdoc />
    public override Int32 GetHashCode() =>
        throw new NotSupportedException("GetHashCode is not supported on this type.");
}

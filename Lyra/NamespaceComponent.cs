// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Represents a block scoped namespace.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
/// <param name="Name">
/// The name of the namespace.
/// </param>
/// <param name="Body">
/// The body of the namespace block.
/// </param>
/// <typeparam name="TBody">
/// The type of body of the namespace block.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
readonly record struct NamespaceComponent<TBody>(String Name, TBody Body) : ICSharpSourceComponent
        where TBody : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Name is null)
        {
            return;
        }

        if (Name is not [])
        {
            builder.AppendLine($"namespace {Name}").AppendLine("{").Indent();
        }

        builder.AppendLine(Body);

        if (Name is not [])
        {
            builder.Detent().Append("}");
        }
    }
}

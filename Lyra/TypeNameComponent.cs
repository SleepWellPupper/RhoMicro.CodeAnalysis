// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Threading;

/// <summary>
/// Represents the name of a type.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct TypeNameComponent : ICSharpSourceComponent
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type">
    /// The type whose name to append.
    /// </param>
    /// <param name="options">
    /// The options to use when appending the type name, or <see langword="null"/>
    /// to use the builders default type name options.
    /// </param>
    public TypeNameComponent(Type type, TypeNameOptions? options = null)
    {
        _type = type;
        _options = options;
        _typeName = null;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="typeName">
    /// The name to append.
    /// </param>
    public TypeNameComponent(String typeName)
    {
        _type = null;
        _typeName = typeName;
    }

    private readonly TypeNameOptions? _options;
    private readonly Type? _type;
    private readonly String? _typeName;

    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_type is not null)
        {
            if (_options is { } o)
            {
                builder.AppendTypeName(_type, o);
            }
            else
            {
                builder.AppendTypeName(_type);
            }
        }
        else if (_typeName is not null)
        {
            builder.Append(_typeName);
        }
    }
}

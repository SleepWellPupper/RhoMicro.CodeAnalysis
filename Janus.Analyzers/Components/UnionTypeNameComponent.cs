// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using Lyra;
using static Lyra.ComponentFactory;

internal readonly record struct UnionTypeNameComponent(
    UnionModel Model,
    Boolean RenderOpenGeneric = false,
    Boolean RenderNullable = false)
    : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        builder.Append(Model.Name);

        if (Model.TypeParameters is [])
        {
            if (RenderNullable && Model.TypeKind is UnionTypeKind.Class)
            {
                builder.Append('?');
            }

            return;
        }

        var rented = RenderOpenGeneric
            ? ArrayPool<String>.Shared.Rent(Model.TypeParameters.Count)
            : null;
        try
        {
            var emptyNames = rented;
            if (emptyNames is not null)
            {
                if (emptyNames.Length != Model.TypeParameters.Count)
                {
                    emptyNames = new String[Model.TypeParameters.Count];
                }

                for (var i = 0; i < emptyNames.Length; i++)
                {
                    emptyNames[i] = String.Empty;
                }
            }

            var typeParameters = RenderOpenGeneric
                ? emptyNames!
                : [..Model.TypeParameters];

            builder.Append($"<{List(typeParameters, static (p, _, _, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(p);
            }, separator: RenderOpenGeneric ? "," : ", ")}>");

            if (RenderNullable && Model.TypeKind is UnionTypeKind.Class)
            {
                builder.Append('?');
            }
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<String>.Shared.Return(rented);
            }
        }
    }

    public override String ToString()
    {
        var builder = new StringBuilder();

        builder.Append(Model.Name);

        if (Model.TypeParameters is [])
        {
            if (RenderNullable && Model.TypeKind is UnionTypeKind.Class)
            {
                builder.Append('?');
            }

            return builder.ToString();
        }

        var rented = RenderOpenGeneric
            ? ArrayPool<String>.Shared.Rent(Model.TypeParameters.Count)
            : null;
        try
        {
            var emptyNames = rented;
            if (emptyNames is not null)
            {
                if (emptyNames.Length != Model.TypeParameters.Count)
                {
                    emptyNames = new String[Model.TypeParameters.Count];
                }

                for (var i = 0; i < emptyNames.Length; i++)
                {
                    emptyNames[i] = String.Empty;
                }
            }

            var typeParameters = RenderOpenGeneric
                ? emptyNames!
                : [..Model.TypeParameters];

            builder.Append('<');

            for (var i = 0; i < typeParameters.Length; i++)
            {
                if (i is not 0)
                {
                    builder.Append(RenderOpenGeneric ? "," : ", ");
                }

                builder.Append(typeParameters[i]);
            }

            builder.Append('>');
            
            if (RenderNullable && Model.TypeKind is UnionTypeKind.Class)
            {
                builder.Append('?');
            }
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<String>.Shared.Return(rented);
            }
        }

        if (RenderNullable && Model.TypeKind is UnionTypeKind.Class)
        {
            builder.Append('?');
        }

        return builder.ToString();
    }
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    {:
        if(model.Flags.HasFlag(PropertyFlags.IsEnumerable))
        {
    :}
    foreach(var child in target.(:model.Name:))
    {
        cancellationToken.ThrowIfCancellationRequested();

        tmp = child.Accept(this, cancellationToken);
        result = Aggregate(result, tmp);
    }
    {:
        } else {
    :}
    tmp = target.(:model.Name:).Accept(this, cancellationToken);
    result = Aggregate(result, tmp);
    {:
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct GenericPropertyTraversalTemplate(PropertyModel model);

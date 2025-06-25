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

        child.Accept(this, cancellationToken);
    }
    {:
        } else {
    :}
    target.(:model.Name:).Accept(this, cancellationToken);
    {:
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct PropertyTraversalTemplate(PropertyModel model);

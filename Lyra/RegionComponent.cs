// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Represents a <c>#region</c>.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
/// <param name="Name">
/// The name of the region.
/// </param>
/// <param name="Body">
/// The body of the region.
/// </param>
/// <typeparam name="TBody">
/// The type of the body.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct RegionComponent<TBody>(String Name, TBody Body) : ICSharpSourceComponent
        where TBody : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        builder.DetentAll(out var indentations)
               .Indent(builder.Options.InitialIndentation)
               .AppendLine($"#region {Name}")
               .Indent(indentations.Skip(builder.Options.InitialIndentation.Length))
               .AppendLine()
               .AppendLine(Body)
               .AppendLine()
               .DetentAll(out indentations)
               .Indent(builder.Options.InitialIndentation)
               .Append("#endregion")
               .Indent(indentations.Skip(builder.Options.InitialIndentation.Length));
    }

    /// <inheritdoc />
    public Boolean Equals(RegionComponent<TBody> other)
    {
        if (other.Name != Name)
        {
            return false;
        }

        if (!EqualityComparer<TBody>.Default.Equals(other.Body, Body))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override Int32 GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(Name);
        hc.Add(Body);
        var result = hc.ToHashCode();

        return result;
    }
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template(
    """
    /// <summary>
    /// Default implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/>.
    /// </summary>
    public sealed partial record (:model.Templates().TypeNames.Immutable:)
        : (:model.Templates().FullyQualifiedTypeNames.Interface:)
    {
    {:
        foreach(var property in model.Properties)
        {
            (:Space4, new ImmutablePropertyTemplate(property):)
            (:'\n':)
        }
    :}}
    """), NonEquatable]
internal readonly partial struct ImmutableOptionsTemplate(OptionsModel model);

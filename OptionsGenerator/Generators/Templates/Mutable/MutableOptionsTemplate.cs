namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <summary>
    /// Provides a mutable implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/> for binding against configurations in the options pattern.
    /// </summary>
    internal sealed partial class (:model.Templates().TypeNames.Mutable:)
        : (:model.Templates().FullyQualifiedTypeNames.Interface:)
    {
    {:
        foreach(var property in model.Properties)
        {
            (:Space4, new MutablePropertyTemplate(property):)
            (:'\n':)
        }
    :}}
    """), NonEquatable]
internal readonly partial struct MutableOptionsTemplate(OptionsModel model);

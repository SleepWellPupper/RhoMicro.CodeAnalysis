namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        foreach(var attribute in model.Attributes)
        {
            (:attribute:)(:'\n':)
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct PropertyAnnotationsTemplate(PropertyModel model);

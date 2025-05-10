namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """

    (:new PropertyAnnotationsTemplate(model):)(:new LineDirectiveTemplate(model.Location, offset: 8 + model.Type.Length):)public (:model.Type:) (:model.Name:) { get; init; }
    (:new LineDefaultDirectiveTemplate(model.Location):)(:new DefaultExpressionTemplate(model):)

    """)]
[NonEquatable]
internal readonly partial struct PropertyTemplate(PropertyModel model);

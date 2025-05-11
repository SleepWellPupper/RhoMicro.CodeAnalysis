namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <inheritdoc/>
    (:new PropertyAnnotationsTemplate(model):)public (:model.Type:) (:model.Name:) { get; set; }(:new MutableDefaultExpressionTemplate(model):)
    """)]
[NonEquatable]
internal readonly partial struct MutablePropertyTemplate(PropertyModel model);

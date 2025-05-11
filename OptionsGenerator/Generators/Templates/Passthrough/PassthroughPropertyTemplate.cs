namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <inheritdoc/>
    public (:Property.Type:) (:Property.Name:) => options.(:source.ValueAccessorName:).(:Property.Name:);
    """
    ), NonEquatable]
internal readonly partial struct PassthroughPropertyTemplate(OptionsModel model, Int32 index, OptionTypeModel source)
{
    private PropertyModel Property => model.Properties[index];
}

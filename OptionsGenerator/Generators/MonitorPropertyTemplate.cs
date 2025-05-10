namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    (:new PropertyAnnotationsTemplate(model):)public (:model.Type:) (:model.Name:) => monitor.CurrentValue.(:model.Name:);

    """)]
[NonEquatable]
internal readonly partial struct MonitorPropertyTemplate(PropertyModel model);

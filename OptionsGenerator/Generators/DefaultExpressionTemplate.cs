namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        if(model.DefaultValueExpression.Length == 0)
        {
            if(model.IsOptions)
            {
    :} = (:model.Type:).Default;{:
            }

            return;
        }
    :}
    (:new LineDirectiveTemplate(model.DefaultValueExpressionLocation, offset: 3):)= (:model.DefaultValueExpression:);
    (:new LineDefaultDirectiveTemplate(model.DefaultValueExpressionLocation):)
    """)]
[NonEquatable]
internal readonly partial struct DefaultExpressionTemplate(PropertyModel model);

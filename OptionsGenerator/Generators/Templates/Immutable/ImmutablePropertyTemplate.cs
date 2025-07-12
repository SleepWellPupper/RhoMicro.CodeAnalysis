// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    /// <inheritdoc/>
    {:
        if(model.Location != LocationModel.Empty)
        {
            var l = model.Location;
            :}#line ((:l.StartLine.ToString():), (:l.StartCol.ToString():)) - ((:l.EndLine.ToString():), (:l.EndCol.ToString():)) (:(renderer.Indentation.Length + 9 + model.Type.Length).ToString():) "(:l.Path:)"{:
            (:'\n':)
        }
    :}
    public (:model.Type:) (:model.Name:) { get; init; }{:
        if(model.Location != LocationModel.Empty)
        {
            (:"\n#line default":)
        }
    :}
    {:
        if(model.DefaultValueExpression.Length == 0)
        {
            if(model.IsOptions)
            {
                (:'\n':)
                :} = (:model.Type:).Default;{:
            }

            return;
        }

        if(model.DefaultValueExpressionLocation != LocationModel.Empty)
        {
            var l = model.DefaultValueExpressionLocation;
            (:'\n':)
            :}#line ((:l.StartLine.ToString():), (:l.StartCol.ToString():)) - ((:l.EndLine.ToString():), (:l.EndCol.ToString():)) (:(renderer.Indentation.Length + 4).ToString():) "(:l.Path:)"{:
            (:'\n':)
        }
    :}
     = (:model.DefaultValueExpression:);{:
        if(model.DefaultValueExpressionLocation != LocationModel.Empty)
        {
            (:"\n#line default":)
        }
     :}
    """, RendererParameterName = "renderer"), NonEquatable]
internal readonly partial struct ImmutablePropertyTemplate(PropertyModel model);

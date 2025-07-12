// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        if(model.DefaultValueExpression.Length == 0)
        {
            if(model.IsOptions)
            {
    :}
     = (:model.Type:).Default;{:
            }

            return;
        }
    :}
     = (:model.DefaultValueExpression:);
    """)]
[NonEquatable]
internal readonly partial struct MutableDefaultExpressionTemplate(PropertyModel model);

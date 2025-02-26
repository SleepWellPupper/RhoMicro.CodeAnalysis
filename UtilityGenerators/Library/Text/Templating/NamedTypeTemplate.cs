namespace RhoMicro.CodeAnalysis.Library.Text.Templating;
using System;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

[Template(
    """
    {:
        if(model.NamespaceParts is { Count: > 0 and var length } parts)
        {
            (:"namespace ":)
            for(var i = 0; i < length; i++)
            {
                if(i > 0)
                    (:'.':)

                (:parts[i]:)
            }
            (:";\n\n":)
        }
    :}
    (:new ContainingType(model.ContainingTypes, 0):)
    <:partial (:model.Kind.Value:) (:model.Name:){:
        if(body is not EmptyTemplate)
        {
        :}

    {
    (:Space4, body:)
    }{:
        } else
        {
            (:';':)
        }
    :}:>
    """, BodyParameterName = "body")]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class NamedTypeTemplate(NamedTypeModel model)
{
    [Template(
        """
        {:
            if(index >= containingTypes.Count)
            {
                (:Body:)
                return;
            }

            var containingType = containingTypes[index];
        :}
        partial (:containingType.Kind.Value:) (:containingType.Name:)(:new TypeArguments(containingType.TypeArguments):)
        {
        (:new ContainingType(containingTypes, index+1):)
        <:(:Space4, Body:):>

        }
        """, BodyParameterName = "Body")]
    [NonEquatable]
    private readonly partial struct ContainingType(EquatableList<ContainingTypeSignatureModel> containingTypes, Int32 index);
    [Template(
        """
        {:
            if(typeArguments.Count == 0)
                return;

            (:'<':)

            for(var i = 0; i < typeArguments.Count; i++)
            {
                if(i != 0)
                    (:", ":)

                (:typeArguments[i].Name:)

            }
        
            (:'>':)
        :}
        """)]
    [NonEquatable]
    private readonly partial struct TypeArguments(EquatableList<TypeModel> typeArguments);
}

namespace RhoMicro.CodeAnalysis.Library.Text.Templating;
using System;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

[Template(
    """
    {:
    var body = new BodyTemplate(model);

    if(model.NamespaceParts is { Count: > 0 } parts)
        (:new NamespaceTemplate(parts), body:)
    else
        (:body:)
    :}
    """)]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal readonly partial struct NamedTypeTemplate(NamedTypeModel model)
{
    [Template(
        """
        namespace {:
        for(var i = 0; i < parts.Count; i++)
        {
            if(i > 0)
                (:'.':)
        
            (:parts[i]:)
        }
        :}

        {
            (:body:)
        }
        """, BodyParameterName = "body")]
    private readonly partial struct NamespaceTemplate(EquatableList<String> parts);
    [Template(
        """
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
    private sealed partial class BodyTemplate(NamedTypeModel model);
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

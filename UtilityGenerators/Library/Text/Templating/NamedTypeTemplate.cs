// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.Templating;
using System;

using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

[Template(
    """
    {:
    var bodyTemplate = new BodyTemplate<TBody>(model, baseList, comment, body);

    if(model.NamespaceParts is { Count: > 0 } parts)
        (:new NamespaceTemplate(parts), bodyTemplate:)
    else
        (:bodyTemplate:)
    :}
    """, BodyParameterName = "body", BodyParameterTypeName = "TBody")]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal readonly partial struct NamedTypeTemplate(NamedTypeModel model, EquatableList<String> baseList, DocsCommentTemplate comment)
{
    public NamedTypeTemplate(NamedTypeModel model) : this(model, [], DocsCommentTemplate.Create(String.Empty)) { }

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
        (:Space4, body:)
        }
        """, BodyParameterName = "body")]
    private readonly partial struct NamespaceTemplate(EquatableList<String> parts);
    [Template(
        """
        (:new ContainingType(model.ContainingTypes, 0):)
        <:(:comment:)(:Accessibility:)partial (:model.Kind.Value:) (:model.Name:){:
            if(baseList.Count > 0)
                :} : {:
        
            for(var i = 0; i < baseList.Count; i++)
            {
                if(i > 0)
                    :}, {:
        
                (:baseList[i]:)
            }
            
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
        """)]
    private sealed partial class BodyTemplate<TBody>(NamedTypeModel model, EquatableList<String> baseList, DocsCommentTemplate comment, TBody body)
        where TBody : ITemplate
    {
        private String Accessibility => model.Accessibility is { } a
            ? $"{SyntaxFacts.GetText(a)} "
            : String.Empty;
    }
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

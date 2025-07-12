// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    {:
        if(returnType is not null)
        {
            // visitors et al.
            (:returnType:)
        }
        else
        {
            // rewriter
            (:model.FullName():)
        }
    :} (:name:)(
        (:model.FullName():) target,
        global::System.Threading.CancellationToken cancellationToken = default){:

        if(isAbstract)
        {
            (:';':)
        }
    :}


    """)]
[NonEquatable]
internal readonly partial struct MethodSignatureTemplate(String returnType, MethodNameTemplate name, NodeModel model, Boolean isAbstract = false)
{
    public MethodSignatureTemplate(MethodNameTemplate name, NodeModel model, Boolean isAbstract = false)
        : this(null!, name, model, isAbstract) { }
}

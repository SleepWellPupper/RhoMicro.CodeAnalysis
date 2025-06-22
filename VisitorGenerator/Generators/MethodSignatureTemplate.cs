namespace RhoMicro.CodeAnalysis;

[Template(
    """
    (:returnType:) (:name:)(
        (:model.FullName():) target, 
        global::System.Threading.CancellationToken cancellationToken = default){:
    
        if(isAbstract)
        {
            (:';':)
        }
    :}


    """)]
[NonEquatable]
internal readonly partial struct MethodSignatureTemplate(String returnType, MethodNameTemplate name, NodeModel model, Boolean isAbstract = false);

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        if(model.Namespace is [{},..])
        {
    :}
    namespace (:model.Namespace:);

    {:
        }
    :}
    """), NonEquatable]
internal readonly partial struct RootNamespaceTemplate(OptionsModel model);
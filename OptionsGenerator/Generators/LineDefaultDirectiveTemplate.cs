namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        if(model.Path.Length == 0)
            return;
    :}
    #line default

    """, RendererParameterName = "renderer")]
[NonEquatable]
internal readonly partial struct LineDefaultDirectiveTemplate(LocationModel model);

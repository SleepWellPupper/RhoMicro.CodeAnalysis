namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        if(model.Path.Length == 0)
            return;
    :}
    #line ((:model.StartLine.ToString():), (:model.StartCol.ToString():)) - ((:model.EndLine.ToString():), (:model.EndCol.ToString():)) (:(renderer.Indentation.Length + 1 + offset).ToString():) "(:model.Path:)"

    """, RendererParameterName = "renderer")]
[NonEquatable]
internal readonly partial struct LineDirectiveTemplate(LocationModel model, Int32 offset = 0);

namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;

internal readonly record struct InitializationMethodParameterModel(String Type, String Name, String PropertyName)
{
    public static InitializationMethodParameterModel Create(IParameterSymbol parameter, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var type = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var name = parameter.Name;
        var propertyName = $"{Char.ToUpperInvariant(parameter.Name[0])}{parameter.Name[1..]}";

        var result = new InitializationMethodParameterModel(
            Type: type,
            Name: name,
            PropertyName: propertyName);

        return result;
    }
}

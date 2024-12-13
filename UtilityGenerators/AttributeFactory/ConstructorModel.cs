namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;

internal sealed record ConstructorModel(
    Int32 Index,
    IList<ParameterModel> Parameters,
    IList<ParameterMapping?> Mappings)
{
    public static ConstructorModel Create(
        IMethodSymbol ctor,
        Int32 index,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var parameters = ctx.CollectionFactory.CreateList<ParameterModel>();
        var mappings = ctx.CollectionFactory.CreateLazyList<ParameterMapping?>();

        for(var parameterIndex = 0; parameterIndex < ctor.Parameters.Length; parameterIndex++)
        {
            ctx.ThrowIfCancellationRequested();

            var parameter = ctor.Parameters[parameterIndex];

            var model = ParameterModel.Create(parameter, parameterIndex, in ctx);

            if(model is { MappedProperty: { } propertyName, Name: { } parameterName })
                mappings[parameterIndex] = new ParameterMapping(index, parameterIndex, ParameterName: parameterName, PropertyName: propertyName);

            parameters.Add(model);
        }

        var result = new ConstructorModel(index, parameters, mappings);

        return result;
    }
    public void AppendConstructorExpressionComment(in SourceBuildingContext ctx, Int32 highlightIndex = -1)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder.Comment
            .SeeCRef(ctx.DisplayString).AppendCore('(');

        for(var i = 0; i < Parameters.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            if(i > 0)
                ctx.SourceBuilder.AppendCore(", ");

            var param = Parameters[i];

            _ = ctx.SourceBuilder.Comment.SeeCRef(param.TypeDisplayString);

            if(param.IsArray)
                ctx.SourceBuilder.AppendCore("[]");

            ctx.SourceBuilder.AppendCore(' ');

            if((i == highlightIndex ||  highlightIndex == -1) && param.MappedProperty is { } mappedProperty )
                ctx.SourceBuilder.Comment.OpenEmphasis().Append(param.Name).Append("->").Append(mappedProperty).CloseBlockCore();
            else
                ctx.SourceBuilder.AppendCore(param.Name);
        }

        ctx.SourceBuilder.AppendCore(')');
    }
    public override String ToString() => $"({String.Join(", ", Parameters)})";
}

namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

/// <summary>
/// 
/// </summary>
/// <param name="Index"></param>
/// <param name="Parameters"></param>
/// <param name="Mappings">Maps property names onto parameter mappings related to this constructor.</param>
internal readonly record struct ConstructorModel(
    Int32 Index,
    EquatableList<ParameterModel> Parameters,
    LazyEquatableDictionary<String, ParameterMapping?> Mappings)
{
    public static ConstructorModel Create(
        IMethodSymbol ctor,
        Int32 index,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var mutabilityContext = new MutabilityContext();

        var parameters = ctx.CollectionFactory.CreateList<ParameterModel>(mutabilityContext);
        var mappings = ctx.CollectionFactory.CreateLazyDictionary<String, ParameterMapping?>(mutabilityContext);

        for(var parameterIndex = 0; parameterIndex < ctor.Parameters.Length; parameterIndex++)
        {
            ctx.ThrowIfCancellationRequested();

            var parameter = ctor.Parameters[parameterIndex];

            var model = ParameterModel.Create(parameter, parameterIndex, in ctx);

            if(model is { MappedProperty: { } propertyName, Name: { } parameterName })
                mappings[propertyName] = new ParameterMapping(index, parameterIndex, ParameterName: parameterName, PropertyName: propertyName);

            parameters.Add(model);
        }

        mutabilityContext.SetImmutable();

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

            _ = ctx.SourceBuilder.Comment.SeeCRef(param.Type.ElementDisplayString);

            if(param.Type.Kind.HasFlagsFast(AttributeParameterTypeKind.Array))
                ctx.SourceBuilder.AppendCore("[]");

            ctx.SourceBuilder.AppendCore(' ');

            if(( i == highlightIndex || highlightIndex == -1 ) && param.MappedProperty is { } mappedProperty)
                ctx.SourceBuilder.Comment.OpenEmphasis().Append(param.Name).Append("->").Append(mappedProperty).CloseBlockCore();
            else
                ctx.SourceBuilder.AppendCore(param.Name);
        }

        ctx.SourceBuilder.AppendCore(')');
    }
    public void AppendConstructorExpression(in SourceBuildingContext ctx, Int32 highlightIndex = -1)
    {
        ctx.ThrowIfCancellationRequested();

        ctx.SourceBuilder
            .Append(ctx.DisplayString)
            .AppendCore('(');

        for(var i = 0; i < Parameters.Count; i++)
        {
            ctx.ThrowIfCancellationRequested();

            if(i > 0)
                ctx.SourceBuilder.AppendCore(", ");

            var param = Parameters[i];

            _ = ctx.SourceBuilder.Append(param.Type.ElementDisplayString);

            if(param.Type.Kind.HasFlagsFast(AttributeParameterTypeKind.Array))
                ctx.SourceBuilder.AppendCore("[]");

            ctx.SourceBuilder.AppendCore(' ');

            if(( i == highlightIndex || highlightIndex == -1 ) && param.MappedProperty is not null)
                ctx.SourceBuilder.Append('<').Append(param.Name).AppendCore('>');
            else
                ctx.SourceBuilder.AppendCore(param.Name);
        }

        ctx.SourceBuilder.AppendCore(')');
    }
    public override String ToString() => $"({String.Join(", ", Parameters)})";
}

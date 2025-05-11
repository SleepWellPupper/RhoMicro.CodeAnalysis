namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

using System;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal sealed record PropertyModel(
    String Name,
    String Type,
    Boolean IsOptions,
    EquatableList<String> Attributes,
    LocationModel Location,
    LocationModel DefaultValueExpressionLocation,
    String DefaultValueExpression)
{
    public static Boolean TryCreate(
        ISymbol member,
        [NotNullWhen(true)] out PropertyModel? result,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(member is not IPropertySymbol
            {
                DeclaringSyntaxReferences: [{ } reference]
            } property)
        {
            result = null;
            return false;
        }

        var defaultValueExpression = String.Empty;
        var defaultValueExpressionLocation = LocationModel.Empty;
        var attributes = ctx.CollectionFactory.CreateList<String>();

        foreach(var attribute in property.GetAttributes())
        {
            ctx.ThrowIfCancellationRequested();

            if(attribute.IsExcludeFromOptionsAttribute())
            {
                result = null;
                return false;
            }

            if(attribute.TryGetDefaultValueExpressionAttributeModel(out var m, cancellationToken: ctx.CancellationToken))
            {
                defaultValueExpression = m.Expression;
                defaultValueExpressionLocation = attribute.ApplicationSyntaxReference?
                    .GetSyntax(ctx.CancellationToken) is AttributeSyntax
                {
                    ArgumentList.Arguments: [{ Expression: ( LiteralExpressionSyntax or InterpolatedStringExpressionSyntax ) and { } arg }]
                }
                    ? LocationModel.Create(arg.GetLocation(), ctx.CancellationToken) //TODO: implement value text span calculation
                    : LocationModel.Empty;
            } else
            {
                var annotation = GetAttributeAnnotation(attribute, ctx.CancellationToken);
                attributes.Add(annotation);
            }
        }

        var location = reference.GetSyntax(ctx.CancellationToken) is PropertyDeclarationSyntax
        {
            Identifier: { } identifier
        }
        ? LocationModel.Create(identifier.GetLocation(), ctx.CancellationToken)
        : LocationModel.Empty;
        var name = property.Name;
        var type = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var isOptions = property.Type.GetAttributes().Any(static a => a.IsOptionsAttribute());

        result = new PropertyModel(
            Name: name,
            Type: type,
            IsOptions: isOptions,
            Attributes: attributes,
            Location: location,
            DefaultValueExpression: defaultValueExpression,
            DefaultValueExpressionLocation: defaultValueExpressionLocation);

        return true;
    }

    private static String GetAttributeAnnotation(AttributeData data, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var resultBuilder = new StringBuilder("[");
        var type = data.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? String.Empty;
        _ = resultBuilder.Append(type).Append('(');

        var ctorParameters = data.AttributeConstructor?.Parameters ?? [];
        var ctorArguments = data.ConstructorArguments;

        for(var i = 0;
            i < data.ConstructorArguments.Length && ctorParameters.Length == ctorArguments.Length;
            i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
                _ = resultBuilder.Append(", ");

            var arg = ctorArguments[i];
            var param = ctorParameters[i];

            _ = resultBuilder
                .Append(param.Name)
                .Append(": ");

            appendFullyQualifiedCSharpString(arg);
        }

        var namedArguments = data.NamedArguments;

        for(var i = 0; i < namedArguments.Length; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(ctorArguments.Length > 0 || i != 0)
                _ = resultBuilder.Append(", ");

            var arg = namedArguments[i];

            _ = resultBuilder
                .Append(arg.Key)
                .Append(" = ");

            appendFullyQualifiedCSharpString(arg.Value);
        }

        var result = resultBuilder.Append(")]").ToString();

        return result;

        void appendFullyQualifiedCSharpString(TypedConstant constant)
        {
            ct.ThrowIfCancellationRequested();

            _ = constant switch
            {
                { IsNull: true } => resultBuilder.Append("null"),
                { Kind: TypedConstantKind.Array } => appendArray(),
                { Kind: TypedConstantKind.Enum } => resultBuilder
                    .Append('(')
                    .Append(constant.Type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    .Append(")(")
                    .Append(constant.Value)
                    .Append(')'),
                _ => constant.TryGetTypeValue(out var t)
                    ? resultBuilder
                        .Append("typeof(")
                        .Append(t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                        .Append(')')
                    : resultBuilder
                        .Append('(')
                        .Append(constant.Type?.ToDisplayString())
                        .Append(')')
                        .Append(SymbolDisplay.FormatPrimitive(
                            constant.Value ?? new(),
                            quoteStrings: true,
                            useHexadecimalNumbers: false))
            };

            StringBuilder appendArray()
            {
                ct.ThrowIfCancellationRequested();

                _ = resultBuilder.Append('[');

                for(var i = 0; i < constant.Values.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    if(i != 0)
                        _ = resultBuilder.Append(", ");

                    var element = constant.Values[i];
                    appendFullyQualifiedCSharpString(element);
                }

                _ = resultBuilder.Append(']');

                return resultBuilder;
            }
        }
    }
}

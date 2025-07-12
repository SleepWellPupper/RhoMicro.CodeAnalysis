// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record InitializationMethodModel(
    EquatableList<InitializationMethodParameterModel> Parameters,
    String Name,
    String StateTypeName,
    String StateTypeDisplayString,
    String? CancellationTokenParameterName)
{
    public static InitializationMethodModel Create(IMethodSymbol method, InitializationMethodAttribute.Model attributeModel, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var parameters = ctx.CollectionFactory.CreateList<InitializationMethodParameterModel>();
        var name = method.Name;
        var stateTypeName = attributeModel.StateTypeName ?? "InitializationState";
        var stateTypeDisplayString = $"{method.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{stateTypeName}";

        GetParameters(method, parameters, out var cancellationTokenParameterName, in ctx);

        var result = new InitializationMethodModel(
            parameters,
            Name: name,
            StateTypeName: stateTypeName,
            StateTypeDisplayString: stateTypeDisplayString,
            cancellationTokenParameterName);

        return result;
    }

    private static void GetParameters(IMethodSymbol method, IList<InitializationMethodParameterModel> parameters, out String? cancellationTokenParameterName, in ModelCreationContext ctx)
    {
        InitializationMethodParameterModel? cancellationTokenParameter = null;
        var cancellationTokenParameterIndex = -1;

        for(var i = 0; i < method.Parameters.Length; i++)
        {
            ctx.ThrowIfCancellationRequested();

            var parameter = method.Parameters[i];
            var model = InitializationMethodParameterModel.Create(parameter, in ctx);

            if(parameter.Type is
                {
                    Name: "CancellationToken",
                    ContainingNamespace:
                    {
                        Name: "Threading",
                        ContainingNamespace:
                        {
                            Name: "System",
                            ContainingNamespace:
                            {
                                IsGlobalNamespace: true
                            }
                        }
                    }
                })
            {
                if(cancellationTokenParameter.HasValue)
                    parameters.Insert(cancellationTokenParameterIndex, cancellationTokenParameter.Value);

                cancellationTokenParameter = model;
                cancellationTokenParameterIndex = i;
            } else
            {
                parameters.Add(model);
            }
        }

        cancellationTokenParameterName = cancellationTokenParameter?.Name;
    }
}

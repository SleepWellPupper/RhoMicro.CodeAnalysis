// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Represents an argument in an argument syntax.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct AttributeArgumentComponent : ICSharpSourceComponent
{
    private AttributeArgumentComponent(
            Char assignmentOperator,
            TypeNameComponent? type,
            ICSharpSourceComponent? expressionComponent,
            String? expression,
            String? name,
            String? member)
    {
        _type = type;
        _expressionComponent = expressionComponent;
        _expression = expression;
        _name = name;
        _assignmentOperator = assignmentOperator;
        _member = member;
    }

    private readonly TypeNameComponent? _type;
    private readonly ICSharpSourceComponent? _expressionComponent;
    private readonly String? _member;
    private readonly String? _expression;
    private readonly String? _name;
    private readonly Char _assignmentOperator;

    /// <summary>
    /// Creates a positional (constructor) argument of type <see cref="Type"/> using a <c>typeof()</c> expression.
    /// </summary>
    /// <param name="type">
    /// The type to append into the <c>typeof()</c> expression assigned to the parameter.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreatePositional(
            TypeNameComponent type,
            String? parameterName = null)
        => new(assignmentOperator: ':',
                type: type,
                expressionComponent: null,
                expression: null,
                name: parameterName,
                member: null);

    /// <summary>
    /// Creates a positional (constructor) argument using a <c>type.member</c> expression.
    /// </summary>
    /// <param name="type">
    /// The type whose member to use.
    /// </param>
    /// <param name="member">
    /// The member to use.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreatePositionalMemberAccess(
            TypeNameComponent type,
            String member,
            String? parameterName = null)
        => new(assignmentOperator: ':',
                type: type,
                expressionComponent: null,
                expression: null,
                name: parameterName,
                member: member);

    /// <summary>
    /// Creates a positional (constructor) argument.
    /// </summary>
    /// <param name="expressionComponent">
    /// The component representing the expression of the argument.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreatePositional(
            ICSharpSourceComponent expressionComponent,
            String? parameterName = null)
        => new(assignmentOperator: ':',
                type: null,
                expressionComponent: expressionComponent,
                expression: null,
                name: parameterName,
                member: null);

    /// <summary>
    /// Creates a positional (constructor) argument.
    /// </summary>
    /// <param name="expression">
    /// The expression to pass as the argument.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreatePositional(String expression, String? parameterName = null)
        => new(assignmentOperator: ':',
                type: null,
                expressionComponent: null,
                expression: expression,
                name: parameterName,
                member: null);

    /// <summary>
    /// Creates a named (property) argument of type <see cref="Type"/>.
    /// </summary>
    /// <param name="type">
    /// The type to append into the <c>typeof()</c> expression assigned to the parameter.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreateNamed(TypeNameComponent type, String propertyName)
        => new(assignmentOperator: '=',
                type: type,
                expressionComponent: null,
                expression: null,
                name: propertyName,
                member: null);

    /// <summary>
    /// Creates a named (property) argument using a <c>type.member</c> expression.
    /// </summary>
    /// <param name="type">
    /// The type whose member to use.
    /// </param>
    /// <param name="member">
    /// The member to use.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreateNamedMemberAccess(
            TypeNameComponent type,
            String member,
            String propertyName)
        => new(assignmentOperator: '=',
                type: type,
                expressionComponent: null,
                expression: null,
                name: propertyName,
                member: member);

    /// <summary>
    /// Creates a named (property) argument.
    /// </summary>
    /// <param name="expressionComponent">
    /// The component representing the expression of the argument.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreateNamed(ICSharpSourceComponent expressionComponent,
                                                         String propertyName)
        => new(assignmentOperator: '=',
                type: null,
                expressionComponent: expressionComponent,
                expression: null,
                name: propertyName,
                member: null);

    /// <summary>
    /// Creates a named (property) argument.
    /// </summary>
    /// <param name="expression">
    /// The expression to pass as the argument.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <returns>
    /// A new positional argument component.
    /// </returns>
    public static AttributeArgumentComponent CreateNamed(String expression, String propertyName)
        => new(assignmentOperator: '=',
                type: null,
                expressionComponent: null,
                expression: expression,
                name: propertyName,
                member: null);

    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_assignmentOperator == 0)
        {
            return;
        }

        if (_name is not null)
        {
            builder.Append($"{_name}");

            if (_assignmentOperator is '=')
            {
                builder.Append(" ");
            }

            builder.Append($"{_assignmentOperator} ");
        }

        if (_member is { } member && _type is { } type)
        {
            builder.Append($"{type}.{member}");
        }
        else if (_type is { } typeOfArgument)
        {
            builder.Append($"typeof({typeOfArgument})");
        }
        else if (_expressionComponent is not null)
        {
            builder.Append(_expressionComponent);
        }
        else if (_expression is not null)
        {
            builder.Append(_expression);
        }
    }
}

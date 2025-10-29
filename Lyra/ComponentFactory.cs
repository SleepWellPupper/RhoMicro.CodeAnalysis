// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Immutable;
using System.Threading;

/// <summary>
/// Creates <see cref="ICSharpSourceComponent"/> instances.
/// </summary>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal static partial class ComponentFactory
{
    /// <summary>
    /// Creates a new list component.
    /// </summary>
    /// <param name="elements">
    /// The elements to enumerate.
    /// </param>
    /// <param name="separator">
    /// The separator separating each element.
    /// </param>
    /// <param name="terminator">
    /// The terminator appended after the final element.
    /// </param>
    /// <returns>
    /// A new list component.
    /// </returns>
    public static ListComponent<String, String> List(
            ImmutableArray<String> elements,
            String separator = "",
            String terminator = "")
        => new(Elements: elements,
                Append: static (e, _, _, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    b.Append(e);
                },
                Separator: separator,
                Separate: static (s, _, _, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    b.Append(s);
                },
                Terminator: terminator);

    /// <summary>
    /// Creates a new list component.
    /// </summary>
    /// <param name="elements">
    /// The elements to enumerate.
    /// </param>
    /// <param name="append">
    /// The callback to invoke to append an element.
    /// </param>
    /// <param name="separator">
    /// The separator separating each element.
    /// </param>
    /// <param name="terminator">
    /// The terminator appended after the final element.
    /// </param>
    /// <returns>
    /// A new list component.
    /// </returns>
    public static ListComponent<TElement, String> List<TElement>(
            ImmutableArray<TElement> elements,
            Action<TElement, Int32, Int32, CSharpSourceBuilder, CancellationToken> append,
            String separator = "",
            String terminator = "")
        => new(Elements: elements,
                Append: append,
                Separator: separator,
                Separate: static (s, _, _, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    b.Append(s);
                },
                Terminator: terminator);

    /// <summary>
    /// Creates a new namespace component.
    /// </summary>
    /// <param name="namespace">
    /// The namespace to use in the component.
    /// </param>
    /// <param name="body">
    /// The body to append into the namespace block.
    /// </param>
    /// <typeparam name="TBody">
    /// The type of body to append into the namespace block.
    /// </typeparam>
    /// <returns>
    /// A new namespace component.
    /// </returns>
    public static NamespaceComponent<TBody> Namespace<TBody>(String @namespace, TBody body)
            where TBody : ICSharpSourceComponent
        => new(@namespace, body);

    /// <summary>
    /// Creates a new type name component.
    /// </summary>
    /// <param name="type">
    /// The type whose name to append.
    /// </param>
    /// <returns>
    /// A new type name component.
    /// </returns>
    public static TypeNameComponent TypeName(Type type) => new(type);

    /// <summary>
    /// Creates a new type name component.
    /// </summary>
    /// <param name="typeName">
    /// The name to append.
    /// </param>
    /// <returns>
    /// A new type name component.
    /// </returns>
    public static TypeNameComponent TypeName(String typeName) => new(typeName);

    /// <summary>
    /// Creates a new attribute argument component.
    /// </summary>
    /// <param name="type">
    /// The type to use as the appended <c>typeof()</c> expression.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter to assign.
    /// </param>
    /// <returns>
    /// A new attribute argument component.
    /// </returns>
    public static AttributeArgumentComponent PositionalAttributeArgument(
            TypeNameComponent type,
            String? parameterName = null)
        => AttributeArgumentComponent.CreatePositional(type, parameterName);

    /// <summary>
    /// Creates a new attribute argument component.
    /// </summary>
    /// <param name="expressionComponent">
    /// The component to append as the appended expression.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter to assign.
    /// </param>
    /// <returns>
    /// A new attribute argument component.
    /// </returns>
    public static AttributeArgumentComponent PositionalAttributeArgument(
            ICSharpSourceComponent expressionComponent,
            String? parameterName = null)
        => AttributeArgumentComponent.CreatePositional(expressionComponent, parameterName);

    /// <summary>
    /// Creates a new attribute argument component.
    /// </summary>
    /// <param name="expression">
    /// The expression to append as the appended expression.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter to assign.
    /// </param>
    /// <returns>
    /// A new attribute argument component.
    /// </returns>
    public static AttributeArgumentComponent PositionalAttributeArgument(
            String expression,
            String? parameterName = null)
        => AttributeArgumentComponent.CreatePositional(expression, parameterName);

    /// <summary>
    /// Creates a new attribute argument component.
    /// </summary>
    /// <param name="type">
    /// The type to use as the appended <c>typeof()</c> expression.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property to assign.
    /// </param>
    /// <returns>
    /// A new attribute argument component.
    /// </returns>
    public static AttributeArgumentComponent NamedAttributeArgument(
            TypeNameComponent type,
            String propertyName)
        => AttributeArgumentComponent.CreateNamed(type, propertyName);

    /// <summary>
    /// Creates a new attribute argument component.
    /// </summary>
    /// <param name="expressionComponent">
    /// The component to append as the appended expression.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property to assign.
    /// </param>
    /// <returns>
    /// A new attribute argument component.
    /// </returns>
    public static AttributeArgumentComponent NamedAttributeArgument(
            ICSharpSourceComponent expressionComponent,
            String propertyName)
        => AttributeArgumentComponent.CreateNamed(expressionComponent, propertyName);

    /// <summary>
    /// Creates a new attribute argument component.
    /// </summary>
    /// <param name="expression">
    /// The expression to append as the appended expression.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property to assign.
    /// </param>
    /// <returns>
    /// A new attribute argument component.
    /// </returns>
    public static AttributeArgumentComponent NamedAttributeArgument(
            String expression,
            String propertyName)
        => AttributeArgumentComponent.CreateNamed(expression, propertyName);

    /// <summary>
    /// Creates a new attribute component.
    /// </summary>
    /// <param name="type">
    /// The type of the attribute to append.
    /// </param>
    /// <param name="arguments">
    /// The list of arguments to append into the attribute.
    /// </param>
    /// <returns>
    /// A new attribute component.
    /// </returns>
    public static AttributeComponent Attribute(
            TypeNameComponent type,
            ImmutableArray<AttributeArgumentComponent> arguments)
        => new(type, arguments);

    /// <summary>
    /// Creates a new type component.
    /// </summary>
    /// <param name="modifiers">
    /// The modifiers to prepend to the name of the type.
    /// </param>
    /// <param name="name">
    /// The name of the type.
    /// </param>
    /// <param name="typeParameters">
    /// The type parameters of the type.
    /// </param>
    /// <param name="baseTypeList">
    /// The list of names of the types implemented by the type.
    /// </param>
    /// <param name="attributes">
    /// The list of attributes applied to the type.
    /// </param>
    /// <param name="body">
    /// The body to append to the type.
    /// </param>
    /// <typeparam name="TBody">
    /// The type of body to append to the type.
    /// </typeparam>
    /// <returns>
    /// A new type component.
    /// </returns>
    public static TypeComponent<TBody> Type<TBody>(
            String modifiers,
            String name,
            TBody body,
            ImmutableArray<String> typeParameters = default,
            ImmutableArray<TypeNameComponent> baseTypeList = default,
            ImmutableArray<AttributeComponent> attributes = default)
            where TBody : ICSharpSourceComponent
        => new(
                Modifiers: modifiers,
                Name: name,
                TypeParameters: typeParameters.IsDefault ? [] : typeParameters,
                BaseTypeList: baseTypeList.IsDefault ? [] : baseTypeList,
                Attributes: attributes.IsDefault ? [] : attributes,
                Body: body);

    /// <inheritdoc cref="Type{T}(String, String, T, ImmutableArray{String},ImmutableArray{TypeNameComponent},ImmutableArray{AttributeComponent})"/>
    public static TypeComponent<EmptyComponent> Type(
            String modifiers,
            String name,
            ImmutableArray<String> typeParameters = default,
            ImmutableArray<TypeNameComponent> baseTypeList = default,
            ImmutableArray<AttributeComponent> attributes = default)
        => Type(modifiers, name, EmptyComponent.Instance, typeParameters, baseTypeList, attributes);

    /// <summary>
    /// Creates a new strategy based component.
    /// </summary>
    /// <param name="append">
    /// The strategy to use when appending.
    /// </param>
    /// <returns>
    /// A new strategy component.
    /// </returns>
    public static StrategyComponent Create(Action<CSharpSourceBuilder, CancellationToken> append) => new(append);
}

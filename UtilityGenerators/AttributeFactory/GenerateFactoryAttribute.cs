namespace RhoMicro.CodeAnalysis;
using System;

using Microsoft.CodeAnalysis;

/// <summary>
/// Annotates attribute types for factory generation. Helper types and
/// extensions for working with the target type will be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS && !RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS || RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[GenerateFactory(GenerateModelTypeAsStruct = false)]
#endif
internal sealed partial class GenerateFactoryAttribute : Attribute
{
    /// <summary>
    /// The default value for <see cref="PropertyAccessorTypeName"/>.
    /// </summary>
    public const String DefaultPropertyAccessorTypeName = "PropertyAccessor";
    /// <summary>
    /// The default value for <see cref="ConstructorArgumentAccessorTypeName"/>.
    /// </summary>
    public const String DefaultConstructorArgumentAccessorTypeName = "ConstructorArgumentAccessor";
    /// <summary>
    /// The default value for <see cref="PropertyIdTypeName"/>.
    /// </summary>
    public const String DefaultPropertyIdTypeName = "PropertyId";
    /// <summary>
    /// The default value for <see cref="ModelTypeName"/>.
    /// </summary>
    public const String DefaultModelTypeName = "Model";
    /// <summary>
    /// The default value for <see cref="GenerateModelTypeAsStruct"/>.
    /// </summary>
    public const Boolean DefaultGenerateModelTypeAsStruct = false;
    /// <summary>
    /// The default value for <see cref="GenerateDefaultInstance"/>.
    /// </summary>
    public const Boolean DefaultGenerateDefaultInstance = true;

    /// <summary>
    /// Gets or sets the name of the generated type providing strongly typed
    /// access to individual target attribute properties from <see
    /// cref="AttributeData"/> instance. The default value is
    /// <c>PropertyAccessor</c>.
    /// </summary>
    [DefaultValue(DefaultPropertyAccessorTypeName)]
    public String PropertyAccessorTypeName { get; set; } = DefaultPropertyAccessorTypeName;
    /// <summary>
    /// The name of the generated type providing strongly typed access to
    /// individual target attribute properties mapped from constructor
    /// parameters from <see cref="AttributeData"/> instances. The default value
    /// is <c>ConstructorArgumentAccessor</c>.
    /// </summary>
    [DefaultValue(DefaultConstructorArgumentAccessorTypeName)]
    public String ConstructorArgumentAccessorTypeName { get; set; } = DefaultConstructorArgumentAccessorTypeName;
    /// <summary>
    /// Gets the name of the generated type providing strongly typed access to
    /// target attribute property ids. The default value is <c>PropertyId</c>.
    /// </summary>
    [DefaultValue(DefaultPropertyIdTypeName)]
    public String PropertyIdTypeName { get; set; } = DefaultPropertyIdTypeName;
    /// <summary>
    /// Gets the name of the generated type providing strongly typed access
    /// to all target attribute properties. The default value is <c>Model</c>.
    /// </summary>
    [DefaultValue(DefaultModelTypeName)]
    public String ModelTypeName { get; set; } = DefaultModelTypeName;
    /// <summary>
    /// Gets the name of the generated type providing extensions for attribute
    /// usage. If set to <see langword="null"/>, a name will be generated based
    /// using the name of the target attribute. The default value is <see
    /// langword="null"/>
    /// </summary>
    [DefaultValue((String?)null)]
    public String? ExtensionsTypeName { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to generate the model type as a
    /// <see langword="struct"/>. If set to <see langword="true"/>, a <see
    /// langword="readonly"/> <see langword="struct"/> will be generated.
    /// Otherwise, a <see langword="sealed"/> <see langword="class"/> will be
    /// generated. The default value is <see langword="false"/>.
    /// </summary>
    [DefaultValue(DefaultGenerateModelTypeAsStruct)]
    public Boolean GenerateModelTypeAsStruct { get; set; } = DefaultGenerateModelTypeAsStruct;
    /// <summary>
    /// Gets or sets a value indicating whether to generate a default instance
    /// on the generated model type The default value is <see langword="true"/>.
    /// </summary>
    [DefaultValue(DefaultGenerateDefaultInstance)]
    public Boolean GenerateDefaultInstance { get; set; } = DefaultGenerateDefaultInstance;
}

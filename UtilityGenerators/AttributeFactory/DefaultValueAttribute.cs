// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

/// <summary>
/// Defines the default value to use for the targeted property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS && !RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS || RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[GenerateFactory(GenerateModelTypeAsStruct = false)]
#endif
internal sealed partial class DefaultValueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Boolean defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Single defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Double defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(SByte defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Int16 defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Int32 defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Int64 defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Byte defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(UInt16 defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(UInt32 defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(UInt64 defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Single[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Double[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(SByte[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Int16[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Int32[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Int64[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(Byte[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(UInt16[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(UInt32[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(UInt64[]? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(String? defaultValue) => DefaultValue = defaultValue;
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to use for the targeted property.
    /// </param>
    public DefaultValueAttribute(String[]? defaultValue) => DefaultValue = defaultValue;

    /// <summary>
    /// Gets the default value to use for the targeted property.
    /// </summary>
    public Object? DefaultValue { get; }
}
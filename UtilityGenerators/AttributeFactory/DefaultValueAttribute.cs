namespace RhoMicro.CodeAnalysis;

using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Defines the default value to use for the targeted property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[NonEquatable]
[IncludeFile]
[GenerateFactory]
#endif
internal sealed partial class DefaultValueAttribute : Attribute
{
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
    [OverloadResolutionPriority(1)]
    public DefaultValueAttribute(Object? defaultValue) => DefaultValue = defaultValue;
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
    /// Gets the default value to use for the targeted property.
    /// </summary>
    public Object? DefaultValue { get; }
}
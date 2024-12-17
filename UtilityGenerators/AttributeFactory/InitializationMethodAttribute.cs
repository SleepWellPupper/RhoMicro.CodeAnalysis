namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Marks the targeted method to be invoked after a model was created. In order
/// to be considered by the generator, the method must be an instance method in
/// a model type generated for an attribute annotated with <see
/// cref="GenerateFactoryAttribute"/>. For each initialization method defined, a
/// dedicated factory method overload will be generated. Only one initialization
/// method will be called per factory invocation. There may not be multiple
/// initialization methods with an empty parameter list or a single parameter of
/// type <see cref="CancellationToken"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[NonEquatable]
[IncludeFile]
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
internal sealed partial class InitializationMethodAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the state type to wrap parameters when passing
    /// them around. The last parameter of type <see cref="CancellationToken"/>
    /// (if present) will not be represented via this state object, but instead
    /// be passed via the <see cref="CancellationToken"/> used by factory
    /// methods.
    /// </summary>
    public String? StateTypeName { get; set; }
}

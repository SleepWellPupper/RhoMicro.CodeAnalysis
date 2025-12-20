// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

using RhoMicro.CodeAnalysis.Library.Text.Templating;

[Template(
    """
    /// <summary>
    /// Provides a passthrough implementation of <see cref="(:model.Templates().FullyQualifiedTypeNames.Interface:)"/>, delegating all property access to an injected <see cref="(:type.FullyQualifiedName:){T}"/> instance.
    /// </summary>
    internal sealed partial class (:name:)
        ((:type.FullyQualifiedName:)<(:model.Templates().FullyQualifiedTypeNames.Mutable:)> options)
        : (:model.Templates().FullyQualifiedTypeNames.Interface:)
    {
    {:
        for(var i = 0; i < model.Properties.Count; i++)
        {
            (:Space4, new PassthroughPropertyTemplate(model, i, type):)
            (:'\n':)
        }
    :}}
    """), NonEquatable]
internal readonly partial struct PassthroughOptionsImplementationTemplate<T>(OptionsModel model, T name, OptionTypeModel type)
    where T : ITemplate;

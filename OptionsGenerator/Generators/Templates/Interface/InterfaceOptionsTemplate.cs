// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    partial interface (:model.Templates().TypeNames.Interface:)
    {
        /// <summary>
        /// Gets the default instance for this options interface.
        /// </summary>
        static (:model.Templates().FullyQualifiedTypeNames.Interface:) Default { get; } = new (:model.Templates().FullyQualifiedTypeNames.Mutable:)();
    }
    """), NonEquatable]
internal readonly partial struct InterfaceOptionsTemplate(OptionsModel model);

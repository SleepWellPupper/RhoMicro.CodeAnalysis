// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

#pragma warning disable CS9113

[Template(
    """
    {:
        if(model.Namespace is [{},..])
        {
    :}
    namespace (:model.Namespace:);

    {:
        }
    :}
    """), NonEquatable]
internal readonly partial struct RootNamespaceTemplate(OptionsModel model);

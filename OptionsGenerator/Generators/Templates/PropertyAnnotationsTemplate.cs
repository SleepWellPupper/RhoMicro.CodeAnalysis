// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

[Template(
    """
    {:
        foreach(var attribute in model.Attributes)
        {
            (:attribute:)(:'\n':)
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct PropertyAnnotationsTemplate(PropertyModel model);

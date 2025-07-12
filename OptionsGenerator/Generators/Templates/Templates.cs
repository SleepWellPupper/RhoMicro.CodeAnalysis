// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

internal readonly struct Templates(OptionsModel model)
{
    public RootTemplate Root => new(model);
    public TypeNameTemplates TypeNames => new(model);
    public FullyQualifiedTypeNameTemplates FullyQualifiedTypeNames => new(model);
}
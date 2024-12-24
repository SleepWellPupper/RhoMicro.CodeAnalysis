namespace RhoMicro.CodeAnalysis.Library.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal readonly record struct ContainingTypeModel(
    String Accessibility,
    String TypeModifier,
    String Name,
    EquatableList<String> TypeParameters) :
    IIndentedStringBuilderAppendable
{
    public static ContainingTypeModel Create(INamedTypeSymbol symbol, in ModelCreationContext ctx)
    {
        var typeParameters = ctx.CollectionFactory.CreateList<String>();

        foreach(var parameter in symbol.TypeParameters)
            typeParameters.Add(parameter.Name);

        var accessibility = SyntaxFacts.GetText(symbol.DeclaredAccessibility);
        var typeModifier = Utils.GetTypeModifiers(symbol);
        var name = symbol.Name;

        var result = new ContainingTypeModel(
            accessibility,
            typeModifier,
            name,
            typeParameters);

        return result;
    }

    public void AppendTo(Text.SourceTexts.IndentedStringBuilder builder)
    {
        _ = builder.Append(Accessibility)
            .Append(' ')
            .Append(TypeModifier)
            .Append(' ')
            .Append(Name);

        if(TypeParameters.Count > 0)
            _ = builder.OpenAngledBlock().AppendJoin(", ", TypeParameters).CloseBlock();
    }

    public Boolean Equals(ContainingTypeModel other) =>
        other.Accessibility == Accessibility &&
        other.TypeModifier == TypeModifier &&
        other.Name == Name &&
        other.TypeParameters.Count == TypeParameters.Count;
    public override Int32 GetHashCode() =>
        (Accessibility, TypeModifier, Name, TypeParameters.Count).GetHashCode();
}

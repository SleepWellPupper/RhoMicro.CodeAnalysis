#pragma warning disable  // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Library.Text.Templating;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

public partial class NamedTypeTemplateTests
{
    [Fact]
    public void RendersBaseList()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: ["RhoMicro", "Test"],
            Name: "Foo"),
            baseList: ["Bar", "IBaz"],
            comment: DocsCommentTemplate.Create("")).ToString();

        Assert.Equal(
            """
            namespace RhoMicro.Test
            {
                partial class Foo : Bar, IBaz;
            }
            """, actual);
    }
    [Theory]
    [InlineData(Accessibility.Private)]
    [InlineData(Accessibility.Public)]
    [InlineData(Accessibility.Protected)]
    [InlineData(Accessibility.Internal)]
    [InlineData(Accessibility.ProtectedAndInternal)]
    public void RendersAccessibility(Accessibility accessibility)
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: accessibility,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: [],
            Name: "Foo"),
            baseList: [],
            comment: DocsCommentTemplate.Create("")).ToString();

        Assert.Equal(
            $"""
            {SyntaxFacts.GetText(accessibility)} partial class Foo;
            """, actual);
    }
    [Fact]
    public void DoesNotRenderNullAccessibility()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: [],
            Name: "Foo"),
            baseList: [],
            comment: DocsCommentTemplate.Create("")).ToString();

        Assert.Equal(
            $"""
            partial class Foo;
            """, actual);
    }
    [Fact]
    public void RendersSinglelineSummaryComment()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: ["RhoMicro", "Test"],
            Name: "Foo"),
            baseList: [],
            comment: DocsCommentTemplate.Create("FooBar")).ToString();

        Assert.Equal(
            """
            namespace RhoMicro.Test
            {
                /// <summary>
                /// FooBar
                /// </summary>
                partial class Foo;
            }
            """, actual);
    }
    [Fact]
    public void RendersMultilineSummaryComment()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: ["RhoMicro", "Test"],
            Name: "Foo"),
            baseList: [],
            comment: DocsCommentTemplate.Create("Foo\nBar\nBaz")).ToString();

        Assert.Equal(
            """
            namespace RhoMicro.Test
            {
                /// <summary>
                /// Foo
                /// Bar
                /// Baz
                /// </summary>
                partial class Foo;
            }
            """, actual);
    }
    [Template("(:text:)")]
    readonly partial struct DisplayTemplate(String text);
    [Fact]
    public void RendersTypeIndentedInNamespace()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: ["RhoMicro", "Test"],
            Name: "Foo")).RenderToString(
                body: new DisplayTemplate("public static int Main() => 0;"));

        Assert.Equal(
            """
            namespace RhoMicro.Test
            {
                partial class Foo
                {
                    public static int Main() => 0;
                }
            }
            """, actual);
    }
    [Fact]
    public void RendersBlockScopedNamespace()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: ["RhoMicro", "Test"],
            Name: "Foo")).ToString();

        Assert.Equal(
            """
            namespace RhoMicro.Test
            {
                partial class Foo;
            }
            """, actual);
    }
    [Template("public static int Main() => 0;")]
    partial struct BodyTemplate;
    [Fact]
    public void RendersBody()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [],
            NamespaceParts: [],
            Name: "Foo")).RenderToString(new BodyTemplate());

        Assert.Equal(
            """
            partial class Foo
            {
                public static int Main() => 0;
            }
            """, actual);
    }
}

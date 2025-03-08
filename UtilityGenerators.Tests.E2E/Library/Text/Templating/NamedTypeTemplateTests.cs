#pragma warning disable  // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Library.Text.Templating;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

public partial class NamedTypeTemplateTests
{
    [Fact]
    public void RendersBlockScopedNamespace()
    {
        var actual = new NamedTypeTemplate(new NamedTypeModel(
            ContainingTypes: [],
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

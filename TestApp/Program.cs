using System.Diagnostics.Tracing;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Templating;

internal partial class Program
{
    [RhoMicro.CodeAnalysis.NonEquatable]
    public partial class Foo;

    private static void Main(String[] _0)
    {
        using var ctx = ModelCreationContext.CreateDefault(CancellationToken.None);
        var template = TemplateSyntaxModel.Parse(
"""
// \§(escaped hole)
// \\§(Name) (unescaped hole)
public §(Accessibility) class §(Name)
{§{
    foreach(var member in Members)
    {
        §(member)
    }
}}
""", in ctx);
    }
}

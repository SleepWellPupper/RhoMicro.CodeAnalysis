namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Library.Text.Templating;
using System;
using System.Collections.Generic;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Text.Templating;

public class TemplateRendererTests
{
    [Theory]
    [InlineData(
        new[] { "<div>\n" },
        new[] { "<h1>", "Greeting", ", ", "foo", "!</h1>\n" },
        new[] { "</div>" },
        "    ",
        """
        <div>
            <h1>Greeting, foo!</h1>
        </div>
        """)]
    public void IndentsCorrectly(String[] header, String[] body, String[] footer, String indentation, String expected)
    {
        // Arrange
        using var renderer = new TemplateRenderer(stackalloc Char[64], stackalloc Char[16]);

        // Act
        foreach(var h in header)
            renderer.Render(h);

        renderer.Indent(indentation);

        foreach(var b in body)
            renderer.Render(b);

        renderer.Detent(indentation.Length);

        foreach(var f in footer)
            renderer.Render(f);

        var actual = renderer.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void DoesNotPrependIndentationIfNotPrecededByNewline()
    {
        // Arrange
        using var renderer = new TemplateRenderer(stackalloc Char[64], stackalloc Char[16]);

        // Act
        renderer.Render("foo");
        renderer.Indent(" ");
        renderer.Render("bar\n");
        renderer.Detent(1);
        var actual = renderer.ToString();

        // Assert
        Assert.Equal("foobar\n", actual);
    }
    [Fact]
    public void PrependsIndentationIfEmpty()
    {
        // Arrange
        using var renderer = new TemplateRenderer(stackalloc Char[64], stackalloc Char[16]);

        // Act
        renderer.Indent(" ");
        renderer.Render("bar\n");
        renderer.Detent(1);
        var actual = renderer.ToString();

        // Assert
        Assert.Equal(" bar\n", actual);
    }
    [Fact]
    public void PrependsIndentationIfPrecededByNewline()
    {
        // Arrange
        using var renderer = new TemplateRenderer(stackalloc Char[64], stackalloc Char[16]);

        // Act
        renderer.Render("\n");
        renderer.Indent(" ");
        renderer.Render("foo");
        renderer.Detent(1);
        var actual = renderer.ToString();

        // Assert
        Assert.Equal("\n foo", actual);
    }
    [Fact]
    public void DoesNotPrependIndentationIfPrecededByNewlineAndEmpty()
    {
        // Arrange
        using var renderer = new TemplateRenderer(stackalloc Char[64], stackalloc Char[16]);

        // Act
        renderer.Indent(" ");
        renderer.Render("\n");
        renderer.Render("\n");
        renderer.Render("foo");
        renderer.Detent(1);
        var actual = renderer.ToString();

        // Assert
        Assert.Equal("\n\n foo", actual);
    }
}

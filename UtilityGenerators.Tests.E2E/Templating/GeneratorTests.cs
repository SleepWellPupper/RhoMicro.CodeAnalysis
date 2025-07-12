// SPDX-License-Identifier: MPL-2.0

#pragma warning disable  // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.E2E.Templating;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

using RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

public partial class GeneratorTests
{
    [Template("")]
    private sealed partial class EmptyTemplate;

    [Fact]
    public void EmptyTemplateImplementsTemplateType() => Assert.IsAssignableFrom<ITemplate>(new EmptyTemplate());
    [Fact]
    public void EmptyTemplateRendersEmptyString()
    {
        var template = new EmptyTemplate();
        var actual = TemplateRenderer.Render(template);

        Assert.Equal("", actual);
    }

    [Template("Hello, World!")]
    private sealed partial class SimpleTextTemplate;
    [Fact]
    public void SimpleTextTemplateRendersExpectedString()
    {
        var template = new SimpleTextTemplate();
        var actual = TemplateRenderer.Render(template);

        Assert.Equal("Hello, World!", actual);
    }
    [Template("(:Foo:)")]
    private sealed partial class SimpleValueTemplate
    {
        public SimpleValueTemplate(String foo) => Foo = foo;
        private String Foo { get; }
    }
    [Template("\n", Newline = Newline.CrLf)]
    private sealed partial class NewlineTemplate;
    [Fact]
    public void NewlineTemplateRenders()
    {
        var actual = new NewlineTemplate().ToString();
        Assert.Equal("\r\n", actual);
    }
    [Template("\r", Newline = Newline.Lf)]
    private sealed partial class CarriageReturnTemplate;
    [Fact]
    public void CarriageReturnTemplateRenders()
    {
        var actual = new CarriageReturnTemplate().ToString();
        Assert.Equal("\n", actual);
    }
    [Template("\r\n", Newline = Newline.Lf)]
    private partial record CarriageReturnCodeTextTemplate;
    [Fact]
    public void CarriageReturnCodeTextTemplateRendersExpectedString()
    {
        var template = new CarriageReturnCodeTextTemplate();
        var actual = TemplateRenderer.Render(template);

        Assert.Equal("\n", actual);
    }
    [Template(
        """
        Prefix
        {:
            for(var i = 0; i < count; i++)
                (:child:)
        :}

        Suffix
        """)]
    private partial class TextChildTemplateTextTemplate(SimpleValueTemplate child, Int32 count);
    [Theory]
    [InlineData("", 0)]                                       // Empty string with count 0
    [InlineData("Hello, World!", 1)]                          // Simple string with count 1
    [InlineData("Lorem ipsum dolor sit amet", 5)]             // Lorem ipsum with arbitrary count
    [InlineData("🚀🌟💻🎉", 4)]                              // Emoji string with count equal to emoji count
    [InlineData("This is a longer string to test", 7)]        // Longer string with word count
    [InlineData("1234567890", 10)]                            // Numeric string with character count
    [InlineData("Special characters: !@#$%^&*()", 8)]         // Special characters with arbitrary count
    [InlineData("Single space ", 1)]                          // String with trailing space and count
    [InlineData(" Leading space", 2)]                         // String with leading space and count
    [InlineData(" Leading space", 0)]                         // String with leading space and count 0
    [InlineData("Line\nBreak", 2)]                            // String with a newline character and count
    [InlineData("Tab\tCharacter", 2)]                         // String with a tab character and count
    [InlineData("NullChar\u0000Here", 3)]                     // String with null character in the middle and count
    [InlineData("こんにちは", 5)]                               // Japanese (Hello) with character count
    [InlineData("你好", 2)]                                    // Chinese (Hello) with character count
    [InlineData("안녕하세요", 5)]                               // Korean (Hello) with character count
    [InlineData("Привет", 6)]                                 // Russian (Hello) with character count
    [InlineData("مرحبا", 5)]                                  // Arabic (Hello) with character count
    [InlineData("String with an emoji 🤖 at the end", 7)]     // Mixed content with count
    [InlineData("Repeat: Repeat: Repeat: Repeat:", 4)]        // Repetitive string with repetition count
    public void TextChildTemplateTextTemplateRendersExpectedString(String value, Int32 count)
    {
        var template = new TextChildTemplateTextTemplate(new(value), count);
        var actual = TemplateRenderer.Render(template);

        Assert.Equal(
            $"""
            Prefix
            {String.Concat(Enumerable.Repeat(value, count))}
            Suffix
            """, actual);
    }

    [Template("Hello, (:Value:)!")]
    private readonly partial record struct StructValueTemplate(String Value);
    [Fact]
    public void StructTemplateImplementsTemplateType() => Assert.IsAssignableFrom<ITemplate>(new StructValueTemplate());
    [Fact]
    public void StructTemplateRendersExpectedString()
    {
        var template = new StructValueTemplate("Struct");
        var actual = TemplateRenderer.Render(template);

        Assert.Equal("Hello, Struct!", actual);
    }
    [Fact]
    public void ToStringRendersExpectedString() => Assert.Equal("Hello, String!", new StructValueTemplate("String").ToString());

    [Template("This (:Value:) should not be rendered via (:nameof(ToString):).", GenerateToString = false)]
    private readonly partial record struct CustomToStringTemplate(String Value)
    {
        public override String ToString() => "Custom ToString";
    }
    [Fact]
    public void UserImplementedToStringRendersExpectedString() => Assert.Equal("Custom ToString", new CustomToStringTemplate("String").ToString());

    [Template(
"""
Dear (:Salutation:),

{:
    foreach (var name in Names)
    {
        (:"  Hello ":)(:name:)(:", hope you are doing well!\n":)
    }
:}

It's always great to stay in touch with everyone!

Best regards,
(:Closing:)
""", GenerateDebugInfo = true)]
    private readonly partial record struct LetterTemplate(String Salutation, List<String> Names, String Closing);
    [Fact]
    public void LetterTemplateRendersExpectedString()
    {
        var actual = new LetterTemplate("Friends", ["Jeff", "Fred", "Tiffany"], "Your Controller").ToString();

        Assert.Equal(
            """
            Dear Friends,

              Hello Jeff, hope you are doing well!
              Hello Fred, hope you are doing well!
              Hello Tiffany, hope you are doing well!

            It's always great to stay in touch with everyone!

            Best regards,
            Your Controller
            """, actual);
    }

    [Template(
"""
{:
    (:Tab, lines:)
:}
""")]
    private partial class IndentationTemplate(String lines);

    [Fact]
    public void IndentationIsAppliedToEveryNewLine()
    {
        var actual = new IndentationTemplate(
            "Foo\nFoo\nFoo\nFoo\nFoo")
            .ToString();

        Assert.Equal("\tFoo\n\tFoo\n\tFoo\n\tFoo\n\tFoo", actual);
    }

    [Template(
        """
        public static void Main()
        {
            Console.WriteLine("Hello, World!");
            return;
        }
        """)]
    private partial class MainMethodTemplate;
    [Template(
        """
        public class (:name:)
        {
        (:Tab, body:)
        }
        """)]
    private partial class ClassTemplate(String name, ITemplate body);

    [Fact]
    public void IndentationIsAppliedToEveryNewLineOfChild()
    {
        var actual = new ClassTemplate("Program", new MainMethodTemplate()).ToString();

        Assert.Equal(
            $$"""
            public class Program
            {
            {{'\t'}}public static void Main()
            {{'\t'}}{
            {{'\t'}}    Console.WriteLine("Hello, World!");
            {{'\t'}}    return;
            {{'\t'}}}
            }
            """, actual);
    }

    [Template(
        """
        <div>
        (:Tab, Body:)
        </div>
        """, BodyParameterName = "Body")]
    partial class LayoutTemplate;

    [Template(
        """
        {:var foo = "foo";:}
        (:new BarTemplate():)(:'\n':)
        (:new LayoutTemplate():)
        <:
        <h1>(:Greeting:), (:foo:)!</h1>
        :>
        """, GenerateDebugInfo = true)]
    partial class HelloWorldInDivTemplate(String Greeting);
    [Template("Bar")]
    partial class BarTemplate;
    [Fact]
    public void BodyIsRendered()
    {
        var actual = new HelloWorldInDivTemplate("'Sup").ToString();
        Assert.Equal("Bar\n\n<div>\n\n\t<h1>'Sup, foo!</h1>\n\n</div>", actual);
    }
    [Template("(::")]
    partial class TopLevelEscapedOpenBlockTemplate;
    [Fact]
    public void RendersTopLevelEscapedOpenBlock()
    {
        var actual = new TopLevelEscapedOpenBlockTemplate().ToString();
        Assert.Equal("(:", actual);
    }
    [Template("::>")]
    partial class TopLevelEscapedCloseBlockTemplate;
    [Fact]
    public void RendersTopLevelEscapedCloseBlock()
    {
        var actual = new TopLevelEscapedCloseBlockTemplate().ToString();
        Assert.Equal(":>", actual);
    }
    [Template("<::}", GenerateDebugInfo = true)]
    partial class EmptyTopLevelBlockTemplate;
    [Fact]
    public void RendersEmptyTopLevelBlock()
    {
        var actual = new EmptyTopLevelBlockTemplate().ToString();
        Assert.Equal("<::}", actual);
    }
    [Template("<child>(:Body:)</child>", BodyParameterName = "Body")]
    partial class SubTemplateChild;
    [Template("<parent>{:static (:new SubTemplateChild():)<:content:>:}</parent>")]
    partial class SubTemplateParent;
    [Fact]
    public void SubTemplateRendersProvidedChild()
    {
        var actual = new SubTemplateParent().ToString();
        Assert.Equal("<parent><child>content</child></parent>", actual);
    }

    [Template("(:Salutation:) (:Name:).", BodyParameterName = "Name")]
    readonly partial record struct Header(String Salutation);
    [Template("(:Salutation:) (:Name:).", BodyParameterName = "Name")]
    readonly partial record struct Footer(String Salutation);
    [Template(
        """
        {:
            (:Header:)<:(:Name:):>
            (:" Why is your name ":)(:Name:)(:"? ":)
            static (:Footer:)<:user:>
        :}
        """)]
    partial record Letter(Header Header, Footer Footer, String Name);
    [Fact]
    public void LetterRendersExpectedString()
    {
        var actual = new Letter(
            new Header("Greetings,"),
            new Footer("Goodbye,"),
            "TeBeCo").ToString();

        Assert.Equal("Greetings, TeBeCo. Why is your name TeBeCo? Goodbye, user.", actual);
    }
}

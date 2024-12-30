#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.E2E.Templating;

using System.Reflection;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

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
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal("", actual);
    }

    [Template("Hello, World!")]
    private sealed partial class SimpleTextTemplate;
    [Fact]
    public void SimpleTextTemplateRendersExpectedString()
    {
        var template = new SimpleTextTemplate();
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal("Hello, World!", actual);
    }
    [Template("§(Foo)")]
    private sealed partial class SimpleValueTemplate
    {
        public SimpleValueTemplate(String foo) => Foo = foo;
        private String Foo { get; }
    }
    [Theory]
    [InlineData("")]                                                        // Empty string
    [InlineData("Hello, World!")]                                           // Classic phrase
    [InlineData("Lorem ipsum dolor sit amet")]                              // Lorem ipsum
    [InlineData("🚀🌟💻🎉")]                                               // Emojis
    [InlineData("This is a longer string to test")]                         // Longer string
    [InlineData("1234567890")]                                              // Numeric string
    [InlineData("Special characters: !@#$%^&*()_+-=[]{}|;:'\",.<>?/\\")]    // Special characters
    [InlineData("Single space ")]                                           // String with trailing space
    [InlineData(" Leading space")]                                          // String with leading space
    [InlineData("Line\nBreak")]                                             // String with a newline character
    [InlineData("Tab\tCharacter")]                                          // String with a tab character
    [InlineData("NullChar\u0000Here")]                                      // String with null character in the middle
    [InlineData("こんにちは")]                                                // Japanese (Hello)
    [InlineData("你好")]                                                     // Chinese (Hello)
    [InlineData("안녕하세요")]                                                // Korean (Hello)
    [InlineData("Привет")]                                                  // Russian (Hello)
    [InlineData("مرحبا")]                                                   // Arabic (Hello)
    [InlineData("String with an emoji 🤖 at the end")]                      // Mixed content
    [InlineData("Repeat: Repeat: Repeat: Repeat:")]                         // Repetitive string
    public void SimpleValueTemplateRendersExpectedString(String value)
    {
        var template = new SimpleValueTemplate(value);
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal(value, actual);
    }
    [Template(
        """
        First text
        §(Foo)
        Second Text
        """)]
    private partial class TextValueTextTemplate
    {
        public TextValueTextTemplate(String foo) => Foo = foo;

        private String Foo { get; }
    }
    [Fact]
    public void TextValueTextTemplateRendersExpectedString()
    {
        var template = new TextValueTextTemplate("FooBar");
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal(
            """
            First text
            FooBar
            Second Text
            """, actual);
    }
    [Template(
        """
        First text
        §{
            for(var i = 0; i < 5; i++)
                §(Foo)
        }
        Second Text
        """)]
    private partial class TextCodeTextTemplate
    {
        public TextCodeTextTemplate(String foo) => Foo = foo;

        private String Foo { get; }
    }
    [Fact]
    public void TextCodeTextTemplateRendersExpectedString()
    {
        var template = new TextCodeTextTemplate("FooBar");
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal(
            """
            First text
            FooBarFooBarFooBarFooBarFooBar
            Second Text
            """, actual);
    }
    [Template(
        """
        Prefix
        §{
            for(var i = 0; i < count; i++)
                §(child)
        }
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
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal(
            $"""
            Prefix
            {String.Concat(Enumerable.Repeat(value, count))}
            Suffix
            """, actual);
    }

    [Template("Hello, §(Value)!")]
    private readonly partial record struct StructValueTemplate(String Value);
    [Fact]
    public void StructTemplateImplementsTemplateType() => Assert.IsAssignableFrom<ITemplate>(new StructValueTemplate());
    [Fact]
    public void StructTemplateRendersExpectedString()
    {
        var buffer = new DynamicallyAllocatedBuffer<Char>();
        var template = new StructValueTemplate("Struct");
        template.Render(ref buffer);
        var actual = new String(buffer.Span);

        Assert.Equal("Hello, Struct!", actual);
    }
    [Fact]
    public void ToStringRendersExpectedString() => Assert.Equal("Hello, String!", new StructValueTemplate("String").ToString());

    [Template("This §(Value) should not be rendered via §(nameof(ToString)).", GenerateToString = false)]
    private readonly partial record struct CustomToStringTemplate(String Value)
    {
        public override String ToString() => "Custom ToString";
    }
    [Fact]
    public void UserImplementedToStringRendersExpectedString() => Assert.Equal("Custom ToString", new CustomToStringTemplate("String").ToString());

    [Template(
"""
Dear §(Salutation),

§{
    foreach (var name in Names)
    {
        §("  Hello ")§(name)§(", hope you are doing well!\n")
    }
}

It's always great to stay in touch with everyone!

Best regards,
§(Closing)
""")]
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
§{
    §(Tab, lines)
}
""")]
    private partial class IndentationTemplate(String lines);

    [Fact]
    public void IndentationIsAppliedToEveryNewLine()
    {
        var actual = new IndentationTemplate(
            """
            Foo
            Foo
            Foo
            Foo
            Foo
            """)
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
        public class §(name)
        {
        §(Tab, body)
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
    //[Template(
    //    """
    //    §{AssertInterned(__template);}
    //    """)]
    //private partial class InterningTemplate
    //{
    //    private static void AssertInterned(String template)
    //    {
    //        var attributeString = typeof(InterningTemplate).GetCustomAttribute<TemplateAttribute>()?.TemplateString;
    //        Assert.Same(attributeString, template);
    //        throw new InvalidOperationException("Marker Exception");
    //    }
    //}
    //[Fact]
    //public void GeneratedTemplateConstIsInterned()
    //{
    //    var ex = Assert.Throws<InvalidOperationException>(new InterningTemplate().ToString);
    //    Assert.Equal("Marker Exception", ex.Message);
    //}
}

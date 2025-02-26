namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Templating;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

public class TemplatingGeneratorTests : TestBase<TemplatingGenerator>
{
    [Theory(Timeout = 0)]
    [InlineData(
        "Header",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("(:Salutation:) (:Name:)", BodyParameterName = "Name")]
        readonly partial record struct Header(String Salutation);
        """")]
    [InlineData(
        "Footer",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("(:Salutation:) (:Name:)", BodyParameterName = "Name")]
        readonly partial record struct Footer(String Salutation);
        """")]
    [InlineData(
        "EmptyTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("")]
        sealed partial class EmptyTemplate;
        """")]
    [InlineData(
        "SimpleTextTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("Hello, World!")]
        sealed partial class SimpleTextTemplate;
        """")]
    [InlineData(
        "SimpleValueTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("(:Foo:)")]
        sealed partial class SimpleValueTemplate
        {
            public SimpleValueTemplate(String foo) => Foo = foo;
            String Foo { get; }
        }
        """")]
    [InlineData(
        "TextValueTextTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
            First text
            (:Foo:)
            Second Text
            """)]
        partial class TextValueTextTemplate
        {
            public TextValueTextTemplate(String foo) => Foo = foo;

            String Foo { get; }
        }
        """")]
    [InlineData(
        "TextCodeTextTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
            First text
            {:
                for(var i = 0; i < 5; i++)
                    (:Foo:)
            :}

            Second Text
            """)]
        partial record TextCodeTextTemplate(String Foo);
        """")]
    [InlineData(
        "TextChildTemplateTextTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
            Prefix
            {:
                for(var i = 0; i < count; i++)
                    (:child:)
            :}
            Suffix
            """)]
        partial class TextChildTemplateTextTemplate(SimpleValueTemplate child, Int32 count);
        [Template("(:Foo:)")]
        sealed partial class SimpleValueTemplate
        {
            public SimpleValueTemplate(String foo) => Foo = foo;
            String Foo { get; }
        }
        """")]
    [InlineData(
        "StructValueTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("Hello, (:Value:)!")]
        readonly partial record struct StructValueTemplate(String Value);
        """")]
    [InlineData(
        "CustomToStringTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("This (:Value:) should not be rendered via (:nameof(ToString):).", GenerateToString = false)]
        readonly partial record struct CustomToStringTemplate(String Value)
        {
            public override String ToString() => "Custom ToString";
        }
        """")]
    [InlineData(
        "LetterTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
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
        """)]
        readonly partial record struct LetterTemplate(String Salutation, List<String> Names, String Closing);
        """")]
    [InlineData(
        "IndentationTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
        """
        {:
            (:Tab, lines:)
        :}
        """)]
        partial class IndentationTemplate(String lines);
        """")]
    [InlineData(
        "MainMethodTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
            public static void Main()
            {
                Console.WriteLine("Hello, World!");
                return;
            }
            """, GenerateDebugInfo = true, Newline = Newline.Newline)]
        partial class MainMethodTemplate;
        """")]
    [InlineData(
        "ClassTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
                public class (:name:)
                {
                (:Tab, body:)
                }
                """)]
        partial class ClassTemplate(String name, ITemplate body);
        """")]
    [InlineData(
        "LayoutTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
                <div>
                (:Tab, Body:)
                </div>
                """, BodyParameterName = "Body")]
        partial class LayoutTemplate;
        """")]
    [InlineData(
        "HelloWorldInDivTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
            {:var foo = "foo";:}
            (:new BarTemplate():)(:'\n':)
            (:new LayoutTemplate():)
            <:
            <h1>(:Greeting:), (:foo:)!</h1>
            :>
            """)]
        partial class HelloWorldInDivTemplate(String Greeting);
        [Template("Bar")]
        partial class BarTemplate;
        [Template(
            """
                <div>
                (:Tab, Body:)
                </div>
                """, BodyParameterName = "Body")]
        partial class LayoutTemplate;
        """")]
    [InlineData(
        "BarTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("Bar")]
        partial class BarTemplate;
        """")]
    [InlineData(
        "TopLevelEscapedOpenBlockTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("(::")]
        partial class TopLevelEscapedOpenBlockTemplate;
        """")]
    [InlineData(
        "TopLevelEscapedCloseBlockTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("::>")]
        partial class TopLevelEscapedCloseBlockTemplate;
        """")]
    [InlineData(
        "EmptyTopLevelBlockTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("<::}")]
        partial class EmptyTopLevelBlockTemplate;
        """")]
    [InlineData(
        "SubTemplateChild",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("<child>(:Body:)</child>", BodyParameterName = "Body")]
        partial class SubTemplateChild;
        """")]
    [InlineData(
        "SubTemplateParent",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("<parent>{:static (:new SubTemplateChild():)<:content:>:}</parent>")]
        partial class SubTemplateParent;
        [Template("<child>(:Body:)</child>", BodyParameterName = "Body")]
        partial class SubTemplateChild;
        """")]
    [InlineData(
        "Header",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("(:Salutation:) (:Name:).", BodyParameterName = "Name")]
        readonly partial record struct Header(String Salutation);
        """")]
    [InlineData(
        "Footer",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("(:Salutation:) (:Name:).", BodyParameterName = "Name")]
        readonly partial record struct Footer(String Salutation);
        """")]
    [InlineData(
        "Letter",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template(
            """
            {:
                (:Header:)<:(:Name:):>
                (:" Why is your name ":)(:Name:)(:"? ":)
                static (:Footer:)<:user:>
            :}
            """)]
        partial record Letter(Header Header, Footer Footer, String Name);
        [Template("(:Salutation:) (:Name:)", BodyParameterName = "Name")]
        readonly partial record struct Header(String Salutation);
        [Template("(:Salutation:) (:Name:)", BodyParameterName = "Name")]
        readonly partial record struct Footer(String Salutation);
        """")]
    [InlineData(
        "CarriageReturnNewlineCodeTextTemplate",
        """"
        #pragma warning disable
        using RhoMicro.CodeAnalysis;
        using RhoMicro.CodeAnalysis.Library.Text.Templating;
        [Template("{: :}\r\n\r\nSecond Text", Newline = Newline.Newline)]
        partial record CarriageReturnNewlineCodeTextTemplate;
        """")]
    public void CompilesTemplate(String name, String template)
    {
        using var cts = new CancellationTokenSource(Debugger.IsAttached ? -1 : 2500);

        try
        {
            TestFactory(template, "", cts.Token);
        } catch(OperationCanceledException ex) when(cts.IsCancellationRequested)
        {
            Assert.Fail($"Compilation of {name} timed out at:\n{ex.StackTrace}\nfor:\n{template}");
        }
    }
}

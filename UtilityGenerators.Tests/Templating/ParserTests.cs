#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Templating;

using System.Collections.Immutable;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

using Xunit.Sdk;

public class ParserTests
{
    public static TheoryData<String, String, Object> Data
    {
        get
        {
            var result = new TheoryData<String, String, Object>();

            addSyntaxData(
                "text",
                "Hello, World",
                s => [new TextSyntaxModel(s)]);
            addSyntaxData(
                "value",
                "§(Foo)",
                s => [new ValueSyntaxModel(s)]);
            addSyntaxData(
                "code",
                "§{Foo}",
                s => [new CodeSyntaxModel(s, [new TextSyntaxModel(new(2, 3, s))])]);
            addSyntaxData(
                "value((text))",
                "§((Foo))",
                s => [new ValueSyntaxModel(new(0, 8, s))]);
            addSyntaxData(
                "code(value)",
                "§{§(Foo)}",
                s => [new CodeSyntaxModel(s, [new ValueSyntaxModel(new(2, 6, s))])]);
            addSyntaxData(
                "code(text value)",
                "§{Hello§(Foo)}",
                s => [new CodeSyntaxModel(s, [new TextSyntaxModel(new(2, 5, s)), new ValueSyntaxModel(new(7, 6, s))])]);
            addSyntaxData(
                "code(value text)",
                "§{§(Foo)Hello}",
                s => [new CodeSyntaxModel(s, [new ValueSyntaxModel(new(2, 6, s)), new TextSyntaxModel(new(8, 5, s))])]);
            addSyntaxData(
                "code({{text}})",
                "§{{{Hello}}}",
                s => [new CodeSyntaxModel(s, [new TextSyntaxModel(new(2, 9, s))])]);
            addSyntaxData(
                "code({text {value} text})",
                "§{foreach(var a in l){§(a)}}",
                s => [new CodeSyntaxModel(s, [new TextSyntaxModel(new(2, 20, s)), new ValueSyntaxModel(new(22, 4, s)), new TextSyntaxModel(new(26, 1, s))])]);
            addSyntaxData(
                "text value",
                "Hello, §(Foo)",
                s => [new TextSyntaxModel(new(0, 7, s)), new ValueSyntaxModel(new(7, 6, s))]);
            addSyntaxData(
                "esc value",
                "\\§(Foo)",
                s => [new TextSyntaxModel(new(1, 6, s))]);
            addSyntaxData(
                "esc esc value",
                "\\\\§(Foo)",
                s => [new TextSyntaxModel(new(1, 1, s)), new ValueSyntaxModel(new(2, 6, s))]);
            addSyntaxData(
                "value esc value",
                "§(Bar)\\§(Foo)",
                s => [new ValueSyntaxModel(new(0, 6, s)), new TextSyntaxModel(new(7, 6, s))]);
            addSyntaxData(
                "value esc esc value",
                "§(Bar)\\\\§(Foo)",
                s => [new ValueSyntaxModel(new(0, 6, s)), new TextSyntaxModel(new(7, 1, s)), new ValueSyntaxModel(new(8, 6, s))]);
            addSyntaxData(
                "text esc value",
                "Bar\\§(Foo)",
                s => [new TextSyntaxModel(new(0, 3, s)), new TextSyntaxModel(new(4, 6, s))]);
            addSyntaxData(
                "text esc esc value",
                "Bar\\\\§(Foo)",
                s => [new TextSyntaxModel(new(0, 3, s)), new TextSyntaxModel(new(4, 1, s)), new ValueSyntaxModel(new(5, 6, s))]);
            addSyntaxData(
                "text value text",
                "Hello, §(Foo)!",
                s => [new TextSyntaxModel(new(0, 7, s)), new ValueSyntaxModel(new(7, 6, s)), new TextSyntaxModel(new(13, 1, s))]);
            addSyntaxData(
                "code(text) text value text",
                "§{Hello}, §(Foo)!",
                s => [
                    new CodeSyntaxModel(new(0, 8, s), [new TextSyntaxModel(new(2, 5, s))]),
                    new TextSyntaxModel(new(8, 2, s)),
                    new ValueSyntaxModel(new(10, 6, s)),
                    new TextSyntaxModel(new(16, 1, s))]);
            addSyntaxData(
                "code(text) text value text",
                "§{using(disposable){Console.WriteLine(disposable.Value);}}, §(Foo)!",
                s => [
                    new CodeSyntaxModel(new(0, 58, s), [new TextSyntaxModel(new(2, 55, s))]),
                    new TextSyntaxModel(new(58, 2, s)),
                    new ValueSyntaxModel(new(60, 6, s)),
                    new TextSyntaxModel(new(66, 1, s))]);
            addSyntaxData(
                "text NL text",
                "Hello,\n World",
                s => [new TextSyntaxModel(s)]);
            addSyntaxData(
                "rsl text NL text",
                """
                Hello, 
                World
                """,
                s => [new TextSyntaxModel(s)]);
            addSyntaxData(
                "complex sample",
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
""",
                s => [
                    new TextSyntaxModel(new(0,3, s)),
                    new TextSyntaxModel(new(4,19, s)),
                    new TextSyntaxModel(new(24,1, s)),
                    new ValueSyntaxModel(new(25,7,s)),
                    new TextSyntaxModel(new(32,25, s)),
                    new ValueSyntaxModel(new(57,16,s)),
                    new TextSyntaxModel(new(73,7, s)),
                    new ValueSyntaxModel(new(80,7, s)),
                    new TextSyntaxModel(new(87,2, s)),
                    new CodeSyntaxModel(new(89,69,s),[new TextSyntaxModel(new(91,50,s)), new ValueSyntaxModel(new(141, 9,s)), new TextSyntaxModel(new(150,7,s))]),
                    new TextSyntaxModel(new(158,1,s))
                    ]);

            return result;

            void addSyntaxData(String description, String source, Func<String, EquatableList<TemplateChildSyntaxModel>> childrenFactory) =>
                result.Add(description, source, new TemplateSyntaxModel(source, childrenFactory.Invoke(source)));
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ParsesExpectedTemplateSyntax(String description, String source, Object expected)
    {
        TemplateSyntaxModel actual;
        using(var ctx = ModelCreationContext.CreateDefault(default))
        {
            actual = TemplateSyntaxModel.Parse(source, in ctx);
        }

        try
        {
            AssertEqual((TemplateSyntaxModel)expected, actual);
        } catch(XunitException ex)
        {
            Assert.Fail($"{description}\n{ex}");
        }
    }

    [Fact]
    public void SourceSpanHasExpectedSourceValue()
    {
        var source = "Hello, World!";
        using var ctx = ModelCreationContext.CreateDefault(default);
        var sourceSpan = TemplateSyntaxModel.Parse(source, in ctx).Source;

        Assert.Equal(source.Length, sourceSpan.Length);
        Assert.Equal(0, sourceSpan.Start);
        Assert.Equal(source, sourceSpan.Text);
        Assert.Equal(source, sourceSpan.AsSpan);
    }

    private static void AssertEqual(TemplateSyntaxBaseModel expected, TemplateSyntaxBaseModel actual)
    {
        if(expected.GetType() != actual.GetType())
            fail("type mismatch");

        switch(expected)
        {
            case TemplateSyntaxModel expectedTemplate:
                var actualTemplate = (TemplateSyntaxModel)actual;
                if(expectedTemplate.Children.Count != actualTemplate.Children.Count)
                    fail($"template children count mismatch: expected {expectedTemplate.Children.Count}, actual was {actualTemplate.Children.Count}");

                for(var i = 0; i < expectedTemplate.Children.Count; i++)
                    AssertEqual(expectedTemplate.Children[i], actualTemplate.Children[i]);

                break;
            case CodeSyntaxModel expectedCode:
                var actualCode = (CodeSyntaxModel)actual;
                if(expectedCode.Children.Count != actualCode.Children.Count)
                    fail($"code children count mismatch: expected {expectedCode.Children.Count}, actual was {actualCode.Children.Count}");

                for(var i = 0; i < expectedCode.Children.Count; i++)
                    AssertEqual(expectedCode.Children[i], actualCode.Children[i]);

                break;
            default:
                if(!expected.Equals(actual))
                    fail("syntax mismatch");

                break;
        }

        void fail(String message) =>
            Assert.Fail($"{message}\nexpected:\n{expected.ToDebugString()}\nactual:\n{actual.ToDebugString()}");
    }
}

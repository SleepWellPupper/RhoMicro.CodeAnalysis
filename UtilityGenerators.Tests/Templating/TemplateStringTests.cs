#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Templating;

public class TemplateStringTests
{
    [Theory]
    [InlineData(
        """
        "foo"
        """, 0, 1, false)]
    [InlineData(
        """
        @"foo"
        """, 0, 2, false)]
    [InlineData(
        """"
        """foo"""
        """", 0, 3, false)]
    [InlineData(
        """""
        """"foo""""
        """"", 0, 4, false)]
    [InlineData(
        """""
        """"
        foo
        """"
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
        foo
        """
        """"", 1, 0, true)]
    [InlineData(
        """""
        """"
        foo
        bar
        """"
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
        foo
        bar
        """
        """"", 1, 0, true)]
    [InlineData(
        """""
        """"
           foo
           """"
        """"", 1, 3, true)]
    [InlineData(
        """""
        """
           foo
           """
        """"", 1, 3, true)]
    [InlineData(
        """""
        """"
           foo
           bar
           """"
        """"", 1, 3, true)]
    [InlineData(
        """""
        """
           foo
           bar
           """
        """"", 1, 3, true)]
    [InlineData(
        """
        "bar"
        """, 0, 1, false)]
    [InlineData(
        """
        @"bar"
        """, 0, 2, false)]
    [InlineData(
        """"
        """bar"""
        """", 0, 3, false)]
    [InlineData(
        """""
        """"bar""""
        """"", 0, 4, false)]
    [InlineData(
        """""
        """"
        bar
        """"
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
        bar
        """
        """"", 1, 0, true)]
    [InlineData(
        """""
        """"
        bar
        baz
        """"
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
        bar
        baz
        """
        """"", 1, 0, true)]
    [InlineData(
        """""
        """"
           bar
           """"
        """"", 1, 3, true)]
    [InlineData(
        """""
        """
           bar
           """
        """"", 1, 3, true)]
    [InlineData(
        """""
        """"
           bar
           baz
           """"
        """"", 1, 3, true)]
    [InlineData(
        """""
        """
           bar
           baz
           """
        """"", 1, 3, true)]
    [InlineData(
        """
        ""
        """, 0, 1, false)]
    [InlineData(
        """""
        @""""
        """"", 0, 2, false)]
    [InlineData(
        """""
        """"
        
        """"
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
        
        """
        """"", 1, 0, true)]
    [InlineData(
        """
        "This is a longer string literal with more varied content."
        """, 0, 1, false)]
    [InlineData(
        """
        @"This is a longer verbatim string literal\nwith line breaks."
        """, 0, 2, false)]
    [InlineData(
        """"
        """
        This is a raw 
        string literal with
        multiple lines.
        """
        """", 1, 0, true)]
    [InlineData(
        """""
        """"
             This is a raw string literal
             with more indentation and 
             line breaks
             """"
        """"", 1, 5, true)]
    [InlineData(
        """""
        """"
        This is a multiline raw string literal
        with no indentation.
        """"
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
         This is a multiline raw string literal 
         with varied indentation.
         """
        """"", 1, 1, true)]
    [InlineData(
        """""
        """
            This string literal
            has inconsistent indentation
               across lines.
          """
        """"", 1, 2, true)]
    [InlineData(
        """""
        """
            This is a multiline string literal
                with nested indentation
            and varying levels.
           """
        """"", 1, 3, true)]
    [InlineData(
        """""
        """
            Line1
                Line2
                    Line3
          """
        """"", 1, 2, true)]
    [InlineData(
        """""
        """"
              Line1
              Line2
                Line3
            """"
        """"", 1, 4, true)]
    [InlineData(
        """""
        """"
            {
                "key1": "value1",
                "key2": {
                    "nestedKey": "nestedValue"
                }
                }   
            """"
        """"", 1, 4, true)]
    [InlineData(
        """""
        """
        This is a string with special characters: !@#$%^&*()_+[]{}|;:',.<>?/~`
        """
        """"", 1, 0, true)]
    [InlineData(
        """""""
        @"Special characters in verbatim: \n\t\r\\""""""."
        """"""", 0, 2, false)]
    [InlineData(
        """"
        """
            Combining
            raw strings
                and nested content
            """
        """", 1, 4, true)]
    [InlineData(
        """""
        """
        Here's a paragraph of text:

            This text is indented,
                but this line is more indented.
        """
        """"", 1, 0, true)]
    [InlineData(
        """""
        """
        Here are some numbers:
            12345
            67890
        """
        """"", 1, 0, true)]
    [InlineData(
        """"
        foo
        bar
            [Template(
                """
                First text
                ((Foo))
                Second Text
                """)]
        """", 4, 8, true)]
    [InlineData(
        """"
        """
        foo
        {{bar}}    
        """
        """", 1, 0, true)]
    [InlineData(
        """"
        """
        foo
        {{
        bar
        }}    
        """
        """", 1, 0, true)]
    public void SourceTextIsCorrectlyCreated(String sourceText, Int32 startLine, Int32 startCharacter, Boolean isMultiline)
    {
        // Arrange
        var token = CSharpSyntaxTree
            .ParseText(sourceText)
            .GetRoot()
            .DescendantTokens(_ => true)
            .Single(t => t.RawKind is
                (Int32)SyntaxKind.StringLiteralToken or
                (Int32)SyntaxKind.MultiLineRawStringLiteralToken or
                (Int32)SyntaxKind.SingleLineRawStringLiteralToken);

        // Act
        var (actualSourceText, path, actualStart, actualIsMultiline) = TemplateString.Create(token, CancellationToken.None);

        // Assert
        Assert.Equal(token.ValueText, actualSourceText);
        Assert.Equal(new SourcePosition(startLine, startCharacter), actualStart);
        Assert.Equal(isMultiline, actualIsMultiline);
        Assert.Empty(path);
    }
}

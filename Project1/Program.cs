// SPDX-License-Identifier: MPL-2.0

using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

internal partial class Program
{
    private static void Main(String[] _)
    {
        Console.WriteLine(
            new Letter(
                new Header("Greetings,"),
                new Footer("Sincerely,"),
                "TebBeCo"));
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

        get some rizz like fr
        """)]
    partial record Letter(Header Header, Footer Footer, String Name);
}
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

internal partial class Program
{
    public static Regex MyPattern = MyRegex();
    private static void Main(String[] _)
    {
        // Original and Modified text
        var oldText = "This is the original text.\nIt has multiple lines.\nThis is the final line.";
        var newText = "This is the updated text.\nIt has several lines.\nThis is the final line.\nAnd an extra line.";

        // Create a diff builder and calculate the diff

        // Generate side-by-side diff as a string
        var result = GetSideBySideDiff(oldText, newText);

        // Display or use the result
        Console.WriteLine(result); // Output to console for demonstration
    }

    static String GetSideBySideDiff(String expected, String actual)
    {
        const Int32 columnWidth = 64;

        var diffBuilder = new SideBySideDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(expected, actual);

        var sb = new StringBuilder();

        // Add headers
        _ = sb.AppendLine(
            CultureInfo.InvariantCulture,
            $"{"expected".PadRight(columnWidth)} | {"actual".PadRight(columnWidth)}");
        _ = sb.AppendLine(new String('-', columnWidth * 2 + 10)); // Divider line

        // Determine the maximum number of lines to process
        var maxLines = Math.Max(diff.OldText.Lines.Count, diff.NewText.Lines.Count);

        for(var i = 0; i < maxLines; i++)
        {
            // Fetch lines from old and new texts
            var oldLine = i < diff.OldText.Lines.Count ? diff.OldText.Lines[i] : null;
            var newLine = i < diff.NewText.Lines.Count ? diff.NewText.Lines[i] : null;

            // Generate the side-by-side display for each line
            var oldText = getFormattedText(oldLine, columnWidth);
            var newText = getFormattedText(newLine, columnWidth);
            var status = getStatusSymbol( newLine);

            _ = sb.AppendLine(CultureInfo.InvariantCulture, $"{oldText} | {status} {newText}");
        }

        return sb.ToString();

    static String getFormattedText(DiffPiece? line, Int32 columnWidth)
    {
        if(line?.Text == null)
            return "".PadRight(columnWidth); // Blank space for missing lines

        // Return the line's text truncated or padded to fit the column width
        return line.Text.PadRight(columnWidth)[..columnWidth];
    }

    static String getStatusSymbol(DiffPiece? newLine) => newLine?.Type switch
    {
        ChangeType.Inserted => "+",
        ChangeType.Modified => "~",
        ChangeType.Deleted or null => "-",
        _ => " "
    };
    }
    //{
    //    Console.WriteLine(typeof(Stream).AssemblyQualifiedName);
    //    //TestIntersectionMapping();
    //    TestDefaultValues();
    //}

    private static void TestDefaultValues()
    {
        //var tree = CSharpSyntaxTree.ParseText(
        //    """
        //    class Foo
        //    {
        //        void M(System.Single a = 2){}
        //        void M(System.Double a = 4){}
        //        void M(System.Byte a = 8){}
        //        void M(System.Int16 a = 16){}
        //        void M(System.Int32 a = 32){}
        //        void M(System.Int64 a = 64){}
        //        void M(System.Char a = 'a'){}
        //        void M(System.String a = "a"){}
        //    }
        //    """);
        //var model = CSharpCompilation.Create(
        //    "TestAssembly",
        //    new[] { tree },
        //    new[] { MetadataReference.CreateFromFile(typeof(Object).Assembly.Location) }).GetSemanticModel(tree);
        //var declaration = tree.GetRoot().ChildNodes().OfType<TypeDeclarationSyntax>().Single();
        //var symbol = model.GetDeclaredSymbol(declaration)!;
        //var defaults = symbol.GetMembers()
        //    .OfType<IMethodSymbol>()
        //    .SelectMany(m => m.Parameters)
        //    .Where(p => p.HasExplicitDefaultValue)
        //    .Select(GetDefaultValue);

        //var h = CSharpGeneratorDriver.Create()

        //Console.WriteLine(String.Join("\n", defaults));
    }

    private static String? GetDefaultValue(IParameterSymbol parameter)
    {
        var result =
            parameter.HasExplicitDefaultValue ?
            parameter.ExplicitDefaultValue switch
            {
                Char => $"'{parameter.ExplicitDefaultValue}'",
                String => $"\"{parameter.ExplicitDefaultValue}\"",
                _ => parameter.ExplicitDefaultValue?.ToString() ?? "default"
            } : String.Empty;

        return result;
    }

    private static void TestIntersectionMapping()
    {
        var tree = CSharpSyntaxTree.ParseText(
            """
            interface T
            {
                public string M<S, U>(System.Int32 a1, System.String a2, out int a3, ref byte a4, params object[] a5);
            }
            """);
        var model = CSharpCompilation.Create(
            "TestAssembly",
            new[] { tree },
            new[] { MetadataReference.CreateFromFile(typeof(Object).Assembly.Location) }).GetSemanticModel(tree);
        var declaration = tree.GetRoot().ChildNodes().OfType<InterfaceDeclarationSyntax>().Single();
        var symbol = model.GetDeclaredSymbol(declaration)!;
        var method = symbol.GetMembers().OfType<IMethodSymbol>().Single();
        Console.WriteLine(GetSignatureString(method));
    }
    private static readonly SymbolDisplayFormat _fullStringFormat =
        SymbolDisplayFormat.FullyQualifiedFormat
                    .WithMiscellaneousOptions(
                    /*
                        get rid of special types

                             10110
                        NAND 00100
                          => 10010

                             10110
                          &! 00100
                          => 10010

                             00100
                           ^ 11111
                          => 11011

                             10110
                           & 11011
                          => 10010
                    */
                    SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions &
                    ( SymbolDisplayMiscellaneousOptions.UseSpecialTypes ^ (SymbolDisplayMiscellaneousOptions)Int32.MaxValue ))
                    .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters);

    private static String GetSignatureString(IMethodSymbol symbol)
    {
        var builder = new StringBuilder();
        AppendSignature(builder, symbol);
        var result = builder.ToString();

        return result;
    }

    private static void AppendSignature(StringBuilder builder, IMethodSymbol symbol)
    {
        if(symbol.DeclaredAccessibility != Accessibility.Public ||
           symbol.IsStatic)
        {
            return;
        }

        if(symbol.ReturnsByRef)
        {
            _ = builder.Append("ref ");
        }

        if(symbol.ReturnsByRefReadonly)
        {
            _ = builder.Append("ref readonly ");
        }

        _ = builder.Append(symbol.ReturnType.ToDisplayString(_fullStringFormat))
            .Append(' ')
            .Append(symbol.ContainingType.ToDisplayString(_fullStringFormat))
            .Append('.')
            .Append(symbol.Name);

        if(symbol.TypeParameters.Length > 0)
        {
            _ = builder.Append('<').Append(symbol.TypeParameters[0].Name);
            for(var i = 1; i < symbol.TypeParameters.Length; i++)
            {
                var parameter = symbol.TypeParameters[i];
                _ = builder.Append(", ").Append(parameter.Name);
            }

            _ = builder.Append('>');
        }

        _ = builder.Append('(');
        if(symbol.Parameters.Length == 0)
        {
            _ = builder.Append(')');
            return;
        }

        AppendParameter(builder, symbol.Parameters[0]);
        for(var i = 1; i < symbol.Parameters.Length; i++)
        {
            _ = builder.Append(", ");
            var parameter = symbol.Parameters[i];
            AppendParameter(builder, parameter);
        }

        _ = builder.Append(')');
    }

    private static void AppendParameter(StringBuilder builder, IParameterSymbol symbol)
    {
        if(symbol.IsParams)
        {
            _ = builder.Append("params ")
                .Append(symbol.Type.ToDisplayString(_fullStringFormat))
                .Append(' ')
                .Append(symbol.Name);

            return;
        }

        var refString = symbol.RefKind switch
        {
            RefKind.In => "in ",
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            RefKind.RefReadOnlyParameter => "ref readonly ",
            _ => String.Empty
        };

        _ = builder.Append(refString)
            .Append(symbol.Type.ToDisplayString(_fullStringFormat))
            .Append(' ')
            .Append(symbol.Name);

        if(symbol.HasExplicitDefaultValue)
        {
            _ = builder.Append(" = ");
            Char? quote = symbol.Type.MetadataName == "System.String" ?
                '"' :
                symbol.Type.MetadataName == "System.Char" ?
                '\'' :
                null;
            _ = builder.Append(quote).Append(symbol.ExplicitDefaultValue ?? "null").Append(quote);
        }
    }
    [GeneratedRegex(@".*")]
    private static partial Regex MyRegex();
}
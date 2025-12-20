// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Text;

using DiffPlex.DiffBuilder.Model;

using DiffPlex.DiffBuilder;
using System.Globalization;
using System.Text.RegularExpressions;

internal static partial class TestHelpers
{
    public static void FailWithDiff(String expected, String? actual, Int32 columnWidth = 96)
    {
        var diff = GetDiff(expected, actual, columnWidth);

        Assert.Fail(diff);
    }

    private static readonly Regex _newlinePattern = new(@"(\r\n)|(\r)|(\n)", RegexOptions.Compiled);

    private static String VisualizeNewlines(String text) =>
         _newlinePattern.Replace(text, static m => m.ValueSpan switch
         {
             ['\r'] => "\\r\n",
             ['\n'] => "\\n\n",
             ['\r', '\n'] => "\\r\\n\n",
             _ => throw new InvalidOperationException("unexpected match")
         });

    public static String GetDiff(String expected, String? actual, Int32 columnWidth)
    {
        var diff = SideBySideDiffBuilder.Diff(
            VisualizeNewlines(expected),
            VisualizeNewlines(actual ?? String.Empty),
            ignoreCase: false,
            ignoreWhiteSpace: false);

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
            var status = getStatusSymbol(newLine);

            _ = sb.AppendLine(CultureInfo.InvariantCulture, $"{oldText} | {status} {newText}");
        }

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
            ChangeType.Deleted or ChangeType.Imaginary or null => "-",
            ChangeType.Unchanged or _ => " "
        };

        return sb.ToString();
    }
}

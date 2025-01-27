#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Text;

using DiffPlex.DiffBuilder.Model;

using DiffPlex.DiffBuilder;
using System.Globalization;

internal static class TestHelpers
{
    public static void FailWithDiff(String left, String right, Int32 columnWidth = 96)
    {
        var diff= GetDiff(left, right, columnWidth);

        Assert.Fail(diff);
    }

    public static String GetDiff(String left, String right, Int32 columnWidth)
    {
        var diff = SideBySideDiffBuilder.Diff(left, right);

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
            ChangeType.Deleted or null => "-",
            _ => " "
        };

        return sb.ToString();
    }
}

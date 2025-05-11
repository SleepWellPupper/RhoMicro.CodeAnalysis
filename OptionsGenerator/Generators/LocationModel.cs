namespace RhoMicro.CodeAnalysis.OptionsGenerator.GeneratorsV2;

using System;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

internal sealed record LocationModel(
    Int32 StartLine,
    Int32 StartCol,
    Int32 EndLine,
    Int32 EndCol,
    String Path)
{
    public static LocationModel Empty { get; } = new(0, 0, 0, 0, String.Empty);

    public static LocationModel Create(Location location, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var lineSpan = location.GetMappedLineSpan();

        var result = new LocationModel(
            StartLine: lineSpan.StartLinePosition.Line + 1,
            StartCol: lineSpan.StartLinePosition.Character + 1,
            EndLine: lineSpan.EndLinePosition.Line + 1,
            EndCol: lineSpan.EndLinePosition.Character + 1,
            Path: lineSpan.Path ?? String.Empty);

        return result;
    }
}

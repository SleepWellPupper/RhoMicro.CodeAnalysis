// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.UnionsGenerator.Analyzers;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.UnionsGenerator.Models;

internal static class Extensions
{
    public static void AddRange(this IDiagnosticsAccumulator<UnionTypeModel> diagnostics, IEnumerable<Diagnostic> range)
    {
        foreach(var diagnostic in range)
            _ = diagnostics.Add(diagnostic);
    }
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Diagnostics;
using System.Threading;

internal sealed class DebugTokenValidator(CancellationToken ct) : TokenValidator(ct)
{
    protected override void OnError(String message) => Debug.Fail(message);
}

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;
using System;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

internal readonly record struct SourceBuildingContext(IndentedStringBuilder SourceBuilder, AttributeFactoryModel Model, String DisplayString, CancellationToken CancellationToken)
{
    public void ThrowIfCancellationRequested() => CancellationToken.ThrowIfCancellationRequested();
}

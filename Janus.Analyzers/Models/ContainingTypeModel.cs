// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Collections.Immutable;
using Library.Models.Collections;

internal readonly record struct ContainingTypeModel(
    String Modifier,
    String Name,
    EquatableList<String> TypeParameters);

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Equa;

using System.Collections.Immutable;
using Library.Models.Collections;
using Microsoft.CodeAnalysis;

internal sealed record RecordModel(
    TypeKind TypeKind,
    Boolean IsRecord,
    String Name,
    String Namespace,
    EquatableList<String> TypeParameters,
    EquatableList<ContainingTypeModel> ContainingTypes,
    EquatableList<MemberModel> VectorMembers,
    EquatableList<MemberModel> ScalarMembers);

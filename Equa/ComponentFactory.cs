// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using Equa;
using Library.Models.Collections;

partial class ComponentFactory
{

    public static ListComponent<EquatableList<MemberModel>, MemberModel, String> List(
        EquatableList<MemberModel> list,
        Action<MemberModel, Int32, Int32, CSharpSourceBuilder, CancellationToken> append,
        String separator = "",
        String terminator = "")
        => List<EquatableList<MemberModel>, MemberModel>(
            list,
            append,
            separator,
            terminator);
}

// SPDX-License-Identifier: MPL-2.0

namespace TestApplication;

using System.Collections.Immutable;
using RhoMicro.CodeAnalysis;

[UnionType<double, int, ImmutableArray<byte>, string>(Groups = ["Foo"])]
[UnionType<Stream>(IsNullable = true, Name = "File", Description = "a file stream")]
[UnionTypeSettings(JsonConverterSetting = JsonConverterSetting.EmitJsonConverter)]
public sealed partial class IntDoubleString;

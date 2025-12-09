// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

public partial class JsonConverterTests
{
    [UnionType<DateTime, Double, String, List<String>>]
    [UnionTypeSettings(JsonConverterSetting = JsonConverterSetting.EmitJsonConverter)]
    private readonly partial struct Union;

    [Fact]
    public void SerializesDateTimeUnion()
    {
        var expected = DateTime.Parse("01/10/2009 7:34", CultureInfo.InvariantCulture);
        Union u = expected;
        var serialized = JsonSerializer.Serialize(u);
        var deserialized = JsonSerializer.Deserialize<Union>(serialized);
        Assert.True(deserialized.TryCastToDateTime(out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerializesDoubleUnion()
    {
        var expected = 32d;
        Union u = expected;
        var serialized = JsonSerializer.Serialize(u);
        var deserialized = JsonSerializer.Deserialize<Union>(serialized);
        Assert.True(deserialized.TryCastToDouble(out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerializesStringUnion()
    {
        var expected = "Hello, World!";
        Union u = expected;
        var serialized = JsonSerializer.Serialize(u);
        var deserialized = JsonSerializer.Deserialize<Union>(serialized);
        Assert.True(deserialized.TryCastToString(out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerializesListUnion()
    {
        List<String> expected = ["Hell", "o, ", "World", "!"];
        Union u = expected;
        var serialized = JsonSerializer.Serialize(u);
        var deserialized = JsonSerializer.Deserialize<Union>(serialized);
        Assert.True(deserialized.TryCastToList(out var actual));
        Assert.True(expected.SequenceEqual(actual));
    }
}

// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Attributes;
using OneOf;
using RhoMicro.CodeAnalysis;

[SimpleJob]
[MemoryDiagnoser]
public partial class SwitchStateBenchmark
{
    [UnionType<Int32, Single>]
    partial struct JanusUnion;

    [Benchmark(Baseline = true)]
    public String Janus()
    {
        var state = "State";

        var result = new JanusUnion(32).Switch(
            state,
            onInt32: static (_, s) => s,
            onSingle: static (_, s) => s);

        return result;
    }

    [Benchmark]
    public String OneOf()
    {
        var state = "State";

        var result = OneOf<Int32, Single>.FromT0(32).Match(
            f0: _ => state,
            f1: _ => state);

        return result;
    }
}

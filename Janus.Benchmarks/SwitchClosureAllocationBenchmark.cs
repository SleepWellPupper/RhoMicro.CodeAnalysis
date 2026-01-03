// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Attributes;
using OneOf;
using RhoMicro.CodeAnalysis;

// Dunet doesn't emit the nested type as part of the hint name, and
// so Roslyn doesn't emit its generated source when conflicting hint
// names are used.
// This is why the Dunet types are not modeled as nested types. 
[Dunet.Union]
public partial record SwitchClosureAllocationBenchmarkDunetUnion
{
    public partial record Int32(System.Int32 Value);
}

[SimpleJob, MemoryDiagnoser]
public partial class SwitchClosureAllocationBenchmark
{
    class OneOfUnion : OneOfBase<Int32>
    {
        public OneOfUnion(OneOf<Int32> input) : base(input)
        {
        }
    }

    [UnionType<Int32>]
    sealed partial class JanusUnion;

    [Params(1)] public Int32 State { get; set; }

    [Benchmark(Baseline = true)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        var union = new JanusUnion(1);
        var state = State;

        for (var i = 0; i < state; i++)
        {
            result += union.Switch(
                state: state,
                onInt32: static (i, s) => i + s);
        }

        return result;
    }

    [Benchmark]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        var union = new OneOfUnion(1);
        var state = State;

        for (var i = 0; i < state; i++)
        {
            result += union.Match(i => i + state);
        }

        return result;
    }


    [Benchmark]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        var union = new SwitchClosureAllocationBenchmarkDunetUnion.Int32(1);
        var state = State;

        for (var i = 0; i < state; i++)
        {
            result += union.Match(
                state: state,
                int32: static (s, i) => i.Value + s);
        }

        return result;
    }
}

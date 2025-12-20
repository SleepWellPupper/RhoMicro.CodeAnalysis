// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Attributes;
using OneOf;
using RhoMicro.CodeAnalysis;

[SimpleJob]
[MemoryDiagnoser]
public partial class UnmanagedOverlayingBenchmark
{
    [UnionType<Int64, Int32, Int16, Byte>]
    sealed partial class JanusUnion;

    class OneOfUnion : OneOfBase<Int64, Int32, Int16, Byte>
    {
        public OneOfUnion(OneOf<Int64, Int32, Int16, Byte> input) : base(input)
        {
        }
    }

    [Benchmark(Baseline = true)]
    public Object Janus() => new JanusUnion(0);

    [Benchmark]
    public Object OneOf() => new OneOfUnion(0);
}

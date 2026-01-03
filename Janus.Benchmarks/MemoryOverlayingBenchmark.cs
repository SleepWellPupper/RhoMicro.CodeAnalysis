// SPDX-License-Identifier: MPL-2.0

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using OneOf;
using RhoMicro.CodeAnalysis;

// Dunet doesn't emit the nested type as part of the hint name, and
// so Roslyn doesn't emit its generated source when conflicting hint
// names are used.
// This is why the Dunet types are not modeled as nested types. 
[Dunet.Union]
public partial record MemoryOverlayingBenchmarkDunetUnion
{
    public partial record Int32(System.Int32 Value);

    public partial record Single(System.Single Value);

    public partial record Double(System.Double Value);

    public partial record Int64(System.Int64 Value);
}

[SimpleJob, MemoryDiagnoser]
public partial class MemoryOverlayingBenchmark
{
    public class ClassOneOfUnion : OneOfBase<Int32, Single, Double, Int64>
    {
        public ClassOneOfUnion(OneOf<Int32, Single, Double, Int64> input) : base(input)
        {
        }
    }

    [UnionType<Int32, Single, Double, Int64>]
    public sealed partial class ClassJanusUnion;

    [UnionType<Int32, Single, Double, Int64>]
    public readonly partial struct StructJanusUnion;

    [Benchmark]
    public StructJanusUnion StructJanusUnionAllocations() => new(GetValue());

    [Benchmark(Baseline = true)]
    public ClassJanusUnion ClassJanusUnionAllocations() => new(GetValue());

    [Benchmark]
    public ClassOneOfUnion ClassOneOfUnionAllocations() => new(GetValue());

    [Benchmark]
    public OneOf<Int32, Single, Double, Int64> StructOneOfUnionAllocations()
        => GetValue(); // OneOf does not publish a ctor

    [Benchmark]
    public MemoryOverlayingBenchmarkDunetUnion DunetUnionAllocations()
        => new MemoryOverlayingBenchmarkDunetUnion.Int32(GetValue());

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Int32 GetValue() => 0;
}

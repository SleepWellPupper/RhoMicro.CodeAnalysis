#pragma warning disable

namespace RhoMicro.CodeAnalysis.UtilityGenerators.Benchmarks;

using System;

using BenchmarkDotNet.Attributes;

[SimpleJob]
[MemoryDiagnoser]
public class DynamicallyAllocatedBufferBenchmark
{
    public static Object[][] Data => [
        [16 * 1024, 16 * 1024],
        [16 * 1024, 32 * 1024],
        [16 * 1024, 64 * 1024],
        [32 * 1024, 16 * 1024],
        [32 * 1024, 32 * 1024],
        [32 * 1024, 64 * 1024],
        [64 * 1024, 16 * 1024],
        [64 * 1024, 32 * 1024],
        [64 * 1024, 64 * 1024]
    ];
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Int32 Run(Int32 initialSize, Int32 iterations)
    {
        var buffer = new Library.Text.Templating.DynamicallyAllocatedBuffer<Byte>(stackalloc Byte[initialSize]);
        while(iterations > 0)
        {
            iterations--;
            buffer.Add(42);
        }

        return buffer.Span.Length;
    }
}

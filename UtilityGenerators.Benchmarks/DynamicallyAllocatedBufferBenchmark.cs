#pragma warning disable

namespace RhoMicro.CodeAnalysis.Benchmarks;

using System;

using BenchmarkDotNet.Attributes;

using RhoMicro.CodeAnalysis.Library.Text.Templating;

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
        var buffer = new DynamicallyAllocatedCharBuffer(stackalloc Char[initialSize]);
        while(iterations > 0)
        {
            iterations--;
            buffer.Add('A');
        }

        return buffer.Span.Length;
    }
}

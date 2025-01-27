namespace Lace.Benchmarks;

using System.Diagnostics;

using BenchmarkDotNet.Running;

using Microsoft.Diagnostics.Tracing.Parsers.Clr;

using RhoMicro.CodeAnalysis.Benchmarks;

internal class Program
{
    static void Main(string[] args)
    {
        var benchmark = new BlockTerminatorsBenchmark();
        benchmark.GlobalSetup();
        Console.WriteLine(benchmark.Count);
        if(benchmark.IsBlockTerminator() is var isBlockTerminatorCount && isBlockTerminatorCount != benchmark.Count)
            throw new Exception("lol git gud (IsBlockTerminator)");
        Console.WriteLine($"IsBlockTerminator: {isBlockTerminatorCount}");
        if(benchmark.BinarySearch() is var binarySearchCount && binarySearchCount != benchmark.Count)
            throw new Exception("lol git gud (BinarySearch)");
        Console.WriteLine($"BinarySearch: {binarySearchCount}");
        if(benchmark.LinearSearch() is var linearSearchCount && linearSearchCount != benchmark.Count)
            throw new Exception("lol git gud (LinearSearch)");
        Console.WriteLine($"LinearSearch: {linearSearchCount}");
        if(benchmark.LinearSearchUnrolled() is var linearSearchUnrolledCount && linearSearchUnrolledCount != benchmark.Count)
            throw new Exception("lol git gud (LinearSearchUnrolled)");
        Console.WriteLine($"LinearSearchUnrolled: {linearSearchUnrolledCount}");
        if(benchmark.LinearSearchUnrolledConditions() is var linearSearchUnrolledConditionsCount && linearSearchUnrolledConditionsCount != benchmark.Count)
            throw new Exception("lol git gud (LinearSearchUnrolledConditions)");
        Console.WriteLine($"LinearSearchUnrolledConditions: {linearSearchUnrolledConditionsCount}");
        if(benchmark.LinearSearchUnrolledPatternMatching() is var linearSearchUnrolledPatternMatchingCount && linearSearchUnrolledPatternMatchingCount != benchmark.Count)
            throw new Exception("lol git gud (LinearSearchUnrolledPatternMatching)");
        Console.WriteLine($"LinearSearchUnrolledPatternMatching: {linearSearchUnrolledPatternMatchingCount}");
        if(benchmark.LinearSearchUnrolledShortCircuiting() is var linearSearchUnrolledShortCircuitingCount && linearSearchUnrolledShortCircuitingCount != benchmark.Count)
            throw new Exception("lol git gud (LinearSearchUnrolledShortCircuiting)");
        Console.WriteLine($"LinearSearchUnrolledShortCircuiting: {linearSearchUnrolledShortCircuitingCount}");
        if(benchmark.LinearSearchUnrolledTernary() is var linearSearchUnrolledTernaryCount && linearSearchUnrolledTernaryCount != benchmark.Count)
            throw new Exception("lol git gud (LinearSearchUnrolledTernary)");
        Console.WriteLine($"LinearSearchUnrolledTernary: {linearSearchUnrolledTernaryCount}");

        //var benchmark = new ParserMatchBenchmark();
        //benchmark.GlobalSetup();
        //Debug.Assert(benchmark.MatchAnyNaive() == 12);
        //Debug.Assert(benchmark.MatchAnyTernary() == 12);

        var summary = BenchmarkRunner.Run<BlockTerminatorsBenchmark>();
        foreach(var report in summary.Reports)
        {
            var workloadMethod = report.BenchmarkCase.Descriptor.WorkloadMethod;
            var typeName = workloadMethod.DeclaringType?.FullName?.Replace('.', '_');
            var methodName = workloadMethod.Name;
            var benchmarkName = $"RHOMICRO_CODEANALYSIS_AUTOBENCHMARK_{typeName}{( typeName is null ? String.Empty : "_" )}{methodName}";
            Console.WriteLine(benchmarkName);
            foreach(var (name, measurement) in report.Metrics)
            {
                Console.WriteLine($"{name}: {measurement.Value.ToString(measurement.Descriptor.NumberFormat)}");
            }
        }
    }
}

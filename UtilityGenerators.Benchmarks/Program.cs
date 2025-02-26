using BenchmarkDotNet.Running;

using RhoMicro.CodeAnalysis.Benchmarks;

if(new TemplateBenchmark().Template() != new TemplateBenchmark().StringBuilder())
{
    Console.WriteLine("Template:");
    Console.WriteLine(new TemplateBenchmark().Template());

    Console.WriteLine("StringBuilder:");
    Console.WriteLine(new TemplateBenchmark().StringBuilder());

    throw new Exception();
}

_ = BenchmarkRunner.Run<TemplateBenchmark>();

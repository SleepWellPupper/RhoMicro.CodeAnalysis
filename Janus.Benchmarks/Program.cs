// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using RhoMicro.BdnLogging;

#if DEBUG
await RegenerateSwitchExecutionBenchmark();
return;
#endif

ValidateBenchmarks();
BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args, SpotlightConfig.Instance);

void Assert(Boolean value, [CallerArgumentExpression(nameof(value))] String? expression = null)
{
    if (!value)
    {
        throw new Exception($"Assertion failed: {expression}");
    }
}

void ValidateBenchmarks()
{
    var validator = new Validator();
    new SwitchExecutionBenchmark1().Validate(validator);
    new SwitchExecutionBenchmark2().Validate(validator);
    new SwitchExecutionBenchmark3().Validate(validator);
    new SwitchExecutionBenchmark4().Validate(validator);
    new SwitchExecutionBenchmark5().Validate(validator);
    new SwitchExecutionBenchmark6().Validate(validator);
    new SwitchExecutionBenchmark7().Validate(validator);
    new SwitchExecutionBenchmark8().Validate(validator);
    new SwitchExecutionBenchmark9().Validate(validator);
    new SwitchExecutionBenchmark10().Validate(validator);
    new SwitchExecutionBenchmark11().Validate(validator);
    new SwitchExecutionBenchmark12().Validate(validator);
    new SwitchExecutionBenchmark13().Validate(validator);
    new SwitchExecutionBenchmark14().Validate(validator);
    new SwitchExecutionBenchmark15().Validate(validator);
    new SwitchExecutionBenchmark16().Validate(validator);
    new SwitchExecutionBenchmark17().Validate(validator);
    new SwitchExecutionBenchmark18().Validate(validator);
    new SwitchExecutionBenchmark19().Validate(validator);
    new SwitchExecutionBenchmark20().Validate(validator);
    new SwitchExecutionBenchmark21().Validate(validator);
    new SwitchExecutionBenchmark22().Validate(validator);
    new SwitchExecutionBenchmark23().Validate(validator);
    new SwitchExecutionBenchmark24().Validate(validator);
    new SwitchExecutionBenchmark25().Validate(validator);
    new SwitchExecutionBenchmark26().Validate(validator);
    new SwitchExecutionBenchmark27().Validate(validator);
    new SwitchExecutionBenchmark28().Validate(validator);
    new SwitchExecutionBenchmark29().Validate(validator);
    new SwitchExecutionBenchmark30().Validate(validator);
    new SwitchExecutionBenchmark31().Validate(validator);
    new SwitchExecutionBenchmark32().Validate(validator);
}

async Task RegenerateSwitchExecutionBenchmark()
{
    var path = new FileInfo("SwitchExecutionBenchmark.cs").FullName;
    Console.WriteLine($"regenerate '{path}'?");
    if (Console.ReadLine() is "y")
    {
        await using var file = File.CreateText(path);

        await file.WriteLineAsync("""
                                  using BenchmarkDotNet.Attributes;
                                  using OneOf;
                                  using RhoMicro.CodeAnalysis;
                                  using Dunet;
                                  using System.Runtime.CompilerServices;
                                  """
        );

        for (int i = 1; i <= 32; i++)
        {
            await file.WriteLineAsync(
                $$"""
                  [Union]
                  partial record SwitchExecutionBenchmark{{i}}DunetUnion
                  {
                      {{String.Join("\n", Enumerable.Range(1, i).Select(j => $"public partial record Variant{j}(SwitchExecutionBenchmark{i}.S{j} Value);"))}}
                  }

                  [SimpleJob]
                  public partial class SwitchExecutionBenchmark{{i}}
                  {
                      public void Validate(Validator validator)
                      {
                          validator.Assert(JanusUnionSwitch() is {{i*(i+1)/2}});
                          validator.Assert(OneOfUnionSwitch() is {{i*(i+1)/2}});
                          validator.Assert(DunetUnionSwitch() is {{i*(i+1)/2}});
                      }
                  
                      {{String.Join("\n", Enumerable.Range(1, i).Select(i => $"public record struct S{i}(int Value);"))}}

                      class OneOfUnion : OneOfBase<{{String.Join(", ", Enumerable.Range(1, i).Select(i => $"S{i}"))}}>
                      {
                          public OneOfUnion(OneOf<{{String.Join(", ", Enumerable.Range(1, i).Select(i => $"S{i}"))}}> input) : base(input) {}
                      }

                      {{String.Join("\n", Enumerable.Range(0, i).GroupBy(i => i / 8).Select(g => $"[RhoMicro.CodeAnalysis.UnionType<{String.Join(", ", g.Select(i => $"S{i + 1}"))}>]"))}}
                      sealed partial class JanusUnion;

                      private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
                      {
                          {{String.Join(",\n", Enumerable.Range(1, i).Select(i => $"new JanusUnion(new S{i}({i}))"))}}
                      };
                      
                      private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
                      {
                          {{String.Join(",\n", Enumerable.Range(1, i).Select(i => $"new OneOfUnion(new S{i}({i}))"))}}
                      };

                      private static readonly SwitchExecutionBenchmark{{i}}DunetUnion[] DunetUnions = new SwitchExecutionBenchmark{{i}}DunetUnion[]
                      {
                          {{String.Join(",\n", Enumerable.Range(1, i).Select(j => $"new SwitchExecutionBenchmark{i}DunetUnion.Variant{j}(new S{j}({j}))"))}}
                      };

                      [Benchmark(Baseline = true)]
                      [MethodImpl(MethodImplOptions.NoInlining)]
                      public Int32 JanusUnionSwitch()
                      {
                          var result = 0;
                          
                          for(var i = 0; i < {{i}}; i++)
                          {
                              var union = JanusUnions[i]; 
                              result += union.Switch(
                                  {{String.Join(",\n", Enumerable.Range(1, i).Select(i => $"static s => s.Value"))}}
                              );
                          }
                          
                          return result;
                      }
                      
                      [Benchmark]
                      [MethodImpl(MethodImplOptions.NoInlining)]
                      public Int32 OneOfUnionSwitch()
                      {
                          var result = 0;
                          
                          for(var i = 0; i < {{i}}; i++)
                          {
                              var union = OneOfUnions[i]; 
                              result += union.Match(
                                  {{String.Join(",\n", Enumerable.Range(1, i).Select(i => $"static s => s.Value"))}}
                              );
                          }
                          
                          return result;
                      }
                      
                      [Benchmark]
                      [MethodImpl(MethodImplOptions.NoInlining)]
                      public Int32 DunetUnionSwitch()
                      {
                          var result = 0;
                          
                          for(var i = 0; i < {{i}}; i++)
                          {
                              var union = DunetUnions[i]; 
                              result += union.Match(
                                  {{String.Join(",\n", Enumerable.Range(1, i).Select(i => $"static s => s.Value.Value"))}}
                              );
                          }
                          
                          return result;
                      }
                  }
                  """);
        }
    }
}

public class Validator
{
    public void Assert(Boolean value, [CallerArgumentExpression(nameof(value))] String expression = "")
    {
        if (value)
        {
            return;
        }

        throw new InvalidOperationException($"Expression `{expression}` did not evaluate to `true`.");
    }
}

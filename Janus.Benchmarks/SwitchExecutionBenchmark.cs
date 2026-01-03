using BenchmarkDotNet.Attributes;
using OneOf;
using RhoMicro.CodeAnalysis;
using Dunet;
using System.Runtime.CompilerServices;
[Union]
partial record SwitchExecutionBenchmark1DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark1.S1 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark1
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 1);
        validator.Assert(OneOfUnionSwitch() is 1);
        validator.Assert(DunetUnionSwitch() is 1);
    }

    public record struct S1(int Value);

    class OneOfUnion : OneOfBase<S1>
    {
        public OneOfUnion(OneOf<S1> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1))
    };

    private static readonly SwitchExecutionBenchmark1DunetUnion[] DunetUnions = new SwitchExecutionBenchmark1DunetUnion[]
    {
        new SwitchExecutionBenchmark1DunetUnion.Variant1(new S1(1))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 1; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 1; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 1; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark2DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark2.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark2.S2 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark2
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 3);
        validator.Assert(OneOfUnionSwitch() is 3);
        validator.Assert(DunetUnionSwitch() is 3);
    }

    public record struct S1(int Value);
public record struct S2(int Value);

    class OneOfUnion : OneOfBase<S1, S2>
    {
        public OneOfUnion(OneOf<S1, S2> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2))
    };

    private static readonly SwitchExecutionBenchmark2DunetUnion[] DunetUnions = new SwitchExecutionBenchmark2DunetUnion[]
    {
        new SwitchExecutionBenchmark2DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark2DunetUnion.Variant2(new S2(2))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 2; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 2; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 2; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark3DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark3.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark3.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark3.S3 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark3
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 6);
        validator.Assert(OneOfUnionSwitch() is 6);
        validator.Assert(DunetUnionSwitch() is 6);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3>
    {
        public OneOfUnion(OneOf<S1, S2, S3> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3))
    };

    private static readonly SwitchExecutionBenchmark3DunetUnion[] DunetUnions = new SwitchExecutionBenchmark3DunetUnion[]
    {
        new SwitchExecutionBenchmark3DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark3DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark3DunetUnion.Variant3(new S3(3))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 3; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 3; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 3; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark4DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark4.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark4.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark4.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark4.S4 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark4
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 10);
        validator.Assert(OneOfUnionSwitch() is 10);
        validator.Assert(DunetUnionSwitch() is 10);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4))
    };

    private static readonly SwitchExecutionBenchmark4DunetUnion[] DunetUnions = new SwitchExecutionBenchmark4DunetUnion[]
    {
        new SwitchExecutionBenchmark4DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark4DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark4DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark4DunetUnion.Variant4(new S4(4))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 4; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 4; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 4; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark5DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark5.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark5.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark5.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark5.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark5.S5 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark5
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 15);
        validator.Assert(OneOfUnionSwitch() is 15);
        validator.Assert(DunetUnionSwitch() is 15);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5))
    };

    private static readonly SwitchExecutionBenchmark5DunetUnion[] DunetUnions = new SwitchExecutionBenchmark5DunetUnion[]
    {
        new SwitchExecutionBenchmark5DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark5DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark5DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark5DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark5DunetUnion.Variant5(new S5(5))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 5; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 5; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 5; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark6DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark6.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark6.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark6.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark6.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark6.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark6.S6 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark6
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 21);
        validator.Assert(OneOfUnionSwitch() is 21);
        validator.Assert(DunetUnionSwitch() is 21);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6))
    };

    private static readonly SwitchExecutionBenchmark6DunetUnion[] DunetUnions = new SwitchExecutionBenchmark6DunetUnion[]
    {
        new SwitchExecutionBenchmark6DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark6DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark6DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark6DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark6DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark6DunetUnion.Variant6(new S6(6))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 6; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 6; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 6; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark7DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark7.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark7.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark7.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark7.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark7.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark7.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark7.S7 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark7
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 28);
        validator.Assert(OneOfUnionSwitch() is 28);
        validator.Assert(DunetUnionSwitch() is 28);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7))
    };

    private static readonly SwitchExecutionBenchmark7DunetUnion[] DunetUnions = new SwitchExecutionBenchmark7DunetUnion[]
    {
        new SwitchExecutionBenchmark7DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark7DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark7DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark7DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark7DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark7DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark7DunetUnion.Variant7(new S7(7))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 7; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 7; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 7; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark8DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark8.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark8.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark8.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark8.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark8.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark8.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark8.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark8.S8 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark8
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 36);
        validator.Assert(OneOfUnionSwitch() is 36);
        validator.Assert(DunetUnionSwitch() is 36);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8))
    };

    private static readonly SwitchExecutionBenchmark8DunetUnion[] DunetUnions = new SwitchExecutionBenchmark8DunetUnion[]
    {
        new SwitchExecutionBenchmark8DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark8DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark8DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark8DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark8DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark8DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark8DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark8DunetUnion.Variant8(new S8(8))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 8; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 8; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 8; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark9DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark9.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark9.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark9.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark9.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark9.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark9.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark9.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark9.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark9.S9 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark9
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 45);
        validator.Assert(OneOfUnionSwitch() is 45);
        validator.Assert(DunetUnionSwitch() is 45);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9))
    };

    private static readonly SwitchExecutionBenchmark9DunetUnion[] DunetUnions = new SwitchExecutionBenchmark9DunetUnion[]
    {
        new SwitchExecutionBenchmark9DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark9DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark9DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark9DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark9DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark9DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark9DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark9DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark9DunetUnion.Variant9(new S9(9))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 9; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 9; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 9; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark10DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark10.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark10.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark10.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark10.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark10.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark10.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark10.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark10.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark10.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark10.S10 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark10
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 55);
        validator.Assert(OneOfUnionSwitch() is 55);
        validator.Assert(DunetUnionSwitch() is 55);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10))
    };

    private static readonly SwitchExecutionBenchmark10DunetUnion[] DunetUnions = new SwitchExecutionBenchmark10DunetUnion[]
    {
        new SwitchExecutionBenchmark10DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark10DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark10DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark10DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark10DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark10DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark10DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark10DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark10DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark10DunetUnion.Variant10(new S10(10))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 10; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 10; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 10; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark11DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark11.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark11.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark11.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark11.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark11.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark11.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark11.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark11.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark11.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark11.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark11.S11 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark11
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 66);
        validator.Assert(OneOfUnionSwitch() is 66);
        validator.Assert(DunetUnionSwitch() is 66);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11))
    };

    private static readonly SwitchExecutionBenchmark11DunetUnion[] DunetUnions = new SwitchExecutionBenchmark11DunetUnion[]
    {
        new SwitchExecutionBenchmark11DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark11DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark11DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark11DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark11DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark11DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark11DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark11DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark11DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark11DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark11DunetUnion.Variant11(new S11(11))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 11; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 11; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 11; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark12DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark12.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark12.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark12.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark12.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark12.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark12.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark12.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark12.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark12.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark12.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark12.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark12.S12 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark12
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 78);
        validator.Assert(OneOfUnionSwitch() is 78);
        validator.Assert(DunetUnionSwitch() is 78);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12))
    };

    private static readonly SwitchExecutionBenchmark12DunetUnion[] DunetUnions = new SwitchExecutionBenchmark12DunetUnion[]
    {
        new SwitchExecutionBenchmark12DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark12DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark12DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark12DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark12DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark12DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark12DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark12DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark12DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark12DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark12DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark12DunetUnion.Variant12(new S12(12))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 12; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 12; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 12; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark13DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark13.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark13.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark13.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark13.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark13.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark13.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark13.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark13.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark13.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark13.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark13.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark13.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark13.S13 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark13
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 91);
        validator.Assert(OneOfUnionSwitch() is 91);
        validator.Assert(DunetUnionSwitch() is 91);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13))
    };

    private static readonly SwitchExecutionBenchmark13DunetUnion[] DunetUnions = new SwitchExecutionBenchmark13DunetUnion[]
    {
        new SwitchExecutionBenchmark13DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark13DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark13DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark13DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark13DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark13DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark13DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark13DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark13DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark13DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark13DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark13DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark13DunetUnion.Variant13(new S13(13))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 13; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 13; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 13; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark14DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark14.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark14.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark14.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark14.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark14.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark14.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark14.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark14.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark14.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark14.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark14.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark14.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark14.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark14.S14 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark14
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 105);
        validator.Assert(OneOfUnionSwitch() is 105);
        validator.Assert(DunetUnionSwitch() is 105);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14))
    };

    private static readonly SwitchExecutionBenchmark14DunetUnion[] DunetUnions = new SwitchExecutionBenchmark14DunetUnion[]
    {
        new SwitchExecutionBenchmark14DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark14DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark14DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark14DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark14DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark14DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark14DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark14DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark14DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark14DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark14DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark14DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark14DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark14DunetUnion.Variant14(new S14(14))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 14; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 14; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 14; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark15DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark15.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark15.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark15.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark15.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark15.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark15.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark15.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark15.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark15.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark15.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark15.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark15.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark15.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark15.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark15.S15 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark15
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 120);
        validator.Assert(OneOfUnionSwitch() is 120);
        validator.Assert(DunetUnionSwitch() is 120);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15))
    };

    private static readonly SwitchExecutionBenchmark15DunetUnion[] DunetUnions = new SwitchExecutionBenchmark15DunetUnion[]
    {
        new SwitchExecutionBenchmark15DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark15DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark15DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark15DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark15DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark15DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark15DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark15DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark15DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark15DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark15DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark15DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark15DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark15DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark15DunetUnion.Variant15(new S15(15))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 15; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 15; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 15; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark16DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark16.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark16.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark16.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark16.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark16.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark16.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark16.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark16.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark16.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark16.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark16.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark16.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark16.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark16.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark16.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark16.S16 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark16
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 136);
        validator.Assert(OneOfUnionSwitch() is 136);
        validator.Assert(DunetUnionSwitch() is 136);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16))
    };

    private static readonly SwitchExecutionBenchmark16DunetUnion[] DunetUnions = new SwitchExecutionBenchmark16DunetUnion[]
    {
        new SwitchExecutionBenchmark16DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark16DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark16DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark16DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark16DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark16DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark16DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark16DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark16DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark16DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark16DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark16DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark16DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark16DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark16DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark16DunetUnion.Variant16(new S16(16))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 16; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 16; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 16; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark17DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark17.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark17.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark17.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark17.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark17.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark17.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark17.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark17.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark17.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark17.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark17.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark17.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark17.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark17.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark17.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark17.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark17.S17 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark17
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 153);
        validator.Assert(OneOfUnionSwitch() is 153);
        validator.Assert(DunetUnionSwitch() is 153);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17))
    };

    private static readonly SwitchExecutionBenchmark17DunetUnion[] DunetUnions = new SwitchExecutionBenchmark17DunetUnion[]
    {
        new SwitchExecutionBenchmark17DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark17DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark17DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark17DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark17DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark17DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark17DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark17DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark17DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark17DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark17DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark17DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark17DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark17DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark17DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark17DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark17DunetUnion.Variant17(new S17(17))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 17; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 17; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 17; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark18DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark18.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark18.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark18.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark18.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark18.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark18.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark18.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark18.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark18.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark18.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark18.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark18.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark18.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark18.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark18.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark18.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark18.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark18.S18 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark18
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 171);
        validator.Assert(OneOfUnionSwitch() is 171);
        validator.Assert(DunetUnionSwitch() is 171);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18))
    };

    private static readonly SwitchExecutionBenchmark18DunetUnion[] DunetUnions = new SwitchExecutionBenchmark18DunetUnion[]
    {
        new SwitchExecutionBenchmark18DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark18DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark18DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark18DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark18DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark18DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark18DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark18DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark18DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark18DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark18DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark18DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark18DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark18DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark18DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark18DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark18DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark18DunetUnion.Variant18(new S18(18))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 18; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 18; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 18; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark19DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark19.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark19.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark19.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark19.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark19.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark19.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark19.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark19.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark19.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark19.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark19.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark19.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark19.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark19.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark19.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark19.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark19.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark19.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark19.S19 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark19
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 190);
        validator.Assert(OneOfUnionSwitch() is 190);
        validator.Assert(DunetUnionSwitch() is 190);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19))
    };

    private static readonly SwitchExecutionBenchmark19DunetUnion[] DunetUnions = new SwitchExecutionBenchmark19DunetUnion[]
    {
        new SwitchExecutionBenchmark19DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark19DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark19DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark19DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark19DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark19DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark19DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark19DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark19DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark19DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark19DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark19DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark19DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark19DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark19DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark19DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark19DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark19DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark19DunetUnion.Variant19(new S19(19))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 19; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 19; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 19; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark20DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark20.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark20.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark20.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark20.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark20.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark20.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark20.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark20.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark20.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark20.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark20.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark20.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark20.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark20.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark20.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark20.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark20.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark20.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark20.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark20.S20 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark20
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 210);
        validator.Assert(OneOfUnionSwitch() is 210);
        validator.Assert(DunetUnionSwitch() is 210);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20))
    };

    private static readonly SwitchExecutionBenchmark20DunetUnion[] DunetUnions = new SwitchExecutionBenchmark20DunetUnion[]
    {
        new SwitchExecutionBenchmark20DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark20DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark20DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark20DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark20DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark20DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark20DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark20DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark20DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark20DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark20DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark20DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark20DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark20DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark20DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark20DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark20DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark20DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark20DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark20DunetUnion.Variant20(new S20(20))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 20; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 20; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 20; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark21DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark21.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark21.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark21.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark21.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark21.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark21.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark21.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark21.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark21.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark21.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark21.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark21.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark21.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark21.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark21.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark21.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark21.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark21.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark21.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark21.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark21.S21 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark21
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 231);
        validator.Assert(OneOfUnionSwitch() is 231);
        validator.Assert(DunetUnionSwitch() is 231);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21))
    };

    private static readonly SwitchExecutionBenchmark21DunetUnion[] DunetUnions = new SwitchExecutionBenchmark21DunetUnion[]
    {
        new SwitchExecutionBenchmark21DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark21DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark21DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark21DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark21DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark21DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark21DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark21DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark21DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark21DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark21DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark21DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark21DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark21DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark21DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark21DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark21DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark21DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark21DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark21DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark21DunetUnion.Variant21(new S21(21))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 21; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 21; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 21; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark22DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark22.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark22.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark22.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark22.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark22.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark22.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark22.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark22.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark22.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark22.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark22.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark22.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark22.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark22.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark22.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark22.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark22.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark22.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark22.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark22.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark22.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark22.S22 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark22
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 253);
        validator.Assert(OneOfUnionSwitch() is 253);
        validator.Assert(DunetUnionSwitch() is 253);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22))
    };

    private static readonly SwitchExecutionBenchmark22DunetUnion[] DunetUnions = new SwitchExecutionBenchmark22DunetUnion[]
    {
        new SwitchExecutionBenchmark22DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark22DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark22DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark22DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark22DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark22DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark22DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark22DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark22DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark22DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark22DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark22DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark22DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark22DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark22DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark22DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark22DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark22DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark22DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark22DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark22DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark22DunetUnion.Variant22(new S22(22))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 22; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 22; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 22; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark23DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark23.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark23.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark23.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark23.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark23.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark23.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark23.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark23.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark23.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark23.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark23.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark23.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark23.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark23.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark23.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark23.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark23.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark23.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark23.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark23.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark23.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark23.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark23.S23 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark23
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 276);
        validator.Assert(OneOfUnionSwitch() is 276);
        validator.Assert(DunetUnionSwitch() is 276);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23))
    };

    private static readonly SwitchExecutionBenchmark23DunetUnion[] DunetUnions = new SwitchExecutionBenchmark23DunetUnion[]
    {
        new SwitchExecutionBenchmark23DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark23DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark23DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark23DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark23DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark23DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark23DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark23DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark23DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark23DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark23DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark23DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark23DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark23DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark23DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark23DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark23DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark23DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark23DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark23DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark23DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark23DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark23DunetUnion.Variant23(new S23(23))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 23; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 23; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 23; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark24DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark24.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark24.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark24.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark24.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark24.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark24.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark24.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark24.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark24.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark24.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark24.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark24.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark24.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark24.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark24.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark24.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark24.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark24.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark24.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark24.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark24.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark24.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark24.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark24.S24 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark24
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 300);
        validator.Assert(OneOfUnionSwitch() is 300);
        validator.Assert(DunetUnionSwitch() is 300);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24))
    };

    private static readonly SwitchExecutionBenchmark24DunetUnion[] DunetUnions = new SwitchExecutionBenchmark24DunetUnion[]
    {
        new SwitchExecutionBenchmark24DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark24DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark24DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark24DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark24DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark24DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark24DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark24DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark24DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark24DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark24DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark24DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark24DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark24DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark24DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark24DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark24DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark24DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark24DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark24DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark24DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark24DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark24DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark24DunetUnion.Variant24(new S24(24))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 24; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 24; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 24; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark25DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark25.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark25.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark25.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark25.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark25.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark25.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark25.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark25.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark25.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark25.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark25.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark25.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark25.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark25.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark25.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark25.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark25.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark25.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark25.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark25.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark25.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark25.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark25.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark25.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark25.S25 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark25
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 325);
        validator.Assert(OneOfUnionSwitch() is 325);
        validator.Assert(DunetUnionSwitch() is 325);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25))
    };

    private static readonly SwitchExecutionBenchmark25DunetUnion[] DunetUnions = new SwitchExecutionBenchmark25DunetUnion[]
    {
        new SwitchExecutionBenchmark25DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark25DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark25DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark25DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark25DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark25DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark25DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark25DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark25DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark25DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark25DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark25DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark25DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark25DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark25DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark25DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark25DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark25DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark25DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark25DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark25DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark25DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark25DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark25DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark25DunetUnion.Variant25(new S25(25))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 25; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 25; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 25; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark26DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark26.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark26.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark26.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark26.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark26.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark26.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark26.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark26.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark26.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark26.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark26.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark26.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark26.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark26.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark26.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark26.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark26.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark26.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark26.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark26.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark26.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark26.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark26.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark26.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark26.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark26.S26 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark26
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 351);
        validator.Assert(OneOfUnionSwitch() is 351);
        validator.Assert(DunetUnionSwitch() is 351);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26))
    };

    private static readonly SwitchExecutionBenchmark26DunetUnion[] DunetUnions = new SwitchExecutionBenchmark26DunetUnion[]
    {
        new SwitchExecutionBenchmark26DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark26DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark26DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark26DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark26DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark26DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark26DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark26DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark26DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark26DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark26DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark26DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark26DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark26DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark26DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark26DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark26DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark26DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark26DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark26DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark26DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark26DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark26DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark26DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark26DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark26DunetUnion.Variant26(new S26(26))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 26; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 26; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 26; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark27DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark27.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark27.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark27.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark27.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark27.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark27.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark27.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark27.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark27.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark27.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark27.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark27.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark27.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark27.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark27.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark27.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark27.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark27.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark27.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark27.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark27.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark27.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark27.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark27.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark27.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark27.S26 Value);
public partial record Variant27(SwitchExecutionBenchmark27.S27 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark27
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 378);
        validator.Assert(OneOfUnionSwitch() is 378);
        validator.Assert(DunetUnionSwitch() is 378);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);
public record struct S27(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26, S27>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26)),
new JanusUnion(new S27(27))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26)),
new OneOfUnion(new S27(27))
    };

    private static readonly SwitchExecutionBenchmark27DunetUnion[] DunetUnions = new SwitchExecutionBenchmark27DunetUnion[]
    {
        new SwitchExecutionBenchmark27DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark27DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark27DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark27DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark27DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark27DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark27DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark27DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark27DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark27DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark27DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark27DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark27DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark27DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark27DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark27DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark27DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark27DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark27DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark27DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark27DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark27DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark27DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark27DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark27DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark27DunetUnion.Variant26(new S26(26)),
new SwitchExecutionBenchmark27DunetUnion.Variant27(new S27(27))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 27; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 27; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 27; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark28DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark28.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark28.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark28.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark28.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark28.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark28.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark28.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark28.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark28.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark28.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark28.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark28.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark28.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark28.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark28.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark28.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark28.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark28.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark28.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark28.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark28.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark28.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark28.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark28.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark28.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark28.S26 Value);
public partial record Variant27(SwitchExecutionBenchmark28.S27 Value);
public partial record Variant28(SwitchExecutionBenchmark28.S28 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark28
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 406);
        validator.Assert(OneOfUnionSwitch() is 406);
        validator.Assert(DunetUnionSwitch() is 406);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);
public record struct S27(int Value);
public record struct S28(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26, S27, S28>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26)),
new JanusUnion(new S27(27)),
new JanusUnion(new S28(28))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26)),
new OneOfUnion(new S27(27)),
new OneOfUnion(new S28(28))
    };

    private static readonly SwitchExecutionBenchmark28DunetUnion[] DunetUnions = new SwitchExecutionBenchmark28DunetUnion[]
    {
        new SwitchExecutionBenchmark28DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark28DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark28DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark28DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark28DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark28DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark28DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark28DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark28DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark28DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark28DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark28DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark28DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark28DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark28DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark28DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark28DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark28DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark28DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark28DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark28DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark28DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark28DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark28DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark28DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark28DunetUnion.Variant26(new S26(26)),
new SwitchExecutionBenchmark28DunetUnion.Variant27(new S27(27)),
new SwitchExecutionBenchmark28DunetUnion.Variant28(new S28(28))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 28; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 28; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 28; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark29DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark29.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark29.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark29.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark29.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark29.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark29.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark29.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark29.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark29.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark29.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark29.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark29.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark29.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark29.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark29.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark29.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark29.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark29.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark29.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark29.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark29.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark29.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark29.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark29.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark29.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark29.S26 Value);
public partial record Variant27(SwitchExecutionBenchmark29.S27 Value);
public partial record Variant28(SwitchExecutionBenchmark29.S28 Value);
public partial record Variant29(SwitchExecutionBenchmark29.S29 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark29
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 435);
        validator.Assert(OneOfUnionSwitch() is 435);
        validator.Assert(DunetUnionSwitch() is 435);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);
public record struct S27(int Value);
public record struct S28(int Value);
public record struct S29(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26, S27, S28, S29>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26)),
new JanusUnion(new S27(27)),
new JanusUnion(new S28(28)),
new JanusUnion(new S29(29))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26)),
new OneOfUnion(new S27(27)),
new OneOfUnion(new S28(28)),
new OneOfUnion(new S29(29))
    };

    private static readonly SwitchExecutionBenchmark29DunetUnion[] DunetUnions = new SwitchExecutionBenchmark29DunetUnion[]
    {
        new SwitchExecutionBenchmark29DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark29DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark29DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark29DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark29DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark29DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark29DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark29DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark29DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark29DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark29DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark29DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark29DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark29DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark29DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark29DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark29DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark29DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark29DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark29DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark29DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark29DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark29DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark29DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark29DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark29DunetUnion.Variant26(new S26(26)),
new SwitchExecutionBenchmark29DunetUnion.Variant27(new S27(27)),
new SwitchExecutionBenchmark29DunetUnion.Variant28(new S28(28)),
new SwitchExecutionBenchmark29DunetUnion.Variant29(new S29(29))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 29; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 29; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 29; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark30DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark30.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark30.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark30.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark30.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark30.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark30.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark30.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark30.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark30.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark30.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark30.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark30.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark30.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark30.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark30.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark30.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark30.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark30.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark30.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark30.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark30.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark30.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark30.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark30.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark30.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark30.S26 Value);
public partial record Variant27(SwitchExecutionBenchmark30.S27 Value);
public partial record Variant28(SwitchExecutionBenchmark30.S28 Value);
public partial record Variant29(SwitchExecutionBenchmark30.S29 Value);
public partial record Variant30(SwitchExecutionBenchmark30.S30 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark30
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 465);
        validator.Assert(OneOfUnionSwitch() is 465);
        validator.Assert(DunetUnionSwitch() is 465);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);
public record struct S27(int Value);
public record struct S28(int Value);
public record struct S29(int Value);
public record struct S30(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29, S30>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29, S30> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26, S27, S28, S29, S30>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26)),
new JanusUnion(new S27(27)),
new JanusUnion(new S28(28)),
new JanusUnion(new S29(29)),
new JanusUnion(new S30(30))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26)),
new OneOfUnion(new S27(27)),
new OneOfUnion(new S28(28)),
new OneOfUnion(new S29(29)),
new OneOfUnion(new S30(30))
    };

    private static readonly SwitchExecutionBenchmark30DunetUnion[] DunetUnions = new SwitchExecutionBenchmark30DunetUnion[]
    {
        new SwitchExecutionBenchmark30DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark30DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark30DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark30DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark30DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark30DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark30DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark30DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark30DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark30DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark30DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark30DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark30DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark30DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark30DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark30DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark30DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark30DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark30DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark30DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark30DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark30DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark30DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark30DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark30DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark30DunetUnion.Variant26(new S26(26)),
new SwitchExecutionBenchmark30DunetUnion.Variant27(new S27(27)),
new SwitchExecutionBenchmark30DunetUnion.Variant28(new S28(28)),
new SwitchExecutionBenchmark30DunetUnion.Variant29(new S29(29)),
new SwitchExecutionBenchmark30DunetUnion.Variant30(new S30(30))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 30; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 30; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 30; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark31DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark31.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark31.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark31.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark31.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark31.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark31.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark31.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark31.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark31.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark31.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark31.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark31.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark31.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark31.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark31.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark31.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark31.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark31.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark31.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark31.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark31.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark31.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark31.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark31.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark31.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark31.S26 Value);
public partial record Variant27(SwitchExecutionBenchmark31.S27 Value);
public partial record Variant28(SwitchExecutionBenchmark31.S28 Value);
public partial record Variant29(SwitchExecutionBenchmark31.S29 Value);
public partial record Variant30(SwitchExecutionBenchmark31.S30 Value);
public partial record Variant31(SwitchExecutionBenchmark31.S31 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark31
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 496);
        validator.Assert(OneOfUnionSwitch() is 496);
        validator.Assert(DunetUnionSwitch() is 496);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);
public record struct S27(int Value);
public record struct S28(int Value);
public record struct S29(int Value);
public record struct S30(int Value);
public record struct S31(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29, S30, S31>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29, S30, S31> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26, S27, S28, S29, S30, S31>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26)),
new JanusUnion(new S27(27)),
new JanusUnion(new S28(28)),
new JanusUnion(new S29(29)),
new JanusUnion(new S30(30)),
new JanusUnion(new S31(31))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26)),
new OneOfUnion(new S27(27)),
new OneOfUnion(new S28(28)),
new OneOfUnion(new S29(29)),
new OneOfUnion(new S30(30)),
new OneOfUnion(new S31(31))
    };

    private static readonly SwitchExecutionBenchmark31DunetUnion[] DunetUnions = new SwitchExecutionBenchmark31DunetUnion[]
    {
        new SwitchExecutionBenchmark31DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark31DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark31DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark31DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark31DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark31DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark31DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark31DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark31DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark31DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark31DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark31DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark31DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark31DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark31DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark31DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark31DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark31DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark31DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark31DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark31DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark31DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark31DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark31DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark31DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark31DunetUnion.Variant26(new S26(26)),
new SwitchExecutionBenchmark31DunetUnion.Variant27(new S27(27)),
new SwitchExecutionBenchmark31DunetUnion.Variant28(new S28(28)),
new SwitchExecutionBenchmark31DunetUnion.Variant29(new S29(29)),
new SwitchExecutionBenchmark31DunetUnion.Variant30(new S30(30)),
new SwitchExecutionBenchmark31DunetUnion.Variant31(new S31(31))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 31; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 31; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 31; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}
[Union]
partial record SwitchExecutionBenchmark32DunetUnion
{
    public partial record Variant1(SwitchExecutionBenchmark32.S1 Value);
public partial record Variant2(SwitchExecutionBenchmark32.S2 Value);
public partial record Variant3(SwitchExecutionBenchmark32.S3 Value);
public partial record Variant4(SwitchExecutionBenchmark32.S4 Value);
public partial record Variant5(SwitchExecutionBenchmark32.S5 Value);
public partial record Variant6(SwitchExecutionBenchmark32.S6 Value);
public partial record Variant7(SwitchExecutionBenchmark32.S7 Value);
public partial record Variant8(SwitchExecutionBenchmark32.S8 Value);
public partial record Variant9(SwitchExecutionBenchmark32.S9 Value);
public partial record Variant10(SwitchExecutionBenchmark32.S10 Value);
public partial record Variant11(SwitchExecutionBenchmark32.S11 Value);
public partial record Variant12(SwitchExecutionBenchmark32.S12 Value);
public partial record Variant13(SwitchExecutionBenchmark32.S13 Value);
public partial record Variant14(SwitchExecutionBenchmark32.S14 Value);
public partial record Variant15(SwitchExecutionBenchmark32.S15 Value);
public partial record Variant16(SwitchExecutionBenchmark32.S16 Value);
public partial record Variant17(SwitchExecutionBenchmark32.S17 Value);
public partial record Variant18(SwitchExecutionBenchmark32.S18 Value);
public partial record Variant19(SwitchExecutionBenchmark32.S19 Value);
public partial record Variant20(SwitchExecutionBenchmark32.S20 Value);
public partial record Variant21(SwitchExecutionBenchmark32.S21 Value);
public partial record Variant22(SwitchExecutionBenchmark32.S22 Value);
public partial record Variant23(SwitchExecutionBenchmark32.S23 Value);
public partial record Variant24(SwitchExecutionBenchmark32.S24 Value);
public partial record Variant25(SwitchExecutionBenchmark32.S25 Value);
public partial record Variant26(SwitchExecutionBenchmark32.S26 Value);
public partial record Variant27(SwitchExecutionBenchmark32.S27 Value);
public partial record Variant28(SwitchExecutionBenchmark32.S28 Value);
public partial record Variant29(SwitchExecutionBenchmark32.S29 Value);
public partial record Variant30(SwitchExecutionBenchmark32.S30 Value);
public partial record Variant31(SwitchExecutionBenchmark32.S31 Value);
public partial record Variant32(SwitchExecutionBenchmark32.S32 Value);
}

[SimpleJob]
public partial class SwitchExecutionBenchmark32
{
    public void Validate(Validator validator)
    {
        validator.Assert(JanusUnionSwitch() is 528);
        validator.Assert(OneOfUnionSwitch() is 528);
        validator.Assert(DunetUnionSwitch() is 528);
    }

    public record struct S1(int Value);
public record struct S2(int Value);
public record struct S3(int Value);
public record struct S4(int Value);
public record struct S5(int Value);
public record struct S6(int Value);
public record struct S7(int Value);
public record struct S8(int Value);
public record struct S9(int Value);
public record struct S10(int Value);
public record struct S11(int Value);
public record struct S12(int Value);
public record struct S13(int Value);
public record struct S14(int Value);
public record struct S15(int Value);
public record struct S16(int Value);
public record struct S17(int Value);
public record struct S18(int Value);
public record struct S19(int Value);
public record struct S20(int Value);
public record struct S21(int Value);
public record struct S22(int Value);
public record struct S23(int Value);
public record struct S24(int Value);
public record struct S25(int Value);
public record struct S26(int Value);
public record struct S27(int Value);
public record struct S28(int Value);
public record struct S29(int Value);
public record struct S30(int Value);
public record struct S31(int Value);
public record struct S32(int Value);

    class OneOfUnion : OneOfBase<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29, S30, S31, S32>
    {
        public OneOfUnion(OneOf<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20, S21, S22, S23, S24, S25, S26, S27, S28, S29, S30, S31, S32> input) : base(input) {}
    }

    [RhoMicro.CodeAnalysis.UnionType<S1, S2, S3, S4, S5, S6, S7, S8>]
[RhoMicro.CodeAnalysis.UnionType<S9, S10, S11, S12, S13, S14, S15, S16>]
[RhoMicro.CodeAnalysis.UnionType<S17, S18, S19, S20, S21, S22, S23, S24>]
[RhoMicro.CodeAnalysis.UnionType<S25, S26, S27, S28, S29, S30, S31, S32>]
    sealed partial class JanusUnion;

    private static readonly JanusUnion[] JanusUnions = new JanusUnion[] 
    {
        new JanusUnion(new S1(1)),
new JanusUnion(new S2(2)),
new JanusUnion(new S3(3)),
new JanusUnion(new S4(4)),
new JanusUnion(new S5(5)),
new JanusUnion(new S6(6)),
new JanusUnion(new S7(7)),
new JanusUnion(new S8(8)),
new JanusUnion(new S9(9)),
new JanusUnion(new S10(10)),
new JanusUnion(new S11(11)),
new JanusUnion(new S12(12)),
new JanusUnion(new S13(13)),
new JanusUnion(new S14(14)),
new JanusUnion(new S15(15)),
new JanusUnion(new S16(16)),
new JanusUnion(new S17(17)),
new JanusUnion(new S18(18)),
new JanusUnion(new S19(19)),
new JanusUnion(new S20(20)),
new JanusUnion(new S21(21)),
new JanusUnion(new S22(22)),
new JanusUnion(new S23(23)),
new JanusUnion(new S24(24)),
new JanusUnion(new S25(25)),
new JanusUnion(new S26(26)),
new JanusUnion(new S27(27)),
new JanusUnion(new S28(28)),
new JanusUnion(new S29(29)),
new JanusUnion(new S30(30)),
new JanusUnion(new S31(31)),
new JanusUnion(new S32(32))
    };
    
    private static readonly OneOfUnion[] OneOfUnions = new OneOfUnion[] 
    {
        new OneOfUnion(new S1(1)),
new OneOfUnion(new S2(2)),
new OneOfUnion(new S3(3)),
new OneOfUnion(new S4(4)),
new OneOfUnion(new S5(5)),
new OneOfUnion(new S6(6)),
new OneOfUnion(new S7(7)),
new OneOfUnion(new S8(8)),
new OneOfUnion(new S9(9)),
new OneOfUnion(new S10(10)),
new OneOfUnion(new S11(11)),
new OneOfUnion(new S12(12)),
new OneOfUnion(new S13(13)),
new OneOfUnion(new S14(14)),
new OneOfUnion(new S15(15)),
new OneOfUnion(new S16(16)),
new OneOfUnion(new S17(17)),
new OneOfUnion(new S18(18)),
new OneOfUnion(new S19(19)),
new OneOfUnion(new S20(20)),
new OneOfUnion(new S21(21)),
new OneOfUnion(new S22(22)),
new OneOfUnion(new S23(23)),
new OneOfUnion(new S24(24)),
new OneOfUnion(new S25(25)),
new OneOfUnion(new S26(26)),
new OneOfUnion(new S27(27)),
new OneOfUnion(new S28(28)),
new OneOfUnion(new S29(29)),
new OneOfUnion(new S30(30)),
new OneOfUnion(new S31(31)),
new OneOfUnion(new S32(32))
    };

    private static readonly SwitchExecutionBenchmark32DunetUnion[] DunetUnions = new SwitchExecutionBenchmark32DunetUnion[]
    {
        new SwitchExecutionBenchmark32DunetUnion.Variant1(new S1(1)),
new SwitchExecutionBenchmark32DunetUnion.Variant2(new S2(2)),
new SwitchExecutionBenchmark32DunetUnion.Variant3(new S3(3)),
new SwitchExecutionBenchmark32DunetUnion.Variant4(new S4(4)),
new SwitchExecutionBenchmark32DunetUnion.Variant5(new S5(5)),
new SwitchExecutionBenchmark32DunetUnion.Variant6(new S6(6)),
new SwitchExecutionBenchmark32DunetUnion.Variant7(new S7(7)),
new SwitchExecutionBenchmark32DunetUnion.Variant8(new S8(8)),
new SwitchExecutionBenchmark32DunetUnion.Variant9(new S9(9)),
new SwitchExecutionBenchmark32DunetUnion.Variant10(new S10(10)),
new SwitchExecutionBenchmark32DunetUnion.Variant11(new S11(11)),
new SwitchExecutionBenchmark32DunetUnion.Variant12(new S12(12)),
new SwitchExecutionBenchmark32DunetUnion.Variant13(new S13(13)),
new SwitchExecutionBenchmark32DunetUnion.Variant14(new S14(14)),
new SwitchExecutionBenchmark32DunetUnion.Variant15(new S15(15)),
new SwitchExecutionBenchmark32DunetUnion.Variant16(new S16(16)),
new SwitchExecutionBenchmark32DunetUnion.Variant17(new S17(17)),
new SwitchExecutionBenchmark32DunetUnion.Variant18(new S18(18)),
new SwitchExecutionBenchmark32DunetUnion.Variant19(new S19(19)),
new SwitchExecutionBenchmark32DunetUnion.Variant20(new S20(20)),
new SwitchExecutionBenchmark32DunetUnion.Variant21(new S21(21)),
new SwitchExecutionBenchmark32DunetUnion.Variant22(new S22(22)),
new SwitchExecutionBenchmark32DunetUnion.Variant23(new S23(23)),
new SwitchExecutionBenchmark32DunetUnion.Variant24(new S24(24)),
new SwitchExecutionBenchmark32DunetUnion.Variant25(new S25(25)),
new SwitchExecutionBenchmark32DunetUnion.Variant26(new S26(26)),
new SwitchExecutionBenchmark32DunetUnion.Variant27(new S27(27)),
new SwitchExecutionBenchmark32DunetUnion.Variant28(new S28(28)),
new SwitchExecutionBenchmark32DunetUnion.Variant29(new S29(29)),
new SwitchExecutionBenchmark32DunetUnion.Variant30(new S30(30)),
new SwitchExecutionBenchmark32DunetUnion.Variant31(new S31(31)),
new SwitchExecutionBenchmark32DunetUnion.Variant32(new S32(32))
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 JanusUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 32; i++)
        {
            var union = JanusUnions[i]; 
            result += union.Switch(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 OneOfUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 32; i++)
        {
            var union = OneOfUnions[i]; 
            result += union.Match(
                static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value,
static s => s.Value
            );
        }
        
        return result;
    }
    
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Int32 DunetUnionSwitch()
    {
        var result = 0;
        
        for(var i = 0; i < 32; i++)
        {
            var union = DunetUnions[i]; 
            result += union.Match(
                static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value,
static s => s.Value.Value
            );
        }
        
        return result;
    }
}

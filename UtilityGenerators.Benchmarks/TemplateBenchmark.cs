#pragma warning disable

namespace RhoMicro.CodeAnalysis.Benchmarks;

using System;
using System.Text;

using BenchmarkDotNet.Attributes;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Library.Text.Templating;

[LongRunJob]
[MemoryDiagnoser]
public partial class TemplateBenchmark
{
    [Template("Colleen Ballinger Uwu delulu opium bird Ambatukam No nut November With Subway\n")]
    private readonly partial struct TemplateText;
    [Template(
        """
        Surfers gameplay at the bottom Sussy imposter Pibby glitch in real life No
        (:Parameter:)
        """)]
    private readonly partial record struct TemplateTextRender(String Parameter);
    [Template(
        """
        Surfers gameplay at the bottom Sussy imposter Pibby glitch in real life No
        {:
            for(var i = 0; i < 10; i++)
                (:Parameter:)
        :}
        """)]
    private readonly partial record struct TemplateTextCodeRender(String Parameter);
    [Template(
        """
        edging in class Based OceanGate T-pose Kai Cenat fanum tax Ambatukam
        (:Child:)
        """, BodyParameterName = "Child")]
    private readonly partial struct TemplateChildRender;
    [Template(
        """
        {:
            static (:new TemplateChildRender():)
            <:(:new TemplateTextRender("Nickeh30 ratio F in the chat Sin City Monday Foot fetish I like ya cut G I"):):>
        :}
        (:new TemplateText():)
        (:new TemplateTextCodeRender("compilation I love lean Gassy No nut November Smurf cat vs strawberry"):)
        """)]
    private readonly partial struct TemplateTemplate;

    [Benchmark(Baseline = true)]
    public String Template() => new TemplateTemplate().ToString();

    private readonly struct StringBuilderText
    {
        public void AppendTo(StringBuilder builder)
        {
            builder.Append("Colleen Ballinger Uwu delulu opium bird Ambatukam No nut November With Subway\n");
        }
    }
    private readonly record struct StringBuilderTextCodeRender(String Parameter)
    {
        public void AppendTo(StringBuilder builder)
        {
            builder.Append("Surfers gameplay at the bottom Sussy imposter Pibby glitch in real life No").Append('\n');
            for(var i = 0; i < 10; i++)
                builder.Append(Parameter);
        }
    }
    private readonly record struct StringBuilderTextRender(String Parameter)
    {
        public void AppendTo(StringBuilder builder)
        {
            builder
                .Append("Surfers gameplay at the bottom Sussy imposter Pibby glitch in real life No")
                .Append('\n')
                .Append(Parameter);
        }
    }
    private readonly struct StringBuilderChildRender
    {
        public void AppendTo(StringBuilder builder, Action<StringBuilder> fragment)
        {
            builder.Append("edging in class Based OceanGate T-pose Kai Cenat fanum tax Ambatukam").Append('\n');
            fragment.Invoke(builder);
        }
    }
    private readonly struct StringBuilderTemplate
    {
        public void AppendTo(StringBuilder builder)
        {
            static void fragment(StringBuilder builder)
            {
                new StringBuilderTextRender("Nickeh30 ratio F in the chat Sin City Monday Foot fetish I like ya cut G I").AppendTo(builder);
            }

            new StringBuilderChildRender().AppendTo(builder, fragment);
            new StringBuilderText().AppendTo(builder);
            builder.Append('\n');
            new StringBuilderTextCodeRender("compilation I love lean Gassy No nut November Smurf cat vs strawberry").AppendTo(builder);
        }
    }
    [Benchmark]
    public String StringBuilder()
    {
        var builder = new StringBuilder();
        new StringBuilderTemplate().AppendTo(builder);
        return builder.ToString();
    }
}

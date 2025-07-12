// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Benchmarks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using static TokenType;

public enum TokenType
{
    A, B, C, D
}

[SimpleJob]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ParserMatchBenchmark
{
    private TokenType[] _types;

    [GlobalSetup]
    public void GlobalSetup() => _types = [A, B, C, D];

    [Benchmark]
    public Int32 MatchAnyNaive()
    {
        var result = 0;

        foreach(var peek in _types)
        {
            if(MatchAnyNaive(peek, _types[0]))
                result++;
            if(MatchAnyNaive(peek, _types[1]))
                result++;
            if(MatchAnyNaive(peek, _types[2]))
                result++;
            if(MatchAnyNaive(peek, _types[3]))
                result++;
            if(MatchAnyNaive(peek, _types[0], _types[1]))
                result++;
            if(MatchAnyNaive(peek, _types[2], _types[3]))
                result++;
            if(MatchAnyNaive(peek, _types[0], _types[1], _types[2], _types[3]))
                result++;
        }

        return result;
    }
    [Benchmark(Baseline = true)]
    public Int32 MatchAnyTernary()
    {
        var result = 0;

        foreach(var peek in _types)
        {
            if(MatchAnyTernary(peek, _types[0]))
                result++;
            if(MatchAnyTernary(peek, _types[1]))
                result++;
            if(MatchAnyTernary(peek, _types[2]))
                result++;
            if(MatchAnyTernary(peek, _types[3]))
                result++;
            if(MatchAnyTernary(peek, _types[0], _types[1]))
                result++;
            if(MatchAnyTernary(peek, _types[2], _types[3]))
                result++;
            if(MatchAnyTernary(peek, _types[0], _types[1], _types[2], _types[3]))
                result++;
        }

        return result;
    }

    private Boolean MatchAnyNaive(TokenType peek, params ReadOnlySpan<TokenType> types)
    {
        foreach(var t in types)
        {
            if(t == peek)
                return true;
        }

        return false;
    }

    private Boolean MatchAnyTernary(TokenType type1, TokenType peek)
    {
        var result = 0;
        result = ( result == 1 || peek == type1 ) ? 1 : 0;

        return result == 1;
    }
    private Boolean MatchAnyTernary(TokenType type1, TokenType type2, TokenType peek)
    {
        var result = 0;
        result = ( result == 1 || peek == type1 ) ? 1 : 0;
        result = ( result == 1 || peek == type2 ) ? 1 : 0;

        return result == 1;
    }
    private Boolean MatchAnyTernary(TokenType type1, TokenType type2, TokenType type3, TokenType peek)
    {
        var result = 0;
        result = ( result == 1 || peek == type1 ) ? 1 : 0;
        result = ( result == 1 || peek == type2 ) ? 1 : 0;
        result = ( result == 1 || peek == type3 ) ? 1 : 0;

        return result == 1;
    }
    private Boolean MatchAnyTernary(TokenType type1, TokenType type2, TokenType type3, TokenType type4, TokenType peek)
    {
        var result = 0;
        result = ( result == 1 || peek == type1 ) ? 1 : 0;
        result = ( result == 1 || peek == type2 ) ? 1 : 0;
        result = ( result == 1 || peek == type3 ) ? 1 : 0;
        result = ( result == 1 || peek == type4 ) ? 1 : 0;

        return result == 1;
    }
}

// SPDX-License-Identifier: MPL-2.0

#pragma warning disable

namespace RhoMicro.CodeAnalysis.Benchmarks;
using System;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using Microsoft.Diagnostics.Tracing.StackSources;

/// <summary>
/// Benchmarks different implementations for checking if a character is a template block terminator.
/// </summary>
[SimpleJob]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class BlockTerminatorsBenchmark
{
    private static partial class BlockTerminators
    {
        // We represent block terminators as single bits in a bitmask:
        //   (((UInt128)1) << '{')
        // | (((UInt128)1) << '}')
        // | (((UInt128)1) << '(')
        // | (((UInt128)1) << ')')
        // | (((UInt128)1) << '<')
        // | (((UInt128)1) << '>')
        // =>
        // 0010 1000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0101 0000 0000 0000 0000 0011 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000
        // =>
        // high bits:
        // 0010 1000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000
        // low bits:
        // 0101 0000 0000 0000 0000 0011 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000
        private static readonly UInt64 _lower = 0b0101000000000000000000110000000000000000000000000000000000000000;
        private static readonly UInt64 _upper = 0b0010100000000000000000000000000000000000000000000000000000000000;

        private static readonly Char[] _orderedChars = ['(', ')', '<', '>', '{', '}'];

        /// <summary>
        /// Gets a value indicating whether <paramref name="c"/> is a block terminator.
        /// </summary>
        /// <param name="c">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="c"/> is a block terminator; otherwise, <see langword="false"/>.
        /// </returns>
        public static Boolean IsBlockTerminator(Char c) => c <= 127 && ( c > 63 ? ( _upper & 1ul << c ) != 0 : ( _lower & 1ul << c ) != 0 );
        public static Boolean BinarySearch(Char c) => Array.BinarySearch(_orderedChars, c) >= 0;
        public static Boolean LinearSearch(Char c)
        {
            for(var i = 0; i < _orderedChars.Length; i++)
            {
                if(_orderedChars[i] == c)
                    return true;
            }

            return false;
        }
        public static Boolean LinearSearchUnrolled(Char c)
        {
            if(c == '(')
                return true;

            if(c == ')')
                return true;

            if(c == '<')
                return true;

            if(c == '>')
                return true;

            if(c == '{')
                return true;

            if(c == '}')
                return true;

            return false;
        }
        public static Boolean LinearSearchUnrolledConditions(Char c)
        {
            var result = false;

            if(c == '(')
                result = true;

            if(c == ')')
                result = true;

            if(c == '<')
                result = true;

            if(c == '>')
                result = true;

            if(c == '{')
                result = true;

            if(c == '}')
                result = true;

            return result;
        }
        public static Boolean LinearSearchUnrolledShortCircuiting(Char c)
        {
#pragma warning disable IDE0078 // Use pattern matching
            var result =
                c == '(' ||
                c == ')' ||
                c == '<' ||
                c == '>' ||
                c == '{' ||
                c == '}';
#pragma warning restore IDE0078 // Use pattern matching

            return result;
        }
        public static Boolean LinearSearchUnrolledPatternMatching(Char c)
        {
            var result =
                c is '('
                or ')'
                or '<'
                or '>'
                or '{'
                or '}';

            return result;
        }
        public static Boolean LinearSearchUnrolledTernary(Char c)
        {
            var result = 0;

            result = ( result == 1 || c == '(' ) ? 1 : 0;
            result = ( result == 1 || c == ')' ) ? 1 : 0;
            result = ( result == 1 || c == '<' ) ? 1 : 0;
            result = ( result == 1 || c == '>' ) ? 1 : 0;
            result = ( result == 1 || c == '{' ) ? 1 : 0;
            result = ( result == 1 || c == '}' ) ? 1 : 0;

            return result == 1;
        }
        public static Boolean LinearSearchUnrolledTernary3(Char c)
        {
            var result = 0;

            result = ( c == '(' ) ? 1 : result;
            result = ( c == ')' ) ? 1 : result;
            result = ( c == '<' ) ? 1 : result;
            result = ( c == '>' ) ? 1 : result;
            result = ( c == '{' ) ? 1 : result;
            result = ( c == '}' ) ? 1 : result;

            return result == 1;
        }
        public static Boolean LinearSearchUnrolledTernary2(Char c)
        {
            var result = 0;

            result |= c == '(' ? 1 : 0;
            result |= c == ')' ? 1 : 0;
            result |= c == '<' ? 1 : 0;
            result |= c == '>' ? 1 : 0;
            result |= c == '{' ? 1 : 0;
            result |= c == '}' ? 1 : 0;

            return result == 1;
        }
    }

    [Params(1000)]
    public Int32 Size { get; set; } = 1000;

    [Params(1, 2, 3)]
    public Int32 Seed { get; set; } = 1;

    private Char[] _characters;
    public Int32 Count { get; private set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rng = new Random(Seed);
        var testData = String.Concat(Enumerable.Range(0, Size).Select(i => (Char)rng.Next(' ', '~')));
        _characters = testData.ToCharArray();
        Count = testData.Where(BlockTerminators.LinearSearchUnrolledTernary).Count();
    }

    [Benchmark]
    public Int32 IsBlockTerminator()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.IsBlockTerminator(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 BinarySearch()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.BinarySearch(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearch()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearch(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearchUnrolled()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolled(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearchUnrolledConditions()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolledConditions(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearchUnrolledShortCircuiting()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolledShortCircuiting(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearchUnrolledPatternMatching()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolledPatternMatching(c) ? 1 : 0;
        return result;
    }
    [Benchmark(Baseline = true)]
    public Int32 LinearSearchUnrolledTernary()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolledTernary(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearchUnrolledTernary2()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolledTernary2(c) ? 1 : 0;
        return result;
    }
    [Benchmark]
    public Int32 LinearSearchUnrolledTernary3()
    {
        var result = 0;
        foreach(var c in _characters)
            result += BlockTerminators.LinearSearchUnrolledTernary3(c) ? 1 : 0;
        return result;
    }
}



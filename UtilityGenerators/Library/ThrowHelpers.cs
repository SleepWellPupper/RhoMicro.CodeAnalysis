// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace RhoMicro.CodeAnalysis.Library;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides helpers for throwing exceptions.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal static class ThrowHelpers
{
    internal static class ArgumentNullException
    {
        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
        /// <param name="argument">The reference type argument to validate as non-null.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        internal static void ThrowIfNull([NotNull] Object? argument, [CallerArgumentExpression(nameof(argument))] String? paramName = null)
        {
            if(argument is null)
                Throw(paramName);
        }

        [DoesNotReturn]
        internal static void Throw(String? paramName) =>
            throw new System.ArgumentNullException(paramName);
    }
    internal static class ArgumentOutOfRangeException
    {
        [DoesNotReturn]
        private static void ThrowZero<T>(T value, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-zero value.");

        [DoesNotReturn]
        private static void ThrowNegative<T>(T value, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative value.");

        [DoesNotReturn]
        private static void ThrowNegativeOrZero<T>(T value, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative and non-zero value.");

        [DoesNotReturn]
        private static void ThrowGreater<T>(T value, T other, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be less than or equal to '{other}'.");

        [DoesNotReturn]
        private static void ThrowGreaterEqual<T>(T value, T other, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be less than '{other}'.");

        [DoesNotReturn]
        private static void ThrowLess<T>(T value, T other, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be greater than or equal to '{other}'.");

        [DoesNotReturn]
        private static void ThrowLessEqual<T>(T value, T other, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be greater than '{other}'.");

        [DoesNotReturn]
        private static void ThrowEqual<T>(T value, T other, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value?.ToString() ?? "null"}') must not be equal to '{other?.ToString() ?? "null"}'.");

        [DoesNotReturn]
        private static void ThrowNotEqual<T>(T value, T other, String? paramName) =>
            throw new System.ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value?.ToString() ?? "null"}') must be equal to '{other?.ToString() ?? "null"}'.");

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(Single value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(Double value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(SByte value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(Int16 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(Int32 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(Int64 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(Byte value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(UInt16 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(UInt32 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfZero(UInt64 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value == 0)
                ThrowZero(value, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegative(Single value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value < 0)
                ThrowNegative(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegative(Double value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value < 0)
                ThrowNegative(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegative(SByte value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value < 0)
                ThrowNegative(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegative(Int16 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value < 0)
                ThrowNegative(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegative(Int32 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value < 0)
                ThrowNegative(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegative(Int64 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value < 0)
                ThrowNegative(value, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegativeOrZero(Single value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value <= 0)
                ThrowNegativeOrZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegativeOrZero(Double value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value <= 0)
                ThrowNegativeOrZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegativeOrZero(SByte value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value <= 0)
                ThrowNegativeOrZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegativeOrZero(Int16 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value <= 0)
                ThrowNegativeOrZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegativeOrZero(Int32 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value <= 0)
                ThrowNegativeOrZero(value, paramName);
        }
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNegativeOrZero(Int64 value, [CallerArgumentExpression(nameof(value))] String? paramName = null)
        {
            if(value <= 0)
                ThrowNegativeOrZero(value, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] String? paramName = null) where T : IEquatable<T>?
        {
            if(EqualityComparer<T>.Default.Equals(value, other))
                ThrowEqual(value, other, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfNotEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] String? paramName = null) where T : IEquatable<T>?
        {
            if(!EqualityComparer<T>.Default.Equals(value, other))
                ThrowNotEqual(value, other, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] String? paramName = null)
            where T : IComparable<T>
        {
            if(value.CompareTo(other) > 0)
                ThrowGreater(value, other, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] String? paramName = null)
            where T : IComparable<T>
        {
            if(value.CompareTo(other) >= 0)
                ThrowGreaterEqual(value, other, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as greatar than or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] String? paramName = null)
            where T : IComparable<T>
        {
            if(value.CompareTo(other) < 0)
                ThrowLess(value, other, paramName);
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as greatar than than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        internal static void ThrowIfLessThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] String? paramName = null)
            where T : IComparable<T>
        {
            if(value.CompareTo(other) <= 0)
                ThrowLessEqual(value, other, paramName);
        }
    }
}

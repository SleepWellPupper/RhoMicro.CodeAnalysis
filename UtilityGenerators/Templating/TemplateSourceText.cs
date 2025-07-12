// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a template source text.
/// </summary>
internal readonly struct TemplateSourceText : IEquatable<TemplateSourceText>
{
    private TemplateSourceText(Boolean isLazy, Object value)
    {
        _isLazy = isLazy;
        _value = value;
    }

    private readonly Boolean _isLazy;
    private readonly Object _value;

    public static implicit operator TemplateSourceText(String value) => new(false, value);
    public static implicit operator TemplateSourceText(Func<String> value) => new(true, value);

    public static Boolean operator ==(TemplateSourceText left, TemplateSourceText right) => left.Equals(right);
    public static Boolean operator !=(TemplateSourceText left, TemplateSourceText right) => !( left == right );

    public override String ToString() => _isLazy ? ( (Func<String>)_value ).Invoke() : (String)_value;
    public override Boolean Equals(Object? obj) => obj is TemplateSourceText text && Equals(text);
    public Boolean Equals(TemplateSourceText other) => _isLazy == other._isLazy && EqualityComparer<Object>.Default.Equals(_value, other._value);
    public override Int32 GetHashCode() => HashCode.Combine(_isLazy, _value);
}

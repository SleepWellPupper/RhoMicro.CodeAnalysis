// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
partial class CSharpSourceBuilder
{
    /// <summary>
    /// Interpolated string handler for supporting interpolated strings on <c>Append</c> methods efficiently.
    /// This type is not intended to be used directly and doing so may corrupt the builders state.
    /// </summary>
    /// <param name="literalLength"></param>
    /// <param name="formattedCount"></param>
    /// <param name="writer"></param>
    [InterpolatedStringHandler, EditorBrowsable(EditorBrowsableState.Never)]
    public partial struct InterpolatedStringHandler(
#pragma warning disable CS9113 // Parameter is unread. Needed for interpolated string handler pattern.
            Int32 literalLength,
            Int32 formattedCount,
#pragma warning restore CS9113 // Parameter is unread.
            CSharpSourceBuilder writer) : IDisposable
    {
        private Boolean _isIndented;

        /// <summary>
        /// Appends a literal to the builder.
        /// </summary>
        /// <param name="literal">
        /// The literal to append.
        /// </param>
        public void AppendLiteral(String literal)
        {
            if (!writer._appendCondition)
            {
                return;
            }

            TryDetent();
            writer.AppendCore(literal, out var lastLineStartText);

            if (!writer._detector.TryDetectIndentation(
                        lastLineStartText,
                        writer._indentationDetectorContext,
                        out var indentation))
            {
                return;
            }

            writer.Indent(indentation);
            _isIndented = true;
        }

        /// <summary>
        /// Formats and appends a placeholder to the builder.
        /// </summary>
        /// <param name="placeholder">
        /// The placeholder to append.
        /// </param>
        /// <param name="invokeDefaultLogic">
        /// Indicates whether to invoke the default implementation of <see cref="AppendFormatted{T}(T)"/>.
        /// Implementers of this method should set this parameter to <see langword="false"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type of the placeholder to format and append. 
        /// </typeparam>
        partial void AppendFormatted<T>(T placeholder, ref Boolean invokeDefaultLogic);

        /// <summary>
        /// Formats and appends a placeholder to the builder.
        /// </summary>
        /// <param name="placeholder">
        /// The placeholder to append.
        /// </param>
        public void AppendFormatted(String placeholder)
        {
            writer.Append(placeholder);
        }

        /// <inheritdoc cref="AppendFormatted(string)"/>
        public void AppendFormatted(Type placeholder)
        {
            writer.AppendTypeName(placeholder);
        }

        /// <inheritdoc cref="AppendFormatted(string)"/>
        public void AppendFormatted(ICSharpSourceComponent placeholder)
        {
            writer.Append(placeholder);
        }

        /// <inheritdoc cref="AppendFormatted(string)"/>
        /// <typeparam name="T">
        /// The type of the placeholder to format and append. 
        /// </typeparam>
        public void AppendFormatted<T>(T placeholder)
        {
            if (!writer._appendCondition)
            {
                return;
            }

            var invokeDefaultLogic = true;
            AppendFormatted(placeholder, ref invokeDefaultLogic);

            if (!invokeDefaultLogic)
            {
                return;
            }

            switch (placeholder)
            {
                case String @string:
                    AppendFormatted(@string);
                    break;
                case Type type:
                    AppendFormatted(type);
                    break;
                case ICSharpSourceComponent component:
                    // invert call in order to allow jit to remove boxing conversion
                    component.AppendTo(writer, writer._cancellationToken);
                    break;
                case null:
                    break;
                default:
                {
                    if (placeholder.ToString() is { } s)
                    {
                        AppendFormatted(s);
                    }

                    break;
                }
            }
        }

        private void TryDetent()
        {
            if (!_isIndented)
            {
                return;
            }

            writer.Detent();
            _isIndented = false;
        }

        /// <inheritdoc />
        public void Dispose() => TryDetent();
    }
}

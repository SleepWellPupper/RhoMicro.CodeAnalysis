// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Globalization;
using System.Text;

internal sealed record JsonNumberModel : JsonValueModel<Number>
{
    public JsonNumberModel(Number value) : base(value) { }

    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append(Value.Match(
            static d => d.ToString("0.#", CultureInfo.InvariantCulture),
            static l => l.ToString("0.#", CultureInfo.InvariantCulture),
            static ul => ul.ToString("0.#", CultureInfo.InvariantCulture)));
    }

    public override String ToString() => base.ToString();
    public Boolean Equals(JsonNumberModel? other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}

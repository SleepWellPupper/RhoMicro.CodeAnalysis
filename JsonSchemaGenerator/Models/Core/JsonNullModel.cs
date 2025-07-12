// SPDX-License-Identifier: MPL-2.0

//namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

//using System.Text;

//sealed record JsonNullModel : JsonValueModel
//{
//    public static JsonNullModel Instance { get; } = new();
//    public override void AppendTo(StringBuilder sb, CancellationToken ct)
//    {
//        ct.ThrowIfCancellationRequested();
//        _ = sb.Append("null");
//    }

//    public override String ToString() => base.ToString();
//    public Boolean Equals(JsonNullModel? other) => base.Equals(other);
//    public override Int32 GetHashCode() => base.GetHashCode();
//}

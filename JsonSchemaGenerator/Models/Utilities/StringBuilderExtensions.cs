namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Text;

static class StringBuilderExtensions
{
    public static StringBuilder AppendModel(this StringBuilder sb, JsonValueModel model, CancellationToken ct)
    {
        model.AppendTo(sb, ct);
        return sb;
    }
}

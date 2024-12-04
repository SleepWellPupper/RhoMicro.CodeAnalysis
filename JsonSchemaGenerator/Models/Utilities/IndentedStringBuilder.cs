namespace RhoMicro.CodeAnalysis.Library.Text;

using RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

partial class IndentedStringBuilder
{
    public IndentedStringBuilder AppendModel(JsonValueModel model)
    {
        model.AppendTo(_builder, Options.AmbientCancellationToken);
        return this;
    }
}

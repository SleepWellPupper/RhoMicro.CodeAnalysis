namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

internal readonly record struct SimpleSchemaModelBuilder(SimpleSchemaModel Model)
{
    public TypeSetModel Type => new(Model.Set("type"));
    public void SetId(Id id) => GetId().Value = id.Absolute;
    public JsonStringModel GetId() => Model.String("$id");
    public StringArrayModel Required => new(Model.Array("required"));
    public PropertiesModel Properties => new(Model.Object("properties"));
    public AdditionalPropertiesModel Additional { get; } = new(Model.Dynamic("additionalProperties", JsonBooleanModel.False));
    public Lazy<JsonSchemaModel> Items { get; } = new(() => Model.Schema("items"));

    public override Int32 GetHashCode() => Model.GetHashCode();
    public Boolean Equals(SimpleSchemaModelBuilder other) => Model.Equals(other.Model);
}

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

readonly record struct AdditionalPropertiesModel(JsonDynamicModel Model)
{
    public void False() => Model.Boolean.Value = false;
    public StringArrayModel Names => new(Model.Array);
    public JsonSchemaModel Schema => Model.Schema;
    public JsonValueModel ModelOrSetDefault
    {
        get
        {
            if(Model.Value is JsonSchemaModel { Size: 0 })
                False();

            return Model.Value;
        }
    }
}

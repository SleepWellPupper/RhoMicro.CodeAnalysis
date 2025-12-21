// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using RhoMicro.CodeAnalysis;
using TestApplication;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    PropertyNamingPolicy = new CustomNamingPolicy(), PropertyNameCaseInsensitive = true,
};

var union = IntDoubleString.Create(42d);
union = "foo";
union = 47;

var typeName = union.Switch(
    onDouble: _ => "double",
    onInt32: _ => "int",
    onImmutableArray: _ => "array",
    onString: _ => "string",
    onFile: _ => "file");

if (union.IsDouble)
{
    var d = union.AsDouble;
}

Console.WriteLine(union);
var serialized = JsonSerializer.Serialize(union, options);
Console.WriteLine(serialized);
serialized = serialized.ToLower().Replace("47", "[9,8,7]").Replace("2", "3");
Console.WriteLine(serialized);
var deserialized =
    JsonSerializer.Deserialize<IntDoubleString>(
        serialized, options);
Console.WriteLine(deserialized);

internal sealed class CustomNamingPolicy : JsonNamingPolicy
{
    public override String ConvertName(String name)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            if (i is not 0)
            {
                sb.Append('-');
            }

            sb.Append(name[i]);
        }

        return sb.ToString();
    }
}

// See https://aka.ms/new-console-template for more information

var ids = IntDoubleString.Create(32);
Console.WriteLine(ids.Variant);
Console.WriteLine(ids.Value);
var id = IntDouble.Create(ids);
Console.WriteLine(id.Variant);
Console.WriteLine(id.Value); 
id = 42d;
Console.WriteLine(id.Variant);
Console.WriteLine(id.Value);
ids = "Foo";
Console.WriteLine(ids.Variant);
Console.WriteLine(ids.Value);

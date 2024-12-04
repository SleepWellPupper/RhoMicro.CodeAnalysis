namespace HelloWorld;

using Library;

using RhoMicro.CodeAnalysis;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

[JsonSchema]
class Settings
{
    public LibrarySettings? Prop { get; set; }
}
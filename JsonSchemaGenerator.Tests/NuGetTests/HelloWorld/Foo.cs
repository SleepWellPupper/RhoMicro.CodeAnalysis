namespace HelloWorld.MyNamespace;

using RhoMicro.CodeAnalysis;

[JsonSchema]
class Foo15
{
    public required Library.LibraryNamespace.Foo FooProperty { get; set; } 
}

[JsonSchema]
class Foo14
{
    public Double DoubleProperty { get; set; }
    public required String[] StringArrayProperty { get; set; }
}

[JsonSchema]
class Foo13
{
    public Foo12 PocoReferenceProperty { get; set; } = new();
}

class Foo12;

[JsonSchema]
class Foo11
{
    public Foo10 SchemaReferenceProperty { get; set; } = new();
}

[JsonSchema]
class Foo10;

enum MyEnum
{
    None,
    First,
    Second = 42
}

[JsonSchema]
class Foo9
{
    public MyEnum EnumProperty { get; set; }
}

[JsonSchema]
class Foo8
{
    public required Object RequiredObjectProperty { get; set; }
}

[JsonSchema]
class Foo7
{
    public Object? NullableObjectProperty { get; set; }
}

[JsonSchema]
class Foo6
{
    public Int32? NullableInt32Property { get; set; }
}

[JsonSchema]
class Foo5
{
    public DateTime DateTimeProperty { get; set; }
    public TimeSpan TimeSpanProperty { get; set; }
    public TimeOnly TimeOnlyProperty { get; set; }
    public DateOnly DateOnlyProperty { get; set; }
    public DateTimeOffset DateTimeOffsetProperty { get; set; }
}

[JsonSchema]
class Foo4
{
    public Single SingleProperty { get; set; }
    public Double DoubleProperty { get; set; }
    public Decimal DecimalProperty { get; set; }
    public SByte SByteProperty { get; set; }
    public Int16 Int16Property { get; set; }
    public Int32 Int32Property { get; set; }
    public Int64 Int64Property { get; set; }
    public Byte ByteProperty { get; set; }
    public UInt16 UInt16Property { get; set; }
    public UInt32 UInt32Property { get; set; }
    public UInt64 UInt64Property { get; set; }
    public Boolean BooleanProperty { get; set; }
    public String StringProperty { get; set; } = String.Empty;
    public Object ObjectProperty { get; set; } = new();
}

[JsonSchema]
class Foo3
{
    public IDictionary<String, Int32> IDictionaryProperty { get; set; } = new Dictionary<String, Int32>();
    public IReadOnlyDictionary<String, Int32> IReadOnlyDictionaryProperty { get; set; } = new Dictionary<String, Int32>();
    public Dictionary<String, Int32> DictionaryProperty { get; set; } = [];
}

[JsonSchema]
class Foo2
{
    public Int32[] ArrayProperty { get; set; } = [];
    public ISet<Int32> ISetProperty { get; set; } = new HashSet<Int32>();
    public IReadOnlySet<Int32> IReadOnlySetProperty { get; set; } = new HashSet<Int32>();
    public HashSet<Int32> HashSetProperty { get; set; } = [];
    public List<Int32> ListProperty { get; set; } = [];
}

[JsonSchema]
class Foo1
{
    public Int32 Int32Property { get; set; }
}
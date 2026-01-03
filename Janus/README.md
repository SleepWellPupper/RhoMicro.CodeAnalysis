![Logo](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/release/Janus/ReadmeLogo.svg)

# Janus

This is a source generator for generating union types.
Read about union types here: https://en.wikipedia.org/wiki/Union_type

## Licensing

This source code generator is licensed to you under the MPL-2.0.

## Features

- generate rich examination and conversion api
- generate conversion operators
- generate meaningful api names like `myUnion.IsResult` or `MyUnion.CreateFromResult(result)`
- group variants and use members like `myUnion.IsNumber`
- use `System.Text.Json` serialization

## Installation

Package Reference:

```
<ItemGroup>
	<PackageReference Include="RhoMicro.CodeAnalysis.Janus" Version="*"/>
</ItemGroup>
```

CLI:

```
dotnet add package RhoMicro.CodeAnalysis.Janus
```

## How To Use

Annotate your union type with the `UnionType` attribute:

```cs
[UnionType<String, Double>]
readonly partial struct Union;
```

Use your union type:

```cs
Union u = "Hello, World!"; //implicitly converted
u = 32; //implicitly converted
u = false; //CS0029	Cannot implicitly convert type 'bool' to 'Union'
```

### `UnionTypeAttribute<T0>` and `UnionTypeAttribute`

#### General Usage

Use `UnionTypeAttribute<T0>` to add `T0` to the list of variants:

```cs
[UnionType<Int32>]
[UnionType<String>]
partial struct IntOrString;
```

Usage:

```cs
IntOrString u = "Hello, World!"; //implicitly converted
u = 32; //implicitly converted
```

Use `UnionTypeAttribute` on type parameters to add the targeted type parameter to the list of variants:

```cs
partial struct GenericUnion<[UnionType] T0, [UnionType] T1>;
```

Usage:

```cs
var u = GenericUnion<Int32, String>.CreateFromT1("Hello, World!");
u = GenericUnion<Int32, String>.CreateFromT0(32);
```

*Note: due to compiler restrictions no conversions from or to generic type parameters are generated. Using factory
methods is an alternative way of creating union instances.*

- `Name`

Define names for generated members using `Name`:

```cs
[UnionType<List<String>>(Name = "MultipleNames")]
[UnionType<String>(Name = "SingleName")]
partial struct Names;
```

Usage:

```cs
Names n = "John";
if(n.IsSingleName)
{
    var singleName = n.AsSingleName;
} else if(n.IsMultipleNames)
{
    var multipleNames = n.AsMultipleNames;
}
```

- `Description`

Provide a description for the variant:

```cs
[UnionType<int>(Description = "This variant is used for integer values.")]
partial struct Result
```

- `IsNullable`

Specify the nullability of a reference type variant:

```cs
NullableStringUnion foo1 = "value";
string value1 = foo1.CastToString; // CS8600 Converting null literal or possible null value to non-nullable type.

NonNullableStringUnion foo2 = "value";
string value2 = foo2.CastToString; // no nullability warning

[UnionType<string>(IsNullable = true)]
partial struct NullableStringUnion;

[UnionType<string>] // implicitly non-nullable
partial struct NonNullableStringUnion;
```

- `Groups`

Group variants into categories by assigning `Groups`:

```cs
[UnionType<Int32, Single>(Groups = ["Number"])]
[UnionType<String, Char>(Groups = ["Text"])]
partial struct GroupedUnion;
```

Usage:

```cs
GroupedUnion u = "Hello, World!";
if(u.Variant.Groups.ContainsNumber)
{
    Assert.Fail("Expected union to be text.");
}
if(!u.Variant.Groups.ContainsText)
{
    Assert.Fail("Expected union to be text.");
}

u = 32f;
if(!u.Variant.Groups.ContainsNumber)
{
    Assert.Fail("Expected union to be number.");
}
if(u.Variant.Groups.ContainsText)
{
    Assert.Fail("Expected union to be number.");
}
```

### `UnionTypeAttribute<T0..Tn>`

The generic `UnionTypeAttribute` types allow to define multiple variants inline:

```cs
[UnionType<int, double, byte>]
partial struct Union;
```

### `UnionTypeSettingsAttribute`

Use the `UnionTypeSettingsAttribute` to supply additional instructions to the generator.
The attribute may be applied to either an assembly or a union type.
When targeting a union type, it defines settings specific to that type.
If, however, the attribute is annotating an assembly, it supplies the default settings for every union type in that
assembly.

#### `ToStringSetting`

Define how implementations of `ToString` should be generated:

- `Inherit`

> Inherits the setting. This is the default value.
> - If the target is a type, it will inherit the setting from its containing assembly.
> - If the target is an assembly, the `Detailed` setting will be used.

- `Detailed`

> The generator will emit an implementation that returns detailed information, including:
> - the name of the union type
> - the set of variants
> - an indication of which variant is being represented by the instance
> - the value currently being represented by the instance

- `None`

> The generator will not generate an implementation of `ToString`.

- `Simple`

> The generator will generate an implementation that returns the result of
> calling `ToString` on the currently represented value.

#### `EqualityOperatorsSetting`

Define if equality operators should be generated:

- `Inherit`

> Inherits the setting. This is the default value.
> - If the target is a type, it will inherit the setting from its containing assembly.
> - If the target is an assembly, the `EmitOperatorsIfValueType` setting will be used.

- `EmitOperatorsIfValueType`

> Equality operators will be emitted only if the target union type is a value type.

- `EmitOperators`

> Equality operators will be emitted.

- `OmitOperators`

> Equality operators will be omitted.

#### `JsonConverterSetting`

Define how JSON support should be generated:

- `Inherit`

> Inherits the setting. This is the default value.
> - If the target is a type, it will inherit the setting from its containing assembly.
> - If the target is an assembly, the `OmitJsonConverter` setting will be used.

- `OmitJsonConverter`

> No JSON converter implementation is emitted.

- `EmitJsonConverter`

> A JSON converter implementation is emitted.

## Compound Example

In our imaginary usecase, a user shall be retrieved from the infrastructure via a name query. The following types will
be found throughout the example:

```cs
sealed record User(String Name);

enum ErrorCode
{
    NotFound,
    Unauthorized
}

readonly record struct MultipleUsersError(Int32 Count);
```

The `User` type represents a user. The `ErrorCode` represents an error that does not contain additional information,
like `MultipleUsersError` does. It represents multiple users having been found while only one was requested.

We define a union type to represent our imaginary query:

```cs
[UnionType<ErrorCode, MultipleUsersError>(Groups = ["Error"])]
[UnionType<User>(Groups = ["Success"])]
readonly partial struct GetUserResult;
```

Instances of `GetUserResult` can represent *either* an instance of `ErrorCode`, `MultipleUsersError` or `User`.

It will be used in a service façade like so:

```cs
interface IUserService
{
    GetUserResult GetUserByName(String name);
}
```

A repository abstracts over the underlying infrastructure:

```cs
interface IUserRepository
{
    IQueryable<User> UsersByName(String name);
}
```

Access violations would be communicated through the repository using the following exception type:

```cs
sealed class UnauthorizedDatabaseAccessException : Exception;
```

An implementation of the `IUserService` is provided as follows:

```cs
sealed class UserService : IUserService
{
    public UserService(IUserRepository repository) => _repository = repository;

    private readonly IUserRepository _repository;

    public GetUserResult GetUserByName(String name)
    {
        IQueryable<User> users;
        try
        {
            users = _repository.UsersByName(name);
        } catch(UnauthorizedDatabaseAccessException)
        {
            return ErrorCode.Unauthorized;
        }

        var reifiedUsers = users.ToArray();
        if(reifiedUsers.Length == 0)
        {
            return ErrorCode.NotFound;
        } else if(reifiedUsers.Length > 1)
        {
            return new MultipleUsersError(reifiedUsers.Length);
        }

        return reifiedUsers[0];
    }
}
```

As you can see, possible representations of `GetUserResult` are implicitly converted and returned by the service. Users
of `OneOf` will be familiar with this.

On the consumer side of this api, a generated `Match` function helps with transforming the union instance to another
type:

```cs
sealed class UserModel
{
    public UserModel(IUserService service) => _service = service;

    private readonly IUserService _service;

    public String ErrorMessage { get; private set; } = String.Empty;
    public User? User { get; private set; }
    public void SetUser(String name)
    {
        var getUserResult = _service.GetUserByName(name);
        User = getUserResult.Switch(
            onErrorCode: HandleErrorCode,
            onMultipleUsersError: HandleMultipleResult,
            onUser: user => user);
    }
    private User? HandleErrorCode(ErrorCode code)
    {
        ErrorMessage = code switch
        {
            ErrorCode.NotFound => "The user could not be located.",
            ErrorCode.Unauthorized => "You are not authorized to access users.",
            _ => throw new NotImplementedException()
        };
        return null;
    }
    private User? HandleMultipleResult(MultipleUsersError result)
    {
        ErrorMessage = $"{result.Count} users have been located. The name was not precise enough.";
        return null;
    }
}
```

## Benchmarks

### `MemoryOverlayingBenchmark`

Janus employs memory overlaying techniques in order to improve union type space efficiency.
This allows for more efficient type layouts in some cases.

`Janus` and `OneOf` support value type unions, so allocation-free unions can be implemented with them.
`Dunet` depends on a type hierarchy between `record class` types, so this is not possible here.

The reference type `Janus` union allocates 8 bytes more than the `Dunet` union as it requires
space for the tag field, whereas `Dunet` relies on type information to discriminate variants.

All benchmarked unions have the following variants: `System.Int32`, `System.Single`, `System.Double`, `System.Int64`

| Method                      |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| StructJanusUnionAllocations | 0.5353 ns | 0.0275 ns | 0.0257 ns |  0.13 |    0.01 |      - |         - |        0.00 |
| ClassJanusUnionAllocations  | 3.9721 ns | 0.0131 ns | 0.0123 ns |  1.00 |    0.00 | 0.0038 |      32 B |        1.00 |
| ClassOneOfUnionAllocations  | 5.3729 ns | 0.1004 ns | 0.0939 ns |  1.35 |    0.02 | 0.0057 |      48 B |        1.50 |
| StructOneOfUnionAllocations | 0.1445 ns | 0.0110 ns | 0.0103 ns |  0.04 |    0.00 |      - |         - |        0.00 |
| DunetUnionAllocations       | 3.4401 ns | 0.0071 ns | 0.0067 ns |  0.87 |    0.00 | 0.0029 |      24 B |        0.75 |

*[Source](https://github.com/SleepWellPupper/RhoMicro.CodeAnalysis/blob/release/Janus.Benchmarks/MemoryOverlayingBenchmark.cs)*

### `SwitchClosureAllocationBenchmark`

`Janus` provides high performance methods for handling variants.
This allows consumers to avoid allocations due to closures when providing callbacks.
`Dunet` relies on the language level `switch` statement/expression, and so can also
achieve closure-free handling of variants.

| Method           | State |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|------------------|-------|---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| JanusUnionSwitch | 1     | 2.960 ns | 0.0126 ns | 0.0118 ns |  1.00 |    0.01 | 0.0029 |      24 B |        1.00 |
| OneOfUnionSwitch | 1     | 9.107 ns | 0.1896 ns | 0.1947 ns |  3.08 |    0.07 | 0.0115 |      96 B |        4.00 |
| DunetUnionSwitch | 1     | 2.595 ns | 0.0222 ns | 0.0208 ns |  0.88 |    0.01 | 0.0029 |      24 B |        1.00 |

*[Source](https://github.com/SleepWellPupper/RhoMicro.CodeAnalysis/blob/release/Janus.Benchmarks/SwitchClosureAllocationBenchmark.cs)*

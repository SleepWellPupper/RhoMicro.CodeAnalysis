![Logo](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/master/Janus/ReadmeLogo.svg)

# Janus

This is a C# source generator for union types.
Read about union types here: https://en.wikipedia.org/wiki/Union_type

## Licensing

This source code generator and the code it generates is licensed to you under the MPL-2.0.

## Features

- generate rich examination and conversion api
- convert between intersecting union types
- generate conversion operators
- generate meaningful api names like `myUnion.IsResult`
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

*Note: The source code generator will generate netstandard2.0 compliant code that potentially uses newer language
features. It is expected that consumers enable the `<LangVersion>14.0</LangVersion>` flag. This could change in the
future to support a wider number of language versions, however the netstandard2.0 constraint will remain. The generator
generates members (
e.g.: [NotNullWhenAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.notnullwhenattribute?view=netstandard-2.1) )
that rely on special types not required by netstandard2.0, therefore under some circumstances (i.e. in analyzers) a
polyfill (see [PolySharp](https://www.nuget.org/packages/PolySharp/#readme-body-tab)) and additional
libraries ([System.Collections.Immutable](https://www.nuget.org/packages/System.Collections.Immutable), [Microsoft.Bcl.HashCode](https://www.nuget.org/packages/Microsoft.Bcl.HashCode), [System.Text.Json](https://www.nuget.org/packages/System.Text.Json))
may be required.*

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

Use `UnionTypeAttribute<T0>` to add `T0` to the set of variants:

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

Use `UnionTypeAttribute` on type parameters to add the targeted type parameter to the set of variants:

```cs
partial struct GenericUnion<[UnionType] T0, [UnionType] T1>;
```

Usage:

```cs
var u = GenericUnion<Int32, String>.Create("Hello, World!");
u = GenericUnion<Int32, String>.Create(32);
```

*Note: due to compiler restrictions no conversions from or to generic type parameters are generated. Using factory
methods is an alternative way of creating union instances.*

- `Name`

Define names for generated members using `Name` , e.g.:

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
    var singleName = n.CastToSingleName;
} else if(n.IsMultipleNames)
{
    var multipleNames = n.CastToMultipleNames;
}
```

#### `IsNullable`

Instructs the generator to treat the reference type variant
as nullable, allowing for `null` arguments in factories, conversions etc.

```cs
[UnionType<String>(IsNullable = true)]
[UnionType<List<String>>]
partial struct NullableStringUnion;
```

Usage:

```cs
NullableStringUnion u = (String?)null;
u = new List<String>();
u = "Nonnull String";
u = (List<String>?)null;
```

#### `Groups`

Group variants into categories by assigning `Groups`:

```cs
[UnionType<Int32, Single>(Groups = [ "Number" ])]
[UnionType<String, Char>(Groups = [ "Text" ])]
partial struct GroupedUnion;
```

Usage:

```cs
GroupedUnion u = "Hello, World!";
if(u.Variant.Group.ContainsNumber)
{
    Assert.Fail("Expected union to be text.");
}
if(!u.Variant.Group.ContainsText)
{
    Assert.Fail("Expected union to be text.");
}

u = 32f;
if(!u.Variant.Group.ContainsNumber)
{
    Assert.Fail("Expected union to be number.");
}
if(u.Variant.Group.ContainsText)
{
    Assert.Fail("Expected union to be number.");
}
```

### `UnionTypeAttribute<T0..Tn>`

The generic `UnionTypeAttribute` types allow defining multiple variants inline.
Except for `Alias`, they support all the features that `UnionTypeAttribute<T0>` and `UnionTypeAttribute` provide.
Any such features will be applied to all representable types listed in the type arguments list.

For sample usage, see [Groups](#groups).

### `UnionTypeSettingsAttribute`

Use the `UnionTypeSettingsAttribute` to supply additional instructions to the generator.
The attribute may be applied to either an assembly or a union type.
When targeting a union type, it defines settings specific to that type.
If, however, the attribute is annotating an assembly, it supplies the default settings for every union type in that
assembly.

Settings inheritance is therefore ordered like so:

- default settings are applied (
  see [UnionTypeSettingsAttribute](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/master/Janus.Analyzers/Attributes/UnionTypeSettingsAttribute.cs))
- if settings could be located on assembly, settings defined therein are applied and override default settings
- if settings could be located on union type, settings defined therein are applied and override default or assembly
  settings

#### `ToStringSetting`

Define how implementations of `ToString` should be generated:

- `Inherit`

> Inherits the setting.

- `Detailed`

> The generator will emit an implementation that returns detailed information, including:
> - the name of the union type
> - a set of variants
> - an indication of which variant is being represented by the instance
> - the value currently being represented by the instance

- `None`

> The generator will not generate an implementation of ToString.

- `Simple`

> The generator will generate an implementation that returns the result of
> calling `ToString` on the currently represented value.

#### `EqualityOperatorsSetting`

Define how equality operators should be generated:

- `Inherit`

> Inherits the setting.

- `EmitOperatorsIfValueType`

> Equality operators will be emitted only if the target union type is a value type.

- `EmitOperators`

> Equality operators will be emitted.

- `OmitOperators`

> Equality operators will be omitted.

#### `JsonConverterSetting`

Define how `System.Text.Json` support should be generated:

- `Inherit`

> Inherits the setting.

- `EmitJsonConverter`

> A JSON converter implementation is emitted.

- `OmitJsonConverter`

> No JSON converter implementation is emitted.

## Benchmarks

### [SwitchStateBenchmark](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/master/Janus.Benchmarks/SwitchStateBenchmark.cs)

Janus allows projections and callbacks (via `Switch`) to use a `state` parameter in order to avoid allocations due to
closures:

| Method |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|--------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Janus  | 0.7062 ns | 0.0079 ns | 0.0074 ns |  1.00 |    0.01 |      - |         - |          NA |
| OneOf  | 2.8939 ns | 0.0115 ns | 0.0102 ns |  4.10 |    0.04 | 0.0029 |      24 B |          NA |

### [UnmanagedOverlayingBenchmark](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/master/Janus.Benchmarks/UnmanagedOverlayingBenchmark.cs)

Janus will overlay unmanaged value types in memory to improve space efficiency:

| Method |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|--------|---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Janus  | 3.146 ns | 0.0325 ns | 0.0304 ns |  1.00 |    0.01 | 0.0038 |      32 B |        1.00 |
| OneOf  | 5.102 ns | 0.0155 ns | 0.0129 ns |  1.62 |    0.02 | 0.0048 |      40 B |        1.25 |

## Contrived Example

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

The `User` type represents a user.
The `ErrorCode` represents an error that does not contain additional information, like `MultipleUsersError` does.
It represents multiple users having been found while only one was requested.

We define a union type to represent our imaginary query:

```cs
[UnionType<ErrorCode, MultipleUsersError, User>]
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

As you can see, possible representations of `GetUserResult` are implicitly converted and returned by the service.
Users of `OneOf` will be familiar with this.

On the consumer side of this api, a generated `Switch` function helps with transforming the union instance to another
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

###

![Logo](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/master/Janus/ReadmeLogo.svg)

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

Settings inheritance is therefore ordered like so:

- default settings are applied (
  see [UnionTypeSettingsAttribute](https://raw.githubusercontent.com/PaulBraetz/RhoMicro.CodeAnalysis/master/Janus/Attributes/UnionTypeSettingsAttribute.cs))
- if settings could be located on assembly, settings defined therein are applied and override inherited settings
- if settings could be located on union type, settings defined therein are applied and override inherited or assembly
  settings

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

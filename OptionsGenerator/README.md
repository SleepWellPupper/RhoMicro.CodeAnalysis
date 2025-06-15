# OptionsGenerator

This generator generates an opinionated implementation of the [options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options) against an interface defining the options type to use.
It allows you to invert control of choosing the desired options lifetime ([default](https://learn.microsoft.com/en-us/dotnet/core/extensions/options#options-interfaces), [snapshot](https://learn.microsoft.com/en-us/dotnet/core/extensions/options#use-ioptionssnapshot-to-read-updated-data), [monitor](https://learn.microsoft.com/en-us/dotnet/core/extensions/options#ioptionsmonitor)) from inside the consumer to the registration site.

The use pattern changes from
```cs
class Service(IOptions<Foo> fooOptions);
```
to
```cs
class Service(IFoo foo);
```
This is an opinionated design that aligns with my use of the pattern.

## Installation

```xml
<PackageReference Include="RhoMicro.CodeAnalysis.OptionsGenerator" Version="*">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

## Quick Start

Define:
```cs
using RhoMicro.CodeAnalysis;

[Options]
public interface IFoo
{
    string Setting { get; }
}
```

Register:
```cs
services.AddFoo();
```

Inject:
```
class Service(IFoo foo)
{
    public void DoStuff()
    {
        Console.WriteLine(foo.Setting);
    }
}
```

## Use

### Generated Code

For a given target interface `IFoo`, the following type declarations are generated into the namespace containing the target interface:

- `IFoo` : a `partial` declaration of the target interface providing a default instance
- `DefaultFoo` : an `internal` implementation of `IFoo` using `IOptions<MutableFoo>` to implement its properties
- `SnapshotFoo` : an `internal` implementation of `IFoo` using `IOptionsSnapshot<MutableFoo>` to implement its properties
- `MonitorFoo` : an `internal` implementation of `IFoo` using `IOptionsMonitor<MutableFoo>` to implement its properties
- `MutableFoo` : an `internal` mutable implementation of `IFoo` for use in the options pattern
- `Foo` : a `public` immutable record implementation of `IFoo` for public consumption
- `FooRegistrationStrategy` : an `internal` helper type for registering implementations of `IFoo` to service collections
- `FooConfiguration` : a `public` helper type for configuring registration of `IFoo`
- `ServiceCollectionExtensions` : a `public` helper type for registering implementations of `IFoo` to service collections

### Annotate the Target Interface

Annotate the interface defining your options type with the `Options` attribute:
```cs
using RhoMicro.CodeAnalysis;

[Options]
public interface IFoo;
```

### Exclude Properties

Exclude properties from generated types using the `ExcludeFromOptions` attribute:
```cs
using RhoMicro.CodeAnalysis;

[Options]
public interface IFoo
{
    [ExcludeFromOptions]
    string ExcludedProperty { get; }
}
```

### Register Options to Services

Register options to service collections via the generated extension method:
```cs
services.AddFoo(c => c.UseDefaultOptions());
services.AddFoo(c => c.UseSnapshotOptions());
services.AddFoo(c => c.UseMonitorOptions());
services.AddFoo(c => c.UseCustomOptions(sp => Foo.Default with { Setting = "Hello, World!" }));
```

### Configure Options Pattern Implementation

Configure the way that options based implementations are configured by implementing the following partial hook method:
```cs
partial class BarRegistrationStrategy
{
    partial class Pattern<T>
    {
        static partial void ConfigureOptionsBuilder(
            OptionsBuilder<MutableBar> builder,
            BarConfiguration configuration)
            => builder.ValidateDataAnnotations().ValidateOnStart();
    }
}
```

### Place Custom Attributes on Properties

Any attribute besides `ExcludeFromOptions` will be included on all implementations of the interface:
```cs
using RhoMicro.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

[Options]
public interface IFoo
{
    [DisallowedValues([null])]
    string StringProperty { get; }
}
```

The generated types will reflect this annotation:
```cs
public sealed partial record Foo
{
    [DisallowedValues([null])]
    string StringProperty { get; init; }
}
```

## Restrictions

- only interfaces may be targeted by the generator
- the target interface declaration must be partial
- the target interface must not be generic
- only properties will be implemented
- included properties must be readonly

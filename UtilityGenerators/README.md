# What is this?

This project contains generators and analyzers that help writing generators and analyzers:

# Installation

```xml
<PackageReference Include="RhoMicro.CodeAnalysis.UtilityGenerators" Version="*">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

# Templating

A string templating engine is provided via the `TemplatingGenerator`.
Templating is done via attributes on template types. A generator analyzes these and generates an implementation for rendering the template, similar to the ASP.Net razor engine.

> Notes:
> - blocks:
> 	- Value (singleline, multiple): `§(foo)` for `builder.Append(foo);`
> 		- builtin conversions to `ReadOnlySpan<Char>` for common types => `TemplateHelpers.GetCharSpan(foo)`
> 		- detect template type and use `builder.Render` instead
> 	- Code (multiline, multiple): `§{foreach(var bar in foo){§(bar)}}` for `foreach(var bar in foo){builder.Append(bar);}`
> 	- -> unlike razor, we require explicit block type when used in code blocks
> 	- escape with `\`
> - generated templates must always be generated using escaped source newlines, so we may efficiently use spans on a oneliner template constant

## Sample Usage
> TODO

## Grammar
The grammar may be found [here](./Templating/Syntax/grammar.txt).

# Building The Project

This project is running the generators contained against itself. To do this, run the `bootstrap.ps1` script.
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
> - holes:
> 	- Value (singleline, multiple): `§(foo)` for `builder.Append(foo);`
> 		- builtin conversions to `ReadOnlySpan<Char>` for common types => `TemplateHelpers.GetCharSpan(foo)`
> 		- detect template type and use `builder.Render` instead
> 	- Code (multiline, multiple): `§{foreach(var bar in foo){§(bar)}}` for `foreach(var bar in foo){builder.Append(bar);}`
> 	- -> unlike razor, we require explicit hole type when used in code blocks
> 	- escape with `\`
> - generated templates must always be generated using escaped source newlines, so we may efficiently use spans on a oneliner template constant

## Sample Usage


```
namespace RhoMicro.CodeAnalysis.Foo;

[Template(
"""
// \§(escaped hole)
// \\§(Name) (unescaped hole)
public §(Accessibility) class §(Name)
{§{
	foreach(var member in Members)
	{
		$(member)
	}
}}
""")]
internal sealed partial class FooTemplate
{
	public FooTemplate(String accessibility, String name)
	{
		Accessibility = accessibility;
		Name = name;
	}
	
	public String Accessibility { get; }
	public String Name { get; }
	public List<MemberTemplate> Members { get; } = [];	
}
```
should generate something like:
```cs
namespace RhoMicro.CodeAnalysis.Foo;

partial class FooTemplate : global::RhoMicro.CodeAnalysis.Library.Text.Templating.Template
{
	public override void Render(global::RhoMicro.CodeAnalysis.Library.Text.Templating.BufferedStringBuilder builder)
	{
		// this should be a one-liner with source newlines
		const string __template = 
"""
// \§(escaped hole)
// \\§(Name) (unescaped hole)
public §(Accessibility) class §(Name)
{§{
	foreach(var member in Members)
	{
		$(member)
	}
}}
""";
		builder.Append(__template.AsSpan()[..57]);
		builder.Append(TemplateUtilities.GetCharSpan(Accessibility));
		// append span of text part
		builder.Append(TemplateUtilities.GetCharSpan(Name));
		// append span of new line, open brace after Name
		// insert newline, tab before each following line in code block
		foreach(var member in Members)
		{
			$(member)
		}		
		// omit trailing whitespace in codeblock up to closing brace
		// append closing brace from template
	}
}
```

## Grammar

### Non-Terminals

#### Template

```abnf
template = *(text / hole)
```

#### Text

```abnf
text = *(%x00-%x5B %x5D-%xA6 / %xA8-%xFF / escaped)
```

#### Escaped

```abnf
escaped = ESC (MRK / ESC)
```

#### Hole

```abnf
hole = value-hole / code-hole
```

#### Value Hole

```abnf
value-hole = MRK OPA identifier CPA
```

#### Code Hole

```abnf
code-hole = MRK OBR *(text / value-hole) CBR
```

#### Identifier

```abnf
identifier = ALPHA *(ALPHA / DIGIT / "_")
```

### Terminals

#### Escape

```abnf
ESC = %x5C
```
> `%5C` = `\`

#### Marker

```abnf
MRK = %xA7
```
> `%A7` = `§`

#### Opening Curly Brace

```abnf
OBR = %x7B
```
> `%7B` = `{`

#### Closing Curly Brace

```abnf
CBR = %x7D
```
> `%7D` = `}`

#### Opening Parenthesis

```abnf
OPA = %x28
```
> `%28` = `(`

#### Closing Parenthesis

```abnf
CPA = %x29
```
> `%29` = `)`

# Building The Project

This project is running the generators contained against itself. To do this, run the `bootstrap.ps1` script.
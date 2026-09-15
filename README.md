# xmlvalidate

Small command-line tools for parsing and formatting XML and JSON, converting between them, and compiling and running legacy JScript.NET source.

## Tools

| Tool | Purpose | Input |
| --- | --- | --- |
| [xmlvalidate](xmlvalidate) | Parse an XML document and print formatted XML. | File paths or URIs as arguments; standard input when no arguments are supplied. |
| [jsonvalidate](jsonvalidate) | Parse and format JSON, with optional XML conversion through Json.NET. | Literal document text as arguments; standard input when no document arguments are supplied. |
| [jsvalidate](jsvalidate) | Compile JScript.NET source in memory with `Microsoft.JScript` and invoke its entry point. | Literal source text as arguments; standard input when no arguments are supplied. |

XML and JSON validation here means parsing the input with the respective parser. There is no XSD or JSON Schema validation.

**`jsvalidate` executes the supplied code with the process's permissions.** It uses the legacy .NET Framework JScript compiler; it is not a modern JavaScript engine or a syntax-only checker.

## Build

For the modern XML and JSON tools, install the .NET 10 SDK and build the individual projects from the repository root:

```sh
dotnet build xmlvalidate/xmlvalidate.csproj -c Release -f net10.0
dotnet build jsonvalidate/jsonvalidate.csproj -c Release -f net10.0
```

The shared [Directory.Build.props](Directory.Build.props) places these outputs in `Release/net10.0/`. Run the DLLs with `dotnet` as shown below. The modern targets use managed XML/JSON APIs and do not require Windows-specific components.

Both projects declare `net35`, `net40`, `netstandard2.0`, `net8.0`, `net9.0`, and `net10.0` targets. The .NET Standard target describes an API compatibility surface, not a standalone runtime. `jsonvalidate` references the `Newtonsoft.Json` NuGet package with a wildcard version.

The solution also includes `jsvalidate`, which targets .NET Framework 2.0 and 4.0 and references `Microsoft.JScript` and `Newtonsoft.Json`. Building and running this legacy tool requires a Windows .NET Framework environment with the corresponding reference assemblies and JScript compiler support. A full solution build therefore has additional requirements beyond the modern XML and JSON projects.

## Usage

The following examples run from the repository root after the Release builds above. Input redirection (`<`) works in `cmd.exe` and POSIX shells; PowerShell uses different input piping syntax.

### XML

Pass one or more file paths, or read a document from standard input:

```sh
dotnet Release/net10.0/xmlvalidate.dll document.xml
dotnet Release/net10.0/xmlvalidate.dll < document.xml
```

Successful parsing writes formatted XML to standard output. With multiple arguments, each document is handled separately.

### JSON and XML conversion

Unlike `xmlvalidate`, `jsonvalidate` treats document arguments as literal content, not filenames. Use standard input to read a file:

```sh
dotnet Release/net10.0/jsonvalidate.dll < document.json
dotnet Release/net10.0/jsonvalidate.dll --noindent < document.json
dotnet Release/net10.0/jsonvalidate.dll --fromxml < document.xml
dotnet Release/net10.0/jsonvalidate.dll --toxml < document.json
```

| Option | Effect |
| --- | --- |
| `--fromxml` | Parse the input as XML and convert it to JSON using Json.NET. |
| `--toxml` | Convert the parsed JSON to XML using Json.NET. |
| `--noindent` | Disable indentation in JSON or XML output. |
| `--` | End option processing. |

Options are case-sensitive and must precede document arguments. XML conversion follows Json.NET's mapping: JSON objects with multiple properties receive a `root` element, while non-object tokens use an `array` wrapper with `arrayElement` content. Conversion does not preserve the original document's exact representation.

Use one document per invocation when redirecting output to a file. Multiple document arguments are processed separately, and JSON results are written without separators.

### Errors and automation

The XML and JSON tools write parsing and conversion errors to standard error, but caught errors do **not** set a failing process exit code. Check diagnostics when using them in scripts; a zero exit code alone does not establish that the input was valid.

`jsvalidate` also has inconsistent error exit behavior: compiler errors from standard input return a failure code, while several argument-processing and exception paths return zero. Its output can include both compiler messages and output from the executed program.

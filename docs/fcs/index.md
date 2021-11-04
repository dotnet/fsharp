# FSharp.Compiler.Service

The [FSharp.Compiler.Service](https://www.nuget.org/packages/FSharp.Compiler.Service) package is a component derived from the F# compiler source code that
exposes additional functionality for implementing F# language bindings, additional
tools based on the compiler or refactoring tools. The package also includes
dynamic execution of F# code that can be used for embedding F# scripting into your applications.

## Available services

The project currently exposes the following services that are tested & documented on this page.
The libraries contain additional public API that can be used, but is not documented here.

* [**F# Language tokenizer**](tokenizer.html) - turns any F# source code into a stream of tokens.
  Useful for implementing source code colorization and basic tools. Correctly handle nested
  comments, strings etc.

* [**Processing SyntaxTree**](untypedtree.html) - allows accessing the syntax tree.
  This represents parsed F# syntax without type information and can be used to implement code formatting
  and various simple processing tasks.

* [**Working with resolved symbols**](symbols.html) - many services related to type checking
  return resolved symbols, representing inferred types, and the signatures of whole assemblies.

* [**Working with resolved expressions**](typedtree.html) - services related to working with
  type-checked expressions and declarations, where names have been resolved to symbols.

* [**Using editor services**](editor.html) - expose functionality for auto-completion, tool-tips,
  parameter information etc. These functions are useful for implementing F# support for editors
  and for getting some type information for F# code.

* [**Working with project-wide analysis**](project.html) - you can request a check of
  an entire project, and ask for the results of whole-project analyses such as find-all-references.

* [**Hosting F# interactive**](interactive.html) - allows calling F# interactive as a .NET library
  from your .NET code. You can use this API to embed F# as a scripting language in your projects.

* [**Hosting the F# compiler**](compiler.html) - allows you to embed calls to the F# compiler.

* [**File system API**](filesystem.html) - the `FSharp.Compiler.Service` component has a global variable
  representing the file system. By setting this variable you can host the compiler in situations where a file system
  is not available.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published

## API Namespaces

### Basics - Syntax, Text and Diagnostics

* [FSharp.Compiler.Diagnostics](reference/fsharp-compiler-diagnostics.html)
* [FSharp.Compiler.IO](reference/fsharp-compiler-io.html)
* [FSharp.Compiler.Syntax](reference/fsharp-compiler-syntax.html)
* [FSharp.Compiler.Text](reference/fsharp-compiler-text.html)

### Tokenization

* [FSharp.Compiler.Tokenization](reference/fsharp-compiler-tokenization.html)

### Symbols and Code Analysis

* [FSharp.Compiler.Symbols](reference/fsharp-compiler-symbols.html)
* [FSharp.Compiler.CodeAnalysis](reference/fsharp-compiler-codeanalysis.html)

### Editor Services

* [FSharp.Compiler.EditorServices](reference/fsharp-compiler-editorservices.html)

### Interactive Execution

* [FSharp.Compiler.Interactive.Shell](reference/fsharp-compiler-interactive-shell.html)

### Internal extension points

* [FSharp.Compiler.AbstractIL](reference/fsharp-compiler-abstractil.html)

## Projects using the F# Compiler Services

Some of the projects using the F# Compiler Services are:

* [**F# in Visual Studio**](https://github.com/dotnet/fsharp/)
* [**F# in Visual Studio for Mac**](https://github.com/mono/monodevelop/tree/master/main/external/fsharpbinding)
* [**FsAutoComplete**](https://github.com/fsharp/FsAutoComplete)
* [**F# in JetBrains Rider**](https://www.jetbrains.com/help/rider/F_Sharp.html)
* [**F# in .NET Interactive Notebooks**](https://github.com/dotnet/interactive)
* [**Fantomas**](https://github.com/fsprojects/fantomas/) - Source code formatting for F#
* [**FSharpLint**](https://fsprojects.github.io/FSharpLint/) - Lint tool for F#
* [**Fable**](https://fable.io/) - F# to JavaScript Compiler and more
* [**WebSharper**](https://websharper.com/) - F# full-stack web framework

Older things:

* [**FsReveal**](https://fsprojects.github.io/FsReveal/) - FsReveal parses markdown and F# script file and generate reveal.js slides
* [**Elucidate**](https://github.com/rookboom/Elucidate) - Visual Studio extension for rich inlined comments using MarkDown
* [**FSharp.Formatting**](http://fsprojects.github.io/FSharp.Formatting/) - F# tools for generating documentation (Markdown processor and F# code formatter)
* [**FAKE**](https://fsprojects.github.io/FAKE/) - "FAKE - F# Make" is a cross platform build automation system

## Contributing and copyright

The F# source code is copyright by Microsoft Corporation and contributors.

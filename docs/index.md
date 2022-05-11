# F# compiler guide

Welcome to [the F# compiler and tools repository](https://github.com/dotnet/fsharp)! This guide discusses the F# compiler source code and implementation from a technical point of view.

## Documentation Topics

* [Overview](overview.md)
* [Coding Standards](coding-standards.md)
* [Compiler Startup Performance](compiler-startup-performance.md)
* [Debug Emit](debug-emit.md)
* [Diagnostics](diagnostics.md)
* [Notes on FSharp.Core](fsharp-core-notes.md)
* [F# Interactive Code Emit](fsi-emit.md)
* [Large inputs and stack overflows](large-inputs-and-stack-overflows.md)
* [Memory usage](memory-usage.md)
* [Optimizations](optimizations.md)
* [Project builds](project-builds.md)
* [Tooling features](tooling-features.md)

[Edit the source for these docs](https://github.com/dotnet/fsharp/tree/main/docs). The docs are published automatically daily [fsharp.github.io/fsharp-compiler-docs/](https://fsharp.github.io/fsharp-compiler-docs/) by [this repo](https://github.com/fsharp/fsharp-compiler-docs).

## Key Folders

* [src/Compiler/Utilities](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Utilities/) - various utilities, largely independent of the compiler

* [src/Compiler/Facilities](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Facilities/) - various items of functionality specific to the compiler

* [src/Compiler/AbstractIL](https://github.com/dotnet/fsharp/tree/main/src/Compiler/AbstractIL/) - the Abstract IL library used for .NET IL

* [src/Compiler/SyntaxTree](https://github.com/dotnet/fsharp/tree/main/src/Compiler/SyntaxTree/) - the SyntaxTree, parsing and lexing

* [src/Compiler/TypedTree](https://github.com/dotnet/fsharp/tree/main/src/Compiler/TypedTree/) - the TypedTree, and utilities associated with it

* [src/Compiler/Checking](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Checking/) - checking logic

* [src/Compiler/Optimize](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Optimize/) - optimization and "lowering" logic

* [src/Compiler/CodeGen](https://github.com/dotnet/fsharp/tree/main/src/Compiler/CodeGen/) - IL code generation logic

* [src/Compiler/Driver](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Driver/) - compiler options, diagnostics and other coordinating functionality

* [src/Compiler/Symbols](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Symbols/) - symbols in the public API to the compiler

* [src/Compiler/Service](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Service/) - the incremental compilation and build logic, plus editor services in the public API to the compiler

* [src/Compiler/Interactive](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Interactive/) - the components forming the interactive REPL and core of the notebook engine

* [src/FSharp.Core](https://github.com/dotnet/fsharp/tree/main/src/FSharp.Core/) - the core library

* [tests](https://github.com/dotnet/fsharp/tree/main/tests) - the tests

* [vsintegration](https://github.com/dotnet/fsharp/tree/main/vsintegration) - the Visual Studio integration

## Resources for learning

* Channel: [F# Software Foundation compiler sessions](https://www.youtube.com/channel/UCsi00IVEgPoK7HvcpWDeSxQ)

* Video: [Learn me some F# Compiler, an online chat with Vlad and Don](https://www.youtube.com/watch?v=-dKf15xSWPY)

* Video: [Understanding the F# Optimizer, and online chat with Vlad and Don](https://www.youtube.com/watch?v=sfAe5lDue7k)

* Video: [Lexer and Parser, an online chat with Vlad and Don](https://www.youtube.com/watch?v=3Zr0HNVcooU)

* Video: [Resumable State Machines, an online chat with Vlad and Don](https://www.youtube.com/watch?v=GYi3ZMF8Pm0)

* Video: [The Typechecker, an online chat with Vlad and Don](https://www.youtube.com/watch?v=EQ9fjOlmwws)

* Video: [FSharp.Compiler.Service, an online chat with Vlad and Don](https://www.youtube.com/watch?v=17a3i8WBQpg)

## Tools to help work with the compiler

* [sharplab.io](https://sharplab.io/) can be used to decompile code.

* [fantomas-tools](https://fsprojects.github.io/fantomas-tools/#/ast) can be used to view the Untyped & Typed Abstract Syntax Tree.

## Attribution

This document is based on an original document published in 2015 by the [F# Software Foundation](http://fsharp.org). It has since been updated substantially.

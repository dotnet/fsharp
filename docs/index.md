# F# compiler guide

This guide discusses the F# compiler source code and implementation from a technical point of view.

## Overview

There are several artifacts involved in the development of F#:

* FSharp.Compiler.Service ([docs](fcs/), [source](https://github.com/dotnet/fsharp/tree/main/src/fsharp)). Contains all logic for F# compilation - including parsing, syntax tree processing, typechecking, constraint solving, optimizations, IL importing, IL writing, pretty printing of F# constructs, and F# metadata format processing - and the F# compiler APIs for tooling.

* The [F# compiler executable](https://github.com/dotnet/fsharp/tree/main/src/fsharp/fsc), called `fsc`, which is called as a console app. It sets the .NET GC into batch mode and then invokes `FSharp.Compiler.Service` with command-line arguments.

* The [FSharp.Core Library](https://github.com/dotnet/fsharp/tree/main/src/fsharp/FSharp.Core), called `FSharp.Core`. Contains all primitive F# types and logic for how they interact, core data structures and library functions for operating on them, structured printing logic, units of measure for scientific programming, core numeric functionality, F# quotations, F# type reflection logic, and asynchronous programming types and logic.

* The [F# Interactive tool](https://github.com/dotnet/fsharp/tree/main/src/fsharp/fsi), called `fsi`. A REPL for F# that supports execution and pretty-printing of F# code and results, loading F# script files, referencing assemblies, and referencing packages from NuGet.

The `FSharp.Compiler.Service` is by far the largest of these components and contains nearly all logic that `fsc` and `fsi` use. It is the primary subject of this guide.

## Resources for learning

* Video: [Learn me some F# Compiler, an online chat with Vlad and Don](https://www.youtube.com/watch?v=-dKf15xSWPY)

* Video: [Understanding the F# Optimizer, and online chat with Vlad and Don](https://www.youtube.com/watch?v=sfAe5lDue7k)

* Video: [Lexer and Parser, an online chat with Vlad and Don](https://www.youtube.com/watch?v=3Zr0HNVcooU)

* Video: [Resumable State Machines, an online chat with Vlad and Don](https://www.youtube.com/watch?v=GYi3ZMF8Pm0)

* Video: [The Typechecker, an online chat with Vlad and Don](https://www.youtube.com/watch?v=EQ9fjOlmwws)

* Video: [FSharp.Compiler.Service, an online chat with Vlad and Don](https://www.youtube.com/watch?v=17a3i8WBQpg)

## Key data formats and representations

The following are the key data formats and internal data representations of the F# compiler code in its various configurations:

* _Input source files_  Read as Unicode text, or binary for referenced assemblies.

* _Input command-line arguments_  See [CompilerOptions.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CompilerOptions.fs) for the full code implementing the arguments table. Command-line arguments are also accepted by the F# Compiler Service API in project specifications, and as optional input to F# Interactive.

* _Tokens_, see [pars.fsy](https://github.com/dotnet/fsharp/blob/main/src/fsharp/pars.fsy), [lex.fsl](https://github.com/dotnet/fsharp/blob/main/src/fsharp/lex.fsl), [lexhelp.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/lexhelp.fs) and related files.

* _Abstract Syntax Tree (AST)_, see [SyntaxTree.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/SyntaxTree.fs), the untyped syntax tree resulting from parsing.

* _Typed Abstract Syntax Tree (Typed Tree)_, see [TypedTree.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/TypedTree.fs), [TypedTreeBasics.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/TypedTree.fs), [TypedTreeOps.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/TypedTreeOps.fs), and related files. The typed, bound syntax tree including both type/module definitions and their backing expressions, resulting from type checking and the subject of successive phases of optimization and representation change.

* _Type checking context/state_, see for example [`TcState` in ParseAndCheckInputs.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/ParseAndCheckInputs.fsi) and its constituent parts, particularly `TcEnv` in [CheckExpressions.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CheckExpressions.fsi) and `NameResolutionEnv` in [NameResolution.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/NameResolution.fsi). A set of tables representing the available names, assemblies etc. in scope during type checking, plus associated information.

* _Abstract IL_, the output of code generation, then used for binary generation, and the input format when reading .NET assemblies, see [`ILModuleDef` in il.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/il.fsi).

* _The .NET Binary format_ (with added "pickled" F# Metadata resource), the final output of fsc.exe, see the ECMA 335 specification and the [ilread.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/ilread.fs) and [ilwrite.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/ilwrite.fs) binary reader/generator implementations. The added F# metadata is stored in a binary resource, see [TypedTreePickle.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/TypedTreePickle.fs).

* _The incrementally emitted .NET reflection assembly,_ the incremental output of fsi.exe. See [ilreflect.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/ilreflect.fs).

## Key constructs and APIs for F# tooling

The following are the most relevant parts of the F# compiler tooling, making up the "engine" and API surface area of `FSharp.Compiler.Service`.

* The incremental project build engine state in [IncrementalBuild.fsi](https://github.com/fsharp/FSharp.Compiler.Service/tree/master/src/fsharp/service/IncrementalBuild.fsi)/[IncrementalBuild.fs](https://github.com/fsharp/FSharp.Compiler.Service/tree/master/src/fsharp/service/IncrementalBuild.fs), a part of the F# Compiler Service API.

* The corresponding APIs wrapping and accessing these structures in the public-facing [`FSharp.Compiler.Service` API](https://github.com/dotnet/fsharp/tree/main/src/fsharp/service) and [Symbol API](https://github.com/dotnet/fsharp/tree/main/src/fsharp/symbols).

* The [F# Compiler Service Caches](https://fsharp.github.io/FSharp.Compiler.Service/caches.html), the various caches maintained by an instance of an `FSharpChecker`.

## Key compiler phases

The following is a diagram of how different phases of F# compiler work:

![F# compiler phases](http://fsharp.github.io/img/fscomp-phases.png)

The following are the key phases and high-level logical operations of the F# compiler code in its various configurations:

* _Basic lexing_. Produces a token stream from input source file text.

* _White-space sensitive lexing_. Accepts and produces a token stream, augmenting per the F# Language Specification.

* _Parsing_. Accepts a token stream and produces an AST per the grammar in the F# Language Specification.

* _Resolving references_. For .NET SDK generally references are resolved explicitly by external tooling.
   There is a legacy aspect to this if references use old .NET Framework references including for
   scripting.  See [ReferenceResolver.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/ReferenceResolver.fs) for the abstract definition of compiler reference resolution. See [LegacyMSBuildReferenceResolver.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/LegacyMSBuildReferenceResolver.fs) for reference resolution used by the .NET Framework F# compiler when running on .NET Framework. See [SimulatedMSBuildReferenceResolver.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/SimulatedMSBuildReferenceResolver.fs) when not using the .NET Framework F# compiler. 
   See [DependencyManager](https://github.com/dotnet/fsharp/tree/main/src/fsharp/DependencyManager) for reference resolution and package management used in `fsi`.

* _Importing referenced .NET binaries_, see [import.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/import.fsi)/[import.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/import.fs). Accepts file references and produces a Typed Tree node for each referenced assembly, including information about its type definitions (and type forwarders if any).

* _Importing referenced F# binaries and optimization information as Typed Tree data structures_, see [TypedTreePickle.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/TypedTreePickle.fs). Accepts binary data and produces  Typed Tree nodes for each referenced assembly, including information about its type/module/function/member definitions.

* _Sequentially type checking files_, see [CheckDeclarations.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CheckDeclarations.fsi)/[CheckDeclarations.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CheckDeclarations.fs). Accepts an AST plus a type checking context/state and produces new Typed Tree nodes
  incorporated into an updated type checking state, plus additional Typed Tree Expression nodes used during code generation.  A key part of this is
  checking syntactic types and expressions, see [CheckExpressions.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CheckDeclarations.fsi)/[CheckExpressions.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CheckDeclarations.fs) including the state held across the checking of a file (see `TcFileState`) and the
  environment active as we traverse declarations and expressions (see `TcEnv`).

* _Pattern match compilation_, see [PatternMatchCompilation.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/PatternMatchCompilation.fsi)/[PatternMatchCompilation.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/PatternMatchCompilation.fs). Accepts a subset of checked Typed Tree nodes representing F# pattern matching and produces Typed Tree expressions implementing the pattern matching. Called during type checking as each construct involving pattern matching is processed.

* _Constraint solving_, see [ConstraintSolver.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/ConstraintSolver.fsi)/[ConstraintSolver.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/ConstraintSolver.fs).A constraint solver state is maintained during type checking of a single file, and constraints are progressively asserted (i.e. added to this state). Fresh inference variables are generated and variables are eliminated (solved). Variables are also generalized at various language constructs, or explicitly declared, making them "rigid". Called during type checking as each construct is processed.

* _Post-inference type checks_, see [PostInferenceChecks.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/PostInferenceChecks.fsi)/[PostInferenceChecks.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/PostInferenceChecks.fs). Called at the end of type checking/inference for each file. A range of checks that can only be enforced after type checking on a file is complete, such as analysis when using `byref<'T>` or other `IsByRefLike` structs.

* _Quotation translation_, see [QuotationTranslator.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/QuotationTranslator.fsi)/[QuotationTranslator.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/QuotationTranslator.fs)/[QuotationPickler.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/QuotationPickler.fsi)/[QuotationPickler.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/QuotationPickler.fs). Generates the stored information for F# quotation nodes, generated from the Typed Tree expression structures of the F# compiler. Quotations are ultimately stored as binary data plus some added type references. "ReflectedDefinition" quotations are collected and stored in a single blob.

* _Optimization phases_, primarily the "Optimize" (peephole/inlining) and "Top Level Representation" (lambda lifting) phases, see [Optimizer.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/Optimizer.fsi)/[Optimizer.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/Optimizer.fs) and [InnerLambdasToTopLevelFuncs.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/InnerLambdasToTopLevelFuncs.fsi)/[InnerLambdasToTopLevelFuncs.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/InnerLambdasToTopLevelFuncs.fs) and [LowerCallsAndSeqs.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/LowerCallsAndSeqs.fs). Each of these takes Typed Tree nodes for types and expressions and either modifies the nodes in place or produces new Typed Tree nodes. These phases are orchestrated in [CompilerOptions.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/CompilerOptions.fs)

* _Code generation_, see [IlxGen.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fsi)/[IlxGen.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fs). Accepts Typed Tree nodes and produces Abstract IL nodes, sometimes applying optimizations.

* _Abstract IL code rewriting_, see [EraseClosures.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/ilx/EraseClosures.fs) and
  [EraseUnions.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/ilx/EraseUnions.fs). Eliminates some constructs by rewriting Abstract IL nodes.
  
* _Binary emit_, see [ilwrite.fsi](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/ilwrite.fsi)/[ilwrite.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/ilwrite.fs).

* _Reflection-Emit_, see [ilreflect.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/absil/ilreflect.fs).

These and transformations used to build the following:

* _The F# Compiler Service API_, see the [Symbol API](https://github.com/dotnet/fsharp/tree/main/src/fsharp/symbols) and [Service API](https://github.com/dotnet/fsharp/tree/main/src/fsharp/service)

* _The F# Interactive Shell_, see [fsi.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/fsi/fsi.fs).

* _The F# Compiler Shell_, see [fsc.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/fsc.fs) and [fscmain.fs](https://github.com/dotnet/fsharp/blob/main/src/fsharp/fscmain.fs).

## Tools to help work with the compiler

* [sharplab.io](https://sharplab.io/) can be used to decompile code.

* [fantomas-tools](https://fsprojects.github.io/fantomas-tools/#/ast) can be used to view the Untyped & Typed Abstract Syntax Tree.

## Adding Error Messages

Adding or adjusting errors emitted by the compiler is usually straightforward (though it can sometimes imply deeper compiler work). Here's the general process:

1. Reproduce the compiler error or warning with the latest F# compiler built from the [F# compiler repository](https://github.com/dotnet/fsharp).
2. Find the error code (such as `FS0020`) in the message.
3. Use a search tool and search for a part of the message. You should find it in `FSComp.fs` with a title, such as `parsMissingTypeArgs`.
4. Use another search tool or a tool like Find All References / Find Usages to see where it's used in the compiler source code.
5. Set a breakpoint at the location in source you found. If you debug the compiler with the same steps, it should trigger the breakpoint you set. This verifies that the location you found is the one that emits the error or warning you want to improve.

From here, you can either simply update the error test, or you can use some of the information at the point in the source code you identified to see if there is more information to include in the error message. For example, if the error message doesn't contain information about the identifier the user is using incorrectly, you may be able to include the name of the identifier based on data the compiler has available at that stage of compilation.

If you're including data from user code in an error message, it's important to also write a test that verifies the exact error message for a given string of F# code.

## Formatting User Text from Typed Tree items

When formatting Typed Tree objects such as `TyconRef`s as text, you normally use either

* The functions in the `NicePrint` module such as `NicePrint.outputTyconRef`. These take a `DisplayEnv` that records the context in which a type was referenced, for example, the open namespaces. Opened namespaces are not shown in the displayed output.

* The `DisplayName` properties on the relevant object. This drops the `'n` text that .NET adds to the compiled name of a type, and uses the F#-facing name for a type rather than the compiled name for a type (for example, the name given in a `CompiledName` attribute).

* The functions such as `Tastops.fullTextOfTyconRef`, used to show the full, qualified name of an item.

When formatting "info" objects, see the functions in the `NicePrint` module.

## Notes on displaying types

When displaying a type, you will normally want to "prettify" the type first. This converts any remaining type inference variables to new, better user-friendly type variables with names like `'a`. Various functions prettify types prior to display, for example, `NicePrint.layoutPrettifiedTypes` and others.

When displaying multiple types in a comparative way, for example, two types that didn't match, you will want to display the minimal amount of infomation to convey the fact that the two types are different, for example, `NicePrint.minimalStringsOfTwoTypes`.

When displaying a type, you have the option of displaying the constraints implied by any type variables mentioned in the types, appended as `when ...`. For example, `NicePrint.layoutPrettifiedTypeAndConstraints`.

## Compiler Startup Performance

Compiler startup performance is a key factor affecting happiness of F# users. If the compiler took 10sec to start up, then far fewer people would use F#.

On all platforms, the following factors affect startup performance:

* Time to load compiler binaries. This depends on the size of the generated binaries, whether they are pre-compiled (for example, using NGEN or CrossGen), and the way the .NET implementation loads them.

* Time to open referenced assemblies (for example, `mscorlib.dll`, `FSharp.Core.dll`) and analyze them for the types and namespaces defined. This depends particularly on whether this is correctly done in an on-demand way.

* Time to process "open" declarations are the top of each file. Processing these declarations have been observed to take time in some cases of  F# compilation.

* Factors specific to the specific files being compiled.

On Windows, the compiler delivered with Visual Studio currently uses NGEN to pre-compile `fsc`, `fsi`, and some assemblies used in Visual Studio tooling. For .NET Core, the CrossGen tool is used to accomplish the same thing. Visual Studio will use _delayed_ NGEN, meaning that it does not run NGEN on every binary up front. This means that F# compilation through Visual Studio may be slow for a few times before it gets NGENed.

### The F# Compiler Service Public Surface Area

The "intended" FCS API is the parts under the namespaces

* FSharp.Compiler.SourceCodeServices.* (analysis, compilation, tooling, lexing)
* FSharp.Compiler.Interactive.Shell.*  (scripting support)
* FSharp.Compiler.AbstractIL.*  (for ILAssemblyReader hook for Rider)
* FSharp.Compiler.Syntax.*  (direct access to full untyped tree)

These sections are generally designed with F#/.NET design conventions (e.g. types in namespaces, not modules, no nesting of modules etc.)
and we will continue to iterate to make this so.

In contrast, the public parts of the compiler directly under `FSharp.Compiler.*` and `FSharp.AbstractIL.*` are
"incidental" and not really designed for public use apart from the hook for JetBrains Rider
(Aside: In theory all these other parts could be renamed to FSharp.Compiler though there's no need to do that right now).  
These internal parts tend to be implemented with the "module containing lots of stuff in one big file" approach for layers of the compiler.

## Optimizations

See [optimizations](optimizations.html)

## Bootstrapping

The F# compiler is bootstrapped. That is, an existing F# compiler is used to build a "proto" compiler from the current source code. That "proto" compiler is then used to compile itself, producing a "final" compiler. This ensures the final compiler is compiled with all relevant optimizations and fixes.

## FSharp.Build

`FSharp.Build.dll` and `Microsoft.FSharp.targets` give MSBuild support for F# projects (`.fsproj`) and contain the. Although not strictly part of the F# compiler, they are essential for using F# in all contexts for .NET, aside from some more targeted scripting scenarios. The targets expose things like the `CoreCompile` and `Fsc` tasks called by MSBuild.

### Attribution

This document is based on an original document published in 2015 by the [F# Software Foundation](http://fsharp.org). It has since been updated substantially.

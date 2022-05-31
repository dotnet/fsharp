---
title: Project builds
category: Language Service Internals
categoryindex: 300
index: 600
---
# Project builds

The compiler is generally built to compile one assembly: the assumption that the compiler is compiling one assembly is baked into several aspects of the design of the Typed Tree. In contrast, FCS supports compiling a graph of projects, each for a different assembly, each undergoing incremental change.

Project builds are currently stateful and reliant on I/O to on-disk assemblies. This causes many problems and we have a plan for how to fix this core issue in the F# Language Service implementation. See [Plan: Modernizing F# Analysis](https://github.com/dotnet/fsharp/issues/11976). Please read this carefully if you plan to work on any of service.fs, IncrementalBuild.fs or FSharpCheckerResults.fs.

Key data structures:

* `IncrementalBuilder`  manages an incremental build graph for the build of an F# project.
  * See also [Plan: Modernizing F# Analysis](https://github.com/dotnet/fsharp/issues/11976) for how this will evolve to `FSharpProject`.

* `FSharpParseFileResults` -  represents the enrichment (e.g. breakpoint validation) available from the parse tree of a file. The enrichment is made up of several pieces such as:
  * `SyntaxTree`/`ParsedInput`
  * diagnostics from the parsing

* `FSharpCheckFileResults` -  represents the enrichment (e.g. tooltips) available after checking a file. The enrichment is made up of several pieces such as
  * `TcGlobals` - the globals for the compilation, also used in command-line build
  * `TcConfig` - the compiler configuration for the compilation, also used in command-line build
  * `TcImports` - the table of imports for the compilation
  * `CcuThunk` - the thunk of the assembly being compiled
  * `TcState` - the state of the compilation up to this point
  * `TcResolutions` - name environments across the file, ultimately from NameResolution.fs
  * `TcSymbolUses` - resolutions of symbols across the file
  * `LoadClosure` - the `#load` closure of a script
  * `TypedImplFile` - the TAST expression results of compilation, may be thrown away if `keepAssemblyContents` is not true

* `FSharpCheckProjectResults` -  represents the enrichment (e.g. find-all symbol uses) available after checking a project
  * `TcGlobals` - the globals for the compilation, also used in command-line build
  * `TcConfig` - the compiler configuration for the compilation, also used in command-line build
  * `TcImports` - the table of imports for the compilation
  * `CcuThunk` - the thunk of the assembly being compiled
  * `TcState` - the final state of the compilation

## Multi-project builds and cross-project references

In FCS, there is no single abstraction for a "solution build" and instead you have multiple project builds. These are all essentially independent, in the sense they each logically represent an invocation of the F# compiler. That is, the Typed Tree (TAST), TcState etc. nodes are **not** shared between different project compilations. 

If you want to understand why this invariant is important, some key things to understand are:

* The `RawFSharpAssemblyData` is the data blob that would normally be stuffed in the F# resource in the generated DLL  in a normal compilation. That's the "output" of checking each project.

* This is used as "input" for the assembly reference of each consuming project (instead of an on-disk DLL)

* Within each consuming project that blob is then resurrected to Typed Tree nodes in `TypedTreePickle.fs`.

Could the compiler share this data across projects? In theory, yes. In practice, it's very tricky business. From a correctness point of view: the process of generating this blob (TypedTreePickle `p_XYZ`) and resurrecting it (TypedTreePickle `u_*`) does some transformations to the Typed Tree that are necessary for correctness of compilation, for example, [in `TypedTreePickle`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/TypedTree/TypedTreePickle.fs#L738). Basically, the Typed Tree nodes from the compilation of one assembly are _not_ valid when compiling a different assembly.

The Typed Tree nodes include `CcuData` nodes, which have access to a number of callbacks into the `TcImports` compilation context for the assembly being compiled. TypedTree nodes are effectively tied to a particular compilation of a particular assembly due to this. There isn't any way to share this data without losing correctness and invalidating many invariants held in the current design. From a lifetime point of view: the Typed Tree nodes are tied together in a graph, so sharing one or two of them might drag across the entire graph and extend lifetimes of that graph. None of these interrelated nodes were designed to be shared across assemblies.


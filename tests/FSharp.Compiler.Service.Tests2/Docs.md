# Parallel type-checking in F#

This document describes the idea and implementation details for parallel type-checking of independent files in the F# compiler.

Performance of F# compilation and code analysis is one of the concerns for big codebases.

Despite several recent improvements in this area, there is still appetite and potential for change.

One idea for such an improvement was originally described in https://github.com/dotnet/fsharp/discussions/11634 by @kerams .
It is going to be the main topic of this page.

But before we dive into the details of the proposal, let's first discuss how the things work at the moment.

## Context and the current state of the compiler

### Current state of type-checking

One of the main phases of compilation is type-checking. Depending on the project in question, it can take as much as 50% of the total compilation time.
Currently, by default all files in a project are type-checked in sequence, one-by-one, leading to increased compilation wall-clock time.

The same is true about code analysis (used by the IDEs), but to an even higher degree - since code analysis skips some of the expensive compilation phases, type-checking represents a bigger fraction of the total wall-clock time, hence any improvements in this area can lead to more drastic total time reduction.

### Maintaining type-checking state

There is a lot of information associated with type-checking individual files and groups of them.

Currently, due to the (mostly) sequential nature of the processing, it is sufficient to maintain a single instance of such information for the whole project.
This instance is incrementally built on as more and more files have been processed.

### Recent addition - "Parallel type checking for impl files with backing sig files"

A recent [change](https://github.com/dotnet/fsharp/pull/13737) introduced in the compiler introduced a level of parallelism in type-checking.
It allows for parallel type-checking of all `.fs` files backed by `.fsi` files. Such `.fs` files by definition cannot be depended upon by any other files w.r.t. type-checking, since all the necessary information is exposed by the corresponding `.fsi` files.

The new feature, when enabled, allows partial parallelisation of type-checking as follows:
1. All `.fsi` files and `.fs` files without backing `.fsi` files are type-checked in sequence, as before.
2. Then all `.fs` files with backing `.fsi` files are type-checked in parallel.

For a project that uses `.fsi` files throughout, such as the `FSharp.Compiler.Service` project, this presents a major speedup.

Some data points:
- [Fantomas](https://github.com/fsprojects/fantomas) solution: total build time 17.49s -> 14.28s - [link](https://github.com/dotnet/fsharp/pull/13737#issuecomment-1223637818)
- F# codebase build time: 112s -> 92s - [link](https://github.com/dotnet/fsharp/pull/13737#issuecomment-1223386853)

#### Enabling the feature

The feature is opt-in and can be enabled in the compiler via a CLI arg & MSBuild property.

## The importance of using Server GC for parallel work

By default .NET processes use Workstation GC, which is single-threaded. What this means is it can become a bottleneck for highly-parallel operations, due to increased GC pressure and the cost of GC pauses being multiplied by the number of threads waiting. 
That is why when increasing parallelisation of the compiler and the compiler service it is important to note the GC mode being used and consider enabling Server GC.

Below is an example showing the difference it can make for a parallel workflow.

### Parallel projects analysis results for a synthetic solution
| GC Mode     | Processing Mode | Time                                    |
|-------------|-----------------|-----------------------------------------|
| Workstation | Sequential      | 16005ms                                 |
| Workstation | Parallel        | 10849ms                                 |
| Server      | Sequential      | 14594ms (-9% vs Workstation Sequential) |
| Server      | Parallel        | 2659ms (-75% vs Workstation Parallel)   |

For more details see https://github.com/dotnet/fsharp/pull/13521

## Parallel type-checking of independent files

### Background
Files in an F# project are ordered and processed from the top (first) and the bottom (last) file.
The compiler ensures that no information, including type information, flows upwards.

Consider the following list of files in a project:
```fsharp
A.fs
B.fs
C.fs
D.fs
```
By default, they are type-checked in the order of appearance: `[A.fs, B.fs, C.fs, D.fs]`

Let's define `allowed dependency` as follows:
> If the contents of 'B.fs' _can_, based on its position in the project hierarchy, influence the type-checking process of 'A.fs', then 'A.fs' -> 'B.fs' is an _allowed dependency_ 

The graph of dependencies between the files that the compiler _allows_ looks as follows:
```
A.fs -> []
B.fs -> [A.fs]
C.fs -> [B.fs; A.fs]
D.fs -> [C.fs; B.fs; A.fs]
```

Type-checking files in the appearance order guarantees that when processing a given file, any files it _might_ depend on w.r.t. type-checking have already been type-checked and their type information is available.

### Necessary dependencies

Let's define a `necessary dependency` too:
> File 'A.fs' _necessarily depends_ on file B for type-checking purposes, if the lack of type-checking information from 'B.fs' would influence the results of type-checking 'A.fs'

And finally a `dependency graph` as follows:
> A _dependency graph_ is any graph that is a subset of the `allowed dependencies` graph and a superset of the `necessary dependencies` graph

A few slightly imprecise/vague statements about all the graphs:
1. The _Necessary dependencies_ graph is a subgraph of the _allowed dependencies_ graph.
2. If there is no path between 'B.fs' and 'C.fs' in the _necessary dependencies_ graph, they can be type-checked in parallel, as long as there is a way to maintain and merge more than one instance of type-checking information.
3. Type-checking _must_ process files in an order that is compatible with the topological order in the _necessary dependencies_ graph.
4. If using a dependency graph as an ordering mechanism for (parallel) type-checking, the closer it is to the _necessary dependencies_ graph, the higher parallelism is possible.
5. Type-checking files in appearance order is equivalent to using the `allowed dependencies` graph for ordering.
6. Removing an edge from a _dependency_ graph _can_ increase (but not decrease) the level of parallelism possible and improve wall-clock time.

Let's look at point `5.` in detail.

### The impact of reducing the dependency graph 
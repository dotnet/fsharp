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

The main idea is quite simple:
- process files in a graph order instead of sequential order
- reduce the dependency graph used for type-checking, increasing parallelism
- implement branching and merging of type-checking information

Below is some quasi-theoretical background on this.

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
6. Removing an edge from the _dependency_ graph used _can_ increase (but not decrease) the level of parallelism possible and improve wall-clock time.

Let's look at point `6.` in more detail.

### The impact of reducing the dependency graph on type-checking parallelisation and wall-clock time.

Let's make a few definitions and assumptions:
1. Time it takes to type-check file f = 'T(f)'
2. Time it takes to type-check files f1...fn in parallel = 'T(f1+...fn)'
3. Time it takes to type-check a file f and all its dependencies = 'D(f)'
4. Type-checking is performed on a machine with infinite number of parallel processors.
5. There is no slowdowns due to parallel processing, ie. T(f1+...+fn) = max(T(f1),...,T(fn))

With the above it can be observed that:
```
D(f) = max(D(n)) + T(f), for n = any necessary dependency of f
```
In other words wall-clock time for type-checking using a given dependency graph is equal to the "longest" path in the graph.

Therefore the main goal that the idea presented here aims to achieve is to replace the _allowed dependencies_ graph as currently used with a reduced graph that's much closer to the _necessary dependencies_ graph, therefore optimising the type-checking process.

## A way to reduce the dependency graph used

For all practical purposes the only way to calculate the _necessary dependencies_ graph fully accurately is to perform the type-checking process, which misses the point of this exercise.

However, there exist cheaper solutions that reduce the initial graph significantly with low computational cost, providing a good trade-off.

As noted in https://github.com/dotnet/fsharp/discussions/11634 , scanning the ASTs can provide a lot information that helps narrow down the set of types, modules/namespaces and files that a given file _might_ depend on.

This is the approach we're taking.

The dependency detection algorithm can be summarised as follows:
1. For each parsed file in parallel:
   1. Extract its top-level module or a list of top-level namespaces
   2. Find all partial module references in the AST by traversing it once. Consider `AutoOpens`, module abbreviations, partial opens etc.
2. Build a single [Trie](https://en.wikipedia.org/wiki/Trie) by adding all top-level items extracted in 1.i. Every module/namespace _segment_ (eg. `FSharp, Compiler, Service in FSharp.Compiler.Service`) is represented by a single edge in the Trie. Note down positions of all added files in the Trie.
3. For each file in parallel:
   1. Clone the Trie.
   2. Start by marking the root as 'reachable'.
   3. Process all partial module references found in this file in 1.ii one-by-one, in order of appearance in the AST:
      1. For a given partial reference:
         1. Start at every 'reachable' node.
         2. 'Extend' the node with the new partial reference by walking down the Trie. If a leaf is reached, do not go further/do not extend the Trie.
         3. Mark the reached node as 'reachable'
   4. Collect all reachable nodes and their ancestors.
   5. Find all files that added one or more modules/namespaces to any of the nodes found in 5.
   6. Return those as dependencies.

### Graph optimisation - going deeper than just top-level module

In 1.i of the above algorithm we only consider the top-level module/namespace(s) for each file.
As soon as some other file is able to navigate to that top-level item, we consider it a dependency.

A more or less straightforward optimisation of this behaviour would be to:
1. In 1.i Collect a tree of modules/namespaces that contain any types/exceptions that can be used for type inference.
2. In 2. add all the leaves from the tree to the Trie.

This should further reduce the graph.

### Edge-case 1. - `[<AutoOpen>]`

Modules with `[<AutoOpen>]` are in a way 'transparent', meaning that all the types/nested modules inside them are surfaced as if they were on a level above.

The algorithm needs to consider that.

The main problem with that is that `AutoOpenAttribute` could be aliased and hide behind a different name.
Therefore it's not easy to see whether the attribute is being used based only on the AST.

There are ways to evaluate this, which involve scanning all module abbreviations in the project and in any referenced dlls.

However, for now, the algorithm simply treats every module with _any_ attributes as an `[<AutoOpen>]` module.

This retains correctness, but reduces efficiency of the graph reduction. Optimisation ideas here are welcome.

### Edge-case 2. - module abbreviations

At the moment the algorithm doesn't support files with module abbreviations. Adding support should be doable.

### Performance

Little to no effort has been invested in optimising the algorithm.

However, initial tests show that in its current unoptimised form it is very performant.

Sample results for `FSharp.Compiler.Service`:

| Phase                                    | Time                                |
|------------------------------------------|-------------------------------------|
| Parallel Parsing                         | 1.70s                               |
| Type-Checking                            | 21.6s (13s with fsi optimisation)   |
| Total compilation time w/o optimisations | 40.3s (33.3s with fsi optimisation) |
| Dependency resolution - total            | 0.23s                               |
| Dependency resolution - AST traversal    | 0.18s                               |
| Dependency resolution - Trie processing  | 0.05s                               |

Things that can be easily improved:
- Quicker AST traversal - tail-recursion, not using `seq`, avoid allocations
- Quicker Trie operations

#### Overhead of dispatching work and branching/merging state

On top of dependency resolution, the feature will add some overhead for dispatching work and allowing separation of type-checking state in the graph.

No timings are available at the moment.

## The problem of maintaining multiple instances of type-checking information

The parallel type-checking idea generates a problem that needs to be solved.
Instead of one instance of the type-checking information, we now have to:
- 'clone' an instance multiple times when passing the state from one file to one or more of its dependants
- 'merge' an instance when receiving state instances from one or more dependencies

We believe this is doable, although no implementation exists as of yet.

### Ordering of diagnostics/errors

As noted in https://github.com/dotnet/fsharp/pull/13737#issuecomment-1224124532 , disrupting the processing order makes it difficult to retain the original behaviour when it comes to the order in which diagnostics are presented and suppressed.

The importance of this problem and potential solutions are open questions.
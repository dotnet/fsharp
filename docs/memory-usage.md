(**
---
title: Memory usage
category: Compiler
categoryindex: 1
index: 4
---
*)
## Compiler Memory Usage

Overall memory usage is a primary determinant of the usability of the F# compiler and instances of the F# compiler service. Overly high memory usage results in poor throughput (particularly due to increased GC times) and low user interface responsivity in tools such as Visual Studio or other editing environments. In some extreme cases, it can lead to Visual Studio crashing or another IDE becoming unusable due to constant paging from absurdly high memory usage. Luckily, these extreme cases are very rare.

### Why memory usage matters

When you do a single compilation to produce a binary, memory usage typically doesn't matter much. It's often fine to allocate a lot of memory because it will just be reclaimed after compilation is over.

However, the F# compiler is not simply a batch process that accepts source code as input and produces an assembly as output. When you consider the needs of editor and project tooling in IDEs, the F# compiler is:

* An engine that processes syntax trees and outputs data at various stages of compilation
* A database of syntactic and semantic data about the code hosted in an IDE
* A server process that accepts requests for syntactic and semantic information
* An API layer for tools to request tooling-specific data (e.g., F# tooltip information)

Thinking about the F# compiler in these ways makes performance far more complicated than just throughput of a batch compilation process.

### Kinds of data processed and served in F# tooling

The following tables are split into two categories: syntactic and semantic. They contain common kinds of information requested, the kind of data that is involved, and roughly how expensive the operation is.

#### IDE actions based on syntax

|  Action | Data inspected | Data returned | Cost (S/M/L/XL) |
|---------|---------------|---------------|-----------------|
| Syntactic Classification | Current document's source text | Text span and classification type for each token in the document | S |
| Breakpoint Resolution | Current document's syntax tree | Text span representing where breakpoing where resolve | S |
| Debugging data tip info | Current document's source text | Text span representing the token being inspected | S |
| Brace pair matching | Current document's source text | Text spans representing brace pairs that match in the input document | S |
| "Smart" indentation | Current document's source text | Indentation location in a document | S |
| Code fixes operating only on syntax | Current document's source text | Small text change for document | S |
| XML doc template generation | Current document's syntax tree | Small (usually) text change for document | S |
| Brace pair completion | Current line in a source document | Additional brace pair inserted into source text | S |
| Souce document navigation (usually via dropdowns) | Current document's syntax tree | "Navigation Items" with optional child navigation items containing ranges in source code | S |
| Code outlining | Current document's source text | Text spans representing blocks of F# code that are collapsable as a group | S - M |
| Editor formatting | Current document's source text | New source text for the document | S - L |
| Syntax diagnostics | Current document's source text | List of diagnostic data including the span of text corresponding to the diagnostic | S |
| Global construct search and navigation | All syntax trees for all projects | All items that match a user's search pattern with spans of text that represent where a given item is located | S-L |

You likely noticed that nearly all of the syntactical operations are marked `S`. Aside from extreme cases, like files with 50k lines or higher, syntax-only operations typically finish very quickly. In addition to being computationally inexpensive, they are also run asynchronously and free-threaded.

Editor formatting is a bit of an exception. Most IDEs offer common commands for format an entire document, and although they also offer commands to format a given text selection, users typically choose to format the whole document. This means an entire document has to be inspected and potentially rewritten based on often complex rules. In practice this isn't bad when working with a document that has already been formatted, but it can be expensive for larger documents with strange stylistic choices.

Most of the syntax operations require an entire document's source text or parse tree. It stands to reason that this could be improved by operating on a diff of a parse tree instead of the whole thing. This is likely a very complex thing to implement though, since none of the F# compiler infrastructure works in this way today.

#### IDE actions based on semantics

|  Action | Data inspected | Data returned | Cost (S/M/L/XL) |
|---------|---------------|---------------|-----------------|
| Most code fixes | Current document's typecheck data | Set (1 or more) of suggested text replacements | S-M |
| Semantic classification | Current document's typecheck data | Spans of text with semantic classification type for all constructs in a document | S-L |
| Code lenses | Current document's typecheck data and top-level declarations (for showing signatures); graph of all projects that reference the current one (for showing references) | Signature data for each top-level construct; spans of text for each reference to a top-level construct with navigation information | S-XL |
| Code generation / refactorings | Current document's typecheck data and/or current resolved symbol/symbols | Text replacement(s) | S-L |
| Code completion | Current document's typecheck data and currently-resolved symbol user is typing at | List of all symbols in scope that are "completable" based on where completion is invoked | S-L |
| Editor tooltips | Current document's typecheck data and resolved symbol where user invoked a tooltip | F# tooltip data based on inspecting a type and its declarations, then pretty-printing them | S-XL |
| Diagnostics based on F# semantics | Current document's typecheck data | Diagnostic info for each symbol with diagnostics to show, including the range of text associated with the diagnostic | M-XL |
| Symbol highlighting in a document | Current document's typecheck data and currently-resolved symbol where user's caret is located | Ranges of text representing instances of that symbol in the document | S-M |
| Semantic navigation (for example, Go to Definition) | Current document's typecheck data and currently-resolved symbol where the user invoked navigation | Location of a symbol's declaration | S-M |
| Rename | Graph of all projects that use the symbol that rename is triggered on and the typecheck data for each of those projects | List of all uses of all symbols that are to be renamed | S-XL |
| Find all references | Graph of all projects that Find References is triggered on and the typecheck data for each of those projects | List of all uses of all symbols that are found | S-XL |
| Unused value/symbol analysis | Typecheck data for the current document | List of all symbols that aren't a public API and are unused | S-M |
| Unused `open` analysis | Typecheck data for the current document and all symbol data brought into scope by each `open` declaration | List of `open` declarations whose symbols it exposes aren't used in the current document | S-L |
| Missing `open` analysis | Typecheck data for the current document, resolved symbol with an error, and list of available namespaces or modules | List of candidate namespaces or modules that can be opened | S-M |
| Misspelled name suggestion analysis | Typecheck data for the current document and resolved symbol with an error | List of candidates that are in scope and best match the misspelled name based on a string distance algorithm | S-M |
| Name simplification analysis | Typecheck data for the current document and all symbol data brought into scope by each `open` declaration | List of text changes available for any fully- or partially-qualified symbol that can be simplified | S-XL |

You likely noticed that every cost associated with an action has a range. This is based on two factors:

1. If the semantic data being operated on is cached
2. How much semantic data must be processed for the action to be completed

Most actions are `S` if they operate on cached data and the compiler determines that no data needs to be re-computed. The size of their range is influenced largely by the _kind_ of semantic operations each action has to do, such as:

* Typechecking a single document and processing the resulting data
* Typechecking a document and its containing project and then processing the resulting data
* Resolving a single symbol in a document
* Resolving the definition of a single symbol in a codebase
* Inspecting all symbols brought into scope by a given `open` declaration
* Inspecting all symbols in a document
* Inspecting all symbols in all documents contained in a graph of projects

For example, commands like Find All References and Rename can be cheap if a codebase is small, hence the lower bound being `S`. But if the symbol in question is used across many documents in a large project graph, they are very expensive because the entire graph must be crawled and all symbols contained in its documents must be inspected.

In contrast, actions like highlighting all symbols in a document aren't terribly expensive even for very large file files. That's because the symbols to be inspected are ultimately only in a single document.

### Analyzing compiler memory usage

In general, the F# compiler allocates a lot of memory. More than it needs to. However, most of the "easy" sources of allocations have been squashed out and what remains are many smaller sources of allocations. The remaining "big" pieces allocate as a result of their current architecture, so it isn't straightforward to address them.

To analyze memory usage of F# tooling, you have two primary avenues:

1. Take a process dump on your machine and analyze it with process dump analysis tools like [dotMemory](https://www.jetbrains.com/dotmemory/)
2. Use a sampling tool like [PerfView](https://github.com/Microsoft/perfview) or [dotTrace](https://www.jetbrains.com/profiler/) to collect a trace of your system while you perform various tasks in an IDE, ideally for 60 seconds or more.

#### Analyzing a process dump file

Process dump files are extremely information-rich data files that can be used to see the distribution of memory usage across various types. Tools like [dotMemory](https://www.jetbrains.com/dotmemory/) will show these distributions and intelligently group things to help identify the biggest areas worth improving. Additionally, they will notice things like duplicate strings and sparse arrays, which are often great ways to improve memory usage since it means more memory is being used than is necessary.

As of F# 5, one of the most prominent sources of memory usage is `ILModuleReader`, often making up more than 20% of total memory usage for a given session. There is a considerably large "long tail" of small chunks of memory usage that in aggreate add up to a lot of resource utilization. Many can be improved.

#### Analyzing a sample trace of IDE usage

The other important tool to understand memory and CPU usage for a given sample of IDE usage is a trace file. These are collected and analyzed by tools like [PerfView](https://github.com/Microsoft/perfview) and [dotTrace](https://www.jetbrains.com/profiler/).

When analyzing a trace, there are a few things to look out for:

1. Overall GC statistics for the sample to give an overall picture of what was going on in the IDE for your sample:
   a. How much CPU time was spent in the GC as a percentage of total CPU time for the IDE process?
   b. What was the peak working set (total memory usage)?
   c. What was the peak allocations per second?
   d. How many allocations were Gen0? Gen1? Gen2?
2. Memory allocations for the sample, typically also ignoring object deaths:
   a. Is `LargeObject` showing up anywhere prominently? If so, that's a problem!
   b. Which objects show up highest on the list? Does their presence that high make sense?
   c. For a type such as `System.String`, which caller allocates it the most? Can that be improved?
3. CPU sampling data, sorted by most CPU time
   a. Are any methods showing up that correspond with high memory allocations? Something showing up prominently in both places is often a sign that it needs work!

After analyzing a trace, you should have a good idea of places that could see improvement. Often times a tuple can be made into a struct tuple, or some convenient string processing could be adjusted to use a `ReadonlySpan<'T>` or turned into a more verbose loop that avoids allocations.

### The cross-project references problem

The compiler is generally built to compile one assembly: the assumption that the compiler is compiling one assembly is baked into several aspects of the design of the Typed Tree.

In contract, FCS supports compiling a graph of projects, each for a different assembly. The Typed Tree nodes are **not** shared between different project compilations. This means that representation of project references is roughly O(n^2) in memory usage. In practice it's not strictly O(n^2), but for very large codebases the proportionality is felt.

Some key things to understand are:

* The `RawFSharpAssemblyData` is the data blob that would normally be stuffed in the F# resource in the generated DLL  in a normal compilation. That's the "output" of checking each project.

* This is used as "input" for the assembly reference of each consuming project (instead of an on-disk DLL)

* Within each consuming project that blob is then resurrected to Typed Tree nodes in `TypedTreePickle.fs`.

The biggest question is: could the compiler share this data across projects? In theory, yes. In practice, it's very tricky business.

From a correctness point of view: the process of generating this blob (TypedTreePickle `p_XYZ`) and resurrecting it (TypedTreePickle `u_*`) does some transformations to the Typed Tree that are necessary for correctness of compilation, for example, [in `TypedTreePickle`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/TypedTreePickle.fs#L738). Basically, the Typed Tree nodes from the compilation of one assembly are _not_ valid when compiling a different assembly.

The Typed Tree nodes include `CcuData` nodes, which have access to a number of callbacks into the `TcImports` compilation context for the assembly being compiled. TypedTree nodes are effectively tied to a particular compilation of a particular assembly due to this.

There isn't any way to share this data without losing correctness and invalidating many invariants held in the current design.

From a lifetime point of view: the Typed Tree nodes are tied together in a graph, so sharing one or two of them might drag across the entire graph and extend lifetimes of that graph. None of these interrelated nodes were designed to be shared across assemblies.


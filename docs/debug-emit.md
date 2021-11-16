---
title: Debug emit
category: Compiler Internals
categoryindex: 200
index: 350
---
# Debug emit

The F# compiler code base emits debug information and attributes. This article documents what we do, how it is implemented and the problem areas in our implementation.

> NOTE: There are mistakes and missing pieces to our debug information. Small improvements can make a major difference. Please help us fix mistakes and get things right.

> NOTE: The file `tests\walkthroughs\DebugStepping\TheBigFileOfDebugStepping.fsx` is crucial for testing the stepping experience for a range of constructs.

## User experiences

Debugging information affects numerous user experiences:

* **Call stacks** during debugging
* **Breakpoint placement** before and during debugging
* **Locals** during debugging
* **Just my code** debugging (which limits the view of debug code to exclude libraries)
* **Exception** debugging (e.g. "first chance" debugging when exceptions occur)
* **Stepping** debugging
* **Watch** window
* **Profiling** results
* **Code coverage** results

Some experiences are un-implemented by F# including:

* **Autos** during debugging
* **Edit and Continue**
* **Hot reload**

## Emitted information

Emitted debug information includes:

* The names of methods in .NET IL
* The PDB file/information (embedded or in PDB file) which contains
  * Debug "sequence" points for IL code
  * Names of locals and the IL code scopes over which those names are active
* The attributes on IL methods such as `CompilerGeneratedAttribute` and `DebuggerNonUserCodeAttribute`, wee below
* We add some codegen to give better debug experiences, see below.

## Design-time services

IDE tooling performs queries into the F# language service, notably:

* `ValidateBreakpointLocation` [(permalink)](https://github.com/dotnet/fsharp/blob/24979b692fc88dc75e2467e30b75667058fd9504/src/fsharp/service/FSharpParseFileResults.fs#L795) is called to validate every breakpoint before debugging is launched. This operates on syntax trees. See notes below.

## Missing debug emit

### Missing debug emit for PDBs

Our PDB emit is missing considerable information:

* Not emitted: [LocalConstants table](https://github.com/dotnet/fsharp/issues/12003)
* Not emitted: [Compilation options table](https://github.com/dotnet/fsharp/issues/12002)
* Not emitted: [Dynamic local variables table](https://github.com/dotnet/fsharp/issues/12001)
* Not emitted: [StateMachineMethod table and StateMachineHoistedLocalScopes table](https://github.com/dotnet/fsharp/issues/12000)
* Not emitted: [ImportScopes table](https://github.com/dotnet/fsharp/issues/1003)

These are major holes in the F# experience. Some are required for things like hot-reload.

### Missing design-time services

Some design-time services are un-implemented by F#:

* Unimplemented: [F# expression evaluator](https://github.com/dotnet/fsharp/issues/2544)
* Unimplemented: [Proximity expressions](https://github.com/dotnet/fsharp/issues/4271) (for Autos window)

These are major holes in the F# experience and should be implemented.

### Missing debug emit for F# Interactive

For F# Interactive [we do not currently emit debug information for script code](https://github.com/dotnet/fsharp/issues/5457). This is because of a missing piece of functionality in the Reflection.Emit APIs, and means we have to change our approach to emitting code fragments in F# Interactive to no longer use dynamic assemblies.

## Debug points

### Terminology

We use the terms "sequence point" and "debug point" interchangeably. The word "sequence" has too many meanings in the F# compiler so in the actual code you'll see "DebugPoint" more often, though for abbreviations you may see `spFoo` or `mFoo`.

### How breakpoints work (high level)

Breakpoints have two existences which must give matching behavior:

* At design-time, before debugging is launched, `ValidateBreakpointLocation` is called to validate every breakpoint.  This operators on the SyntaxTree and forms a kind of "gold-standard" about the exact places where break points are valid.

* At run-time, breakpoints are "mapped" by the .NET runtime to actual sequence points found in .NET methods. The runtime searches all methods with debug points for the relevant document and determines where to "bind" the actual breakpoint to.  A typical debugger can bind a breakpoint to multiple locations.

This means there is an invariant that `ValidateBreakpointLocation` and the emitted IL debug points correspond.

> NOTE: The IL code can contain extra debug points. It won't be possible to set a breakpoint for these, but they will appear in stepping

### Debug points for control-flow constructs

The intended debug points for control-flow constructs are as follows:

|  Construct   | Debug points     |
|:-----------|:----------------|
| `let ..`    |   See below |
| `let rec ..`   | Implicit on body |
| `if .. then ..`   | `if .. then` and implicit on body |
| `if .. then .. else ..`    | `if .. then` and implicit on branches |
| `match .. with ..`   | `match .. with` and `when` patterns and implicit on case targets |
| `while .. do ..`   | `while .. do` and implicit on body |
| `for .. do`  | `for .. do` and implicit on body |
| `try .. with ..`  | `try` and `with` and implicit on body and handler |
| `try .. finally ..`   | `try` and `finally` and implicit on body and handler |
| `use ..` | See below for `let` |
| `expr1; expr` sequential | On `expr1` and implicit on `expr2` |

### Debug points for let-bindings

`let` bindings get immediate debug points if the thing is not a function and the implementation is not control flow. For example

```fsharp
let f () =
    let x = 1 // debug point for whole of `let x = 1`
    let f x = 1 // no debug point on `let f x =`, debug point on `1`
    let x = if today then 1 else tomorrow // no debug point on `let x =`, debug point on `if today then` and `1` and `tomorrow`
    let x = let y = 1 in y + y // no debug point on `let x =`, debug point on `let y = 1` and `y + y`
    ...
```

### Debug points for nested control-flow

Debug points are not generally emitted for non-statement constructs, e.g. consider:

```fsharp
let h1 x = g (f x)
let h2 x = x |> f |> g
```

Here `g (f x)` gets one debug point. Note that the corresponding pipelining gets three debug points.

If however a nested expression is control-flow, then debug points start being emitted again e.g.

```fsharp
let h3 x = f (if today then 1 else 2)
```

Here debug points are at `if today then` and `1` and `2` and all of `f (if today then 1 else 2)`

> NOTE: these debug points are overlapping

### Debug points internally in the compiler

Most (but not all) debug points are noted by the parser by adding `DebugPointAtTarget`, `DebugPointAtSwitch`, `DebugPointAtSequential`, `DebugPointAtTry`, `DebugPointAtWith`, `DebugPointAtFinally`, `DebugPointAtFor`, `DebugPointAtWhile` or `DebugPointAtBinding`.

These are then used by `ValidateBreakpointLocation`. These same values are also propagated unchanged all the way through to `IlxGen.fs` for actual code generation, and used for IL emit, e.g. a simple case like this:

```fsharp
    match spTry with
    | DebugPointAtTry.Yes m -> CG.EmitDebugPoint cgbuf m ... 
    | DebugPointAtTry.No -> ...
    ...
```

For many constructs this is adequate. However, in practice the situation is far more complicated.

### Implicit debug points

Some debug points are implicit. In particular, whenever a non-control-flow expression (e.g. a constant or a call) is used in statement position (e.g. as the implementation of a method, or the body of a `while`) then there is an implicit debug point.

* "Statement position" is tracked by the `spAlways` argument within ValidateBreakpointLocation ([permalink](https://github.com/dotnet/fsharp/blob/24979b692fc88dc75e2467e30b75667058fd9504/src/fsharp/service/FSharpParseFileResults.fs#L481))
* "Statement position" is similarly tracked by `SPAlways` within IlxGen.fs [permalink](https://github.com/dotnet/fsharp/blob/24979b692fc88dc75e2467e30b75667058fd9504/src/fsharp/IlxGen.fs#L2290)

Implicit debug points but they also arise in some code-generated constructs or in backup paths in the compiler implementation. In general we want to remove or reduce the occurrence of these and make things more explicit. However they still exist, especially for "lowered" constructs.  

> For example, `DebugPointAtTry.Body` represents a debug point implicitly located on the body of the try (rather than a `try` keyword).  Searching the source code, this is generated in the "try/finally" implied by a "use x = ..." construct ([permalink](https://github.com/dotnet/fsharp/blob/24979b692fc88dc75e2467e30b75667058fd9504/src/fsharp/CheckExpressions.fs#L10337)).  Is a debug point even needed here? Yes, because otherwise the body of the "using" wouldn't get a debug point.

### Debug points for `[ ...]`, `[| ... |]` code

The implementation of debug points for list and array expressions is conceptually simple but a little complex.

Conceptually the task is easy, e.g. `[ while check() do yield x + x ]` is lowered to code like this:

```fsharp
let $collector = ListCollector<int>()
while check() do
    $collector.Add(x+x)
$collector.Close()
```

Note the `while` loop is still a `while` loop - no magic here - and the debug points for the `while` loop can also apply to the actual generated `for` loop.

However, the actual implementation is more complicated because there is a TypedTree representation of the code in-between that at first seems to bear little resemblance to what comes in.

```text
SyntaxTree --[CheckComputationExpressions.fs]--> TypedTree --> IlxGen -->[LowerComputedListOrArrayExpr.fs]--> IlxGen
```

The TypedTree is a functional encoding into `Seq.toList`, `Seq.singleton` and so on. How do the debug points get propagated?

* In [`CheckComputationExpressions.fs`](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/CheckComputationExpressions.fs#L1783-L1787) we "note" the debug point for the For loop and attach it to one of the lambdas generated in the TypedTreeForm
* In [`LowerCallsAndSeq.fs`](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/LowerCallsAndSeqs.fs#L138-L139) we "recover" the debug point from precisely that lambda.
* This becomes [an actual debug point in the actual generated "while" loop](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/LowerCallsAndSeqs.fs#L887)

This then gives accurate debug points for these constructs.

### Debug Points for `seq { .. .}` code

Debug points for `seq { .. }` compiling to state machines poses similar problems.

* The de-sugaring is as for list and array expressions
* The debug points are recovered in the state machine generation, for example [here (permalink)](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/LowerCallsAndSeqs.fs#L367)

### Debug Points for `task { .. .}` code

Debug points for `task { .. }` poses much harder problems. We use "while" loops as an example:

* The de-sugaring is for computation expressions, and in CheckComputationExpressions.fs "notes" the debug point ranges for the relevant constructs attaching them to the `task.While(...)` call ([example permalink](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/CheckComputationExpressions.fs#L960))
* The code is then checked and optimized, and all the resumable code is inlined, e.g. [`task.While`](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/FSharp.Core/tasks.fs#L64) becomes [`Resumable.While`](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/FSharp.Core/resumable.fs#L176-L191) which contains a resumable code while loop.
* When inlining the code for `task.While(...)` and all associated transitive inlining, the `remarkExpr` routine is invoked as usual to rewrite all ranges throughout all inlined code to be the range of the outer expression, that is, precisely the earlier noted range. Now [`remarkExpr` is "hacked" to note that the actual resumable "while" loop is being inlined at a noted range](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/TypedTreeOps.fs#L5827-L5832), and places a debug point for that resumable while loop.
* The debug ranges are now attached to the resumable code which is then checked for resumable-code validity and emitted, e.g. see [this](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/LowerStateMachines.fs#L298)

This however only works fully for those constructs with a single debug point that can be recovered. In particular `TryWith` and `TryFinally` have separate problems

* `task.TryWith(...)` becomes a resumable code try/with, see [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/FSharp.Core/resumable.fs#L216-L230)
* `task.TryFinally(...)` becomes a resumable code try/with, see [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/FSharp.Core/resumable.fs#L272-L305)
* Some debug points associated with these `try/with` are suppressed in [`remarkExpr`](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/TypedTreeOps.fs#L5862-L5880)
* The debug points for the `with` and `finally` are not currently recovered.

### Debug Points for other computation expressions

Other computation expressions such as `async { .. }` have significant problems with their debug points, for multiple reasons:

* The debug points are largely lost during de-sugaring
* The computations are often "cold-start" anyway, leading to a two-phase debug problem

Debug points can often be placed for user code, e.g. sequential imperative statements or `let` bindings. However debug points for control constructs are often lossy or buggy.

> NOTE: A systematic solution for quality debugging of computation expressions and resumable code is still elusive.  It really needs the de-sugaring to explicitly or implicitly pass down the debug points through the process of inlining code.  For example consider the de-sugaring:

```fsharp
   builder { for x in xs do ... } --> builder.For(xs, fun x -> ...)
```

Here the debug points could be made explicit and passed as "compile-time parameters" (assuming inlining)

```fsharp
   builder { for[dp] x in xs do ... } --> builder.For(dp, xs, fun x -> ...)
```

These could then be used in the implementation:

```fsharp
type MuBuilder() =
    // Some builder implementation of "For" - let's say it prints at each iteration of the loop
    member inline _.For(dp, xs, f) =
        for[dp] x in xs do
           printfn "loop..."
           f x
```

Adding such compile-time parameters would be over-kill, but it may be possible to augment the compiler to keep a well-specified environment through the process of inlining, e.g.

```fsharp
   builder { for[dp] x in xs do ... } --> builder.For["for-debug-point"-->dp](xs, fun x -> ...)
```

And then there is some way to access this and attach to various control constructs:

```fsharp
type MuBuilder() =
    // Some builder implementation of "For" - let's say it prints at each iteration of the loop
    member inline _.For(dp, xs, f) =
        for["for-debug-point"] x in xs do
           printfn "loop..."
           f x
```

If carefully used this would allow reasonable debugging across multiple-phase boundaries.

> NOTE: The use of library code to implement "async" and similar computation expressions also interacts badly with "Just My Code" debugging, see https://github.com/dotnet/fsharp/issues/5539 for example.

> NOTE: The use of many functions to implement "async" and friends implements badly with "Step Into" and "Step Over" and related attributes, see for example https://github.com/dotnet/fsharp/issues/3359

### FeeFee and F00F00 debug points (Hidden and JustMyCodeWithNoSource)

Some fragments of code use constructs like calls that should not have debug points.

These are generated in IlxGen as "FeeFee" debug points. See the [old blog post on this](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.sequencepoint.hiddenline?view=net-5.0).

There is also the future prospect of generating `JustMyCodeWithNoSource` (0xF00F00) debug points but these are not yet emitted by F#.  We should check when the C# compiler emits these.

We always make space for a debug point at the head of each method by [emitting a FeeFee debug sequence point](https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fs#L1953). This may be immediately replaced by a "real" debug point [here](https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fs#L2019). For more information on FeeFee debug points see [this old blog post](https://blogs.msdn.microsoft.com/jmstall/2005/06/19/line-hidden-and-0xfeefee-sequence-points/).

## Generated code

The F# compiler generates entire IL classes and methods for constructs such as records, closures, state machines and so on. Each time code is generated we must carefully consider what attributes and debug points are generated.

### Generated "augment" methods for records, unions and structs

We currently always emit a debug sequence point for all generated code coming from AugmentWithHashCompare.fs (also  anything coming out of optimization etc.)  The `SPAlways` at https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fs#L4801 has the effect that a debug point based on the range of the method will always appear.

### Generated "New*", "Is*", "Tag" etc. for unions

Discriminated unions generate `NewXYZ`, `IsXYZ`, `Tag` etc. members and the implementations of these lay down debug points. See [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseUnions.fs#L644) for the data that drives this and track back and forth to the production and consumption points of that data.

These all get `CompilerGeneratedAttribute`, and `DebuggerNonUserCodeAttribute`, e.g. [here (permalink)](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseUnions.fs#L635)

> TODO: generating debug points for these appears wrong, being assessed at time of writing

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing

### Generated closures for lambdas

The codegen involved in closures is as follows:

|  Source         | Construct         | Debug Points | Attributes   | Notes |
|:----------------|:------------------|:-------------|:-------------|:------|
| (fun x -> ...)  | Closure class     |              |              |       |
|                 | `.ctor` method    |      | CompilerGenerated, DebuggerNonUserCode | |
|                 | `Invoke` method   |      |                                        | |
| generic local defn  | Closure class |      |                                        | |
|                 | `.ctor` method    |      | CompilerGenerated, DebuggerNonUserCode | |
|                 | `Specialize` method |    |                                        | |
|  Intermediate closure classes   |   | See [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseClosures.fs#L459) and [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseClosures.fs#L543).  | CompilerGeneratedAttribute, DebuggerNonUserCodeAttribute     | These are for long curried closures `fun a b c d e f -> ...`.  |

> TODO: generating debug points for the intermediate closures appears wrong, this is being assessed at time of writing

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing

### Generated state machines for `seq { .. }`

Sequence expressions generate class implementations which resemble closures.

The debug points recovered for the generated state machine code for `seq { ... }` is covered up above. The other codegen is as follows:

|  Source         | Construct         | Debug Points | Attributes   | Notes |
|:----------------|:------------------|:-------------|:-------------|:------|
| seq { ... }     | State machine class |            |  "Closure"             |       |
|                 | `.ctor` method    |   none?      |  [none?](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5150) | |
|                 | `GetFreshEnumerator`   |  [none?](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5108)  | CompilerGenerated, DebuggerNonUserCode | |
|                 | `LastGenerated`   |  [none?](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5146-L5148)  | CompilerGenerated, DebuggerNonUserCode | |
|                 | `Close`   |  [none?](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5124-L5127)  | none!? | |
|                 | `get_CheckClose`  |  [none?](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5130-L5133)  | none!? | |
|                 | `GenerateNext`    |  from desugaring, and [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5136-L5143) | none | |

> NOTE: it appears from the code that extraneous debug points are not being generated, which is good, though should be checked

> TODO: we should likely be generating attributes for the "Close" and other methods

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing

### Generated state machines for `task { .. }`

[Resumable state machines](https://github.com/fsharp/fslang-design/blob/main/FSharp-6.0/FS-1087-resumable-code.md) used for `task { .. }` also generate struct implementations which resemble closures.

The debug points recovered for the generated state machine code for `seq { ... }` is covered up above. The other codegen is as follows:

|  Source         | Construct         | Debug Points | Attributes   | Notes |
|:----------------|:------------------|:-------------|:-------------|:------|
| task { ... }     | State machine struct |          |  "Closure"   |       |
|                 | `.ctor` method    |   none?      |  none?       | |
|                 | TBD               |              |              | |

> TODO: we should be generating attributes for some of these

> TODO: we should assess that only the "MoveNext" method gets any debug points at all

### Generated code for delegate constructions `Func<int,int>(fun x y -> x + y)`

TBD.  A closure class is generated.

### Generated code for constant-sized array and list expressions

TBD. These are not generally problematic for debug.

### Generated code for large constant arrays

TBD

### Generated code for pattern matching

TBD

### Generated code for conditionals and boolean logic

See for example [this proposed feature improvement](https://github.com/dotnet/fsharp/issues/11980)

### Capture and closures

Captured locals are available via the `this` pointer of the immediate closure.  Un-captured locals are **not** available as things stand.  See for example [this proposed feature improvement](https://github.com/dotnet/fsharp/issues/11262).

Consider this code:

```fsharp
let F() =
    let x = 1
    let y = 2
    (fun () -> x + y)
```

Here `x` and `y` become closure fields of the closure class generated for the final lambda. When inspecting locals in the inner closure, the C# expression evaluator we rely on for Visual Studio takes local names like `x` and `y` and is happy to look them up via `this`. This means hovering over `x` correctly produces the value stored in `this.x`.

For nested closures, values are implicitly re-captured, and again the captured locals will be available.

However this doesn't work with "capture" from a class-defined "let" context. Consider the following variation:

```fsharp
type C() =
    let x = 1
    member _.M() = 
        let y = 2
        (fun () -> x + y)
```

Here the implicitly captured local is `y`, but `x` is **not** captured, instead it is implicitly rewritten by the F# compiler to `c.x` where `c` is the captured outer "this" pointer of the invocation of `M()`.  This means that hovering over `x` does not produce a value. See [issue 3759](https://github.com/dotnet/fsharp/issues/3759).

### Provided code

Code provided by erasing type providers has all debugging points removed.  It isn't possible to step into such code or if there are implicit debug points they will be the same range as the construct that was macro-expanded by the code erasure. 

> For example, a [provided if/then/else expression has no debug point](https://github.com/dotnet/fsharp/blob/main/src/fsharp/MethodCalls.fs#L1805)

## Added code generation for better debugging

We do some "extra" code gen to improve debugging. It is likely much of this could be removed if we had an expression evaluator for F#.

### 'this' value

For `member x.Foo() = ...` the implementation of the member adds a local variable `x` containing the `this` pointer from `ldarg.0`. THis means hovering over `x` in the method produces the right value, as does `x.Property` etc.

### Pipeline debugging

For pipeline debugging we emit extra locals for each stage of a pipe and debug points at each stage.

See [pipeline debugging mini-spec](https://github.com/dotnet/fsharp/pull/11957).

### Shadowed locals

For shadowed locals we change the name of a local for the scope for which it is shadowed.

See [shadowed locals mini-spec](https://github.com/dotnet/fsharp/pull/12018).

### Discriminated union debug display text

For discriminated union types and all implied subtypes we emit a `DebuggerDisplayAttrubte` and a private `__DebugDisplay()` method that uses `sprintf "%+0.8A" obj` to format the object.

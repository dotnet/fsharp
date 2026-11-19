---
title: Optimizations
category: Compiler Internals
categoryindex: 200
index: 400
---
# Code Optimizations

Code optimizations are in [`Compiler/Optimize/*`](https://github.com/dotnet/fsharp/tree/main/src/Compiler/Optimize).

Some of the optimizations performed in [`Optimizer.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/Optimizer.fs) are:

* Propagation of known values (constants, x = y, lambdas, tuples/records/union-cases of known values)
* Inlining of known lambda values
* Eliminating unused bindings
* Eliminating sequential code when there is no side-effect
* Eliminating switches when we determine definite success or failure of pattern matching
* Eliminating getting fields from an immutable record/tuple/union-case of known value
* Expansion of tuple bindings "let v = (x1,...x3)" to avoid allocations if it's not used as a first class value
* Splitting large functions into multiple methods, especially at match cases, to avoid massive methods that take a long time to JIT
* Removing tailcalls when it is determined that no code in the transitive closure does a tailcall nor recurses

In [`DetupleArgs.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/DetupleArgs.fs), tuples at call sites are eliminated if possible. Concretely, functions that accept a tuple at all call sites are replaced by functions that accept each of the tuple's arguments individually. This may require inlining to activate.

Considering the following example:

```fsharp
let max3 t =
    let (x, y, z) = t
    max x (max y z)

max3 (1, 2, 3)
```

The `max3` function gets rewritten to simply accept three arguments, and depending on how it gets called it will either get inlined at the call site or called with 3 arguments instead of a new tuple. In either case, the tuple allocation is eliminated.

However, sometimes this optimization is not applied unless a function is marked `inline`. Consider a more complicated case:

```fsharp
let rec runWithTuple t offset times =
    let offsetValues x y z offset =
        (x + offset, y + offset, z + offset)
    if times <= 0 then
        t
    else
        let (x, y, z) = t
        let r = offsetValues x y z offset
        runWithTuple r offset (times - 1)
```

The inner function `offsetValues` will allocate a new tuple when called. However, if `offsetValues` is marked as `inline` then it will no longer allocate a tuple.

Currently, these optimizations are not applied to `struct` tuples or explicit `ValueTuple`s passed to a function. In most cases, this doesn't matter because the handling of `ValueTuple` is well-optimized and may be erased at runtime. However, in the previous `runWithTuple` function, the overhead of allocating a `ValueTuple` each call ends up being higher than the previous example with `inline` applied to `offsetValues`. This may be addressed in the future.

In [`InnerLambdasToTopLevelFuncs.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/InnerLambdasToTopLevelFuncs.fs), inner functions and lambdas are analyzed and, if possible, rewritten into separate methods that do not require an `FSharpFunc` allocation.

Consider the following implementation of `sumBy` on an F# list:

```fsharp
let sumBy f xs =
    let rec loop xs acc =
        match xs with
        | [] -> acc
        | x :: t -> loop t (f x + acc)
    loop xs 0
```

The inner `loop` function is emitted as a separate static method named `loop@2` and incurs no overhead involved with allocating an `FSharpFunc` at runtime.

In [`LowerCalls.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/LowerCalls.fs): 

* Performs eta-expansion on under-applied values applied to lambda expressions and does a beta-reduction to bind any known arguments

In [`LowerSequences.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/LowerSequences.fs): 

* Analyzes a sequence expression and translates it into a state machine so that operating on sequences doesn't incur significant closure overhead

### Potential future optimizations: Better Inlining

Consider the following example:

```fsharp
let inline f k = (fun x -> k (x + 1))
let inline g k = (fun x -> k (x + 2))

let res = (f << g) id 1 // 4
```

Intermediate values that inherit from `FSharpFunc` are allocated at the call set of `res` to support function composition, even if the functions are marked as `inline`. Currently, if this overhead needs removal, you need to rewrite the code to be more like this:

```fsharp
let f x = x + 1
let g x = x + 2

let res = id 1 |> g |> f // 4
```

The downside of course being that the `id` function can't propagate to composed functions, meaning the code is now different despite yielding the same result.

More generally, any time a first-order function is passed as an argument to a second-order function, the first-order function is not inlined even if everything is marked as `inline`. This results in a performance penalty.

## Optimization phases

[`OptimizeInputs.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Driver/OptimizeInputs.fs) (`ApplyAllOptimizations`) runs these phases on each implementation file, in order:

1. `firstLoop`: [`Optimizer.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/Optimizer.fs). Only this pass produces the cross-module optimization data saved for other assemblies.
2. `lowerRuntimeAsync`: [`LowerRuntimeAsync.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/LowerRuntimeAsync.fs), see [Runtime-async optimizations](#runtime-async-optimizations).
3. `lowerLocalMutables`: [`LowerLocalMutables.fs`](https://github.com/dotnet/fsharp/blob/main/src/Compiler/Optimize/LowerLocalMutables.fs) turns mutable locals captured by closures into reference cells.
4. `ExtraLoop`: `Optimizer.fs` again, only with `--extraoptimizationloops:n` for n > 0.
5. `Detuple`: `DetupleArgs.fs`, only when optimizing.
6. `InnerLambdasToToplevelFuncs`: `InnerLambdasToTopLevelFuncs.fs`, only when optimizing.
7. `LowerCalls`: `LowerCalls.fs`.
8. `FinalSimplify`: `Optimizer.fs` again, only when optimizing.

Sequence expressions, resumable state machines, list and array comprehensions, and runtime-async sequences are lowered later, during code generation in `IlxGen.fs`.

## Runtime-async optimizations

[Runtime async](runtime-async.md) adds rewrites that keep suspensions inside the runtime-async method they belong to. They run only in a runtime-async context or in the `lowerRuntimeAsync` phase, never on ordinary code; a new rewrite should keep that boundary. [`runtime-async.md`](runtime-async.md#custom-optimizations) describes each rewrite in detail.

### Terms

* **Return marker**: `__runtimeAsyncReturn`, `__runtimeAsyncReturnValueTask`, or their `Unit` forms.
* **Body**: the argument of a return marker. It is consumed in place: at the top of a method or closure it becomes that method's IL body; anywhere else, code generation wraps it in a closure that is invoked at once.
* **Sequence marker**: `__runtimeAsyncSequence`.
* **Recipe**: the `unit -> seq<'T>` argument of a sequence marker. It is a thunk rather than a `seq<'T>` so that it stays a known lambda value through inlining. `LowerAsyncSeq.fs` applies it at compile time and lowers the sequence into a runtime-async `MoveNextAsync` method; code before its sequence expression runs on each enumeration.
* **Marker**: a return or sequence marker.
* **Suspension**: a call to `AsyncHelpers.Await`, `AwaitAwaiter`, or `UnsafeAwaitAwaiter`.
* **Runtime-async code**: an expression containing a marker or a suspension, directly or through the inline and local values it references (`RuntimeAsyncAnalyzer.ContainsRuntimeAsyncCode`). Inline values containing it are inlined even with `--optimize-`.
* **Runtime-async context**: code that ends up in a body or recipe, including code inlined there; object-expression methods inside it are excluded. It is `runtimeAsyncContext` in `Optimizer.fs` and `inContext` in `LowerRuntimeAsync.fs`.
* **Callback**: a compiler-owned, single-argument `InlineIfLambda` function or delegate, typically a builder continuation. A suspending callback contains a suspension.
* **Construction**: the expression bound to a callback. Its **construction prefix** is the bindings, effects, and debug wrappers in front of the lambda (`RuntimeAsyncConstructionPrefix`).
* **Body preparation**: the last step of `lowerRuntimeAsync` for each body. It reports byref, byref-like, and pinned locals used after a suspension (FS3919) and rewrites exception handlers that contain suspensions.
* **`TOp.DebugLocalScope`**: a backend-only node that shows existing storage under a source name for a lexical scope. It keeps the names of captures hoisted out of callbacks visible in the debugger.

### Rewrites

* **Await-input local elimination** (`Optimizer.fs`): folds a single-use local into the argument of a suspension.
* **Callback-capture floating** (`Optimizer.fs`): moves the bindings and effects in front of a callback's lambda out of its binding, so the callback is a plain lambda that ordinary inlining handles.
* **Returned-closure reduction** (`RuntimeAsyncAnalysis.fs`): after a forced inlining, pushes an application through bindings (recursive ones included), sequencing, and branches to beta-reduce a closure returned by a builder combinator.
* **Single-use delegate inlining** (`LowerRuntimeAsync.fs`): substitutes a suspending delegate's body at its only invocation.
* **Callback defunctionalization** (`LowerRuntimeAsync.fs`): replaces a suspending callback, typically one chosen by a branch, with a branch tag and hoisted captures; each invocation dispatches to a copy of the chosen body. Copies are limited to 2000 expression nodes per callback across all invocations.

The optimizer also changes some ordinary behavior for runtime-async code:

* Values containing runtime-async code are inlined even with `--optimize-`, and the result is reoptimized with inlining forced.
* A large expression that contains a suspension is never split into a separate method, because that method would not be runtime-async.
* A method whose body is a return marker is never exported as an inline definition.
* A debug pipe chain in a runtime-async context is reoptimized once its synthetic input bindings are in scope.

A suspension that none of these can move into a runtime-async method is reported as FS3918.


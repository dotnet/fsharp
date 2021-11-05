---
title: Optimizations
category: Compiler Internals
categoryindex: 200
index: 400
---
# Code Optimizations

Code optimizations are performed in [`Optimizer.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/Optimizer.fs), [`DetupleArgs.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/DetupleArgs.fs), [`InnerLambdasToTopLevelFuncs.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/InnerLambdasToTopLevelFuncs.fs) and [`LowerCallsAndSeqs.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/LowerCallsAndSeqs.fs).

Some of the optimizations performed in [`Optimizer.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/Optimizer.fs) are:

* Propagation of known values (constants, x = y, lambdas, tuples/records/union-cases of known values)
* Inlining of known lambda values
* Eliminating unused bindings
* Eliminating sequential code when there is no side-effect
* Eliminating switches when we determine definite success or failure of pattern matching
* Eliminating getting fields from an immutable record/tuple/union-case of known value
* Expansion of tuple bindings "let v = (x1,...x3)" to avoid allocations if it's not used as a first class value
* Splitting large functions into multiple methods, especially at match cases, to avoid massive methods that take a long time to JIT
* Removing tailcalls when it is determined that no code in the transitive closure does a tailcall nor recurses

In [`DetupleArgs.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/DetupleArgs.fs), tuples at call sites are eliminated if possible. Concretely, functions that accept a tuple at all call sites are replaced by functions that accept each of the tuple's arguments individually. This may require inlining to activate.

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

In [`InnerLambdasToTopLevelFuncs.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/InnerLambdasToTopLevelFuncs.fs), inner functions and lambdas are analyzed and, if possible, rewritten into separate methods that do not require an `FSharpFunc` allocation.

Consider the following implementation of `sumBy` on an F# list:

```fsharp
let sumBy f xs =
    let rec loop xs acc =
        match xs with
        | [] -> acc
        | x :: t -> loop t (f x + acc)
    loop xs 0
```

The inner `loop` function is emitted as a separate static method named `loop@2` and incurs no overhead involved with allocatin an `FSharpFunc` at runtime.

In [`LowerCallsAndSeqs.fs`](https://github.com/dotnet/fsharp/blob/main/src/fsharp/LowerCallsAndSeqs.fs), a few optimizations are performed:

* Performs eta-expansion on under-applied values applied to lambda expressions and does a beta-reduction to bind any known arguments
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


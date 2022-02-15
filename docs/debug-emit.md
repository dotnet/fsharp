---
title: Debug emit
category: Compiler Internals
categoryindex: 200
index: 350
---
# Debug emit

The F# compiler code base emits debug information and attributes. This article documents what we do, how it is implemented and the problem areas in our implementation.

There are mistakes and missing pieces to our debug information. Small improvements can make a major difference. Please help us fix mistakes and get things right.

The file `tests\walkthroughs\DebugStepping\TheBigFileOfDebugStepping.fsx` is crucial for testing the stepping experience for a range of constructs.

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

We almost always now emit the [Portable PDB](https://github.com/dotnet/runtime/blob/main/docs/design/specs/PortablePdb-Metadata.md) format.

## Design-time services

IDE tooling performs queries into the F# language service, notably:

* `ValidateBreakpointLocation` [(permalink)](https://github.com/dotnet/fsharp/blob/24979b692fc88dc75e2467e30b75667058fd9504/src/fsharp/service/FSharpParseFileResults.fs#L795) is called to validate every breakpoint before debugging is launched. This operates on syntax trees. See notes below.

## Debugging and optimization

Nearly all optimizations are **off** when debug code is being generated.

* The optimizer is run for forced inlining only
* List and array expressions do generate collector code
* State machines are generated for tasks and sequences
* "let mutable" --> "ref" promotion happens for captured local mutables
* Tailcalls are off by default and not emitted in IlxGen.

Otherwise, what comes out of the type checker is pretty much what goes into IlxGen.fs.

## Debug points

### Terminology

We use the terms "sequence point" and "debug point" interchangeably. The word "sequence" has too many meanings in the F# compiler so in the actual code you'll see "DebugPoint" more often, though for abbreviations you may see `spFoo` or `mFoo`.

### How breakpoints work (high level)

Breakpoints have two existences which must give matching behavior:

* At design-time, before debugging is launched, `ValidateBreakpointLocation` is called to validate every breakpoint.  This operators on the SyntaxTree and forms a kind of "gold-standard" about the exact places where break points are valid.

* At run-time, breakpoints are "mapped" by the .NET runtime to actual sequence points found in the PDB data for .NET methods. The runtime searches all methods with debug points for the relevant document and determines where to "bind" the actual breakpoint to.  A typical debugger can bind a breakpoint to multiple locations.

This means there is an invariant that `ValidateBreakpointLocation` and the emitted IL debug points correspond.

> NOTE: The IL code can and does contain extra debug points that don't pass ValidateBreakpointLocation. It won't be possible to set a breakpoint for these, but they will appear in stepping.

### Intended debug points based on syntax

The intended debug points for constructs are determined by syntax as follows.  Processing depends on whether a construct is being processed as "control-flow" or not. This means at least one debug point will be placed, either over the whole expression or some of its parts.

* The bodies of functions, methods, lambdas and initialization code for top-level-bindings are all processed as control flow

* Each CAPITAL-EXPR below is processed as control-flow (the bodies of loops, conditionals etc.)

* Leaf expressions are the other composite expressions like applications that are not covered by the other constructs.

* The sub-expressions of leaf expressions are not processed as control-flow.

|  Construct   | Debug points     |
|:-----------|:----------------|
| `let x = leaf-expr in BODY-EXPR`  |  Debug point over `let x = leaf-expr`.  |
| `let x = NON-LEAF-EXPR in BODY-EXPR`  |   |
| `let f x = BODY-EXPR in BODY-EXPR`    |    |
| `let rec f x = BODY-EXPR and g x = BODY-EXPR in BODY-EXPR`   |  |
| `if guard-expr then THEN-EXPR`   | Debug point over `if guard-expr then` |
| `if guard-expr then THEN-EXPR else ELSE-EXPR`    | Debug point over `if .. then` |
| `match .. with ...`   | Debug point over `match .. with`  |
| `... -> TARGET-EXPR`   |   |
| `... when WHEN-EXPR -> TARGET-EXPR`   |   |
| `while .. do BODY-EXPR`   | Debug point over `while .. do` |
| `for .. in collection-expr do BODY-EXPR`  | Debug points over `for`, `in` and `collection-expr`   |
| `try TRY-EXPR with .. -> HANDLER-EXPR`  | Debug points over `try` and `with` |
| `try TRY-EXPR finally .. -> FINALLY-EXPR`   | Debug points `try` and `finally` |
| `use x = leaf-expr in BODY-EXPR`  |  Debug point over `use x = leaf-expr`.  |
| `use x = NON-LEAF-EXPR in BODY-EXPR`  |   |
| `EXPR; EXPR` | |
| `(fun .. -> BODY-EXPR)` | Not a leaf, do not produce a debug point on outer expression, but include them on BODY-EXPR |
| `{ new C(args) with member ... = BODY-EXPR }` | |
| Pipe `EXPR1 &amp;&amp; EXPR2` | |
| Pipe `EXPR1 &#124;&#124; EXPR2` | |
| Pipe `EXPR1 &#124;> EXPR2` |  |
| Pipe `(EXPR1, EXPR2) &#124;&#124;> EXPR3` |  |
| Pipe `(EXPR1, EXPR2, EXPR3) &#124;&#124;&#124;> EXPR4` | |
| `yield leaf-expr` | Debug point over 'yield expr' |
| `yield! leaf-expr` | Debug point over 'yield! expr' |
| `return leaf-expr` | Debug point over 'return expr' |
| `return! leaf-expr` | Debug point over 'return! expr' |
| `[ BODY ]` | See notes below. If a computed list expression with yields (explicit or implicit) then process as control-flow. Otherwise treat as leaf |
| `[| BODY |]` | See notes below. If a computed list expression with yields (explicit or implicit) then process as control-flow. Otherwise treat as leaf |
| `seq { BODY }` | See notes below |
| `builder { BODY }` | See notes below |
| `f expr`, `new C(args)`, constants or other leaf | Debug point when being processed as control-flow. The sub-expressions are processed as non-control-flow. |

#### Intended debug points for let-bindings

Simple `let` bindings get debug points that extend over the `let` (if the thing is not a function and the implementation is a leaf expression):

```fsharp
let f () =
    let x = 1 // debug point for whole of `let x = 1`
    let f x = 1 // no debug point on `let f x =`, debug point on `1`
    let x = if today then 1 else tomorrow // no debug point on `let x =`, debug point on `if today then` and `1` and `tomorrow`
    let x = let y = 1 in y + y // no debug point on `let x =`, debug point on `let y = 1` and `y + y`
    ...
```

#### Intended debug points for nested control-flow

Debug points are not generally emitted for constituent parts of non-leaf constructs, in particular function applications, e.g. consider:

```fsharp
let h1 x = g (f x)
let h2 x = x |> f |> g
```

Here `g (f x)` gets one debug point covering the whole expression. The corresponding pipelining gets three debug points.

If however a nested expression is control-flow, then debug points start being emitted again e.g.

```fsharp
let h3 x = f (if today then 1 else 2)
```

Here debug points are at `if today then` and `1` and `2` and all of `f (if today then 1 else 2)`

> NOTE: these debug points are overlapping. That's life.

### Intended debug points for `[...]`, `[| ... |]` code

The intended debug points for computed list and array expressions are the same as for the expressions inside the constructs. For example

```fsharp
let x = [ for i in 1 .. 10 do yield 1 ]
```

This will have debug points on `for i in 1 .. 10 do` and `yield 1`.

### Intended debug points for `seq { .. }` and `task { .. }` code

The intended debug points for tasks is the same as for the expressions inside the constructs. For example

```fsharp
let f() = task { for i in 1 .. 10 do printfn "hello" }
```

This will have debug points on `for i in 1 .. 10 do` and `printfn "hello"`.

> NOTE: there are glitches, see further below

### Intended debug points for other computation expressions

Other computation expressions such as `async { .. }` or `builder { ... }` get debug points as follows:

* A debug point for `builder` prior to the evaluation of the expression

* In the de-sugaring of the computation expression, each point a lambda is created implicitly, then the body of that
  lambda as specified by the F# language spec is treated as control-flow and debug points added per the earlier spec.

* For every `builder.Bind`, `builder.BindReturn` and similar call that corresponds to a `let` where there would be a debug point, a debug point is added immediately prior to the call.

* For every `builder.For` call, a debug point covering the `for` keyword is added immediately prior to the call.  No debug point is added for the `builder.For` call itself even if used in statement position.

* For every `builder.While` call, a debug point covering the `while` keyword plus guard expression is added immediately prior to the execution of the guard within the guard lambda expression. No debug point is added for the `builder.While` call itself even if used in statement position.

* For every `builder.TryFinally` call, a debug point covering the `try` keyword is added immediately within the body lambda expression. A debug point covering the `finally` keyword is added immediately within the finally lambda expression. No debug point is added for the `builder.TryFinally` call itself even if used in statement position.

* For every `builder.Yield`, `builder.Return`, `builder.YieldFrom` or `builder.ReturnFrom` call, debug points are placed on the expression as if it were control flow. For example `yield 1` will place a debug point on `1`and `yield! printfn "hello"; [2]` will place two debug points.

* No debug point is added for the `builder.Run`, `builder.Run` or `builder.Delay` calls at the entrance to the computation expression, nor the `builder.Delay` calls implied by `try/with` or `try/finally` or sequential `Combine` calls.

The computations are often "cold-start" anyway, leading to a two-phase debug problem.

The "step-into" and "step-over" behaviour for computation expressions is often buggy because it is performed with respect to the de-sugaring and inlining rather than the original source.
For example, a "step over" on a "while" with a non-inlined `builder.While` will step over the whole call, when the used expects it to step the loop.
One approach is to inline the `builder.While` method, and apply `[<InlineIfLambda>]` to the body function. This however has only limited success
as at some points inlining fails to fully flatten. Builders implemented with resumable code tend to be much better in this regards as
more complete inlining and code-flattening is applied.

### Intended debug points for implicit constructors

* The `let` and `do` bindings of an implicit constructor generally gets debug points as if it were a function.
* `inherits SubClass(expr)` gets a debug point. If there is no inherits, an initial debug point is placed over the text of the arguments.

e.g.
```fsharp
type C(args) =        
    let x = 1+1         // debug point over `let x = 1+1` as the only side effect
    let f x = x + 1
    member _.P = x + f 4

type C(args) =        
    do printfn "hello"         // debug point over `printfn "hello"` as side effect
    static do printfn "hello"         // debug point over `printfn "hello"` as side effect for static init
    let f x = x + 1
    member _.P = x + f 4

type C(args) =        // debug point over `(args)` since there's no other place to stop on object construction
    let f x = x + 1
    member _.P = 4
```

## Internal implementation of debug points in the compiler

Most (but not all) debug points are noted by the parser by adding `DebugPointAtTry`, `DebugPointAtWith`, `DebugPointAtFinally`, `DebugPointAtFor`, `DebugPointAtWhile`, `DebugPointAtBinding` or `DebugPointAtLeaf`.

These are then used by `ValidateBreakpointLocation`. These same values are also propagated unchanged all the way through to `IlxGen.fs` for actual code generation, and used for IL emit, e.g. a simple case like this:

```fsharp
    match spTry with
    | DebugPointAtTry.Yes m -> CG.EmitDebugPoint cgbuf m ... 
    | DebugPointAtTry.No -> ...
    ...
```

For many constructs this is adequate. However, in practice the situation is far more complicated.

### Internals: Debug points for `[...]`, `[| ... |]`

The internal implementation of debug points for list and array expressions is conceptually simple but a little complex.

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

### Internals: debug points for `seq { .. .}` code

Debug points for `seq { .. }` compiling to state machines poses similar problems.

* The de-sugaring is as for list and array expressions
* The debug points are recovered in the state machine generation, for example [here (permalink)](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/LowerCallsAndSeqs.fs#L367)

### Internals: debug points for `task { .. .}` code

Debug points for `task { .. }` poses much harder problems. We use "while" loops as an example:

* The de-sugaring is for computation expressions, and in CheckComputationExpressions.fs places a debug point for `while` directly before the evaluation of the guard
* The code is then checked and optimized, and all the resumable code is inlined, and this debug point is preserved throughout this process.

### Internals: debug points for other computation expressions

As mentioned above, other computation expressions such as `async { .. }` have significant problems with their debug points.

The main problem is stepping: even after inlining the code for computation expressions is rarely "flattened" enough, so, for example, a "step-into" is required to get into the second part of an `expr1; expr2` construct (i.e. an `async.Combine(..., async.Delay(fun () -> ...)))`) where the user expects to press "step-over".  

Breakpoints tend to be less problematic.

> NOTE: A systematic solution for quality debugging of computation expressions code is still elusive, and especially for `async { ... }`.  Extensive use of inlining and `InlineIfLambda` can succeed in flattening most simple computation expression code. This is however not yet fully applied to `async` programming.

> NOTE: The use of library code to implement "async" and similar computation expressions also interacts badly with "Just My Code" debugging, see https://github.com/dotnet/fsharp/issues/5539 for example.

> NOTE: As mentioned, the use of many functions to implement "async" and friends implements badly with "Step Into" and "Step Over" and related attributes, see for example https://github.com/dotnet/fsharp/issues/3359

### FeeFee and F00F00 debug points (Hidden and JustMyCodeWithNoSource)

Some fragments of code use constructs generate calls and other IL code that should not have debug points and not participate in "Step Into", for example. These are generated in IlxGen as "FeeFee" debug points. See the [the Portable PDB spec linked here](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.sequencepoint.hiddenline?view=net-5.0).

> TODO: There is also the future prospect of generating `JustMyCodeWithNoSource` (0xF00F00) debug points but these are not yet emitted by F#.  We should check what this is and when the C# compiler emits these.

> NOTE: We always make space for a debug point at the head of each method by [emitting a FeeFee debug sequence point](https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fs#L1953). This may be immediately replaced by a "real" debug point [here](https://github.com/dotnet/fsharp/blob/main/src/fsharp/IlxGen.fs#L2019).

## Generated code

The F# compiler generates entire IL classes and methods for constructs such as records, closures, state machines and so on. Each time code is generated we must carefully consider what attributes and debug points are generated.

### Generated "augment" methods for records, unions and structs

Generated methods for equality, hash and comparison on records, unions and structs do not get debug points at all.

> NOTE: Methods without debug points (or with only 0xFEEFEE debug points) are shown as "no code available" in Visual Studio - or in Just My Code they are hidden altogether - and are removed from profiling traces (in profiling, their costs are added to the cost of the calling method).

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing, however the absence of debug points should be sufficient to exclude these.

### Generated "New*", "Is*", "Tag" etc. for unions

Discriminated unions generate `NewXYZ`, `IsXYZ`, `Tag` etc. members. These do not get debug points at all.

These methods also get `CompilerGeneratedAttribute`, and `DebuggerNonUserCodeAttribute`, e.g. [here (permalink)](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseUnions.fs#L635)

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing, however the absence of debug points should be sufficient to exclude these.

> TODO: the `NewABC` methods are missing `CompilerGeneratedAttribute`, and `DebuggerNonUserCodeAttribute`. However the absence of debug points should be sufficient to exclude these from code coverage and profiling.

### Generated closures for lambdas

The debug codegen involved in closures is as follows:

|  Source         | Construct         | Debug Points | Attributes   |
|:----------------|:------------------|:-------------|:-------------|
| (fun x -> ...)  | Closure class     |              |              |
|                 | `.ctor` method    | [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseClosures.fs#L584)     | CompilerGenerated, DebuggerNonUserCode |
|                 | `Invoke` method   | from body of closure     |                                        |
| generic local defn  | Closure class |      |                                        |
|                 | `.ctor` method    | [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseClosures.fs#L486)     | CompilerGenerated, DebuggerNonUserCode |
|                 | `Specialize` method |  from body of closure  |                                        |
|  Intermediate closure classes   |  For long curried closures `fun a b c d e f -> ...`.  | See [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseClosures.fs#L459) and [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/ilx/EraseClosures.fs#L543).  | CompilerGenerated, DebuggerNonUserCode     |

Generated intermediate closure methods do not get debug points, and are labelled CompilerGenerated and DebuggerNonUserCode.

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing

### Generated state machines for `seq { .. }`

Sequence expressions generate class implementations which resemble closures.

The debug points recovered for the generated state machine code for `seq { ... }` is covered up above. The other codegen is as follows:

|  Source         | Construct         | Debug Points | Attributes   |
|:----------------|:------------------|:-------------|:-------------|
| seq { ... }     | State machine class |            |  "Closure"             |
|                 | `.ctor` method    |   none      |  [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5150) |
|                 | `GetFreshEnumerator`   |  [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5108)  | CompilerGenerated, DebuggerNonUserCode |
|                 | `LastGenerated`   |  [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5146-L5148)  | CompilerGenerated, DebuggerNonUserCode |
|                 | `Close`   |  [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5124-L5127)  | none |
|                 | `get_CheckClose`  |  [none](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5130-L5133)  | none |
|                 | `GenerateNext`    |  from desugaring, and [here](https://github.com/dotnet/fsharp/blob/db2c9da8d1e76d11217d6da53a64253fd0df0246/src/fsharp/IlxGen.fs#L5136-L5143) | none |

> NOTE: it appears from the code that extraneous debug points are not being generated, which is good, though should be checked

> TODO: we should likely be generating `CompilerGeneratedAttribute` and `DebuggerNonUserCodeAttribute` attributes for the `Close` and `get_CheckClose` and `.ctor` methods

> TODO: we should also consider emitting `ExcludeFromCodeCoverageAttribute`, being assessed at time of writing

### Generated state machines for `task { .. }`

[Resumable state machines](https://github.com/fsharp/fslang-design/blob/main/FSharp-6.0/FS-1087-resumable-code.md) used for `task { .. }` also generate struct implementations which resemble closures.

The debug points recovered for the generated state machine code for `seq { ... }` is covered up above. The other codegen is as follows:

|  Source         | Construct         | Debug Points | Attributes   | Notes |
|:----------------|:------------------|:-------------|:-------------|:------|
| task { ... }     | State machine struct |          |  "Closure"   |       |
|                 | `.ctor` method    |   none      |  none       | |
|                 | TBD               |              |              | |

> TODO: we should be generating attributes for some of these

> TODO: we should assess that only the "MoveNext" method gets any debug points at all

> TODO: Currently stepping into a task-returning method needs a second `step-into` to get into the MoveNext method of the state machine.  We should emit the `StateMachineMethod` and `StateMachineHoistedLocalScopes` tables into the PDB to get better debugging into `task` methods. See https://github.com/dotnet/fsharp/issues/12000.

### Generated code for delegate constructions `Func<int,int,int>(fun x y -> x + y)`

A closure class is generated.  Consider the code

```fsharp
open System
let d = Func<int,int,int>(fun x y -> x + y)
```

There is one debug point over all of `Func<int,int,int>(fun x y -> x + y)` and one over `x+y`.

### Generated code for constant-sized array and list expressions

These are not generally problematic for debug.

### Generated code for large constant arrays

These are not generally problematic for debug.

### Generated code for pattern matching

The implementation is a little gnarly and complicated and has historically had glitches.

### Generated code for conditionals and boolean logic

Generally straight-forward. See for example [this proposed feature improvement](https://github.com/dotnet/fsharp/issues/11980)

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

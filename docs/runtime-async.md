---
title: Runtime async
category: Compiler Internals
categoryindex: 200
index: 375
---

# Runtime async

This document describes the current proof-of-concept implementation of F#
support for the .NET runtime-async feature. It describes the code as
implemented, not an aspirational design. The .NET design is still evolving:

* [Runtime-async specification](https://github.com/dotnet/runtime/blob/main/docs/design/specs/runtime-async.md)
* [Runtime-async code-generation contract](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/botr/runtime-async-codegen.md)
* [Roslyn runtime async design](https://github.com/dotnet/roslyn/blob/main/docs/compilers/CSharp/Runtime%20Async%20Design.md) —
  how C# lowers `await` (including the exception-handling hoisting described below)

The implementation targets functions, lambdas, and members returning
`System.Threading.Tasks.Task<'T>`. A computation-expression builder exists in
the component tests and works for a subset of the surface, but is not part of
FSharp.Core.

## Runtime contract

Runtime-async methods are CIL methods marked with
`MethodImplOptions.Async` (`0x2000`). The runtime, rather than a compiler
generated state machine and method builder, owns suspension and resumption.

Only the generic return shape `System.Threading.Tasks.Task<'T>` is supported.
Non-generic `Task` and `ValueTask`/`ValueTask<'T>` returns are not.

Suspension is explicit, via `System.Runtime.CompilerServices.AsyncHelpers`:

* `Await` for `Task`, `ValueTask`, and configured awaitables
* `AwaitAwaiter` for awaiters (used by the test builder's SRTP `Bind`)

The compiler emits the adjacent IL sequence the runtime specification expects:

```il
call Task<int32> SomeAsyncMethod(...)
call int32 AsyncHelpers::Await<int32>(Task<int32>)
```

Known runtime restrictions (currently **not** diagnosed by the F# compiler):

* `tail.` and `localloc` are forbidden.
* suspension cannot occur inside exception-handling regions. Awaiting in a
  `try` body now works on the current runtime; awaiting inside a `finally`
  handler compiles and then terminates the process at execution
  (`0xC0000409`). See `RuntimeTasksAsyncDisposalException.fs`, which is
  compile-only for this reason.

  C# avoids this by rewriting EH-region awaits at lowering time (see the
  Roslyn design doc): `try B finally { await x }` becomes
  `try B catch-all { pend e }`, then `await x` outside the region, then
  rethrow the pending exception. The test `RuntimeTaskBuilder.Using`
  prototypes this pattern in F# source: it captures the body result/exception
  in a `Choice`, runs `DisposeAsync` (possibly suspending) *outside* the
  `try`, then restores a pending exception. This makes `use` on an
  `IAsyncDisposable` work under runtime async (`testUsingAsyncDisposableSync`
  executes).
* Byref, byref-like, and pinned locals cannot be preserved across suspension.

## F# surface

The source-level marker is the compiler intrinsic
`Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__runtimeAsync`,
declared in `resumable.fsi` alongside the other compiler intrinsics:

```fsharp
val __runtimeAsync<'T> : 'T -> System.Threading.Tasks.Task<'T>
```

Its FSharp.Core implementation throws; the compiler consumes every
occurrence before code generation, so the body is never executed. It is
marked `NoInlining` so a missed consumption does not silently fold into a
caller.

The feature is gated on `langversion:preview`
(`LanguageFeature.RuntimeAsync`) and on the target reference assemblies
exposing `MethodImplOptions.Async` (see "Runtime capability check" below).
Without the language version the checker reports error 3350; without runtime
support it reports 3351.

Typical forms:

```fsharp
let add (x: int) (y: int) : Task<int> =
    __runtimeAsync (
        let first = AsyncHelpers.Await (getTask x)
        first + y)

type C() =
    member _.Add(x: int, y: int) : Task<int> =
        __runtimeAsync (
            AsyncHelpers.Await (getTask x) + y)

// Let-bound value (not a function): also supported.
let answer : Task<int> = __runtimeAsync 42
```

There is no implicit awaiting: the argument of `__runtimeAsync` is checked
as the logical `'T` result, and flattening requires an explicit
`AsyncHelpers.Await`.

## Type checking

`__runtimeAsync` is an ordinary generic value in the typed tree; no new
expression node or `Val` flag is added. Type checking special-cases its
application in two places in `CheckExpressions.fs`:

* `Propagate` skips function-type propagation for the intrinsic so the
  argument is not checked against a function domain.
* `TcApplicationThen` (`tryTcRuntimeAsyncApplication`) recognises the
  intrinsic (possibly type-applied), gates the language feature and runtime
  capability, extracts the result type `'T` from the intrinsic's own
  instantiated signature `'T -> Task<'T>`, and checks the argument against
  `'T` with `TcExprFlex2`. The result type of the application is `Task<'T>`,
  which unifies with the declared return type of the enclosing binding in
  the usual way. A non-`Task<'T>` declared return type therefore fails with
  the ordinary FS0001 type-mismatch error.

User code that defines its own `__runtimeAsync` is unaffected: the intrinsic
is only recognised when the `ValRef` resolves (via `valRefEq`) to the
FSharp.Core declaration.

## Optimization

`Optimizer.fs` preserves the marker application as-is, optimizing only its
argument. The marked expression is forced to `HasEffect = true` and
`UnknownValue`, so the optimizer never inlines, duplicates, or discards it.
The marker therefore survives optimization as an ordinary `Expr.App` node;
nothing else in the typed tree records that a method is runtime-async.

## Code generation

`IlxGen.fs` recognises the marker in three placements
(`TryUnwrapRuntimeAsyncExpr`, which strips `DebugPoint` wrappers):

1. **Method body** (`GenMethodForBinding`): the marker is unwrapped from the
   top of the method lambda body; the generated `ILMethodDef` gets
   `.WithAsync(true)`, which sets impl attribute bit `0x2000`
   (`MethodImplOptions.Async`, written as a literal because older reference
   assemblies do not define the enum member). `NoInlining` is forced on the
   method.
2. **Closure body** (`GenClosureAsLocalTypeFunction` and
   `GenClosureAsFirstClassFunction`): the same unwrapping marks the closure
   `Invoke` method's IL body (`ILMethodBody.IsRuntimeAsync`).
   `EraseClosures.convIlxClosureDef` copies that flag onto the emitted
   method, again with `NoInlining`.
3. **Any other expression position** (`GenRuntimeAsyncAsStartedTask`), e.g.
   a `let`-bound value initializer: the marker application is wrapped in a
   fresh `fun () -> ...` lambda that is immediately applied to `unit` and
   regenerated. The lambda flows through the closure path (2), producing a
   generated runtime-async helper method whose call starts the task. This
   relies on `GenApp` never beta-reducing a lambda application (it always
   emits a closure plus an indirect call); see the comment at
   `GenRuntimeAsyncAsStartedTask`.

A marker that ends up wrapped in anything other than `DebugPoint` at the top
of a method or closure body is not detected there, but still reaches the
catch-all case (3), so compilation stays correct — the cost is an extra
nested runtime-async helper method rather than marking the enclosing method
directly.

## Runtime capability check

`InfoReader` gates `LanguageFeature.RuntimeAsync` on the target reference
assemblies: it looks up the `Async` field on
`System.Runtime.CompilerServices.MethodImplOptions`. This is a metadata-only
probe of the *reference* assemblies; it does not prove the *executing* host
JIT supports runtime-async. Compiling against new reference assemblies and
running on an older runtime is not a supported configuration.

## Test infrastructure

Tests live in `tests/FSharp.Compiler.ComponentTests/Language/RuntimeAsync*`:

* The component test project sets `<Features>runtime-async=on</Features>`
  (the .NET runtime opt-in), as does the project template in
  `FSharp.Test.Utilities` used by `compileExeAndRun`.
* Type-check tests assert the preview gate (3350) and the unsupported-runtime
  gate (3351, on non-.NET-Core targets).
* IL tests verify direct `AsyncHelpers.Await` calls appear without
  intervening delegates.
* Execution tests (`RuntimeAsyncBasic.fs`, `RuntimeTasks.fs` with the shared
  `RuntimeTaskBuilder.fs`) run with `compileExeAndRun`, so they compile with
  the compiler under test and execute on the host runtime.
* `RuntimeTasksAsyncDisposalException.fs` documents the known
  EH-region-suspension crash: it is compiled but not executed.

### Test builder

`RuntimeTaskBuilder.fs` is a quasi-synchronous builder aiming for feature
parity with FSharp.Core's `task` builder: `Delay` is the identity on
`unit -> 'T`, so all combinators are plain inline functions over delayed
code; only `Run` introduces `__runtimeAsync` and returns `Task<'T>`.
`Bind` lowers directly to `AsyncHelpers.Await` with SRTP fallbacks
(`AwaitAwaiter`) for arbitrary task-likes, as do `ReturnFrom` and
`MergeSources`. `MergeSources` awaits its sources sequentially, matching the
task builder — concurrency comes from the sources being hot tasks.
`Async<'T>` binds via `Async.StartImmediateAsTask`, matching `task {}`'s
current-thread semantics.

`RuntimeTasks.fs` ports the TaskBuilder test suite
(`tests/FSharp.Core.UnitTests/.../Tasks.fs`) test-for-test with
`task {` replaced by `runtimeTask {`. Tests that hit the known runtime-async
restrictions or divergences are kept in the file with `knownFailing_` /
`knownDivergent_` prefixes, compiled but not run:

* suspension in `try/finally`, or in `try/with` in non-tail position
  (forbidden by the runtime contract; crashes with `0xC0000409` or loses the
  finally);
* `use`/`use!` whose disposal awaits an `IAsyncDisposable` (the `Using`
  compensation suspends in a `finally`);
* tests requiring synchronous (hot) start of the body before the first
  suspension — on the current runtime build the body is not observably run
  before the returned `Task` is awaited;
* `SynchronizationContext` capture: with a sync context installed, the task
  completes without the body observably running.

Two `task {}` inference behaviors are not matched by the overload set:
element-type propagation through `Bind` without an annotation, and unannotated
`return! failwith ...` (both need explicit annotations in the port).

## Not yet implemented

* Diagnostics for suspension in exception-handling regions, byref/byref-like
  or pinned locals across suspension, `tail.`, and `localloc`.
* Non-generic `Task` and `ValueTask`/`ValueTask<'T>` return shapes.
* Any FSharp.Core builder (the test builder is test-only).
* Compile-time enforcement that the marker was actually consumed before
  code generation (a missed marker throws only when its FSharp.Core stub is
  reached at run time, or produces invalid IL as described above).

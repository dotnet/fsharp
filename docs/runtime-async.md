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
`System.Threading.Tasks.Task<'T>`, `Task`, `ValueTask<'T>`, or `ValueTask`.
Inline computation-expression builders can use the feature, but no such
builder is currently part of FSharp.Core.

## Runtime contract

Runtime-async methods are CIL methods marked with
`MethodImplOptions.Async` (`0x2000`). The runtime, rather than a compiler
generated state machine and method builder, owns suspension and resumption.

The compiler provides a return intrinsic for each of these carrier shapes:
generic and non-generic `Task`, and generic and non-generic `ValueTask`.

Suspension is explicit, via `System.Runtime.CompilerServices.AsyncHelpers`:

* `Await` for `Task`, `ValueTask`, and configured awaitables
* `AwaitAwaiter` and `UnsafeAwaitAwaiter` for awaiters (used by SRTP
  awaitable bindings)

The compiler emits the adjacent IL sequence the runtime specification expects:

```il
call Task<int32> SomeAsyncMethod(...)
call int32 AsyncHelpers::Await<int32>(Task<int32>)
```

Known runtime restrictions (currently **not** diagnosed by the F# compiler):

* `tail.` and `localloc` are forbidden.
* generated suspension points cannot occur inside exception-handling regions.
  Awaiting in a protected `try` body now works on the current runtime. Direct
  intrinsic bodies rewrite suspending `try/with` handlers and filters, and
  `try/finally` compensations, so the suspension runs outside the EH region.

  C# avoids this by rewriting EH-region awaits at lowering time (see the
  Roslyn design doc): `try B finally { await x }` becomes
  `try B catch-all { pend e }`, then `await x` outside the region, then
  rethrow the pending exception. The compiler applies the same transformation
  to a suspending compensation: it captures the body result or exception,
  runs `DisposeAsync` (possibly suspending) *outside* the `try`, then restores
  the pending exception. This makes `use` on an `IAsyncDisposable` work under
  runtime async.
Byref, byref-like, and pinned locals that are used after a suspension are
rejected with diagnostic FS3917.

Calls to `AsyncHelpers` suspension methods emitted outside a runtime-async
method are rejected during code generation. Explicitly `inline` method bodies
are treated as templates and checked at their eventual use site.

## F# surface

The source-level markers are compiler intrinsics on
`Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers`, available from
the `net10.0` FSharp.Core target and declared in `resumable.fsi` alongside the
other compiler intrinsics:

```fsharp
val __runtimeAsyncReturn<'T> : 'T -> System.Threading.Tasks.Task<'T>
val __runtimeAsyncReturnValueTask<'T> : 'T -> System.Threading.Tasks.ValueTask<'T>
val __runtimeAsyncReturnUnit : unit -> System.Threading.Tasks.Task
val __runtimeAsyncReturnValueTaskUnit : unit -> System.Threading.Tasks.ValueTask
```

Their FSharp.Core implementations throw; the compiler consumes every
occurrence before code generation, so those bodies are never executed. They
are marked `NoInlining` so a missed consumption does not silently fold into a
caller.

The feature is gated on `langversion:preview`
(`LanguageFeature.RuntimeAsync`) and on the target reference assemblies
exposing `MethodImplOptions.Async` (see "Runtime capability check" below).
Without the language version the checker reports error 3350; without runtime
support it reports 3351.

Typical forms:

```fsharp
let add (x: int) (y: int) : Task<int> =
    __runtimeAsyncReturn (
        let first = AsyncHelpers.Await (getTask x)
        first + y)

type C() =
    member _.Add(x: int, y: int) : Task<int> =
        __runtimeAsyncReturn (
            AsyncHelpers.Await (getTask x) + y)

// Let-bound value (not a function): also supported.
let answer : Task<int> = __runtimeAsyncReturn 42
```

There is no implicit awaiting: the argument of a generic return marker is
checked as the logical `'T` result, and flattening requires an explicit
`AsyncHelpers.Await`.

## Type checking

The return intrinsics are ordinary values in the typed tree; no new expression
node or `Val` flag is added. Type checking special-cases their applications in
two places in `CheckExpressions.fs`:

* `Propagate` skips function-type propagation for the intrinsic so the
  argument is not checked against a function domain.
* `TcApplicationThen` (`tryTcRuntimeAsyncApplication`) recognises the
  intrinsic (possibly type-applied), gates the language feature and runtime
  capability, extracts the result carrier and argument type from the
  intrinsic's instantiated signature, and checks the argument with
  `TcExprFlex2`. The result carrier then unifies with the declared return
  type of the enclosing binding in the usual way.

User code that defines its own same-named marker is unaffected: the intrinsic is
only recognised when the `ValRef` resolves (via `valRefEq`) to the FSharp.Core
declaration.

## Optimization

`Optimizer.fs` preserves the marker application as-is, optimizing its
argument and rewriting any suspending exception handlers in that argument.
The marked expression is forced to `HasEffect = true` and `UnknownValue`, so
the optimizer never inlines, duplicates, or discards it. The marker therefore
survives optimization as an ordinary `Expr.App` node; nothing else in the
typed tree records that a method is runtime-async.

Inline values whose bodies contain a return marker or an `AsyncHelpers`
suspension are recursively specialized at their call sites, including when
optimization is disabled. The analysis follows inline and local values with a
cycle guard, and `InlineIfLambda` arguments are forced through when the caller
is already in a runtime-async context. The optimizer follows nested inline
calls and does not create a generated helper method for the specialized
suspension fragment, keeping every suspension in the eventual runtime-async
method.

After specialization, lambda arguments are substituted and their applications
are beta-reduced before and after runtime-async reoptimization. This includes
debug-point-wrapped lambdas, compiler-generated `let` wrappers, curried
applications, and multi-argument lambdas.
That step is required for computation-expression shapes where `Bind` returns a
closure containing `Await`, and later `Combine`/`Delay` calls apply that closure.

When runtime-async specialization is forced in a debug build, the builder
combinator is copied with its definition-site debug ranges remarked before
arguments are substituted. User continuation arguments keep their own ranges,
so `let!`, `do!`, `yield`, and other
computation-expression statements remain associated with the source that
authored them without exposing the implementation ranges of `Run`, `Bind`,
`Combine`, or `Yield`.

Dead branches eliminated by optimization do not reach code generation and do
not produce a suspension-outside-runtime-async diagnostic.

Runtime-async boundary recognition is centralized in
`TypedTree/RuntimeAsync.fs`. The `RuntimeAsyncBoundary` type distinguishes a
return marker from a suspension call, and consumers use the shared
recognizers rather than matching typed-tree shapes independently.

The optimizer uses a context-local `RuntimeAsyncAnalyzer`. It memoizes
completed expression results by reference identity and inline-value results by
value stamp, with a visiting set for recursive inline-value graphs. The cache
is not global: optimizer environments can provide different inline bodies, and
optimization creates new expression trees. Context-dependent decisions such as
`runtimeAsyncContext` remain outside the cached facts.

## Code generation

`IlxGen.fs` recognises the return-marker family in three placements through the
shared runtime-async boundary contract, which strips `DebugPoint` wrappers:

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
3. **Any other expression position** (`GenRuntimeAsyncReturnAsStartedTask`), e.g.
   a `let`-bound value initializer: the marker application is wrapped in a
   fresh `fun () -> ...` lambda that is immediately applied to `unit` and
   regenerated. The lambda flows through the closure path (2), producing a
   generated runtime-async helper method whose call starts the task. This
   relies on `GenApp` never beta-reducing a lambda application (it always
   emits a closure plus an indirect call); see the comment at
   `GenRuntimeAsyncReturnAsStartedTask`.

A marker that ends up wrapped in anything other than `DebugPoint` at the top
of a method or closure body is not detected there, but still reaches the
catch-all case (3), so compilation stays correct — the cost is an extra
nested runtime-async helper method rather than marking the enclosing method
directly.

## Debug stepping and call stacks

The compiler emits ordinary Portable PDB sequence points for runtime-async
methods. It does not emit `StateMachineMethod` or async state-machine stepping
records because runtime-async methods have no compiler-generated `MoveNext`
method. Forced inlining therefore preserves user computation-expression
sequence points in the generated runtime-async method while remapping the
inlined builder implementation ranges.

Suspension, continuation mapping, and reconstruction of logical async call
stacks are owned by the runtime and debugger through the `Async` method
implementation flag and the runtime-async debug information contract. Missing
logical frames after a continuation cannot be repaired by inventing F# state
machine metadata; such cases must be validated against the target runtime and
tracked with the runtime/debugger implementation.

Case (3) re-homes the marker argument into a compiler-synthesized closure
during code generation, *after* `LowerLocalMutables` has run. Without special
handling, mutable locals used both in that body and in the enclosing scope
would be copied into the closure by value, silently disconnecting the two
copies. `LowerLocalMutables` therefore treats the marker argument as a lambda
body (`DecideExpr`), promoting its free mutable locals to reference cells so
the synthesized closure and the enclosing scope share them.

`InvokeFast` is not a separate runtime-async path. It is the closure-erasure
shape for an indirect call with multiple arguments. Fragment substitution and
beta reduction happen before closure erasure; if a suspending fragment survives
until an indirect `InvokeFast` call, it is still outside a runtime-async method
and is rejected by code generation.

## Runtime capability check

`InfoReader` gates `LanguageFeature.RuntimeAsync` on the target reference
assemblies: it looks up the `Async` field on
`System.Runtime.CompilerServices.MethodImplOptions`. This is a metadata-only
probe of the *reference* assemblies; it does not prove the *executing* host
JIT supports runtime-async. Compiling against new reference assemblies and
running on an older runtime is not a supported configuration.

## Computation-expression usage

The feature is usable from an inline computation-expression builder. A
task-like builder can keep `Delay` and its other combinators synchronous and
inline; `Run` introduces the return marker:

```fsharp
type RuntimeTaskBuilder() =
    member inline _.Delay([<InlineIfLambda>] generator: unit -> 'T) = generator
    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) =
        __runtimeAsyncReturn (code ())
    member inline _.Bind(task: Task<'T>, [<InlineIfLambda>] continuation: 'T -> 'U) =
        continuation (AsyncHelpers.Await task)
```

`Bind`, `ReturnFrom`, and `MergeSources` can use `Await` for known
`Task`/`ValueTask` types and SRTP awaiter operations for arbitrary task-like
values. `MergeSources` awaits its already-started sources sequentially.
`Async<'T>` can be adapted with `Async.StartImmediateAsTask`.

An async-sequence builder can use the same pattern to produce
`IAsyncEnumerable<'T>`. Its `Run` creates a producer that is started when
`GetAsyncEnumerator` is called. A `ManualResetValueTaskSourceCore` handshake
makes enumeration pull-driven: `yield` publishes one item and waits for the
next `MoveNextAsync` request. `yield!` and `for` can consume synchronous or
asynchronous enumerables, and nested async enumerables receive the caller's
cancellation token. A single active `MoveNextAsync` is enforced. A builder may
also hand off directly between compatible producers for `YieldFromFinal`,
avoiding a second enumeration handshake.

These builders are examples rather than FSharp.Core APIs. Applications can
define their own inline builders over the same intrinsics, subject to the
runtime-async restrictions and inline-fragment rules described above.

### Unsupported inline-fragment positions

An inline fragment that escapes as a first-class value, is passed to a
non-inline function, or is dynamically dispatched cannot be preserved as a
runtime-async suspension fragment. If the suspension remains in the generated
non-runtime-async method, code generation reports FS3916 rather than emitting
an unsafe closure. Fragments in statically eliminated branches do not trigger
this diagnostic.

## Not yet implemented

* Complete diagnostics for runtime restrictions. The optimizer rewrites
  suspending `try/with` handlers and filters, and `try/finally` compensations,
  so they execute outside exception-handling regions. There is no general
  diagnostic for runtime-contract violations in other generated or imported
  shapes, and `localloc` has no dedicated diagnostic. Runtime-async methods
  suppress `tail.` emission rather than reporting it.
* A builder in FSharp.Core; builders using the feature are currently
  application/library code.
* Compile-time enforcement that the marker was actually consumed before
  code generation (a missed marker throws only when its FSharp.Core stub is
  reached at run time, or produces invalid IL as described above).

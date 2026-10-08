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
argument. The marked expression is forced to `HasEffect = true` and
`UnknownValue`, so the optimizer never inlines, duplicates, or discards it.
The marker therefore survives optimization as an ordinary `Expr.App` node;
nothing else in the typed tree records that a method is runtime-async.

Inline values whose bodies contain a return marker or an `AsyncHelpers`
suspension are recursively specialized at their call sites, including when
optimization is disabled. The analysis follows inline and local values with a
cycle guard, and `InlineIfLambda` arguments are forced through when the caller
is already in a runtime-async context. Ordinary inlining is tried first;
applications of returned lambdas are reduced when their remaining arguments
can safely move across the returned closure's construction. This handles
computation-expression shapes where `Bind` returns a closure containing
`Await`, and later `Combine`/`Delay` calls apply it. Inside a runtime-async expansion, captures
that wrap an `InlineIfLambda` lambda or delegate (`let p = (let c = e in fun
...)`) are floated above the binding so the callback can be inlined; they are
still evaluated at the same point.

## Runtime-async lowering

`LowerRuntimeAsync.fs` runs once per file after the first optimization loop,
so it sees the final inlined shape rather than an intermediate optimizer
state, and before `LowerLocalMutables`, so mutable locals it introduces that
closures capture are promoted to reference cells. It has two steps.

First, if a compiler-owned `InlineIfLambda` function or delegate still
contains a suspension, a fully rewritable, single-argument callback is
inlined into the enclosing method. The ordinary optimizer already inlines a
callback whose construction ends in one lambda, including after `let` or
effect prefixes and at repeated invocations. What remains is a construction
that selects a lambda by branching (`if`/`match`). It is defunctionalized:
the construction runs once, in place, and each lambda in tail position is
replaced by an assignment of its branch tag and of the construction locals it
captures to mutable locals of the enclosing method. Each invocation binds its
argument once and dispatches on the tag to a copy of the selected lambda body.
Construction effects happen once, captured state is shared across
invocations, and every `Await` stays in the runtime-async method, so
`ExecutionContext` changes such as `AsyncLocal` writes behave as in source
order. Only the specialized copy changes, not the exported inline definition
or source signature. This applies within a runtime-async body or sequence
recipe, and to a callback bound immediately before a runtime-async body
together with the callbacks its construction captures. Opaque consumers,
escaping callbacks, unsupported callback shapes (including `try`/`with` in
tail position), and unsafe byref or pinned captures are not rewritten;
suspensions remaining in an ordinary method, such as a closure, are diagnosed
with FS3918.

The ordinary optimizer can inline an `Invoke` on a constructed delegate.
For a residual `InlineIfLambda` delegate bound to a local, a single direct
invocation can also be inlined through simple, effect-free conditional
construction. Otherwise, directly rewritable invocations dispatch on the
branch tag. Inner callback bindings are processed before the outer delegate.
Effectful precomputations that capture a delegate's inputs are not moved to
its invocation, so a pending operation is created once and repeated invokes
share captured state. A delegate that escapes or is consumed opaquely cannot
be converted. IlxGen checks each generated delegate `Invoke` as its own
method: an `Await` left there without a return marker produces FS3918, even
when the enclosing method is runtime-async. Exported inline definitions remain
unchanged.

Each invocation copies every branch body, so code size grows with the number
of invocations times the number of branches. When the copies would exceed
2000 expression nodes, the callback is left as a closure and a suspension in
it is reported as FS3918.

Second, every return-marker body is prepared once after callbacks are inlined,
innermost first: locals that cannot be preserved across a suspension are
reported (once per range across the compilation), and suspending exception
handlers are rewritten. Preparing afterwards means an `Await` that callback
or delegate inlining moves into a handler is still rewritten.
`__runtimeAsyncSequence` recipes are not prepared here; `LowerAsyncSeq`
prepares their `MoveNextAsync` body after state-machine conversion, which is
where sequence `try`/`with` is lowered.

When runtime-async specialization is forced in a debug build, the builder
combinator is copied with its definition-site debug ranges remarked at the
call site. User continuation arguments keep their own ranges, and the marked
method retains a call-site sequence point even when forced inlining reduces
its intermediate closures.

Dead branches eliminated by optimization do not reach code generation and do
not produce a suspension-outside-runtime-async diagnostic.

Runtime-async boundary recognition is centralized in
`TypedTree/RuntimeAsync.fs`. `IsRuntimeAsyncBoundary` (with the more specific
`TryGetRuntimeAsyncReturn` and `IsRuntimeAsyncSuspensionExpr`) lets consumers
use the shared recognizers rather than matching typed-tree shapes
independently.

The optimizer and the lowering pass use a context-local `RuntimeAsyncAnalyzer`. It memoizes
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
   assemblies do not define the enum member).
2. **Closure body** (`GenClosureAsLocalTypeFunction` and
   `GenClosureAsFirstClassFunction`): the same unwrapping marks the closure
   `Invoke` method's IL body (`ILMethodBody.IsRuntimeAsync`).
   `EraseClosures.convIlxClosureDef` copies that flag onto the emitted
   method.
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
shape for an indirect call with multiple arguments. Forced inlining and
eligible callback inlining happen before closure erasure; a suspension left
behind an opaque indirect invocation is rejected by code generation.

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

Sample builders for tasks (`runtimeTask`), cancellable computations (`async2`) and async sequences (`asyncSeq2`) live with the [component tests](../tests/FSharp.Compiler.ComponentTests/Language/RuntimeAsync/README.md). They are examples, not FSharp.Core APIs.

### Unsupported inline-fragment positions

An inline fragment that escapes as a first-class value, is passed to a
non-inline function, or is dynamically dispatched cannot be preserved as a
runtime-async suspension fragment. If the suspension remains in the generated
non-runtime-async method, code generation reports FS3916 rather than emitting
an unsafe closure. Fragments in statically eliminated branches do not trigger
this diagnostic.

## Async sequences

`__runtimeAsyncSequence` turns a statically known `unit -> seq<'T>` recipe into an `IAsyncEnumerable<'T>`. It reuses sequence lowering, but emits a single runtime-async `MoveNextAsync(): ValueTask<bool>` state machine. Awaits in the recipe stay in this method, and yield positions persist between calls. Ordinary nested sequences remain synchronous.

```fsharp
let values (work: Task<int>) =
    __runtimeAsyncSequence (fun () -> seq {
        yield AsyncHelpers.Await work
        try
            yield AsyncHelpers.Await work + 1
        with _ ->
            AsyncHelpers.Await(Task.Delay 10)
            yield -1
    })
```

`__runtimeAsyncSequenceCancellationToken()` returns the token passed to `GetAsyncEnumerator`. Each resume checks this token before running recipe code; awaiting a non-cancellable operation does not make it cancellable.

Generated types derive from `GeneratedRuntimeAsyncSequenceBase<'T>`, which implements the enumerator interfaces:

* The first `GetAsyncEnumerator` reuses the sequence instance; later calls return independent clones.
* A second `MoveNextAsync`, or a `DisposeAsync`, while a move is pending throws `InvalidOperationException`. The generated `MoveNextAsync` releases this guard through `CompleteMoveNext` before its result completes.

`try/with` and `try/finally` blocks are part of the `MoveNextAsync` state machine, so bodies, guards, handlers and `finally` blocks can all await:

* A fault runs the pending `finally` blocks, then jumps to the innermost handler. Cleanup completes before guards run, as in ordinary `try/with`; a cleanup failure replaces the original exception.
* `DisposeAsync` marks the enumerator as disposing and runs `MoveNextAsync` once more. That run executes the pending `finally` blocks and skips handlers.

Other behavior:

* Recipes are normalized even with `--optimize-`. This can remove intermediate recipe locals but does not optimize surrounding code.
* Unhandled faults rethrow with their original dispatch information. Successful moves allocate no exception-transport object.
* Early disposal keeps ordinary sequence precedence: an outer cleanup failure replaces an inner one.
* Runtime-async inputs that are consumed immediately stay adjacent to their awaits, so the runtime can fuse the calls.
* Opaque recipes are rejected.
* The compiler does not hand off tail `yield!` calls; a builder can, as the sample `asyncSeq2` does.
* Builder-generated `MoveNextAsync` methods can lose sequence points, so source stepping is not guaranteed.

Run the async-sequence tests with:

```sh
./build.sh -c Release
dotnet test --project tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Release --no-build --filter-class 'Language.RuntimeAsyncSequenceTests'
```

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

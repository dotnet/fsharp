## Runtime Async CE Library Sample

This sample demonstrates a `runtimeTask` computation expression (CE) defined in a library project and consumed by a separate app project. The key design insight is that **consumer API functions need no `[<MethodImplAttribute(0x2000)>]` at all** — the compiler automatically marks them as `cil managed async` based on body analysis (detecting `AsyncHelpers.Await` calls after inlining).

The design is inspired by [IcedTasks](https://github.com/TheAngryByrd/IcedTasks)'s `TaskBuilderBase_Net10.fs`, using a fully-inlined `Run` method with `[<InlineIfLambda>]`.

It is wired to the repo-built compiler so runtime-async IL is emitted end-to-end.

### Projects

- `RuntimeAsync.Library`: defines `RuntimeTaskBuilder` and task-returning library APIs using `runtimeTask`, plus `SimpleAsyncResource` (IAsyncDisposable) and `AsyncRange` (IAsyncEnumerable) helper types
- `RuntimeAsync.Demo`: references the library and runs all 11 example scenarios

### Key Design

The working solution uses a **fully-inlined Run + Await sentinel** pattern:

#### RuntimeAsync Attribute

`[<RuntimeAsync>]` on the builder class is the single entry point for all runtime-async compiler behavior:

- It implicitly applies `NoDynamicInvocation` to all public inline members, so no explicit `[<NoDynamicInvocation>]` is needed on `Bind`, `Using`, or `For`.
- It gates the optimizer anti-inlining behavior (Fix 2 below).
- Consumers need no `[<MethodImplAttribute(0x2000)>]` — the compiler detects `AsyncHelpers.Await` calls in the inlined body and marks the consumer as `cil managed async` automatically.

```fsharp
[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    // Delay returns a thunk (unit -> 'T), NOT a Task
    member inline _.Delay(f: unit -> 'T) : unit -> 'T = f

    // Run is fully inline — its body gets inlined into each consumer function.
    // The Await sentinel ensures the consumer always gets 'cil managed async'
    // even when the CE body has no let!/do! bindings.
    // NO [<MethodImplAttribute(0x2000)>] needed on consumers.
    member inline _.Run([<InlineIfLambda>] f: unit -> 'T) : Task<'T> =
        AsyncHelpers.Await(ValueTask.CompletedTask)  // sentinel
        RuntimeTaskBuilderHelpers.cast (f())

    // Bind members — NoDynamicInvocation is implicit from [<RuntimeAsync>] on the type.
    // No explicit [<NoDynamicInvocation>] needed.
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    // ... overloads for Task, ValueTask<'T>, ValueTask,
    //     ConfiguredTaskAwaitable, ConfiguredTaskAwaitable<'T>,
    //     ConfiguredValueTaskAwaitable, ConfiguredValueTaskAwaitable<'T>

    // IAsyncDisposable and IAsyncEnumerable as intrinsic members
    // (higher priority than IDisposable/seq extensions)
    member inline this.Using(resource: 'T when 'T :> IAsyncDisposable, body: 'T -> 'U) : 'U = ...
    member inline _.For(sequence: IAsyncEnumerable<'T>, body: 'T -> unit) : unit = ...

// Extension (lower priority): generic Bind for any awaitable via SRTP + UnsafeAwaitAwaiter
type RuntimeTaskBuilder with
    member inline _.Bind(awaitable: ^Awaitable, f: ^TResult -> 'U) : 'U
        when ^Awaitable : (member GetAwaiter: unit -> ^Awaiter)
        and ^Awaiter :> ICriticalNotifyCompletion = ...
```

Consumer API functions use `runtimeTask { ... }` with **no attribute**:

```fsharp
// No [<MethodImplAttribute>] needed here!
let addFromTaskAndValueTask (left: Task<int>) (right: ValueTask<int>) : Task<int> =
    runtimeTask {
        let! l = left
        let! r = right
        return l + r
    }
```

After inlining, the consumer function's body directly contains `AsyncHelpers.Await` calls. The compiler detects these and emits `cil managed async` on the consumer method.

### How It Works (Technical Details)

#### The Inlining Pattern

1. `Run` is `member inline` with `[<InlineIfLambda>]` on `f` — **no** `[<MethodImplAttribute(0x2000)>]`
2. After inlining, the consumer function's body contains `AsyncHelpers.Await` calls
3. `ExprContainsAsyncHelpersAwaitCall` in `IlxGen.fs` detects these (`Await`, `AwaitAwaiter`, `UnsafeAwaitAwaiter`) and applies `cil managed async`
4. `RuntimeTaskBuilderHelpers.cast(f())` is a no-op reinterpret cast — the runtime wraps the raw return value into `Task<T>` for `cil managed async` methods

#### The Await Sentinel

`AsyncHelpers.Await(ValueTask.CompletedTask)` in `Run` ensures that **every** consumer gets `cil managed async`, even CEs with no `let!/do!` bindings (e.g., `runtimeTask { return 42 }`). Without it, the body analysis would find no `Await` calls and the method would be emitted as regular `cil managed`, causing the `cast` trick to fail.

#### Two Required Compiler Fixes

Both fixes are gated on `[<RuntimeAsync>]` being present on the builder class — the attribute is what enables these behaviors.

**Fix 1 — IlxGen.fs return-type guard:** `ExprContainsAsyncHelpersAwaitCall` body analysis must only propagate `cil managed async` when the method returns a Task-like type (`Task`, `Task<T>`, `ValueTask`, `ValueTask<T>`). Without this guard, the optimizer might inline an async function into a non-Task-returning method (e.g., `main : int`), and the runtime would reject it with `TypeLoadException`.

**Fix 2 — Optimizer.fs anti-inlining guard:** Functions whose optimized bodies contain `AsyncHelpers.Await`/`AwaitAwaiter`/`UnsafeAwaitAwaiter` calls must not be cross-module inlined by the optimizer. Their optimization data is replaced with `UnknownValue`. Without this, the optimizer inlines async functions into non-async callers, causing `NullReferenceException` from the `cast` trick being used outside a `cil managed async` context.

#### Nested CE Limitation

Inline-nested `runtimeTask { ... }` CEs within the same function do **not** work. The inner CE's `cast(raw_value)` produces a fake `Task<T>` that the outer CE's `Bind` tries to `AsyncHelpers.Await` — causing `NullReferenceException`. The `cast` trick only works for the final return value of a `cil managed async` method.

**Workaround:** Each nesting level must be a separate function so that each gets its own `cil managed async` method:

```fsharp
// Each function is a separate 'cil managed async' method
let private innerInnerTask () : Task<int> =
    runtimeTask { return 10 }

let private innerTask () : Task<int> =
    runtimeTask {
        let! b = innerInnerTask ()
        return b + 20
    }

let deeplyNestedRuntimeTask () : Task<int> =
    runtimeTask {
        let! a = innerTask ()
        return a + 70
    }
```

### Examples

The sample includes 11 examples in `Api.fs`:

| Example | Demonstrates |
|---|---|
| `addFromTaskAndValueTask` | Binding `Task<T>` and `ValueTask<T>` |
| `bindUnitTaskAndUnitValueTask` | Binding unit `Task` and unit `ValueTask` via `do!` |
| `safeDivide` | `try/with` inside runtimeTask |
| `nestedRuntimeTask` | Composing runtimeTask functions |
| `deeplyNestedRuntimeTask` | 3-level deep nesting via helper functions |
| `consumeOlderTaskCE` | Consuming standard `task { }` CE results |
| `taskDelayYieldAndRun` | `Task.Delay`, `Task.Yield()` (generic awaitable), `Task.Run` |
| `useAsyncDisposable` | `use` with `IAsyncDisposable` resource |
| `iterateAsyncEnumerable` | `for` over `IAsyncEnumerable<T>` |
| `configureAwaitExample` | `.ConfigureAwait(false)` on Task and Task<T> |
| `inlineNestedRuntimeTask` | Nesting runtimeTask CEs via separate functions |

### Prerequisites

- .NET 10 SDK
- F# preview language enabled (already set in each project)
- .NET SDK restore access (normal `dotnet run` prerequisites)
- `DOTNET_RuntimeAsync=1` set before launching the process (required for loading runtime-async methods)

### Build

```bash
dotnet build docs/samples/runtime-async-library/RuntimeAsync.Demo/RuntimeAsync.Demo.fsproj -c Release
```

### Run

**Linux/macOS:**
```bash
DOTNET_RuntimeAsync=1 dotnet run --project docs/samples/runtime-async-library/RuntimeAsync.Demo/RuntimeAsync.Demo.fsproj -c Release
```

**Windows (PowerShell):**
```powershell
$env:DOTNET_RuntimeAsync = "1"
dotnet run --project docs/samples/runtime-async-library/RuntimeAsync.Demo/RuntimeAsync.Demo.fsproj -c Release
```

### Expected Output

```
Task<T> + ValueTask<T> -> 15
Task + ValueTask -> completed
try/with -> 0
nested runtimeTask -> 44
deeply nested runtimeTask -> 100
consume older task CE -> 84
Task.Delay + Task.Yield + Task.Run -> 42
IAsyncDisposable -> async resource used
IAsyncEnumerable sum -> 15
ConfigureAwait(false) -> 99
inline-nested runtimeTask -> 40
```

### IL Verification

Both projects have an `ILDasm.targets` file that runs ILDasm after build, producing `.il` files in their respective output directories.

To verify manually:

```bash
# Build the library
dotnet build docs/samples/runtime-async-library/RuntimeAsync.Library/RuntimeAsync.Library.fsproj -c Release
```

In the output IL:

- All `Api::*` functions → should show `cil managed async` (they contain inlined `AsyncHelpers.Await`/`UnsafeAwaitAwaiter` calls)
- `RuntimeTaskBuilder::Run` → should show `cil managed async` (non-inlined fallback copy)
- `Program::main` → should show `cil managed` (NOT `cil managed async` — return-type guard prevents this)

The return-type guard in IlxGen.fs ensures that only methods returning Task-like types get `cil managed async`, even if their bodies contain `AsyncHelpers.Await` calls from inlining.

> **Note:** Running without `DOTNET_RuntimeAsync=1` fails with `TypeLoadException` because runtime-async methods are not enabled for that process.
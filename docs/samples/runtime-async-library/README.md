## Runtime Async CE Library Sample

This sample demonstrates a `runtimeTask` computation expression (CE) defined in a library project and consumed by a separate app project. The key design insight is that **`Run` is non-inline with `[<MethodImplAttribute(0x2000)>]`** — the compiler emits it as `cil managed async` directly, and CE body closures are also `cil managed async` (because they contain `AsyncHelpers.Await` calls from inlined `Bind` members).

It is wired to the repo-built compiler so runtime-async IL is emitted end-to-end.

### Projects

- `RuntimeAsync.Library`: defines `RuntimeTaskBuilder` and task-returning library APIs using `runtimeTask`, plus `SimpleAsyncResource` (IAsyncDisposable) and `AsyncRange` (IAsyncEnumerable) helper types
- `RuntimeAsync.Demo`: references the library and runs all 12 example scenarios

### Key Design

The working solution uses a **non-inline Run + async closures** pattern:

#### RuntimeAsync Attribute

`[<RuntimeAsync>]` on the builder class is the single entry point for all runtime-async compiler behavior:

- It implicitly applies `NoDynamicInvocation` to all public inline members, so no explicit `[<NoDynamicInvocation>]` is needed on `Bind`, `Using`, or `For`.
- It gates the optimizer anti-inlining behavior (Fix 2 below).

```fsharp
[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    // Delay wraps the CE body in a closure that is 'cil managed async'.
    // The compiler automatically injects the sentinel (AsyncHelpers.Await(ValueTask.CompletedTask))
    // into every Delay closure body, ensuring cloIsAsync = true even with no let!/do! bindings.
    // The compiler also handles 'T → Task<'T> bridging automatically for [<RuntimeAsync>] builders,
    // so no cast helper is needed.
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> =
        fun () -> f()

    // Run is non-inline with [<MethodImplAttribute(0x2000)>] — emitted as 'cil managed async'.
    // Delay closure returns Task<'T> at runtime (the 'cil managed async' runtime wraps T→Task<T>).
    // Run awaits the closure result, then wraps T→Task<T> (because Run itself is 'cil managed async').
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> =
        AsyncHelpers.Await(f())

    // Bind members — NoDynamicInvocation is implicit from [<RuntimeAsync>] on the type.
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
// No [<MethodImplAttribute>] needed here — consumer just calls Run and returns the Task<T>.
let addFromTaskAndValueTask (left: Task<int>) (right: ValueTask<int>) : Task<int> =
    runtimeTask {
        let! l = left
        let! r = right
        return l + r
    }
```

The consumer function calls `Run(closure)` and returns the `Task<int>` that `Run` returns. The consumer itself is NOT `cil managed async` — only `Run` and the CE body closures are.

### How It Works (Technical Details)

#### The Non-Inline Run Pattern

1. `Run` is `member _` (non-inline) with `[<MethodImplAttribute(0x2000)>]` — the compiler emits it as `cil managed async`
2. CE body closures contain `AsyncHelpers.Await` calls (from inlined `Bind` members, or the auto-injected sentinel) — they are also `cil managed async`
3. At runtime, the closure's `Invoke` returns `Task<'T>` (because it's `cil managed async` — the runtime wraps `'T → Task<'T>` automatically)
4. The compiler handles `'T → Task<'T>` bridging automatically for `[<RuntimeAsync>]` builders — no `cast` helper is needed
5. `AsyncHelpers.Await(Task<'T>)` unwraps `Task<'T>` to `'T`
6. `Run` wraps `'T` back to `Task<'T>` (because it's `cil managed async`)

#### True Inline-Nested CEs

Because `Run` is non-inline and returns a real `Task<T>`, `runtimeTask { ... }` CEs can be nested directly inside each other:

```fsharp
let trueInlineNestedRuntimeTask () : Task<int> =
    runtimeTask {
        let! a =
            runtimeTask {
                return 21
            }
        let! b =
            runtimeTask {
                return 21
            }
        return a + b  // 42
    }
```

The inner `runtimeTask { return 21 }` calls `Run` which returns a real `Task<int>`. The outer `Bind` calls `AsyncHelpers.Await<int>(Task<int>)` → gets 21. This works because `Run` is a real `cil managed async` method, not an inlined cast.

#### Three Required Compiler Fixes

**Fix 1 — IlxGen.fs return-type guard:** `ExprContainsAsyncHelpersAwaitCall` body analysis must only propagate `cil managed async` when the method returns a Task-like type (`Task`, `Task<T>`, `ValueTask`, `ValueTask<T>`). Without this guard, the optimizer might inline an async function into a non-Task-returning method (e.g., `main : int`), and the runtime would reject it with `TypeLoadException`.

**Fix 2 — Optimizer.fs anti-inlining guard:** Functions whose optimized bodies contain `AsyncHelpers.Await`/`AwaitAwaiter`/`UnsafeAwaitAwaiter` calls must not be cross-module inlined by the optimizer. Their optimization data is replaced with `UnknownValue`. Without this, the optimizer inlines async functions into non-async callers, causing `NullReferenceException` at runtime.

**Fix 3 — EraseClosures.fs async closure emission:** CE body closures contain `AsyncHelpers.Await` calls (from inlined `Bind` members, or the auto-injected sentinel). The `cloIsAsync` field in `IlxClosureInfo` is set when the closure body contains these calls. `EraseClosures.fs` emits the closure's `Invoke` method as `cil managed async` when `cloIsAsync = true`. Without this, the runtime rejects the closure with `TypeLoadException` because `AsyncHelpers.Await` can only be called from `cil managed async` methods.

**Fix 4 — CheckExpressions.fs type-checking coercion:** When inside an inline member of a `[<RuntimeAsync>]` type, the compiler allows `fun () -> f()` where `f()` returns `'T` but the closure's declared return type is `Task<'T>`. The compiler unwraps the Task-like return type for the lambda body, so the library author writes `fun () -> f()` without any cast helper.

**Fix 5 — IlxGen.fs Lambdas_return fix:** When the closure's declared return type is `Task<'T>` but the body type is `'T` (due to the coercion in Fix 4), the IL generator uses the declared `Task<'T>` for `Lambdas_return` so the closure's `Invoke` method declares the correct return type in IL.

**Fix 6 — CheckComputationExpressions.fs automatic sentinel injection:** When the builder type has `[<RuntimeAsync>]`, the CE desugaring automatically injects `AsyncHelpers.Await(ValueTask.CompletedTask)` as the first expression in ALL `Delay` closure bodies. This ensures `cloIsAsync = true` even when the CE body has no `let!`/`do!` bindings (e.g., `runtimeTask { return 42 }`), so the closure is always emitted as `cil managed async`.

### Examples

The sample includes 12 examples in `Api.fs`:

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
| `trueInlineNestedRuntimeTask` | True inline-nested runtimeTask CEs (enabled by non-inline Run) |

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
true inline-nested runtimeTask -> 42
```

### IL Verification

Both projects have an `ILDasm.targets` file that runs ILDasm after build, producing `.il` files in their respective output directories.

To verify manually:

```bash
# Build the library
dotnet build docs/samples/runtime-async-library/RuntimeAsync.Library/RuntimeAsync.Library.fsproj -c Release
```

In the output IL:

- `RuntimeTaskBuilder::Run` → should show `cil managed async` (non-inline, has `[<MethodImplAttribute(0x2000)>]`)
- CE body closures (e.g., `addFromTaskAndValueTask@57`) → `Invoke` method should show `cil managed async` (contains `AsyncHelpers.Await` calls)
- `Api::*` consumer functions → should show `cil managed` (NOT `cil managed async` — they just call `Run` and return the `Task<T>`)
- `Program::main` → should show `cil managed` (NOT `cil managed async`)

> **Note:** Running without `DOTNET_RuntimeAsync=1` fails with `TypeLoadException` because runtime-async methods are not enabled for that process.

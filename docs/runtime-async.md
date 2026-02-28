# Runtime Async in F#

Runtime async is a .NET 10.0+ feature that lets you write async methods without a state machine. Instead of the compiler generating a state machine (as `task { }` and `async { }` do), the runtime itself handles suspension and resumption. The result is flatter IL, lower overhead, and simpler generated code.

F# exposes this feature in two ways:

- The `runtimeTask { }` computation expression, for most use cases.
- Direct `[<MethodImpl(MethodImplOptions.Async)>]` annotation, for library authors who need full control.

---

## Prerequisites

Before using runtime async, you need:

**Target framework**: `net10.0` or later.

```xml
<TargetFramework>net10.0</TargetFramework>
```

**Language version**: `preview` (the feature is gated behind preview).

```xml
<LangVersion>preview</LangVersion>
```

Or pass `--langversion:preview` to the compiler directly.

**Runtime environment variable**: `DOTNET_RuntimeAsync=1` must be set **before** the CLR loads any type that contains runtime-async methods. Setting it inside your program code is too late.

```bash
# On Linux/macOS
export DOTNET_RuntimeAsync=1
dotnet run

# On Windows (Command Prompt)
set DOTNET_RuntimeAsync=1
dotnet run

# On Windows (PowerShell)
$env:DOTNET_RuntimeAsync = "1"
dotnet run
```

> **Important**: If `DOTNET_RuntimeAsync=1` is not set before the CLR loads the type, the runtime will throw at the call site. This is a runtime check, not a compile-time check.

---

## Using `runtimeTask { }`

The `runtimeTask` computation expression is the primary way to write runtime-async methods in F#. It produces a `Task<'T>` and emits flat IL with `AsyncHelpers.Await` calls rather than a state machine.

Add `#nowarn "57"` to suppress the preview feature warning.

### Basic Usage

```fsharp
#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

let greet (name: string) : Task<string> =
    runtimeTask {
        return $"Hello, {name}!"
    }
```

### Awaiting Tasks

You can `let!` bind `Task<'T>`, `Task`, `ValueTask<'T>`, and `ValueTask`:

```fsharp
#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

let fetchAndDouble (t: Task<int>) : Task<int> =
    runtimeTask {
        let! value = t
        return value * 2
    }

let awaitUnitTask (t: Task) : Task<unit> =
    runtimeTask {
        let! () = t
        return ()
    }

let awaitValueTask (t: ValueTask<int>) : Task<int> =
    runtimeTask {
        let! value = t
        return value + 1
    }
```

### Multiple Awaits

```fsharp
#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

let addResults (a: Task<int>) (b: Task<int>) : Task<int> =
    runtimeTask {
        let! x = a
        let! y = b
        return x + y
    }
```

### Control Flow

`while` loops and `for` loops over sequences work as expected:

```fsharp
#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

let processItems (items: seq<Task<int>>) : Task<int> =
    runtimeTask {
        let mutable total = 0
        for item in items do
            let! value = item
            total <- total + value
        return total
    }

let countdown (start: int) : Task<unit> =
    runtimeTask {
        let mutable i = start
        while i > 0 do
            printfn "%d" i
            i <- i - 1
    }
```

### Error Handling

`try/with` and `try/finally` both work inside `runtimeTask { }`:

```fsharp
#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

let safeAwait (t: Task<int>) : Task<int> =
    runtimeTask {
        try
            let! value = t
            return value
        with ex ->
            printfn "Error: %s" ex.Message
            return -1
    }

let withCleanup (t: Task<int>) : Task<int> =
    runtimeTask {
        try
            let! value = t
            return value
        finally
            printfn "Cleanup complete"
    }
```

### Disposing Resources

Use `use` (not `use!`) to dispose `IDisposable` resources:

```fsharp
#nowarn "57"
open System.IO
open System.Threading.Tasks
open Microsoft.FSharp.Control

let readFile (path: string) : Task<string> =
    runtimeTask {
        use reader = new StreamReader(path)
        let! line = Task.Run(fun () -> reader.ReadLine())
        return line
    }
```

---

## Limitations Compared to `task { }`

`runtimeTask { }` is intentionally minimal. It covers the common cases but does not replicate every feature of `task { }`.

| Feature | `task { }` | `runtimeTask { }` |
|---|---|---|
| `let! x = someTask` | Yes | Yes |
| `return x` | Yes | Yes |
| `return!` | Yes | **No** |
| `and!` (parallel bind) | Yes | **No** |
| `use!` (async dispose) | Yes | **No** (only `use` for `IDisposable`) |
| `do! Task.Yield()` | Yes | **No** |
| `let! x = async { ... }` | Yes | **No** |
| `IAsyncDisposable` in `use` | Yes | **No** |
| Returns `Task<'T>` | Yes | Yes |
| Returns `Task` | Yes | **No** |
| Returns `ValueTask<'T>` | Yes | **No** |
| Background variant | `backgroundTask { }` | **No** |

If you need any of the unsupported features, use `task { }` instead. You can freely interop between the two: a `runtimeTask { }` method can `let!` the result of a `task { }` method, and vice versa.

```fsharp
#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

// task CE produces a Task<int>
let computeWithStateMachine () : Task<int> =
    task { return 42 }

// runtimeTask CE awaits it without a state machine
let consumeWithRuntimeAsync () : Task<int> =
    runtimeTask {
        let! result = computeWithStateMachine()
        return result * 2
    }
```

---

## Direct Usage with `[<MethodImpl(MethodImplOptions.Async)>]`

For library authors or cases where you need more control, you can annotate a method directly with `[<MethodImpl(MethodImplOptions.Async)>]` and call `AsyncHelpers.Await` yourself.

This approach supports all four task-like return types: `Task`, `Task<'T>`, `ValueTask`, and `ValueTask<'T>`.

Add `#nowarn "57"` for the preview warning and `#nowarn "SYSLIB5007"` when calling `AsyncHelpers.Await` directly.

### Supported Return Types

```fsharp
#nowarn "57"
#nowarn "SYSLIB5007"
open System.Runtime.CompilerServices
open System.Threading.Tasks

// Returns Task<'T>
[<MethodImpl(MethodImplOptions.Async)>]
let asyncReturnInt () : Task<int> = 42

// Returns Task (non-generic)
[<MethodImpl(MethodImplOptions.Async)>]
let asyncReturnUnit () : Task = ()

// Returns ValueTask<'T>
[<MethodImpl(MethodImplOptions.Async)>]
let asyncReturnValueTask () : ValueTask<int> = 42

// Returns ValueTask (non-generic)
[<MethodImpl(MethodImplOptions.Async)>]
let asyncReturnValueTaskUnit () : ValueTask = ()
```

### Awaiting with AsyncHelpers.Await

```fsharp
#nowarn "57"
#nowarn "SYSLIB5007"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImpl(MethodImplOptions.Async)>]
let awaitAndDouble (t: Task<int>) : Task<int> =
    let value = AsyncHelpers.Await t
    value * 2

[<MethodImpl(MethodImplOptions.Async)>]
let awaitMultiple (a: Task<int>) (b: Task<int>) : Task<int> =
    let x = AsyncHelpers.Await a
    let y = AsyncHelpers.Await b
    x + y

// Generic method
[<MethodImpl(MethodImplOptions.Async)>]
let genericAwait<'T> (t: Task<'T>) : Task<'T> =
    AsyncHelpers.Await t
```

### Error Handling

```fsharp
#nowarn "57"
#nowarn "SYSLIB5007"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImpl(MethodImplOptions.Async)>]
let safeAwait (t: Task<int>) : Task<int> =
    try
        AsyncHelpers.Await t
    with _ ->
        -1

[<MethodImpl(MethodImplOptions.Async)>]
let withFinally (t: Task<int>) : Task<int> =
    try
        AsyncHelpers.Await t
    finally
        printfn "done"
```

### Interop with `task { }`

```fsharp
#nowarn "57"
#nowarn "SYSLIB5007"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImpl(MethodImplOptions.Async)>]
let interopWithTaskCE () : Task<int> =
    let t = task { return 42 }
    AsyncHelpers.Await t
```

---

## Compiler Errors

The compiler enforces several rules when you use `[<MethodImpl(MethodImplOptions.Async)>]`. These errors apply to direct usage; the `runtimeTask { }` CE handles them internally.

### Error 3884: Invalid return type

Methods marked with `MethodImplOptions.Async` must return `Task`, `Task<'T>`, `ValueTask`, or `ValueTask<'T>`.

```fsharp
// Error FS3884: Methods marked with MethodImplOptions.Async must return Task,
// Task<'T>, ValueTask, or ValueTask<'T>. The actual return type is 'int'.
[<MethodImpl(MethodImplOptions.Async)>]
let bad () : int = 42  // wrong return type
```

### Error 3885: Conflict with Synchronized

`MethodImplOptions.Async` cannot be combined with `MethodImplOptions.Synchronized`.

```fsharp
// Error FS3885: Methods marked with MethodImplOptions.Async cannot also use
// MethodImplOptions.Synchronized.
[<MethodImpl(MethodImplOptions.Async ||| MethodImplOptions.Synchronized)>]
let bad () : Task<int> = 42
```

### Error 3886: Byref return type

Methods marked with `MethodImplOptions.Async` cannot return byref types.

```fsharp
// Error FS3886: Methods marked with MethodImplOptions.Async cannot return byref types.
[<MethodImpl(MethodImplOptions.Async)>]
let bad (x: byref<int>) : Task<byref<int>> = ...
```

### Error 3887: Runtime not supported

If you target a framework older than .NET 10.0, the compiler emits this error.

```
Error FS3887: Methods marked with MethodImplOptions.Async are not supported in this context.
```

Make sure your project targets `net10.0` or later and uses `--langversion:preview`.

---

## For Library Authors

### RuntimeAsyncAttribute

`RuntimeAsyncAttribute` is a marker attribute in `Microsoft.FSharp.Control` for annotating types that contain runtime-async methods. It's intended for future library extensibility.

```fsharp
open Microsoft.FSharp.Control

[<RuntimeAsync>]
type MyAsyncService() =
    member _.DoWork() : Task<int> =
        runtimeTask { return 42 }
```

**Important**: The compiler does not read `RuntimeAsyncAttribute` to propagate the async IL flag (0x2000). The attribute is a no-op marker today. It exists to signal intent and reserve the design space for future tooling.

The attribute targets classes only (`AttributeTargets.Class`) and cannot be applied multiple times to the same type.

### How the Async Flag Propagates

The 0x2000 IL flag is what tells the runtime to use runtime-async semantics instead of a state machine. Here's how it gets onto your methods:

1. The `RuntimeTaskBuilder.Run` method has `[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]` on it.
2. All builder members are `inline`, so the entire `runtimeTask { }` body gets inlined into the call site.
3. The F# compiler's IL generator (`IlxGen.fs`) detects `AsyncHelpers.Await` call sites in the inlined method body via `ExprContainsAsyncHelpersAwaitCall`.
4. When detected, the compiler emits the 0x2000 flag on the containing method.

For direct `[<MethodImpl(MethodImplOptions.Async)>]` usage, the flag comes from the attribute itself.

The key insight: **the flag propagates through `AsyncHelpers.Await` detection, not through `RuntimeAsyncAttribute`**. If you write a custom builder or helper that calls `AsyncHelpers.Await`, the compiler will propagate the flag automatically.

### Writing a Custom Builder

If you want to write your own computation expression builder that produces runtime-async methods, follow the same pattern as `RuntimeTaskBuilder`:

1. Make all members `inline` with `[<InlineIfLambda>]` on continuation parameters.
2. Put `[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]` on the `Run` member.
3. Call `AsyncHelpers.Await` in your `Bind` members.

```fsharp
#nowarn "57"
#nowarn "SYSLIB5007"
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<Sealed>]
type MyRuntimeBuilder() =
    member inline _.Return(x: 'T) : 'T = x
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : 'T = f()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member inline _.Run([<InlineIfLambda>] f: unit -> 'T) : Task<'T> =
        // Cast is needed because the body returns 'T but Run must return Task<'T>.
        // The runtime handles the wrapping when the 0x2000 flag is present.
        (# "" (f()) : Task<'T> #)

let myRuntime = MyRuntimeBuilder()
```

> **Note**: The `(# "" ... #)` cast is an internal F# IL trick. In practice, use `RuntimeTaskBuilder` directly rather than reimplementing it.

---

## Quick Reference

```fsharp
// Project file requirements:
// <TargetFramework>net10.0</TargetFramework>
// <LangVersion>preview</LangVersion>

// Environment (before CLR loads the type):
// DOTNET_RuntimeAsync=1

#nowarn "57"
open System.Threading.Tasks
open Microsoft.FSharp.Control

// CE usage (most common)
let example1 (t: Task<int>) : Task<int> =
    runtimeTask {
        let! x = t
        return x + 1
    }

// Direct attribute usage (library authors)
#nowarn "SYSLIB5007"
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.Async)>]
let example2 (t: Task<int>) : Task<int> =
    let x = AsyncHelpers.Await t
    x + 1
```

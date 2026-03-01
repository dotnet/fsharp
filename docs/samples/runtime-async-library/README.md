## Runtime Async CE Library Sample

This sample demonstrates a `runtimeTask` computation expression (CE) defined in a library project and consumed by a separate app project. The key design insight is that **`[<MethodImplAttribute(0x2000)>]` lives only on `RuntimeTaskBuilder.Run`** — consumer API functions need no attribute at all.

It is wired to the repo-built compiler so runtime-async IL is emitted end-to-end.

### Projects

- `RuntimeAsync.Library`: defines `RuntimeTaskBuilder` and task-returning library APIs using `runtimeTask`
- `RuntimeAsync.Demo`: references the library and runs several task-like scenarios

### Key Design

The working solution uses a **thunk-based Delay + attributed Run** pattern:

```fsharp
[<Sealed>]
type RuntimeTaskBuilder() =
    // Delay returns a thunk (unit -> 'T), NOT a Task
    member inline _.Delay(f: unit -> 'T) : unit -> 'T = f

    // Run is the ONLY method with [<MethodImplAttribute(0x2000)>]
    // It calls the thunk and casts the result to Task<'T>
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member inline _.Run(f: unit -> 'T) : Task<'T> =
        RuntimeTaskBuilderUnsafe.cast (f())

    // Bind members have [<NoDynamicInvocation>] to prevent propagation
    // of `cil managed async` to those helpers
    [<NoDynamicInvocation>]
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    // ... other Bind overloads
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

The `runtimeTask { ... }` block desugars to a call to `RuntimeTaskBuilder.Run`, which carries the `0x2000` attribute. The consumer function itself is just a regular function that calls `Run`.

### Why It Works (Technical Details)

Two sub-problems were solved to make this design work:

1. **FS3519 fix**: `[<InlineIfLambda>]` cannot be on `Run`'s parameter when `Run` returns `Task<'T>` (not a function type). The parameter is typed as `unit -> 'T` without `[<InlineIfLambda>]`.

2. **Double-wrapping fix**: Consumer functions with `let!`/`do!` were incorrectly getting `cil managed async` because `ExprContainsAsyncHelpersAwaitCall` in `IlxGen.fs` walked into `Expr.Lambda` bodies, finding `AsyncHelpers.Await` calls in the Delay thunk. Fixed by changing the `Expr.Lambda` case to return `false` (stop recursing into lambda bodies). This prevents the compiler from marking consumer functions as `cil managed async` when they merely call `Run` (which is already `cil managed async`).

Additional compiler fixes:
- `CheckExpressions.fs`: skip special async handling for inline methods with `0x2000` attribute
- `PostInferenceChecks.fs`: allow `[<InlineIfLambda>]` on runtime-async method parameters

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
```

### IL Verification

To confirm that `cil managed async` appears **only on `RuntimeTaskBuilder.Run`** and NOT on consumer functions:

```bash
# Build the library
dotnet build docs/samples/runtime-async-library/RuntimeAsync.Library/RuntimeAsync.Library.fsproj -c Release

# Disassemble
ildasm /utf8 /out=artifacts/runtime-async-library.il artifacts/bin/RuntimeAsync.Library/Release/net10.0/RuntimeAsync.Library.dll
```

In the output IL:

- `RuntimeTaskBuilder::Run` → should show `cil managed async`
- `Api::addFromTaskAndValueTask` → should show `cil managed` (NOT `cil managed async`)
- `Api::bindUnitTaskAndUnitValueTask` → should show `cil managed` (NOT `cil managed async`)
- `Api::safeDivide` → should show `cil managed` (NOT `cil managed async`)
- `Api::nestedRuntimeTask` → should show `cil managed` (NOT `cil managed async`)

This confirms the attribute is an implementation detail of the builder, not something consumers need to know about.

> **Note:** Running without `DOTNET_RuntimeAsync=1` fails with `TypeLoadException` because runtime-async methods are not enabled for that process.

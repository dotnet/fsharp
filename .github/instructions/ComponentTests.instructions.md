---
applyTo:
  - "tests/FSharp.Compiler.ComponentTests/**/*.fs"
---

# ComponentTests DSL

Tests use a pipeline: **Create → Configure → Action → Assert**. The action verb is mandatory — never pipe a `CompilationUnit` directly into an assertion.

## Actions (pick the cheapest that covers what you're testing)

```fsharp
// Type errors only, no codegen — fastest
FSharp "..."
|> typecheck
|> shouldSucceed

// Full compile, produces assembly
FSharp "..."
|> compile
|> shouldSucceed

// IL shape verification (needs compile)
FSharp "..."
|> compile
|> verifyILContains [".method"]

// Compile + execute (needs EntryPoint and asExe)
FSharp "..."
|> compileExeAndRun
|> shouldSucceed

// FSI in-process
Fsx "..."
|> eval
|> withEvalValueEquals 2

// FSI subprocess
Fsx "..."
|> runFsi
|> withStdOutContains "hello"
```

Use `typecheck` for anything that doesn't need IL or runtime. Use `compile` only when you need the assembly (IL checks, interop). Use `compileExeAndRun` only when testing runtime behavior.

## Diagnostics

```fsharp
// Exact diagnostic with location and message
FSharp "..."
|> typecheck
|> shouldFail
|> withDiagnostics [
    (Error 73, Line 3, Col 5, Line 3, Col 10, "internal error: ...")
    (Warning 20, Line 5, Col 1, Line 5, Col 8, "The result of this expression...")
]

// Just error code (when message/location don't matter)
FSharp "..."
|> typecheck
|> shouldFail
|> withErrorCode 73
```

## C# interop

```fsharp
let csLib =
    CSharp """public interface I { void M(out int v); }"""
    |> withName "CsLib"

FSharp """
open CsLib
type T() = interface I with member _.M(v) = v <- 42
"""
|> withReferences [csLib]
|> compile
|> shouldSucceed
```

## Regression test template

```fsharp
// https://github.com/dotnet/fsharp/issues/NNNNN
[<Fact>]
let ``Issue NNNNN - brief description`` () =
    FSharp """
// minimal repro from issue
    """
    |> asLibrary
    |> typecheck
    |> shouldSucceed
```

## Local helpers

If configuration becomes long and boilerplate, extract a local helper or use an existing one in the same module:

```fsharp
// Local helper for repeated configuration
let typecheckWithPreview source =
    source
    |> asLibrary
    |> withLangVersionPreview
    |> withOptions ["--test:ErrorRanges"]
    |> typecheck

// Usage
FSharp "..."
|> typecheckWithPreview
|> shouldSucceed
```

## Common mistakes

- Missing action: `FSharp "..." |> shouldSucceed` — won't compile, `shouldSucceed` expects `CompilationResult`
- Missing `asExe` for runtime tests: `FSharp "..." |> compile |> run` fails on library output
- Using `compile` when `typecheck` suffices — wastes CI time

## Platform attributes

Use `[<Fact>]` unless the test requires a specific runtime:
- `[<FactForNETCOREAPP>]` — .NET Core only
- `[<FactForDESKTOP>]` — .NET Framework only
- `[<FactForWINDOWS>]` — Windows only

## Workflow

1. Write test with explicit pipeline
2. Build: `dotnet build tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj -c Release`
3. Run: `dotnet test --project tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj -c Release --no-build -- --filter-method "*TestName*"`
4. Format: `dotnet fantomas <file.fs>`

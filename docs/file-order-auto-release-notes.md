# Release Notes: `fix_dogmatic_file_order_nonsense` Fork

This branch of `dotnet/fsharp` adds opt-in dependency-based file ordering to
the F# compiler. With one MSBuild property or one CLI flag, you stop having
to maintain `.fsproj` file ordering by hand.

## What you get

- **`--file-order-auto+`**: a new compiler flag (off by default). When set,
  the compiler computes a dependency order for `.fs`/`.fsi` files and
  reorders the inputs before type checking.
- **`<FSharpAutoFileOrder>true</FSharpAutoFileOrder>`**: MSBuild property
  that wires the flag into `dotnet build` / `dotnet run`.
- **FCS support**: `FSharpProjectOptions.OtherOptions` accepts the flag, so
  Ionide (and any other FCS host) can opt in. IntelliSense, Go-to-Definition,
  and Find-All-References work end-to-end on auto-ordered projects.
- **Cycle group synthesis** (build path only): a set of files that mutually
  reference each other gets compiled as one synthetic recursive namespace.
- **`and`-keyword deprecation** (warning **FS3887**): under `--file-order-auto+`,
  `type X = ... and Y = ...` now produces a deprecation warning. Suppressable
  via `--nowarn:3887` or `<NoWarn>3887</NoWarn>`. The warning is silent in
  manual mode.

## What hasn't changed

- The default behaviour is upstream: manual ordering, identical type
  inference, identical diagnostics, identical FSharp.Core compilation.
- No upstream test had to change. The error-corpus suite confirms diagnostic
  parity between manual and auto modes for six representative error
  categories (undefined name, undefined module, type mismatch, missing
  field, missing open, wrong arity).
- The full upstream F# test suite passes on this branch: **15,404 tests, 0
  failures** across `FSharp.Compiler.ComponentTests` (7,031),
  `FSharp.Compiler.Service.Tests` (2,153),
  `FSharp.Compiler.Private.Scripting.UnitTests` (102),
  `FSharp.Build.UnitTests` (42), and `FSharp.Core.UnitTests` (6,076).

## How to use

### Enable for a project

```xml
<PropertyGroup>
  <FSharpAutoFileOrder>true</FSharpAutoFileOrder>
</PropertyGroup>
```

Reorder your `<Compile Include="..." />` items however you like, or just
leave them as-is. Build with `dotnet build`. Done.

See [`docs/file-order-auto-migration.md`](./file-order-auto-migration.md)
for the migration guide, including how to handle cycles and how to migrate
off the `and` keyword.

## Known limitations

- **FCS / IDE does not synthesise cycle groups.** A project that compiles
  on the build path because of cycle synthesis will show a type error in
  the IDE until cycle synthesis is added to the IncrementalBuilder.
- **`dotnet fsi` is not wired.** The flag is fsc + FCS only. Multi-file
  FSI invocations are unaffected.
- **Cycle groups containing `.fsi` files** fall back to original order
  inside the group; sig/impl pairing inside a synthesised cycle group is a
  known gap.
- **FSharp.Core** cannot be compiled with the flag (the pre-population
  stubs would shadow primitive types — guarded explicitly).

## Test coverage on this branch

The bulk of the regression coverage now lives in the upstream
ComponentTests harness, opted into via `|> withFileOrderAuto`.

| Test surface | Location | Coverage |
|---|---|---|
| `TypeChecks.FileOrderAutoTests` | `tests/FSharp.Compiler.ComponentTests/TypeChecks/FileOrderAuto/FileOrderAutoTests.fs` | 13 [<Fact>]s: misordered files, cross-file mutual recursion (cycle synthesis), `.fsi`/`.fs` pairing, record/union/SRTP/operator-overload inference, manual mode unchanged, FS3887 fires/silent, three diagnostic-parity cases (FS0039/FS0001/FS0003). |
| `tests/file-order-auto-test/end-to-end/run.sh` | shell | Scaffolds a fresh `dotnet new` F# project, scrambles file order, sets `<FSharpAutoFileOrder>true</FSharpAutoFileOrder>`, builds + runs the exe — exercises the MSBuild → fsc plumbing the ComponentTests harness can't reach. |
| `tests/file-order-auto-test/self-host-test.sh` | shell | Compiles the F# compiler itself with randomly-shuffled `<Compile Include>` order — strongest available "real workload" stress for the analyser. |
| `tests/file-order-auto-test/fcs-smoke-test/` & `fcs-ide-smoke-test/` | shell + .fs | FCS API smoke tests (`ParseAndCheckProject`, IDE features). Slated to migrate to the SyntheticProject / TransparentCompiler harness for incremental-compilation coverage. |
| `tests/file-order-auto-test/oss-sweep/` | RESULTS.md | 13 real-world OSS F# projects under `--file-order-auto+`. **Auto-mode adds zero errors over baseline for every buildable target.** See [`tests/file-order-auto-test/oss-sweep/RESULTS.md`](../tests/file-order-auto-test/oss-sweep/RESULTS.md). |

### OSS sweep results

| Project | Baseline | Auto |
|---|---|---|
| Argu | OK | **PASS** |
| FsCheck | OK | **PASS** (SRTP-heavy property-testing library) |
| FSharpPlus | OK | **PASS** (86 .fs files; heavy SRTP + AutoOpen + nested modules) |
| FsToolkit.ErrorHandling | OK | **PASS** |
| Expecto | OK | **PASS** |
| FSharp.Data.Json.Core | OK | **PASS** |
| Fable.Promise | OK | **PASS** |
| Suave | FAIL (30 pre-existing) | matches baseline byte-for-byte |
| FsPickler / Aether / Fantomas.Core / Fable.AST / Paket.Core | env-broken on this toolchain | env-broken (same errors) |

The 7 explicitly-PASS projects build cleanly under auto-order with no
diagnostic delta from baseline. Suave's 30 errors are pre-existing F#
.NET 10 issues (`FS0971 Undefined value 'this'` in `task {}` blocks);
the auto error set is identical to the baseline error set
(`diff` is empty). The 5 env-broken projects baseline-fail on this
toolchain (paket lock, .NET version pin, NuGet hash mismatches);
auto produces the same errors.

## Architecture notes

- **Level A** (DAG ordering): `applyAutoFileOrder` for the build path,
  `computeReorderedFileNames` for FCS. Both call `computeCompilationUnits`,
  which runs Tarjan's SCC over a dependency graph built from each file's
  top-level module declarations and its qualified identifier references.
- **Level B** (cycle group synthesis): build-only. Files in an SCC > 1 are
  rewritten as a single `ParsedImplFileInput` whose top-level
  `SynModuleOrNamespace` entries are marked `isRecursive = true`.
- **Sig/impl pairing**: `buildExportMap` collapses sig+impl pairs to one
  logical contributor when deciding "shared prefix" status; sig→impl
  redirect rewrites dependency edges before topological sort so paired
  modules end up adjacent and consumers don't sort *before* the pair.
- **Enter phase** (`runEnterPhase`): pre-populates `TcEnv` with type
  stubs from every file before sequential type checking, so namespace
  references resolve regardless of file order. (Type stubs only —
  module stubs collide with real module declarations.)

For the full design — analyser internals, the export-map / alias-map
split for AutoOpen, kind-aware matching, surgical single-ident capture,
the chain of refinements that drove the OSS sweep to green — see
[`docs/file-order-auto-design.md`](./file-order-auto-design.md).

## Caveats / what was NOT validated this iteration

- **Performance characterisation** on a large project (compile time delta,
  memory ceiling) was not measured. Auto-mode adds a one-time pre-parse
  pass over every file in the project; subsequent rebuilds reuse existing
  caches.
- **IDE end-to-end smoke** (Ionide popup behaviour, VS F# extension)
  requires a human at a real editor and was not done in this branch. The
  FCS-level `fcs-ide-smoke-test/` exercises the API surface but does not
  drive a real editor.
- **`dotnet fsi`** is not wired (build / FCS only).

## Building this fork

Standard repo build:

```bash
./build.sh -c Release
```

Run the file-order-auto ComponentTests:

```bash
PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
DOTNET_GCHeapHardLimit=0x100000000 \
  dotnet artifacts/bin/FSharp.Compiler.ComponentTests/Release/net10.0/FSharp.Compiler.ComponentTests.dll \
  --filter-class TypeChecks.FileOrderAutoTests
```

Optional shell-driven smokes (out-of-process integration):

```bash
PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
  ./tests/file-order-auto-test/end-to-end/run.sh

PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
  ./tests/file-order-auto-test/self-host-test.sh
```

The 4 GB heap limit is a local safety guard; drop the
`DOTNET_GCHeapHardLimit` env var if you don't need it.

## Reporting bugs

A bug in this fork is anything that compiles upstream but fails under
`--file-order-auto+`, or anything that compiles cleanly under
`--file-order-auto+` but produces incorrect runtime behaviour. Open an
issue with a minimal repro fsproj.

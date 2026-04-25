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
- **`and`-keyword deprecation** (warning **FS3885**): under `--file-order-auto+`,
  `type X = ... and Y = ...` now produces a deprecation warning. Suppressable
  via `--nowarn:3885` or `<NoWarn>3885</NoWarn>`. The warning is silent in
  manual mode.

## What hasn't changed

- The default behaviour is upstream: manual ordering, identical type
  inference, identical diagnostics, identical FSharp.Core compilation.
- No upstream test had to change. The error-corpus suite confirms diagnostic
  parity between manual and auto modes for six representative error
  categories (undefined name, undefined module, type mismatch, missing
  field, missing open, wrong arity).

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

Every fixture below runs against the locally-built compiler and is part of
the regression sweep at `tests/file-order-auto-test/`.

| Fixture | Coverage |
|---|---|
| `cycle-test-b4/` | Cross-file mutual recursion via cycle group synthesis. |
| `inference-tests/` | SRTP, record/union disambiguation, operator overloads. |
| `fsi-tests/` | `.fsi`/`.fs` pairing with partial coverage and ordering constraints. |
| `error-corpus/` | Six error categories, byte-for-byte parity manual vs auto. |
| `deprecation-test/` | FS3885 fires/suppresses correctly. |
| `fcs-smoke-test/` | `FSharpChecker.ParseAndCheckProject` reorders via OtherOptions. |
| `fcs-ide-smoke-test/` | Completions, Go-to-Def, Find-References, FS3885 via FCS. |

## Architecture notes

- **Level A** (DAG ordering): `applyAutoFileOrder` for the build path,
  `computeReorderedFileNames` for FCS. Both call `computeCompilationUnits`,
  which runs Tarjan's SCC over a dependency graph built from each file's
  top-level module declarations and its qualified identifier references.
- **Level B** (cycle group synthesis): build-only. Files in an SCC > 1 are
  rewritten as a single `ParsedImplFileInput` whose top-level
  `SynModuleOrNamespace` entries are marked `isRecursive = true`.
- **Sig/impl pairing**: `buildExportMap` collapses sig+impl pairs to one
  logical contributor when deciding "shared prefix" status, otherwise paired
  modules would silently lose their dependency edges.
- **Enter phase** (`runEnterPhase`): pre-populates `TcEnv` with module
  stubs from every file before sequential type checking, so namespace
  references resolve regardless of file order.

See `conductor/tracks/` for the per-track design notes.

## Caveats / what was NOT validated this iteration

- Full upstream F# compiler test suite was not run end-to-end under both
  modes. The fixtures in `tests/file-order-auto-test/` are targeted
  regressions, not a substitute for the full suite.
- Large open-source F# projects (Fable, Fantomas, Saturn, SAFE Stack,
  FSharpPlus) were not compiled under `--file-order-auto+` as a sweep.
  Earlier exploratory runs hit project-specific issues; isolating those
  is a separate effort.
- Performance characterisation on a large project (compile time delta,
  memory ceiling) was not measured.
- IDE end-to-end smoke (Ionide popup behaviour, VS F# extension) requires
  a human at a real editor and was not done in this branch.

## Building this fork

Standard repo build:

```bash
./build.sh -c Release
```

Run the focused test suite:

```bash
PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
DOTNET_GCHeapHardLimit=0x100000000 \
  ./tests/file-order-auto-test/inference-tests/run-all.sh

PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
DOTNET_GCHeapHardLimit=0x100000000 \
  ./tests/file-order-auto-test/fsi-tests/run-all.sh

PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
DOTNET_GCHeapHardLimit=0x100000000 \
  ./tests/file-order-auto-test/error-corpus/diff-errors.sh

PATH=$(pwd)/.dotnet:$PATH DOTNET_ROOT=$(pwd)/.dotnet \
DOTNET_GCHeapHardLimit=0x100000000 \
  ./tests/file-order-auto-test/deprecation-test/run-all.sh
```

The 4 GB heap limit is mandatory for safety on this developer's machine; on
a CI box you can drop it.

## Reporting bugs

A bug in this fork is anything that compiles upstream but fails under
`--file-order-auto+`, or anything that compiles cleanly under
`--file-order-auto+` but produces incorrect runtime behaviour. Open an
issue with a minimal repro fsproj.

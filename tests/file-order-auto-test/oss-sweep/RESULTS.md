# Open-Source F# Project Sweep

Test of how `--file-order-auto+` behaves against real-world F# projects, run against this fork's fsc.

## How to reproduce

```bash
# 1. Clone targets
mkdir -p /tmp/fsharp-oss-sweep && cd /tmp/fsharp-oss-sweep
git clone --depth 1 https://github.com/fsprojects/Argu
git clone --depth 1 https://github.com/fsprojects/FSharpPlus
git clone --depth 1 https://github.com/fscheck/FsCheck

# 2. Build with the fork's fsc + --file-order-auto+
FSC=$(pwd)/../fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll  # adjust path
dotnet build <project>.fsproj -c Release \
    -p:DotnetFscCompilerPath="$FSC" \
    -p:OtherFlags="--file-order-auto+ --nowarn:3885"
```

`--nowarn:3885` is needed to bypass the `and`-keyword deprecation warning if
the project's `<TreatWarningsAsErrors>` is on.

## Results

| Project | Auto-order | Notes |
|---|---|---|
| **Argu** | **PASS** | ~30 .fs files, conventional library. |
| **FsCheck** | **PASS** | ~26 .fs files, SRTP-heavy property-testing library with cross-namespace structure. |
| **FSharpPlus** | **PASS** | 86 .fs files, heavy SRTP + AutoOpen + nested modules across `FSharpPlus.Control`, `FSharpPlus.Math`, etc. |

All three target projects build cleanly under `--file-order-auto+`.

## How the analyser handles real F#

The path from "broken on real code" to "works" required a sequence of
analyser refinements layered on top of the original Track 01-04 design:

### Capture full identifier paths from the AST

`FileContentMapping.PrefixedIdentifier` (upstream) drops the trailing
segment of every long ident — fine for upstream's parallel checker, fatal
for ours because it makes `Random.CreateWithSeedAndGamma` (project type's
static method) and `Result.isOk` (FSharp.Core method) indistinguishable.
`SymbolCollection.collectFullPathRefs` walks the AST keeping each
LongIdent's full path.

### Register members + values in the export map

`TypeDeclStub.MemberNames` carries the names of static and instance
members declared on each type. `buildExportMap` registers them as
`qualName.TypeName.MemberName`, plus module-level let bindings as
`qualName.bindingName`. `Random.CreateWithSeedAndGamma` then resolves to
a real entry in the map and links to the right file.

### Kind-aware matching with bare-Type rejection

A new `ExportKind` (Module, Type, Value, Member) distinguishes what each
entry is. When prefix-iterating to find a cross-file dep, a Module match
counts (legitimate qualifier); a bare-Type match is rejected unless the
trailing path matches a registered Member. This kills the
`Random.X` / `Result.X` collision.

### Opens skip shared prefixes

`open FsCheck` from a file already inside `namespace FsCheck` was adding
every contributor to the namespace as a dep, manufacturing a giant SCC.
Opens declare scope; identifier refs declare specific cross-file deps.
Opens skip shared-prefix matches.

### Opens-as-prefixes (with local-name shadowing)

For each `open Foo.Bar` whose target is a known module, `Foo.Bar` becomes
an additional resolution prefix for ident refs in the file. Lets
`TypeClass.TypeClass<...>` from a file with `open FsCheck.Internals`
resolve to `FsCheck.Internals.TypeClass.TypeClass`. **But** suppressed
when the ref's first segment is locally defined — `Prop.X` from inside
Testable.fs (which has `open FsCheck.FSharp` AND a local `module Prop`)
refers to the local one.

### NamedModule vs DeclaredNamespace prefix scoping

NamedModule files (`module X.Y`) implicitly see siblings of their parent
namespace, so all enclosing prefixes are tried. DeclaredNamespace files
(`namespace X.Y`) don't — only the file's own namespace is used,
preventing `Result.isOk` in `namespace FsCheck.Internals` from falsely
matching `FsCheck.Result` via the parent prefix.

### Type stubs include type parameters

`mkTypeEntityStub` was creating empty `Typars=[]` for every type, making
forward refs to `MyType<'A>` fail with FS0033. Now we synthesise rigid
typars matching `TypeParamCount`.

### No module stubs, only type stubs

The biggest single fix. The enter phase used to create
ModuleOrNamespace stubs for every module. F# saw the stub as an
already-declared entity and rejected the real `module X = ...` with
FS0245 "X is not a concrete module or type". We now skip module stubs
entirely (private/internal modules and types are also filtered out as
unreachable from other files anyway).

### Cross-namespace cycle synthesis guard

When a cycle group spans multiple namespaces, synthesis would wrap a
`namespace X.Y` file as a nested `module Y` inside `namespace rec X`.
The original `namespace X.Y` declaration would then conflict (FS0247
"namespace and module both occur"). Now we detect this case and fall
back to original order rather than synthesise.

### Hoist opens recursively in synthesised cycle groups

F#'s `namespace rec` requires `open` decls to be first in each
module/namespace body. Real F# code interleaves opens with let bindings
in nested modules — legal in non-recursive code, FS3200 in recursive.
Synthesis now recursively reorders each module body so opens come first.

## Regression sweep

All existing fixtures still pass after each change:

- `inference-tests`: 4/4 (SRTP, record/union disambiguation, operator overloads).
- `fsi-tests`: 2/2 (partial-fsi, fsi-ordering with sig+impl pairs).
- `error-corpus`: 6/6 byte-for-byte identical between manual and auto modes.
- `deprecation-test`: 3/3 (FS3885 fires/silent/suppressable).
- `end-to-end`: PASS (scaffold-and-build-fresh-project).
- `fcs-smoke-test`: PASS (FSharpChecker.ParseAndCheckProject).
- `fcs-ide-smoke-test`: 7/7 (Completions, Go-to-Def, Find References, FS3885).
- `cycle-test-b4`: PASS (cross-file mutual recursion via cycle synthesis).

## Commit history of the OSS unblock work

- `9901547fe` — Phase 1: custom AST walker for full-path identifier refs.
- `27083004e` — Phase 2-4: type member registration, kind-aware matching,
  opens skip shared.
- `6117008f1` — Typar stubs, cross-namespace cycle guard, opens-as-prefixes
  with local-name shadowing.
- `ae9beb404` — Stop stubbing modules — type stubs only. **FsCheck builds.**
- `49a380e7a` — Hoist opens recursively when synthesising cycle groups.
  **FSharpPlus builds.**

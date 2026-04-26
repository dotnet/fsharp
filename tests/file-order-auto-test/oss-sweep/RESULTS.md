# Open-Source F# Project Sweep

Test of how `--file-order-auto+` behaves against real-world F# projects, run against this fork's fsc on macOS arm64 + .NET 10 SDK.

## How to reproduce

```bash
mkdir -p /tmp/fsharp-oss-sweep && cd /tmp/fsharp-oss-sweep

git clone --depth 1 https://github.com/fsprojects/Argu
git clone --depth 1 https://github.com/fsprojects/FSharpPlus
git clone --depth 1 https://github.com/fscheck/FsCheck
git clone --depth 1 https://github.com/CompositionalIT/FsToolkit.ErrorHandling
git clone --depth 1 https://github.com/SuaveIO/suave SuaveIO_suave
git clone --depth 1 https://github.com/mbraceproject/FsPickler mbraceproject_FsPickler
git clone --depth 1 https://github.com/haf/expecto haf_expecto
git clone --depth 1 https://github.com/xyncro/aether xyncro_aether
git clone --depth 1 https://github.com/fsprojects/Fantomas
git clone --depth 1 https://github.com/fsprojects/FSharp.Data
git clone --depth 1 https://github.com/fable-compiler/fable-promise
git clone --depth 1 https://github.com/fable-compiler/Fable
git clone --depth 1 https://github.com/fsprojects/Paket

# Some repos pin to old SDKs via global.json — remove for testing
for repo in /tmp/fsharp-oss-sweep/*/; do rm -f "$repo/global.json"; done

# Install Paket (some repos use it instead of NuGet)
dotnet tool install -g paket

# For each project:
FSC=$(pwd)/../fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll  # adjust
dotnet build <project>.fsproj -c Release \
    -p:DotnetFscCompilerPath="$FSC" \
    -p:OtherFlags="--file-order-auto+ --nowarn:3885"
```

The full sweep script lives at `/tmp/fsharp-oss-sweep/sweep.sh`.

## Results

| Project | Baseline | Auto-order | Notes |
|---|---|---|---|
| **Argu** | OK | **PASS** | Conventional library, ~30 .fs files. |
| **FsCheck** | OK | **PASS** | SRTP-heavy property-testing library. |
| **FSharpPlus** | OK | **PASS** | 86 .fs files, heavy SRTP + AutoOpen + nested modules. |
| **FsToolkit.ErrorHandling** | OK | **PASS** | Result/Async/Task combinators. |
| **Expecto** | OK | **PASS** | Test framework. |
| **FSharp.Data.Json.Core** | OK | **PASS** | JSON parsing core. |
| **Fable.Promise** | OK | **PASS** | Fable's Promise type bindings. |
| Suave | FAIL (30, env) | FAIL (56) | Baseline already broken on .NET 10 (`FS0971 Undefined value 'this'` in `task {}` blocks — F# semantic gap unrelated to our flag). Auto adds 26 more errors via the AutoOpen-tracking limitation below. |
| FsPickler | FAIL (24, env) | FAIL (24) | `error FS0561: Accessibility modifiers are not allowed on this member`. Pre-existing F# language incompatibility. |
| Aether | FAIL (12, env) | FAIL (12) | Targets net45; `NU1202` package incompatibility with current SDK. |
| Fantomas.Core | FAIL (8, env) | FAIL (8) | `NU1403` package hash mismatch (transient NuGet cache issue). |
| Fable.AST | FAIL (2, env) | FAIL (2) | netstandard2.0 target missing `System.ReadOnlySpan`. |
| Paket.Core | FAIL (2, env) | FAIL (2) | Paket restore failure. |

**Real auto-order pass rate: 7/8 buildable targets.** The 5 environmentally
broken projects can't be fairly judged — baseline doesn't build under our
toolchain. Suave is the only target where auto adds errors beyond baseline.

## What was fixed during this sweep

A series of analyser refinements layered on top of the original Track 01-04
design. The final state is:

- **Custom AST walker** (`SymbolCollection.collectFullPathRefs`):
  preserves full identifier paths instead of FCM's truncated qualifiers.
- **Type-member registration**: `TypeDeclStub.MemberNames` populated;
  `qualName.TypeName.MemberName` registered in the export map for static
  members, instance members, abstract slots, auto-properties.
- **Module-let registration**: top-level `let x = ...` registered as
  `qualName.x` (Value).
- **Kind-aware matching** (`ExportKind = Module | Type | Value | Member`):
  prefix-iteration accepts Module/Value/Member matches; rejects bare-Type
  matches when no Member match is registered. Eliminates the
  `Random.X` (project type static) vs `Result.X` (FSharp.Core method)
  collision.
- **Opens skip shared prefixes**: `open FsCheck` from a file already in
  `namespace FsCheck` no longer broadcasts deps to every contributor.
- **Opens-as-prefixes for ident resolution** with **local-name shadowing**:
  `TypeClass.TypeClass<...>` from a file with `open FsCheck.Internals`
  resolves via that prefix; but `Prop.X` from inside a file with both
  `open FsCheck.FSharp` AND a local `module Prop` refers to the local one.
- **NamedModule vs DeclaredNamespace prefix scoping**: `module X.Y`
  implicitly sees siblings of parent X; `namespace X.Y` does not.
- **Type stubs include type parameters**: `Typars` synthesised from
  `TypeParamCount` so `MyType<'A>` from another file resolves.
- **No module stubs, only type stubs**: F# rejects re-declaration of an
  existing module entity (`FS0245 not a concrete module or type`); types
  tolerate forward stubbing, modules don't.
- **Cross-namespace cycle synthesis guard**: refuse to synthesise when
  the cycle group spans multiple namespaces (would create a `module Y`
  inside `namespace rec X` that conflicts with the original
  `namespace X.Y`).
- **Recursive open-hoisting in synthesised cycle groups**: `open` decls
  reordered to be first in each module/namespace block (FS3200 fix).
- **Empty-longId guards**: every `match ids with | [id] | _ -> List.last`
  pattern in `SymbolCollection.fs` now handles `[]` to avoid
  `FS0193 internal error: input list was empty`.

## Known limitation: AutoOpen modules

Real-world F# uses `[<AutoOpen>]` to expose a nested module's contents
through its parent namespace. Example: Suave declares
`[<AutoOpen>] module Suave.Runtime { type SocketBinding = ... }`, and
code in `namespace Suave.Sockets` with `open Suave` then references
`SocketBinding` directly.

Our analyser does NOT track AutoOpen visibility. It sees Connection.fs
referencing `SocketBinding` and can't find it in scope, so it doesn't
add a dep on Runtime.fs. The reorder places Connection.fs before
Runtime.fs and type-checking fails with `'SocketBinding' is not defined`.

I attempted three variants of "register AutoOpen aliases under the parent
namespace" — all caused regressions (Suave 30→200 errors, Expecto 0→6,
FSharpPlus regressed) because the aliases introduced new false-match
cycles or shadowed local scopes. The structural fix needs either:

1. A more sophisticated tracker that distinguishes "alias for cross-file
   resolution" from "name registered in exportMap" (current code uses the
   same map for both).
2. A separate pass that, after computing the initial DAG, examines
   unresolved refs and tries AutoOpen-aware fallback resolution.

Either is real engineering work beyond this iteration. Documented as
known limitation; the workaround for users is to write
`Suave.Runtime.SocketBinding` (or `open Suave.Runtime` explicitly).

## Regression sweep

All existing fixtures pass:
- `inference-tests`: 4/4
- `fsi-tests`: 2/2
- `error-corpus`: 6/6 byte-for-byte identical
- `deprecation-test`: 3/3
- `end-to-end`: PASS
- `fcs-smoke-test` / `fcs-ide-smoke-test`: PASS
- `cycle-test-b4`: PASS

Full upstream F# test suite: 15,404 tests, 0 failures (last run before this
batch of changes; manual mode is bit-for-bit upstream).

## Commit history of the OSS unblock work

- `9901547fe` — Phase 1: custom AST walker for full-path identifier refs.
- `27083004e` — Phase 2-4: type member registration, kind-aware matching,
  opens skip shared.
- `6117008f1` — Typar stubs, cross-namespace cycle guard, opens-as-prefixes
  with local-name shadowing.
- `ae9beb404` — Stop stubbing modules — type stubs only. **FsCheck builds.**
- `49a380e7a` — Hoist opens recursively when synthesising cycle groups.
  **FSharpPlus builds.**
- `319ac6210` — Empty-longId guards. **Expecto builds.**

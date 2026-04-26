# Open-Source F# Project Sweep

`--file-order-auto+` against real-world F# projects. macOS arm64, .NET 10 SDK.

## Results

**Auto-mode adds zero errors over baseline for every buildable target.**

| Project | Baseline | Auto-order | Notes |
|---|---|---|---|
| **Argu** | OK | **PASS** | Conventional library, ~30 .fs files. |
| **FsCheck** | OK | **PASS** | SRTP-heavy property-testing library. |
| **FSharpPlus** | OK | **PASS** | 86 .fs files. Heavy SRTP + AutoOpen + nested modules across `FSharpPlus.Control`, `FSharpPlus.Math`, etc. |
| **FsToolkit.ErrorHandling** | OK | **PASS** | Result/Async/Task combinators. |
| **Expecto** | OK | **PASS** | Test framework. |
| **FSharp.Data.Json.Core** | OK | **PASS** | JSON parsing core. |
| **Fable.Promise** | OK | **PASS** | Fable's Promise type bindings. |
| **Suave** | FAIL (30) | FAIL (30) | All 30 errors are pre-existing baseline failures (`FS0971 Undefined value 'this'` in `task {}` blocks — F# .NET 10 semantic gap unrelated to our flag). `diff` of baseline vs auto error sets is empty. **Auto adds zero errors.** |
| FsPickler | FAIL (24, env) | FAIL (24) | `error FS0561` — pre-existing F# language incompatibility. |
| Aether | FAIL (12, env) | FAIL (12) | Targets net45; `NU1202` package incompatibility. |
| Fantomas.Core | FAIL (2, env) | FAIL (2) | NuGet hash mismatch (transient). |
| Fable.AST | FAIL (2, env) | FAIL (2) | netstandard2.0 missing `System.ReadOnlySpan`. |
| Paket.Core | FAIL (2, env) | FAIL (2) | Paket restore fails. |

**8/8 buildable targets — auto matches baseline exactly.** The 5 env-broken projects baseline-fail on this toolchain; auto produces the same errors.

## Reproduction

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

## What was needed to make this work

A long sequence of analyser refinements layered on the original Track 01-04
design. The chain that took us from "FsCheck/FSharpPlus fail with cycle
errors" to "every buildable target matches baseline":

- **Custom AST walker** (`SymbolCollection.collectFullPathRefs`):
  preserves full identifier paths instead of FCM's truncated qualifiers.
- **Type-member registration**: `TypeDeclStub.MemberNames` populated;
  `qualName.TypeName.MemberName` registered for static members,
  instance members, abstract slots, auto-properties.
- **Module-let registration**: top-level `let x = ...` registered as
  `qualName.x` (Value).
- **Kind-aware matching** (`ExportKind = Module | Type | Value | Member`):
  prefix-iteration accepts Module/Value/Member; rejects bare-Type
  matches when no Member is registered. Eliminates the `Random.X` (project
  type static) vs `Result.X` (FSharp.Core method) collision.
- **Opens skip shared prefixes**: `open FsCheck` from a file already in
  `namespace FsCheck` no longer broadcasts deps to every contributor.
- **Opens-as-prefixes for ident resolution** with **local-name shadowing**:
  `TypeClass.TypeClass<...>` from a file with `open FsCheck.Internals`
  resolves via that prefix; `Prop.X` from inside a file with both
  `open FsCheck.FSharp` AND a local `module Prop` refers to the local one.
- **NamedModule vs DeclaredNamespace prefix scoping**: `module X.Y`
  implicitly sees siblings of parent X; `namespace X.Y` does not.
- **Type stubs include type parameters**: `Typars` synthesised from
  `TypeParamCount` so `MyType<'A>` from another file resolves.
- **No module stubs, only type stubs**: F# rejects re-declaration of an
  existing module entity (`FS0245`); types tolerate forward stubbing.
- **Cross-namespace cycle synthesis guard**: refuse to synthesise when
  the cycle group spans multiple namespaces (would create a `module Y`
  inside `namespace rec X` that conflicts with the original
  `namespace X.Y` → FS0247).
- **Recursive open-hoisting in synthesised cycle groups**: `open` decls
  reordered to be first in each module/namespace block (FS3200 fix).
- **Empty-longId guards**: every `match ids with` pattern now handles
  `[]` to avoid `FS0193 internal error: input list was empty`.
- **Separate `aliasMap` for AutoOpen** (final missing piece): when a
  module is `[<AutoOpen>]`, its content is registered as resolution
  shortcut in `aliasMap` (consulted only as fallback in
  `addDepFromExportMap`). NEVER mixed into the main exportMap, so
  aliases can't trigger false sharedPrefix/cycle matches.
- **Sig→impl redirect**: refs to `.fsi` files are redirected to their
  paired `.fs` impls before topological sort, so the pair-rewriting
  step preserves consumer ordering.
- **Surgical single-ident capture**: `SynExpr.App(funcExpr=SynExpr.Ident)`
  captures the function ident as a 1-segment ref so `transferStream conn`
  can resolve via `open Suave.Sockets` AutoOpen alias. Capturing every
  `SynExpr.Ident` broke FsToolkit by matching local parameters; the
  surgical version targets only function-application heads.

## Regression sweep

All existing fixtures pass after each change:
- `inference-tests`: 4/4
- `fsi-tests`: 2/2
- `error-corpus`: 6/6 byte-for-byte identical
- `deprecation-test`: 3/3
- `end-to-end`: PASS
- `fcs-smoke-test` / `fcs-ide-smoke-test`: PASS
- `cycle-test-b4`: PASS

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
- `2d703ba69` — Separate aliasMap for AutoOpen, sig→impl redirect,
  surgical single-ident capture. **Suave matches baseline.**

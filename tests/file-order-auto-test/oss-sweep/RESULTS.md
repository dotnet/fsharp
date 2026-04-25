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

## Current results

| Project | Auto-order | Notes |
|---|---|---|
| **Argu** | **OK** | ~30 .fs files, real-world success on a non-trivial library. |
| FsCheck | FAIL (136 errors) | Cycle problem ELIMINATED. Remaining errors are real type-resolution issues — legitimate cross-file deps via `open`d namespaces are missed. See "Remaining work" below. |
| FSharpPlus | FAIL (2 internal compiler errors) | Cycle problem ELIMINATED. New failure: `FS0193 ... Key: 'Control'` from our enter-phase stub building when FSharpPlus's many `namespace FSharpPlus.Control` files create a conflict. Bug in our stub building, not the analyzer. |
| Saturn | n/a | Uses Paket (not installed); environmental skip. |

## What was fixed (commits `9901547fe` and `27083004e`)

1. **Custom AST walker** (`collectFullPathRefs`) replaces FCM-based ident
   collection so `Random.CreateWithSeedAndGamma` is captured as a 2-segment
   path instead of being truncated to single-segment `["Random"]`.
2. **Type member registration**: `TypeDeclStub` now carries `MemberNames`,
   and `buildExportMap` registers each as `qualName.TypeName.MemberName`.
   Module-level let bindings are also registered as `qualName.bindingName`.
3. **Kind-aware matching**: a new `ExportKind` (Module/Type/Value/Member)
   tracks what each export-map entry is. The matching policy walks prefixes
   longest-first; bare-Type prefix matches are rejected when no Member match
   is registered. This kills the `Random.CreateWithSeedAndGamma` ↔
   `Result.isOk` collision.
4. **Opens skip shared prefixes**: `open FsCheck` from a file already in
   `namespace FsCheck` no longer broadcasts deps to every file in that
   namespace. Opens declare scope, identifier refs declare cross-file deps.

The combined effect: file-order analysis is no longer producing false cycles
in the projects we've tested. FsCheck and FSharpPlus's failures are now
*beyond* the analyzer — real ordering gaps and a bug in stub building.

## Remaining work

### FsCheck — 136 type-resolution errors

The analyser correctly identifies a DAG. But some legitimate cross-file
deps via `open`d namespaces aren't detected. Example: `ArbMap.fs` opens
`FsCheck.Internals` and uses `TypeClass.TypeClass<...>`. The full path is
`["TypeClass"; "TypeClass"]`. We try to resolve via the file's own
namespace prefix `[FsCheck]`, getting `FsCheck.TypeClass` — no match
(TypeClass is in `FsCheck.Internals`). We don't try the open's namespace
`[FsCheck; Internals]` as a prefix.

A first attempt at "use open paths as resolution prefixes" regressed FsCheck
to 200 errors (different kind: `FS0247` namespace-vs-module collisions),
suggesting the broadcast was over-eager. The right rule is something like
"use open paths only when the open target is non-shared." Bounded follow-up
work; not landed in this iteration.

### FSharpPlus — 2 internal compiler errors

`FS0193 ... An element with the same key but a different value already
exists. Key: 'Control'`. FSharpPlus has 60+ files contributing to
`namespace FSharpPlus.Control`. Our enter phase synthesises stubs per file
and adds them via `AddLocalRootModuleOrNamespace`. With the volume of files
contributing to the same namespace, the underlying F# entity-store rejects
a stub addition.

This is a bug in `buildFileStub`/`runEnterPhase`'s interaction with F#'s
`AddLocalRootModuleOrNamespace`, not the dependency analyzer. Likely fix:
collapse stubs across all files that share a namespace before folding into
TcEnv. Substantial change to enter-phase semantics; not landed in this
iteration.

## Recommendation

Argu builds cleanly under `--file-order-auto+`. The cycle-detection bug
that blocked FsCheck/FSharpPlus is gone. The remaining failure modes are
real but bounded — neither is a fundamental design limitation. For a v1
PR this is a good-faith honest state: the feature works on conventional
libraries, real obstacles for SRTP-heavy and namespace-fragmented projects
are documented with concrete next steps.

## Commit history of the unblock work

- `9901547fe` — Phase 1: custom AST walker for full-path identifier refs.
- `27083004e` — Phase 2-4: type member registration, kind-aware matching,
  opens-skip-shared.

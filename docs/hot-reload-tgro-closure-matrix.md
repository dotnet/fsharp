# Hot Reload: T-Gro Feedback Closure Matrix

Last updated: 2026-03-02
Source comments: NatElkins/fsharp#1 (T-Gro top-level review comments, 2026-02-20)

## Goal

Track each major review concern with objective status and evidence so follow-up work is explicit and review risk remains scoped.

## Status legend

- Addressed: implemented and guarded by tests/scripts.
- Partially addressed: meaningful progress, but boundary/risk item still open.
- Open: design/implementation work still required.

## Matrix

### 1) Plugin boundary / layering safety-first

- Status: **Addressed**
- Evidence:
  - `fsc` emit path routes through a generic emit hook abstraction rather than direct hot reload APIs: `src/Compiler/Driver/fsc.fs`.
  - Hot reload hook bootstrap remains explicit-only (`--enable:hotreloaddeltas`) and wires hook behavior per compilation invocation: `src/Compiler/Driver/CompilerEmitHookBootstrap.fs`.
  - Ambient compiler emit-hook mutation has been removed; hook resolution is now explicit-config-only with no process-wide mutable fallback: `src/Compiler/Driver/CompilerEmitHookState.fs`.
  - Hot reload service no longer mutates compiler-wide hook state during session start/end: `src/Compiler/Service/service.fs`.
- Checker compile now injects explicit hook-only enablement (`--enable:hotreloadhook`) while a session is active, preserving synthesized-name replay without ambient mutable hooks: `src/Compiler/Service/service.fs`, `src/Compiler/Driver/CompilerOptions.fs`.
  - `fsc` still does not import hot reload implementation modules directly and resolves hooks through the bootstrap boundary adapter: `src/Compiler/Driver/fsc.fs`, `src/Compiler/Driver/CompilerEmitHookBootstrap.fs`.
  - Architecture guards enforce explicit-only/no-ambient wiring boundaries: `tests/FSharp.Compiler.Service.Tests/HotReload/ArchitectureGuardTests.fs`.
  - Output parity regression proves non-hot-reload artifacts stay unchanged when the flag is toggled: `tests/FSharp.Compiler.Service.Tests/HotReload/HotReloadCheckerTests.fs` (`Compiler outputs stay byte-identical when hot reload capture flag is toggled`).

### 2) Remove IlxGen-specific hot reload naming hook drift

- Status: **Addressed**
- Evidence:
  - `hotReloadIlxName` removed; centralized naming wrappers now enforce one path in `IlxGen`.
  - Naming-path guard script enforces wrapper-only direct generator access: `tests/scripts/check-ilxgen-name-path.sh`.

### 3) Extract checker-owned hot reload state

- Status: **Addressed**
- Evidence:
  - `FSharpHotReloadService` owns session orchestration and state transitions; checker delegates through thin APIs: `src/Compiler/Service/service.fs`.

### 4) Keep normal compilation naming semantics upstream-equivalent when hot reload is off

- Status: **Addressed**
- Evidence:
  - `CompilerGlobalState` non-map path uses file-index + start-line + 1-based increment semantics: `src/Compiler/TypedTree/CompilerGlobalState.fs`.

### 5) opDigest wildcard catch-all silent-risk

- Status: **Addressed**
- Evidence:
  - `opDigest` is wildcard-free.
  - Guard test enforces no `| _ ->` in `opDigest`: `tests/FSharp.Compiler.Service.Tests/HotReload/ArchitectureGuardTests.fs`.

### 6) State-machine/query string heuristics

- Status: **Addressed**
- Evidence:
  - Declaring-type string heuristic removed.
  - Operation-name list heuristics were removed from lowered-shape collection/classification (`isLikelyQueryOperationName` / `isLikelyStateMachineOperationName` no longer exist): `src/Compiler/TypedTree/TypedTreeDiff.fs`.
  - Lowered-shape digests are now structural-only (`formatLoweredShapeDigest` emits `struct=[...]`), and synthesized classification uses structural evidence plus the explicit `MoveNext` sentinel: `src/Compiler/TypedTree/TypedTreeDiff.fs`.
  - Structural trait-call fingerprints (`traitConstraintShapeDigest`) remain in `TraitCall`/`WitnessArg` paths, preserving query-lowering evidence without name-list matching: `src/Compiler/TypedTree/TypedTreeDiff.fs`.
  - Architecture guards now enforce structural-only lowered-shape classification and explicit absence of operation-name heuristics: `tests/FSharp.Compiler.Service.Tests/HotReload/ArchitectureGuardTests.fs`.
  - Service regressions verify query-like/state-machine-like member names without lowered rewrites do not emit query/state-machine rude edits: `tests/FSharp.Compiler.Service.Tests/HotReload/TypedTreeDiffTests.fs`.
  - Existing async/query lowered-shape edits are now explicitly locked to structural-only fallback (`LambdaShapeChange`) when no dedicated query/state structural marker is present, so classification no longer depends on operation-name lists: `tests/FSharp.Compiler.Service.Tests/HotReload/TypedTreeDiffTests.fs`.

### 7) String-based symbol identity chain

- Status: **Addressed**
- Evidence:
  - `TypedTreeDiff.SymbolId` now transports typed runtime signature identity (`RuntimeTypeIdentity`) for method parameters/return values instead of string signatures: `src/Compiler/TypedTree/TypedTreeDiff.fs`, `src/Compiler/TypedTree/TypedTreeDiff.fsi`.
  - Typed-tree signature encoding now includes void/array/byref/native pointer identities and method generic type-variable ordinals, keeping symbol-side signatures structurally comparable to emitted IL signatures: `src/Compiler/TypedTree/TypedTreeDiff.fs`.
  - `DeltaBuilder` now converts baseline `ILType`/`ILTypeSpec` signatures into the same typed `RuntimeTypeIdentity` model and performs structural identity matching in both pre-index and fallback disambiguation paths: `src/Compiler/HotReload/DeltaBuilder.fs`.
  - Existing fail-closed behavior is preserved: incomplete/ambiguous runtime method identity still returns full-rebuild diagnostics rather than permissive token binding: `src/Compiler/HotReload/DeltaBuilder.fs`.
  - Regression coverage updated to validate typed method-signature identity mapping and mismatch fail-closed behavior under the new typed identity path: `tests/FSharp.Compiler.Service.Tests/HotReload/DeltaBuilderTests.fs`.


### 8) Manual metadata serialization evolution risk

- Status: **Addressed**
- Evidence:
  - Delta metadata emission now supports a parallel `System.Reflection.Metadata` writer path that consumes the same row model as the hand-rolled serializer (`DeltaMetadataSrmWriter`) so preview runs can exercise both implementations without perturbing the default writer: `src/Compiler/CodeGen/DeltaMetadataSrmWriter.fs`, `src/Compiler/CodeGen/FSharpDeltaMetadataWriter.fs`.
  - `FSharpDeltaMetadataWriter` now supports strict SRM shadow parity checks (`FSHARP_HOTRELOAD_COMPARE_SRM_METADATA=1`) and optional SRM output mode (`FSHARP_HOTRELOAD_USE_SRM_TABLES=1`), with fail-fast structural diagnostics over tracked table row-counts plus `EncLog`/`EncMap` entries so table-shape drift cannot hide: `src/Compiler/CodeGen/FSharpDeltaMetadataWriter.fs`.
  - Automated parity gate now executes with SRM shadow comparison enabled before mdv component validation: `tests/scripts/check-hotreload-metadata-parity.sh`, `tests/FSharp.Compiler.Service.Tests/HotReload/SrmParityTests.fs`, `tests/FSharp.Compiler.ComponentTests/HotReload/MdvValidationTests.fs`.
  - Existing serializer hardening remains in place (heap-offset validation + malformed index tests): `src/Compiler/CodeGen/DeltaMetadataSerializer.fs`, `tests/FSharp.Compiler.Service.Tests/HotReload/FSharpDeltaMetadataWriterTests.fs`.

### 9) Large `IlxDeltaEmitter` single-function blast radius

- Status: **Addressed**
- Evidence:
  - `emitDelta` now routes metadata row assembly through explicit helper phases (`buildMethodAndParameterRows`, `buildPropertyEventAndSemanticsRows`, `buildCustomAttributeRows`).
  - Final payload assembly (`added/changed method projection`, `PDB delta`, `baseline apply`) now runs through dedicated `finalizeDeltaArtifacts` helpers (`buildAddedOrChangedMethods`, `buildDeltaToUpdatedMethodTokenMap`) instead of inline logic.
  - Metadata reference remapping (`TypeRef`, `MemberRef`, `MethodSpec`, `AssemblyRef`, entity-token dispatch) is extracted into `createMetadataReferenceRemapper`: `src/Compiler/CodeGen/IlxDeltaEmitter.fs`.
  - Definition-token remapping is extracted into `createDefinitionTokenRemapper` and consumed separately for definition/association resolution (`Property`/`Event`) so metadata-reference remap flow no longer carries definition-map dictionaries: `src/Compiler/CodeGen/IlxDeltaEmitter.fs`.
  - Architecture guards now enforce both explicit emitter phases and remapper separation (`MetadataReferenceRemapContext` stays reference-focused while emit flow wires both remappers explicitly): `tests/FSharp.Compiler.Service.Tests/HotReload/ArchitectureGuardTests.fs`.


### 10) HR files in core directories

- Status: **Addressed**
- Evidence:
  - Hot reload namespaced modules live under `src/Compiler/HotReload/` (e.g., `DefinitionMap.fs`, `FSharpSymbolChanges.fs`).

### 11) `isEnvVarTruthy` duplication

- Status: **Addressed**
- Evidence:
  - Shared helper used from `Utilities/EnvironmentHelpers.fs`.

### 12) ApplyUpdate setup duplication

- Status: **Addressed**
- Evidence:
  - Shared test helper extracted in `tests/FSharp.Compiler.ComponentTests/HotReload/ApplyUpdateShared.fs`.

### 13) Construct coverage breadth (Tier1/Tier2)

- Status: **Addressed (baseline matrix added)**
- Evidence:
  - Runtime integration construct matrix tests cover Tier1 and Tier2 edit/apply scenarios: `tests/FSharp.Compiler.ComponentTests/HotReload/RuntimeIntegrationTests.fs`.

### 14) Maintain `.fsi` stability relative to `main`

- Status: **Partially addressed**
- Evidence:
  - Guard now enforces allowlist + mandatory hash-locking for every drifted `.fsi`: `tests/scripts/check-main-fsi-drift.sh`.
  - Refresh helper added: `tests/scripts/refresh-main-fsi-drift-hashes.sh`.
  - Reduced one main-relative signature drift by localizing hot-reload activity tag literals in `EditAndContinueLanguageService` and removing `Activity.fsi` from the allowlisted drift set (`10 -> 9` files).
  - Removed hot-reload-specific `FSharpCheckProjectResults` signature exposure (`TypedImplementationFiles`, `HotReloadOptimizationData`) and switched service retrieval to non-public reflection so this branch no longer grows explicit hot-reload API surface in `FSharpCheckerResults.fsi`.
  - Removed stale `FSharpCheckerResults.fsi` entries from the main-relative `.fsi` drift allowlist/hash lock once the file returned to parity with `origin/main`, reducing tracked drift surface to 8 files.
- Remaining gap:
  - The allowlisted drift set is still non-trivial and should be reduced through targeted refactors.

## Validation performed for this update

- `./.dotnet/dotnet build FSharp.sln -c Debug -v minimal`
- `./.dotnet/dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj -c Debug --no-build --filter FullyQualifiedName~HotReload -v minimal` (`328` passed)
- `./.dotnet/dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj -c Debug --no-build --filter FullyQualifiedName~HotReload -v minimal` (`110` passed)
- `./tests/scripts/check-hotreload-metadata-parity.sh`

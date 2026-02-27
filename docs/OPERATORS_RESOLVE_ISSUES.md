# RFC FS-1043 — Resolve Issues Before PR

> **Branch**: `feature-operators-extensions` (57 commits + 1 merge with `origin/main`)
> **Feature**: Extension members become available to solve operator trait constraints
> **Feature flag**: `LanguageFeature.ExtensionConstraintSolutions` → `--langversion:preview`
> **Build status**: ✅ Compiler builds. All 283 SRTP tests pass. All 6 CompilerCompat cross-version tests pass (pre-merge).

---

## What This Branch Does

Implements [RFC FS-1043](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md):
extension methods now participate in SRTP constraint resolution. Also adds
`[<AllowOverloadOnReturnType>]` and suppresses weak resolution for inline code
when extension candidates are in scope.

### RFC documents on this branch

| Document | Purpose |
|----------|---------|
| `docs/RFC_Original.md` | Original RFC text for diffing |
| `docs/RFC_Changes.md` | Proposed additions to the RFC |
| `docs/EXTENSIONS.md` | Work items with verification steps |
| `docs/OPERATORS.md` | Branch orientation, architecture, TODO list |
| `docs/srtp-guide.md` | User-facing guide to writing SRTP code |
| `docs/release-notes/.Language/preview.md` | Release notes |

### Key implementation files

| File | Role |
|------|------|
| `TypedTree.fs/fsi` | `ITraitContext` interfaces, `TTrait` extended with `traitCtxt` field |
| `CheckBasics.fs/fsi` | `TcEnv` implements `ITraitContext`, `TraitContext` property |
| `ConstraintSolver.fs/fsi` | `GetRelevantMethodsForTrait`, `CanonicalizePartialInferenceProblemForExtensions`, `CreateImplFileTraitContext`, `AllowOverloadOnReturnType` in `ResolveOverloading` |
| `CheckExpressions.fs` | FS1215 gating, canonicalization branch, trait context threading |
| `NameResolution.fs` | `SelectExtensionMethInfosForTrait`, `SelectExtMethInfosForType` |
| `TypeHierarchy.fs` | `CopyTyparConstraints`/`FixupNewTypars` with trait context |
| `IlxGen.fs` | FS3882 warning, `ExprRequiresWitness` structured error handling |
| `Optimizer.fs` | Fallback trait context, `referencedCcus` tracking |
| `infos.fs` | `HasAllowOverloadOnReturnType` property on `MethInfo` |
| `PostInferenceChecks.fs` | Allow return-type-only overloads with attribute |
| `prim-types.fs/fsi` | `AllowOverloadOnReturnTypeAttribute` definition |
| `LanguageFeatures.fs/fsi` | `ExtensionConstraintSolutions` feature flag |
| `FSComp.txt` + xlf | `featureExtensionConstraintSolutions`, warning 3882 |
| `TcGlobals.fs/fsi` | `attrib_AllowOverloadOnReturnTypeAttribute` |
| `TypedTreePickle.fs` | `traitCtxt` correctly NOT serialized |

### Primary test file

`tests/FSharp.Compiler.ComponentTests/Conformance/Types/TypeConstraints/IWSAMsAndSRTPs/IWSAMsAndSRTPsTests.fs`
— ~116 test methods covering extension operators, weak resolution, witnesses, cross-assembly, breaking changes, IWSAM interaction, AllowOverloadOnReturnType, UoM, struct types, computation expressions, etc.

---

## Problem: Orphan Changes

The branch contains **~15 unrelated code removals/changes** that were introduced in early commits. These delete features and code that exist on `main` (introduced by sibling PRs). They are deeply interleaved with the RFC changes in the same files — especially `ConstraintSolver.fs`.

**This is the #1 blocker for PR submission.** A reviewer will immediately flag these as unexplained deletions of shipping code.

### Complete Orphan Inventory

Each entry lists: what was removed, which files, and what the correct resolution is.

#### O1. `OverloadResolutionCache` (entire feature deleted)

- **Files**: `FSharp.Compiler.Service.fsproj` (removes `OverloadResolutionCache.fsi`/`.fs` from compilation), `ConstraintSolver.fs` (removes `open FSharp.Compiler.OverloadResolutionCache`, deletes `ResolveOverloadingCore` extracted function, removes cache lookup/store calls, removes `cacheKeyOpt`/`cachedHit` logic)
- **Root cause**: The branch replaced the extracted+cached `ResolveOverloadingCore` with an inlined version that adds `AllowOverloadOnReturnType` support. The `AllowOverloadOnReturnType` changes to `ResolveOverloading` should be merged INTO the existing caching structure, not replace it.
- **Resolution**: Re-add `OverloadResolutionCache.fsi`/`.fs` to fsproj. Modify `ResolveOverloading` to add `hasAllowOverloadOnReturnType` and `alwaysConsiderReturnType` logic while preserving `ResolveOverloadingCore` and the cache. Pass `alwaysConsiderReturnType` through to the cache key computation.

#### O2. `LanguageFeature.MethodOverloadsCache` (feature flag deleted)

- **Files**: `LanguageFeatures.fs/fsi`, `FSComp.txt`
- **Root cause**: The branch replaced both `MethodOverloadsCache` and `ImplicitDIMCoverage` with `ExtensionConstraintSolutions`. All three should coexist.
- **Resolution**: Keep `MethodOverloadsCache` and `ImplicitDIMCoverage`. Add `ExtensionConstraintSolutions` alongside them. (This was partially fixed in the merge commit but the ConstraintSolver still references the removed cache.)

#### O3. `LanguageFeature.ImplicitDIMCoverage` (feature flag + impl deleted)

- **Files**: `LanguageFeatures.fs/fsi`, `MethodOverrides.fs/fsi`, `CheckExpressions.fs`
- **What was removed**: `HasImplicitDIMCoverage` on `RequiredSlot`, `isExplicitInterfaceImpl` parameter on `CheckDispatchSlotsAreImplemented`, filtering of DIM-covered slots in `CheckOverridesAreAllUsedOnce`
- **Resolution**: Restore `MethodOverrides.fs/fsi` from `main`. Fix `CheckExpressions.fs` to pass `isExplicitInterfaceImpl` to `CheckDispatchSlotsAreImplemented`.

#### O4. `Nullness.KnownFromConstructor` (DU case deleted)

- **Files**: `TypedTree.fs/fsi` (removes DU case + `Normalize()` method), `ConstraintSolver.fs` (removes `Normalize()` calls, removes `KnownFromConstructor` match arms), `TypedTreeOps.fs` (removes handling in `TypeNullIsExtraValueNew`)
- **Root cause**: This nullness feature was introduced by a sibling PR. The branch deleted it.
- **Resolution**: Restore `KnownFromConstructor` case in `Nullness` DU. Restore `Normalize()` method. Add back all `KnownFromConstructor` match arms in ConstraintSolver. Restore `isFromConstructor` check in `SolveTypeUseNotSupportsNull`.

#### O5. `ContextInfo.NullnessCheckOfCapturedArg` + `getNullnessWarningRange`

- **Files**: `ConstraintSolver.fs` (removes DU case + helper function, changes all `getNullnessWarningRange csenv` calls to `csenv.m`), `CompilerDiagnostics.fs`, `FSharpDiagnostic.fs`
- **Resolution**: Restore `NullnessCheckOfCapturedArg` case in `ContextInfo`. Restore `getNullnessWarningRange` function. Change `csenv.m` back to `getNullnessWarningRange csenv` at all nullness warning sites.

#### O6. `ResolveOverloadingCore` (function inlined)

- **Files**: `ConstraintSolver.fs`
- **Root cause**: The branch inlined `ResolveOverloadingCore` back into `ResolveOverloading` to add `AllowOverloadOnReturnType`. The cache infrastructure depended on the extracted function.
- **Resolution**: See O1. Keep the extracted function, add `alwaysConsiderReturnType` parameter.

#### O7. `isMeasureableValueType` (function deleted)

- **Files**: `TypedTreeOps.fs` (definition removed), `MethodCalls.fs` (call site removed)
- **Resolution**: Restore function in `TypedTreeOps.fs` and call site in `MethodCalls.fs`.

#### O8. `typarsAEquivWithFilter` / `typarsAEquivWithAddedNotNullConstraintsAllowed`

- **Files**: `TypedTreeOps.fs` (functions deleted), `CheckDeclarations.fs` (call sites changed to use `typarsAEquiv` directly)
- **Resolution**: Restore both functions. Restore `checkTyparsForExtension` local in `CheckDeclarations.fs`.

#### O9. `CalledMeth` lazy named props → eager evaluation

- **Files**: `MethodCalls.fs`
- **What changed**: `computeAssignedNamedProps` was a deferred lazy computation; the branch made it eager and restructured the tuple destructuring.
- **Resolution**: Restore `MethodCalls.fs` from `main`. The RFC changes don't touch this file.

#### O10. `Caches.fs/fsi` changes

- **Files**: `Caches.fs`, `Caches.fsi` (~57 lines diff)
- **Resolution**: Restore from `main`. The RFC changes don't touch these files.

#### O11. `TypeHashing.fs` — `MaxTokenCount` constant removed

- **Files**: `TypeHashing.fs` (replaces named constant with inline `256`)
- **Resolution**: Restore from `main`.

#### O12. `BuildGraph.fsi` — `culture` field removed

- **Files**: `BuildGraph.fsi`
- **Resolution**: Restore from `main`.

#### O13. `service.fs/fsi` — small changes

- **Files**: `service.fs`, `service.fsi` (~22 lines each)
- **Resolution**: Restore from `main`.

#### O14. `ItemsAreEffectivelyEqual` constructor cross-match removed

- **Files**: `NameResolution.fs`
- **What was removed**: Cross-matching constructor `ValRef`s with `CtorGroup` items in `ItemsAreEffectivelyEqual`/`ItemsAreEffectivelyEqualHash`
- **Resolution**: These lines ARE on `main`. Restore them. They were removed by the branch but they belong to PR #14902.

#### O15. `TemporarilySuspendReportingTypecheckResultsToSink` usage removed

- **Files**: `CheckDeclarations.fs`
- **What was removed**: The `checkAttributeTargetsErrors` local function that used `TemporarilySuspendReportingTypecheckResultsToSink` was inlined. This changes behavior — duplicate attribute symbols may appear.
- **Resolution**: Restore the `checkAttributeTargetsErrors` function with `TemporarilySuspendReportingTypecheckResultsToSink`.

---

## Resolution Strategy

### Option A: Surgical file restore (recommended)

For each orphan, restore the file from `main` and then re-apply only the RFC-related changes. This is safe for files where orphans and RFC changes don't overlap:

```bash
# Files with ZERO RFC changes — just restore from main:
git checkout origin/main -- \
  src/Compiler/FSharp.Compiler.Service.fsproj \
  src/Compiler/Checking/MethodOverrides.fs \
  src/Compiler/Checking/MethodOverrides.fsi \
  src/Compiler/Checking/MethodCalls.fs \
  src/Compiler/Driver/CompilerDiagnostics.fs \
  src/Compiler/Driver/GraphChecking/TrieMapping.fs \
  src/Compiler/Service/service.fs \
  src/Compiler/Service/service.fsi \
  src/Compiler/Symbols/FSharpDiagnostic.fs \
  src/Compiler/TypedTree/TypedTreeBasics.fs \
  src/Compiler/Utilities/Caches.fs \
  src/Compiler/Utilities/Caches.fsi \
  src/Compiler/Utilities/TypeHashing.fs \
  src/Compiler/Facilities/BuildGraph.fsi
```

For files with **both** orphan and RFC changes interleaved (the hard ones):

| File | Orphan changes | RFC changes | Approach |
|------|---------------|-------------|----------|
| `TypedTree.fs/fsi` | `KnownFromConstructor` removal | `ITraitContext`, `TTrait` 8th field, `traitCtxtNone` | Reset to main, re-add ITraitContext + TTrait changes |
| `TypedTreeOps.fs/fsi` | `isMeasureableValueType`, `typarsAEquivWithFilter` removal | TTrait 7→8 field pattern updates | Reset to main, sed all `TTrait(…, _)` to `TTrait(…, _, _)` |
| `ConstraintSolver.fs/fsi` | `OverloadResolutionCache`, `ResolveOverloadingCore`, `KnownFromConstructor`, `getNullnessWarningRange`, `NullnessCheckOfCapturedArg` | `TraitContext` alias, `FreshenMethInfo`/`FreshenTypeInst` signatures, `GetRelevantMethodsForTrait` extension lookup, `SolveMemberConstraint` trait context, `AllowOverloadOnReturnType`, `CanonicalizePartialInferenceProblemForExtensions`, `CreateImplFileTraitContext` | **Most complex file.** Reset to main, then apply each RFC function one at a time. |
| `CheckDeclarations.fs` | `checkTyparsForExtension`/`typarsAEquivWithAddedNotNull`, `TemporarilySuspendReporting` | `FreshenObjectArgType` with trait context, `LightweightTcValForUsingInBuildMethodCall` with trait context | Reset to main, re-add only the `env.TraitContext` threading |
| `CheckExpressions.fs` | `isExplicitInterfaceImpl` parameter removal | FS1215 gating, canonicalization branch, all `env.TraitContext` threading, `FreshenObjectArgType`/`FreshenMethInfo`/etc. with trait context | Reset to main, re-apply trait context threading and feature flag checks |
| `NameResolution.fs/fsi` | `ItemsAreEffectivelyEqual` ctor cross-match removal | `SelectExtensionMethInfosForTrait`, `SelectExtMethInfosForType`, `FreshenTypars`/`FreshenMethInfo` signatures, `RegisterUnionCaseTesterForProperty` | Reset to main, re-add extension method functions and signature changes |

### Option B: Fresh branch from main

Start a new branch from current `main`. Cherry-pick only the RFC-specific commits (skipping fixup commits that introduced orphan removals). Apply the test file and docs wholesale. This is cleaner but loses commit history.

---

## Merge Conflicts to Expect

When merging with or rebasing onto `main`, expect conflicts in:

| File | Conflict source |
|------|----------------|
| `LanguageFeatures.fs/fsi` | Branch replaced `MethodOverloadsCache`+`ImplicitDIMCoverage` with `ExtensionConstraintSolutions`; main has all three entries |
| `FSComp.txt` + all xlf files | Same — feature string entries added/removed |
| `ConstraintSolver.fs` | `ResolveOverloading` restructured; cache code removed; will conflict with any main changes to overload resolution |
| `MethodOverrides.fs` | `ImplicitDIMCoverage` removed; conflicts with any main changes to dispatch slot checking |
| `TypedTree.fs` | `KnownFromConstructor` removed; conflicts with any nullness changes on main |
| `CheckExpressions.fs` | `CheckDispatchSlotsAreImplemented` signature changed; conflicts with any main changes |
| Test infrastructure files | xUnit3 upgrade on main (`XunitHelpers.fs` deleted, `XunitSetup.fs` changed, fsproj package references changed) |

**Resolution for each**: Take main's version of the conflicted code, then re-apply only the RFC additions on top.

---

## Remaining TODOs After Orphan Cleanup

### Code Quality

1. **Run `dotnet fantomas .`** on all changed `.fs` files (repo coding guidelines require it)
2. **Strip `Debug.Assert(false, …)` from `IlxGen.fs`** error paths — convert to proper diagnostics or remove (8 occurrences from commit `dc971b9fa`)
3. **Fix grammar in `RFC_Changes.md:407`**: "to happening" → "to happen"
4. **Add trailing newline to `FSComp.txt`** EOF

### Missing Tests

5. **C# extension methods (`ILExtMem`) in SRTP**: `CreateImplFileTraitContext` only finds F# `Val`-based extensions. Add a negative test documenting this limitation.
6. **`AllowOverloadOnReturnType` positive e2e**: Blocked until attribute ships in SDK FSharp.Core. Current test correctly expects error 39.
7. **Performance benchmark**: No measurement with 50+ extension candidates. Profile constraint solving.

### Correctness

8. **`CreateImplFileTraitContext` IL extension gap**: Documented in code. Optimizer SRTP resolution won't find C# extension methods. Low practical impact but worth a comment in the RFC.

### Deferred

9. **FSharpPlus coordination**: Deferred per RFC. Workarounds documented in `RFC_Changes.md`.
10. **`AllowOverloadOnReturnType`** has no `LanguageFeature` flag — by design (attribute-gated, self-gating).
11. **Performance profiling**: RFC says "ongoing", no results included.

---

## Verification Commands

```bash
# Build the compiler:
BUILDING_USING_DOTNET=true dotnet build src/Compiler/FSharp.Compiler.Service.fsproj -c Debug

# Run SRTP tests:
dotnet test tests/FSharp.Compiler.ComponentTests -c Release \
  --filter "FullyQualifiedName~IWSAMsAndSRTPs" /p:BUILDING_USING_DOTNET=true

# Run cross-version compat tests:
dotnet fsi tests/FSharp.Compiler.ComponentTests/CompilerCompatibilityTests.fsx

# Run surface area tests (after AllowOverloadOnReturnType changes):
TEST_UPDATE_BSL=1 dotnet test tests/FSharp.Core.UnitTests --filter "SurfaceAreaTest" -c Release

# Full validation:
./build.sh -c Release --testcoreclr
```

> **Note on .NET 10 SDK**: `dotnet test` may require `-p:TestingPlatformDotnetTestSupport=true` or direct `dotnet exec` invocation due to the VSTest→MTP migration.

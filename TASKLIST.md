# Nullable Reference Type Cleanup - Sprint Tracker

**Goal**: Remove NRT shims/hacks/ifdefs now that .NET SDK fully supports nullable reference types.

---

## Sprint Progress

- [ ] **Sprint 1**: FSharp.Core `BUILDING_WITH_LKG` Cleanup
  - Files: `prim-types.fsi` (7 blocks), `prim-types.fs` (2 blocks), `event.fsi` (2 blocks), `event.fs` (2 blocks)
  - Action: Remove `#if BUILDING_WITH_LKG || BUILD_FROM_SOURCE` conditionals, keep the `#else` branch with `not null` constraints and `IsError=true`
  - Update surface area baselines

- [ ] **Sprint 2**: FSharpEmbedResourceText.fs Cleanup
  - Files: `src/FSharp.Build/FSharpEmbedResourceText.fs` (1 block)
  - Action: Remove `#if BUILDING_WITH_LKG` / `#nowarn "3261"` section

- [ ] **Sprint 3**: MaybeNull Replacement - Utilities Layer (~15 files)
  - Files: `NullnessShims.fs`, `illib.fs`, `lib.fs`, `lib.fsi`, `FileSystem.fs`, `Activity.fs`, `Cancellable.fs`, `Cancellable.fsi`, `LruCache.fs`
  - Action: Replace `MaybeNull<'T>` with `'T | null`

- [ ] **Sprint 4**: MaybeNull Replacement - TypedTree Layer (~10 files)
  - Files: `TypeProviders.fs`, `TypeProviders.fsi`, `tainted.fs`, `tainted.fsi`, `TypedTreeOps.fs`, `TypedTreeOps.fsi`
  - Action: Replace `MaybeNull<'T>` with `'T | null`

- [ ] **Sprint 5**: MaybeNull Replacement - Compiler Core (~15 files)
  - Files: `QuickParse.fs`, `QuickParse.fsi`, `import.fs`, `import.fsi`, `MethodCalls.fs`, `AccessibilityLogic.fs`, `infos.fs`, `ilread.fs`, `ilreflect.fs`, `ilsign.fs`, `ilnativeres.fs`, `fsi.fs`, `FxResolver.fs`, `CompilerImports.fs`, `IlxGen.fs`, `SimulatedMSBuildReferenceResolver.fs`, `CompilerLocation.fs`, `DependencyProvider.fsi`
  - Action: Replace `MaybeNull<'T>` with `'T | null`

- [ ] **Sprint 6**: MaybeNull Replacement - FSharp.Build (~5 files)
  - Files: `Fsc.fs` (~33 usages), `Fsi.fs` (~11 usages), `FSharpCommandLineBuilder.fs` (~4 usages), `WriteCodeFragment.fs` (~2 usages)
  - Action: Replace `MaybeNull<'T>` with `'T | null`

- [ ] **Sprint 7**: `(^)` Operator Removal
  - Files: `NullnessShims.fs` (definition), `fsi.fs` (1 usage at line 981)
  - Action: Remove operator definition, inline `Option.bind`-style replacement at usage

- [ ] **Sprint 8**: NullnessShims.fs → NullHelpers.fs Rename
  - Projects: `FSharp.Compiler.Service.fsproj`, `FSharp.Build.fsproj`, `FSharp.DependencyManager.Nuget.fsproj`, `FSharp.VS.FSI.fsproj`, `VisualFSharp.Salsa.fsproj`, `VisualFSharp.UnitTests.fsproj`
  - Action: Rename file and update all project references

---

## Summary of Work

| Category | Files | Count | Status |
|----------|-------|-------|--------|
| `BUILDING_WITH_LKG` blocks in FSharp.Core | 4 | 13 blocks | Pending |
| `BUILDING_WITH_LKG` in FSharpEmbedResourceText | 1 | 1 block | Pending |
| `MaybeNull` usages | ~35 | ~155 | Pending |
| `(^)` operator | 2 | definition + 1 usage | Pending |
| File rename | 6 projects | 1 file | Pending |

---

## What Gets Removed

- `MaybeNull` type alias → replaced with `'T | null`
- `(^)` null-propagation operator → inlined at call site
- All NRT-related `#if BUILDING_WITH_LKG || BUILD_FROM_SOURCE` conditionals

## What Stays

- `BUILDING_WITH_LKG` define itself (for future features)
- `!!` operator (widely used)
- `isNotNull` helper
- `nullSafeEquality` helper
- `objEqualsArg` type alias
- `(|NonEmptyString|_|)` active pattern
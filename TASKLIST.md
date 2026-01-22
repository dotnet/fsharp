# Nullable Reference Type Cleanup - Task Tracker

**Goal**: Remove NRT shims/hacks/ifdefs now that .NET SDK fully supports nullable reference types.

> ⚠️ **CONSTRAINT DISCOVERED**: `MaybeNull<'T>` cannot be replaced with `'T | null` in `.fsi` signature files due to F# type system nullability coercion differences. The type alias must be kept for API compatibility.

---

## Completed ✅

- [x] **BUILDING_WITH_LKG blocks removed** (all 13 blocks in FSharp.Core + FSharpEmbedResourceText.fs)
- [x] **MaybeNull cleanup in Utilities/*.fs** (all .fs files cleaned; .fsi unchanged per constraint)
- [x] **Sprint 1: MaybeNull cleanup in FSharp.Build** (all 50 usages replaced with `'T | null`)

---

## Sprint 2: MaybeNull Cleanup in Compiler Core (~80 usages)

### TypedTree (26 usages)
- [ ] `TypeProviders.fs` (22 usages)
- [ ] `tainted.fs` (2 usages)
- [ ] `TypedTreeOps.fs` (2 usages)

### Checking (7 usages)
- [ ] `infos.fs` (3 usages)
- [ ] `MethodCalls.fs` (2 usages)
- [ ] `AccessibilityLogic.fs` (1 usage)
- [ ] `import.fs` (1 usage)

### AbstractIL (8 usages)
- [ ] `ilreflect.fs` (4 usages)
- [ ] `ilsign.fs` (2 usages)
- [ ] `ilnativeres.fs` (1 usage)
- [ ] `ilread.fs` (1 usage)

### Service (4 usages)
- [ ] `QuickParse.fs` (4 usages)

### Interactive (7 usages)
- [ ] `fsi.fs` (7 usages)

### Driver (3 usages)
- [ ] `FxResolver.fs` (2 usages)
- [ ] `CompilerImports.fs` (1 usage)

### Facilities (2 usages)
- [ ] `SimulatedMSBuildReferenceResolver.fs` (1 usage)
- [ ] `CompilerLocation.fs` (1 usage)

### CodeGen (1 usage)
- [ ] `IlxGen.fs` (1 usage)

**DoD**: Build succeeds, all tests pass

---

## Sprint 3: Remove `(^)` Operator (Dead Code)

The null-propagation operator is no longer used anywhere in the codebase.

- [ ] Remove `(^)` operator definition from `NullnessShims.fs` (lines 12-15)

**DoD**: Build succeeds, all tests pass

---

## Sprint 4: Rename NullnessShims.fs (Optional Cleanup)

- [ ] Rename `NullnessShims.fs` → `NullHelpers.fs`
- [ ] Update `FSharp.Compiler.Service.fsproj` reference

**DoD**: Build succeeds, all tests pass

---

## Files That KEEP MaybeNull (.fsi signatures)

These 7 signature files MUST retain `MaybeNull` for API compatibility:
- `Cancellable.fsi` (1 usage)
- `TypeProviders.fsi` (18 usages)
- `tainted.fsi` (3 usages)
- `TypedTreeOps.fsi` (1 usage)
- `import.fsi` (1 usage)
- `QuickParse.fsi` (3 usages)
- `DependencyProvider.fsi` (3 usages)

---

## Summary

| Category | Count | Status |
|----------|-------|--------|
| `BUILDING_WITH_LKG` blocks | 0 | ✅ Done |
| `MaybeNull` in Utilities/*.fs | 0 | ✅ Done |
| `MaybeNull` in FSharp.Build | 50 | 🔧 Sprint 1 |
| `MaybeNull` in Compiler core | ~58 | 🔧 Sprint 2 |
| `(^)` operator | 1 def | 🔧 Sprint 3 |
| File rename | 1 file | 🔧 Sprint 4 (optional) |
| `.fsi` files with MaybeNull | 7 | ⚠️ Keep (constraint) |

---

## What Gets Removed

- `MaybeNull` usages in `.fs` files → replaced with `'T | null`
- `(^)` null-propagation operator → removed (dead code)

## What Stays

- `MaybeNull` type alias definition (for .fsi compatibility)
- `MaybeNull` usages in `.fsi` signature files
- `!!` operator (widely used)
- `isNotNull` helper
- `nullSafeEquality` helper
- `objEqualsArg` type alias
- `(|NonEmptyString|_|)` active pattern
# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Extract storeCacheResult helper

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Cache lookup optimization

**Summary:** Modified `storeCacheResult` to accept the cache directly instead of TcGlobals, eliminating 4 redundant `getOverloadResolutionCache` calls. Updated:
- `storeCacheResult` signature: now takes `cache` instead of `g: TcGlobals`
- `ResolveOverloadingCore` signature: added `cache` parameter
- `GetMostApplicableOverload` signature: added `cache` parameter
- All 4 call sites pass the cache captured at line 3820

**DoD verification:**
- Build: 0 errors, 0 warnings
- Tests: 8 OverloadCache tests passed
- Formatting: fantomas --check passes

**Files touched:** src/Compiler/Checking/ConstraintSolver.fs

---

## Sprint 3: ResizeArray pattern fix

**Summary:** Replaced mutable list + prepend + List.rev pattern with ResizeArray in `tryComputeOverloadCacheKey` (lines 476-551):
- Changed `objArgStructures` from `mutable list` to `ResizeArray()`
- Changed `argStructures` from `mutable list` to `ResizeArray()`
- Use `.Add()` instead of prepending with `::`
- Use `Seq.toList` instead of `List.rev` for conversion

**DoD verification:**
- Build: 0 errors, 0 warnings
- Tests: 8 OverloadCache tests passed (all frameworks)
- Formatting: fantomas --check passes
- List.rev calls eliminated

**Files touched:** src/Compiler/Checking/ConstraintSolver.fs

---

## Sprint 4: ProvidedMeth hash improvement

**Summary:** Improved hash computation for ProvidedMeth in `computeMethInfoHash` (line 448):
- Changed from hashing only the method name to hashing both method name AND declaring type FullName
- Before: `hash (mb.PUntaint((fun m -> m.Name), range0))`
- After: `hash (mb.PUntaint((fun m -> m.Name, (nonNull<ProvidedType> m.DeclaringType).FullName |> string), range0))`
- This prevents cache collisions between same-named methods in different type providers

**DoD verification:**
- Build: 0 errors, 0 warnings
- Tests: 8 OverloadCache tests passed (net10.0: 8 passed, net472: 8 passed)
- ProvidedMeth case hashes both method name and declaring type
- No functional change in behavior

**Files touched:** src/Compiler/Checking/ConstraintSolver.fs

---

## Sprint 5: MaxTokenCount constant

**Summary:** Defined `[<Literal>] let MaxTokenCount = 256` constant in TypeHashing.fs and replaced all 10 magic number occurrences with it:
- Added constant definition after line 46 in HashingPrimitives module with doc comment
- Replaced 256 at lines: 418 (ResizeArray initial capacity), 448, 456, 463, 469, 483, 490, 497, 554, 575

**DoD verification:**
- Build: 0 errors, 0 warnings
- Tests: 8 OverloadCache tests passed (net10.0: 8 passed, net472: 8 passed)
- MaxTokenCount constant defined
- All 10 magic number 256 occurrences replaced
- No hardcoded 256 remains in type structure generation (only in constant definition)

**Files touched:** src/Compiler/Utilities/TypeHashing.fs

---

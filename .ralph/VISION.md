# RFC FS-NNNN: Equally Named Abstract Slots - Implementation Vision

## High-Level Goal

Implement the RFC to simplify interface hierarchies where a derived interface shadows or re-declares a member from a base interface using a Default Interface Member (DIM) implementation. F# should no longer require explicit interface declarations for slots already covered by DIMs.

This unblocks BCL evolution (e.g., making `ICollection<T>` implement `IReadOnlyCollection<T>`) without causing source-breaking changes for F# consumers.

## Core Problem

Currently, F# emits FS0361 error when a member implementation matches multiple interface slots, even when some slots are covered by DIMs:

```fsharp
// C# interface: IB : IA with DIM covering IA.M
type C() =
    interface IB with
        member _.M() = 42  // Error FS0361
```

## Proposed Solution

Modify the FS0361 error logic in `MethodOverrides.fs` (around line 630-640) to:
1. Filter out dispatch slots that have DIM coverage in the implementing interface hierarchy
2. Only emit FS0361 when multiple **uncovered** slots remain

## Key Files to Modify

1. **`src/Compiler/Checking/MethodOverrides.fs`** - Core logic change
2. **`src/Compiler/Facilities/LanguageFeatures.fs`** - New language feature flag (gate on F# version)
3. **`src/Compiler/FSComp.txt`** - Feature description string
4. **Test files** - Comprehensive test coverage

## Implementation Approach

The existing `DefaultInterfaceImplementationSlot` DU case already tracks DIM presence. We need to:
1. Add a new `LanguageFeature` entry for gating ✅ DONE (Sprint 1)
2. Add test scaffolding with expected-failing tests ✅ DONE (Sprint 1)
3. Add a helper function `slotHasDIMCoverage` to check if a slot is covered by a DIM ✅ DONE (Sprint 2)
4. Modify the dispatch slot filtering logic to exclude DIM-covered slots from conflict detection ✅ DONE (Sprint 2)
5. Modify slot-to-override matching to skip DIM-covered slots during IL generation ✅ DONE (Sprint 2)

## Key Design Decisions

1. **Language Version Gating**: This feature will be gated on a new language version to maintain backward compatibility
2. **DIM Coverage Definition**: A slot S in interface IA is "DIM-covered" when implementing IB if:
   - IB inherits from IA
   - IB provides a default implementation for IA.S
3. **IL Generation Changes**: The slot-to-override matching must ALSO skip DIM-covered slots to avoid generating invalid MethodImpls

## Edge Cases to Handle

1. **Simple DIM Shadowing** - Primary use case, should work ✅ DONE (Sprint 2)
2. **Diamond with Single DIM** - Should work (one path provides DIM)
3. **Diamond with Conflicting DIMs** - Should still error (no most-specific)
4. **Re-abstracted Members** - Should still error
5. **Properties with Mixed Accessors** - DIM on getter should cover getter slot
6. **Generic Interfaces with Different Instantiations** - Each instantiation is separate
7. **Object Expressions** - Same rules as class types
8. **Explicit Override of DIM** - Still requires explicit interface declaration

## Test Categories

1. **C# Interop Tests** - C# interfaces with DIMs consumed in F#
2. **F# Pure Tests** - Ensure no regression for F#-defined interfaces
3. **Error Preservation Tests** - Ensure real conflicts still error
4. **Object Expression Tests** - Both class and object expression scenarios
5. **Language Version Tests** - Old versions should still emit FS0361

## Potentially Breaking Tests

Based on analysis, these existing tests may need attention:
- `tests/fsharp/typecheck/sigs/neg26.fs` - Tests FS0361 for interface hierarchies
- `tests/fsharpqa/Source/Conformance/InferenceProcedures/DispatchSlotInference/E_MoreThanOneDispatchSlotMatch01.fs` - Tests FS0361

**CRITICAL**: These tests use F#-defined interfaces (no DIMs), so they should NOT be affected by this change since F# interfaces cannot have DIMs.

## Constraints

1. This is an **additive change** - previously invalid code becomes valid
2. Must not affect existing valid code
3. Must preserve errors for genuine conflicts (multiple uncovered slots)
4. Must work with both class types and object expressions

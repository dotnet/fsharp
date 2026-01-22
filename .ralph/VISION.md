# Vision: OverloadResolutionPriorityAttribute Support (RFC FS-XXXX)

## High-Level Goal

Implement F# support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9 attribute) that allows library authors to explicitly prioritize method overloads. This complements the existing "Most Concrete Tiebreaker" (already implemented on this branch) by providing **explicit** prioritization vs. the implicit type-structure-based approach.

## Key Design Decisions

### 1. Algorithm Position: Pre-Filter (Not Tiebreaker Rule)

Per the RFC and C# behavior, OverloadResolutionPriority is implemented as a **pre-filter** that runs **before** tiebreaker comparison, NOT as another tiebreaker rule. This is critical:

- Candidates are **grouped by declaring type**
- Within each group, only **highest-priority candidates survive**
- Groups are recombined
- **Then** all existing tiebreaker rules apply (including MoreConcrete)

This means a method with `[OverloadResolutionPriority(1)]` will **always** beat one with priority 0 from the same declaring type, regardless of type concreteness.

### 2. Integration Point

The pre-filter should be applied in `ConstraintSolver.fs` at `GetMostApplicableOverload`:
- After `applicableMeths` list is formed (line ~3668)
- Before the `better` comparison loop
- Filter by grouping on `Method.ApparentEnclosingType` and keeping only max-priority within each group

### 3. Rule Engine Alignment

**NOT adding a new TiebreakRuleId.** The RFC explicitly says priority is a pre-filter. Adding it as a rule would violate the semantics where priority **overrides** all tiebreakers including concreteness.

However, we may add tracking to report which methods were eliminated by priority filtering.

### 4. Attribute Reading Pattern

Follow existing pattern in `infos.fs`:
```fsharp
member x.GetOverloadResolutionPriority() : int =
    match x with
    | ILMeth(_, ilMethInfo, _) ->
        ilMethInfo.RawMetadata.CustomAttrs.AsList()
        |> List.tryPick (fun attr -> 
            if attr.Method.DeclaringTypeRef.FullName = 
               "System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute" then
                match attr.Elements with
                | [ILAttribElem.Int32 priority] -> Some priority
                | _ -> Some 0
            else None)
        |> Option.defaultValue 0
    | _ -> 0
```

### 5. Language Feature Gating

- Add `LanguageFeature.OverloadResolutionPriority` (F# 10.0)
- When disabled, the attribute is **silently ignored** (not an error)
- This matches C# behavior for consuming BCL types that use the attribute

### 6. Diagnostics

- FS3577 already exists for concreteness - add FS3578 for priority selection (off by default)
- Error if `[OverloadResolutionPriority]` applied to an override (new error)

## Important Context

### Existing Infrastructure

1. **`OverloadResolutionRules.fs`**: Contains the tiebreaker rule engine with 15 rules, including `MoreConcrete` (rule 13). We do NOT add priority here.

2. **`ConstraintSolver.fs`**: Contains `GetMostApplicableOverload` (line 3646+) which:
   - Creates indexed applicable methods
   - Uses `better` function to compare pairs
   - Calls `evaluateTiebreakRules` from the rule engine

3. **`LanguageFeatures.fs`**: Contains `LanguageFeature` enum and version mapping. `MoreConcreteTiebreaker` is at line 107, version 10.0.

4. **`TiebreakerTests.fs`**: Comprehensive test file with patterns for testing overload resolution.

### Files to Modify

| File | Purpose |
|------|---------|
| `src/Compiler/Checking/infos.fs` + `.fsi` | Add `GetOverloadResolutionPriority()` to MethInfo |
| `src/Compiler/Checking/ConstraintSolver.fs` | Add priority pre-filter in `GetMostApplicableOverload` |
| `src/Compiler/Facilities/LanguageFeatures.fs` + `.fsi` | Add `OverloadResolutionPriority` feature |
| `src/Compiler/FSComp.txt` | Add FS3578 diagnostic and feature string |
| `tests/.../TiebreakerTests.fs` | Add comprehensive tests |
| Release notes | Document the feature |

## Constraints & Gotchas

1. **Inheritance**: Priority is read from **least-derived declaration**. Applying to override is an error.

2. **Extension methods**: Priority is scoped per-declaring-type, not global. Different extension types compete independently.

3. **F# methods**: For F#-defined methods, we need to handle Attrib records, not just IL attributes.

4. **Polyfill**: The attribute only exists in .NET 9+. Older targets may need polyfill or graceful ignore.

5. **SRTP**: Skip priority logic for SRTP methods (matches MoreConcrete behavior).

## TDD Approach

1. **Tests First**: Write failing tests covering all RFC examples before implementation
2. **Minimal Changes**: Surgical edits - don't refactor unrelated code
3. **Feature Gating**: All new code gated on `LanguageFeature.OverloadResolutionPriority`
4. **Integration**: Tests should pass with existing MoreConcrete tests

# Vision: OverloadResolutionPriorityAttribute Support (RFC FS-XXXX)

## High-Level Goal

Implement F# support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9 attribute) that allows library authors to explicitly prioritize method overloads. This complements the existing "Most Concrete Tiebreaker" (already implemented on this branch) by providing **explicit** prioritization vs. the implicit type-structure-based approach.

## Current Status (Replan Summary)

The following work has been completed:
- ✅ `LanguageFeature.OverloadResolutionPriority` enum value exists (LanguageFeatures.fsi:99, LanguageFeatures.fs:108)
- ✅ Feature mapped to `languageVersion100` (F# 10.0) (LanguageFeatures.fs:246)
- ✅ FSComp.txt has feature string `featureOverloadResolutionPriority` (FSComp.txt:1808)
- ✅ `GetOverloadResolutionPriority()` method added to MethInfo (infos.fs:1262-1284)

**What remains to be implemented:**
1. Pre-filter logic in `ConstraintSolver.fs` at `GetMostApplicableOverload`
2. Comprehensive tests in `TiebreakerTests.fs`
3. Optional diagnostic FS3578 (off by default) for priority-based resolution
4. Release notes documentation

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

```fsharp
/// Filter applicable methods by OverloadResolutionPriority attribute.
/// Groups methods by declaring type and keeps only highest-priority within each group.
let filterByOverloadResolutionPriority (g: TcGlobals) (applicableMeths: list<CalledMeth<_> * _ * _ * _>) =
    if not (g.langVersion.SupportsFeature(LanguageFeature.OverloadResolutionPriority)) then
        applicableMeths
    else
        applicableMeths
        |> List.groupBy (fun (calledMeth, _, _, _) -> calledMeth.Method.ApparentEnclosingType)
        |> List.collect (fun (_, group) ->
            let maxPriority = group |> List.map (fun (cm, _, _, _) -> cm.Method.GetOverloadResolutionPriority()) |> List.max
            group |> List.filter (fun (cm, _, _, _) -> cm.Method.GetOverloadResolutionPriority() = maxPriority))
```

### 3. Rule Engine Alignment

**NOT adding a new TiebreakRuleId.** The RFC explicitly says priority is a pre-filter. Adding it as a rule would violate the semantics where priority **overrides** all tiebreakers including concreteness.

However, we may add tracking to report which methods were eliminated by priority filtering.

### 4. Attribute Reading Pattern (ALREADY IMPLEMENTED)

The `GetOverloadResolutionPriority()` method exists in `infos.fs`:
```fsharp
member x.GetOverloadResolutionPriority() : int =
    let overloadResolutionPriorityAttributeName =
        "System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute"
    match x with
    | ILMeth(_, ilMethInfo, _) ->
        ilMethInfo.RawMetadata.CustomAttrs.AsArray()
        |> Array.tryPick (fun attr ->
            if attr.Method.DeclaringType.TypeRef.FullName = overloadResolutionPriorityAttributeName then
                match attr.Elements with
                | [ ILAttribElem.Int32 priority ] -> Some priority
                | _ -> Some 0
            else None)
        |> Option.defaultValue 0
    | MethInfoWithModifiedReturnType(mi, _) -> mi.GetOverloadResolutionPriority()
    | FSMeth _ -> 0  // F#-defined methods need IL-based check
    | DefaultStructCtor _ -> 0
    | ProvidedMeth _ -> 0
```

### 5. Language Feature Gating (ALREADY IMPLEMENTED)

- `LanguageFeature.OverloadResolutionPriority` exists in F# 10.0
- When disabled, the attribute is **silently ignored** (not an error)
- This matches C# behavior for consuming BCL types that use the attribute

### 6. Diagnostics

- FS3575/FS3576 exist for concreteness - add FS3578 for priority selection (off by default)
- Error if `[OverloadResolutionPriority]` applied to an override (out of scope for MVP)

## Constraints & Gotchas

1. **Inheritance**: Priority is read from **least-derived declaration**. Applying to override is an error. (Post-MVP)

2. **Extension methods**: Priority is scoped per-declaring-type, not global. Different extension types compete independently.

3. **F# methods**: For F#-defined methods with the attribute applied in F#, we need to handle FSMeth case. MVP can defer this.

4. **Polyfill**: The attribute only exists in .NET 9+. Older targets may need polyfill. Tests should use C# interop for the attribute.

5. **SRTP**: Skip priority logic for SRTP methods (matches MoreConcrete behavior).

## TDD Approach

1. **Tests First**: Write failing tests covering all RFC examples before implementation
2. **Minimal Changes**: Surgical edits - don't refactor unrelated code
3. **Feature Gating**: All new code gated on `LanguageFeature.OverloadResolutionPriority`
4. **Integration**: Tests should pass with existing MoreConcrete tests

## Test Strategy

Tests use **inline C# compilation** with the attribute applied, following the established codebase pattern (see `CSharp """..."""` DSL in ComponentTests). This approach:
1. Defines test types with `[OverloadResolutionPriority]` directly in test code
2. Compiles them dynamically with `withCSharpLanguageVersionPreview`
3. References from F# via `withReferences [csharpPriorityLib]`

This is preferred over a separate C# project as it keeps tests self-contained and follows existing patterns in ExtensionMethodTests.fs, ParamArray.fs, etc.

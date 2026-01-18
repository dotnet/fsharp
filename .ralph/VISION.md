# Event Nullness Fix for F# 9 - VISION

## High-level Goal
Fix nullness warnings (FS3261) that appear when implementing INotifyPropertyChanged or ICommand interfaces using F# Events with --checknulls enabled. The issues are tracked in:
- https://github.com/dotnet/fsharp/issues/18361
- https://github.com/dotnet/fsharp/issues/18349

## Problem Description
When users write:
```fsharp
type XViewModel() =
    let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = propertyChanged.Publish
```

They get warning FS3261:
```
warning FS3261: Nullness warning: The types 'System.Delegate' and 'System.Delegate | null' do not have compatible nullability.
```

The second issue (#18349) shows the same problem with ICommand:
```fsharp
type internal Command(execute, canExecute) =
  let canExecuteChanged = Event<EventHandler, EventArgs>()
  interface ICommand with
    [<CLIEvent>]
    member _.CanExecuteChanged = canExecuteChanged.Publish
```

## Root Cause Analysis
The issue stems from multiple layers:

1. **`IDelegateEvent<'Delegate>` interface** - The `AddHandler` and `RemoveHandler` methods currently don't have nullable annotations, but internally the Event implementation uses `Delegate.Combine`/`Delegate.Remove` which return `Delegate | null`.

2. **Mismatch at CLIEvent generation** - When F# generates CLI event add/remove methods for `[<CLIEvent>]` properties, there's a nullness mismatch between what the user's interface expects (e.g., `INotifyPropertyChanged` or `ICommand`) and what F#'s event system provides.

3. **Handler<'T> delegate** - Defined as `delegate of sender:objnull * args:'T -> unit`, correctly marks sender as nullable.

---

## ARCHITECTURAL REVIEW (2026-01-18)

### ❌ CURRENT IMPLEMENTATION IS WRONG

The current implementation adds delegate type checks (`isDelegateTy`) as a hack in the generic nullness-solving functions:
- `SolveNullnessEquiv` (lines 1063-1077)
- `SolveNullnessSubsumesNullness` (lines 1110-1130)

**Why this is wrong:**
1. These are **low-level type unification routines** that handle ALL nullness equivalence checks
2. Adding delegate-specific logic here is a **leaky abstraction** - it pollutes generic infrastructure with domain-specific exceptions
3. This is **overly broad** - it suppresses nullness warnings for ALL delegate types, not just CLIEvent scenarios
4. It doesn't follow the established pattern in the codebase (see `TType_app` pattern matches for byref special handling)

### ✅ CORRECT APPROACH

The fix should be at one of these higher abstraction levels:

#### Option A: Fix at the SOURCE (PREFERRED)
**Location:** `FindDelegateTypeOfPropertyEvent` in `infos.fs` (already partially done)

When extracting the delegate type for CLIEvent, ensure it has non-null nullness. The current implementation already does this with `replaceNullnessOfTy KnownWithoutNull`, but something in the flow is losing this information.

#### Option B: Handle in SolveTypeEqualsType/SolveTypeSubsumesType pattern matches
**Location:** `SolveTypeEqualsType` in `ConstraintSolver.fs` (lines 1263+)

Add a specific `TType_app` case for delegate types, similar to how byref types get special handling at line 1563:
```fsharp
// Special subsumption rule for byref tags
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2 && g.byref2_tcr.CanDeref && tyconRefEq g g.byref2_tcr tc1 ->
```

This would be:
```fsharp
// Special handling for delegate types - ignore nullness when same delegate type
| TType_app (tc1, l1, nullness1), TType_app (tc2, l2, nullness2) when tyconRefEq g tc1 tc2 && isDelegateTy g sty1 ->
    trackErrors {
        do! SolveTypeEqualsTypeEqns csenv ndeep m2 trace None l1 l2
        // Skip nullness check for delegates - they come from C# interfaces without annotations
    }
```

#### Option C: Handle in synthetic binding generation
**Location:** `CheckExpressions.fs` lines 2645-2684

When generating the synthetic `add_`/`remove_` bindings for CLIEvent, explicitly set the handler parameter type to have non-null nullness.

### RECOMMENDED ACTION

1. **Remove the hack** from `SolveNullnessEquiv` and `SolveNullnessSubsumesNullness`
2. **Verify Option A works** - the `FindDelegateTypeOfPropertyEvent` fix should be sufficient
3. If not, **implement Option B** - add a delegate-specific case in the `TType_app` pattern match in `SolveTypeEqualsType`/`SolveTypeSubsumesType`

### Pattern to Follow

Look at how byref types are handled specially in `SolveTypeSubsumesType`:
```fsharp
// Special subsumption rule for byref tags (line 1562-1573)
| TType_app (tc1, l1, _)  , TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2  && g.byref2_tcr.CanDeref && tyconRefEq g g.byref2_tcr tc1 ->
```

This is the correct abstraction level for type-specific special cases.

## Key Design Decisions

### Approach: Two-Pronged Fix

**Layer 1: FSharp.Core Event Implementation**
- The internal `multicast` field in Event types may need `| null` annotation
- The `Publish` property implementation creates an object expression that may need adjustment

**Layer 2: Compiler CLIEvent Handling**
- The compiler's nullness checking for CLIEvent-attributed properties may need special handling
- Located in `src/Compiler/CodeGen/IlxGen.fs` - function `GenEventForProperty`
- Likely needs adjustment in `src/Compiler/Checking/` for typechecking events

### Bootstrap Considerations
Since FSharp.Core is compiled by the old compiler during bootstrap:
- Use `#if BUILDING_WITH_LKG || BUILD_FROM_SOURCE` guards for new nullable syntax
- Pattern found in prim-types.fsi lines 1222, 1235, 1243, 1251, 1259
- FSharp.Core must build with BOTH old compiler (bootstrap/LKG) and new compiler

## Test Strategy
- **TDD approach**: Write failing tests first that reproduce exact issue scenarios
- **Run tests in Debug mode for net10.0**
- **Required env vars**:
  - `BUILDING_USING_DOTNET=true`
  - `SKIP_VERSION_CHECK=1`
- **Test command**:
  ```bash
  BUILDING_USING_DOTNET=true SKIP_VERSION_CHECK=1 dotnet test tests/FSharp.Compiler.ComponentTests -c Debug -f net10.0 --filter 'FullyQualifiedName~EventNullness'
  ```

## Key Files
- `src/FSharp.Core/prim-types.fsi` - IDelegateEvent/IEvent interfaces (lines 6208-6247)
- `src/FSharp.Core/prim-types.fs` - Implementation (lines 7342-7353)
- `src/FSharp.Core/event.fsi` - Event type signatures
- `src/FSharp.Core/event.fs` - Event implementation (DelegateEvent, Event<'Delegate,'Args>, Event<'T>)
- `src/Compiler/CodeGen/IlxGen.fs` - CLIEvent generation (lines 9093-9115)
- `tests/FSharp.Compiler.ComponentTests/Language/Nullness/` - Existing nullness tests

## Constraints/Gotchas
1. FSharp.Core must build with BOTH old compiler (bootstrap/LKG) and new compiler
2. Surface area baselines will need updates if public API nullness annotations change
3. Tests run in Debug mode for net10.0 with env vars
4. May need release notes entry in FSharp.Core section if public API changes

## Current State
- Branch: `tgro/event-nullness-fix` 
- Base: `main` (d87469d01)
- Clean working directory (only .ralph/ untracked)

## Investigation Findings (Subtask 3)

### FS3261 Warning Generation
The nullness warnings are generated in **`src/Compiler/Checking/ConstraintSolver.fs`**:

1. **`SolveNullnessEquiv`** (lines 1037-1067) - For type equivalence
2. **`SolveNullnessSubsumesNullness`** (lines 1071-1104) - For type subsumption

The warning is emitted at line 1102 when:
- Target (`n1`) is `WithoutNull` (non-nullable)
- Source (`n2`) is `WithNull` (nullable)

### Where the Warning is Triggered
The specific error message "A non-nullable 'PropertyChangedEventHandler' was expected but this expression is nullable" comes from `ConstraintSolverNullnessWarningWithTypes`.

### CLIEvent Type Checking Flow

1. **Event Property Detection** (`src/Compiler/Checking/infos.fs` line 32-33):
   ```fsharp
   member x.IsFSharpEventProperty g = x.IsMember && CompileAsEvent g x.Attribs && not x.IsExtensionMember
   ```

2. **Event Property Override Handling** (`src/Compiler/Checking/MethodOverrides.fs`):
   - Lines 187, 802, 853 - CLIEvent properties are flagged as `isFakeEventProperty`
   - Line 270 uses `TypeEquivEnv.EmptyIgnoreNulls` for interface matching - nullness is IGNORED here
   - Line 855-857 - CLIEvent properties bypass normal dispatch slot matching and use pre-computed `ImplementedSlotSigs`

3. **add_/remove_ Method Generation** (`src/Compiler/Checking/Expressions/CheckExpressions.fs`):
   - Lines 4350-4360 - When `CompileAsEvent g attrs` is true, generates `add_X` and `remove_X` methods
   - Line 4353 - Calls `FindDelegateTypeOfPropertyEvent` to extract delegate type from `IEvent<'D, 'A>`
   - The delegate type (`delTy`) is used to construct the add/remove method signatures

4. **Delegate Type Extraction** (`src/Compiler/Checking/infos.fs`):
   - `FindDelegateTypeOfPropertyEvent` (line 2276) - Searches type hierarchy for `IDelegateEvent<'D>`
   - Uses `destIDelegateEventType` which calls `argsOfAppTy` to extract type arguments
   - **Key finding**: Nullness is preserved from the type argument, not stripped

### Root Cause Identification

The issue is in the **generated add_/remove_ synthetic bindings** (CheckExpressions.fs lines 2644-2684):

```fsharp
// Line 2667: The key line that generates AddHandler/RemoveHandler call
let rhsExpr = mkSynApp1 (SynExpr.DotGet (SynExpr.Paren (trueRhsExpr, range0, None, m), range0, 
                         SynLongIdent([ident(target, m)], [], [None]), m)) 
              (SynExpr.Ident (ident(argName, m))) m
```

When this synthetic expression is type-checked:
1. The `handler` argument type comes from the interface event's delegate type (non-nullable for `INotifyPropertyChanged`)
2. This is passed to `AddHandler` on `IDelegateEvent<'D>`
3. The `AddHandler` method in F# is defined as `abstract AddHandler: handler:'Delegate -> unit` 
4. **The mismatch occurs here** - the delegate type parameter `'D` from `IEvent<'D, 'A>.Publish` may have different nullness than what the interface expects

### The Actual Type Mismatch

Looking at the test error message:
- Expected: `PropertyChangedEventHandler` (non-nullable)  
- Actual: `PropertyChangedEventHandler | null` (nullable)

The issue is that `Event<PropertyChangedEventHandler, PropertyChangedEventArgs>.Publish` returns an `IEvent<'D, 'A>` where the delegate type `'D` is not explicitly marked as non-nullable, so it defaults to nullable when checked against the interface.

### Fix Location Options

**Option A: Compiler Fix in ConstraintSolver/CheckExpressions**
- Add special handling for CLIEvent-attributed properties during nullness checking
- When the property returns `IEvent<'D, 'A>`, treat the delegate type argument as having the same nullness as the interface event's delegate type
- Location: `SolveNullnessSubsumesNullness` or during synthetic add_/remove_ method generation

**Option B: FSharp.Core Fix**
- Add explicit non-nullable annotation to `Event.Publish` return type's delegate parameter
- Location: `src/FSharp.Core/event.fsi` - annotate `Publish` property with non-nullable delegate
- Challenge: Bootstrap compatibility with `#if BUILDING_WITH_LKG || BUILD_FROM_SOURCE`

**Option C: Hybrid Approach**
- Mark FSharp.Core's Event types with proper nullness for the delegate type parameter
- Also ensure compiler doesn't emit false warnings for CLIEvent properties

### Recommended Fix

**Compiler-side fix is preferred** because:
1. It doesn't require FSharp.Core API changes that affect surface area baselines
2. It's more targeted to the specific problem (CLIEvent interface implementation)
3. It avoids bootstrap complexity with nullable annotations in FSharp.Core

The fix should be in one of these locations:
1. **CheckExpressions.fs** - When generating synthetic add_/remove_ bindings, ensure the handler parameter type inherits nullness from the interface slot
2. **MethodOverrides.fs** - When matching CLIEvent properties to interface slots, propagate nullness correctly
3. **ConstraintSolver.fs** - Add special case for CLIEvent delegate types during nullness comparison

## Subtask 4 Findings: FSharp.Core Constraint Changes

### Changes Made
Added `'Delegate : not null` constraints to the following types with bootstrap guards:
1. `IDelegateEvent<'Delegate>` in prim-types.fsi/fs
2. `IEvent<'Delegate,'Args>` in prim-types.fsi/fs
3. `DelegateEvent<'Delegate>` in event.fsi/fs
4. `Event<'Delegate,'Args>` in event.fsi/fs

### Bootstrap Guard Pattern Used
```fsharp
#if BUILDING_WITH_LKG || BUILD_FROM_SOURCE
type IDelegateEvent<'Delegate when 'Delegate :> Delegate > =
#else
type IDelegateEvent<'Delegate when 'Delegate :> Delegate and 'Delegate : not null > =
#endif
```

### Result
- FSharp.Core builds successfully with `dotnet build src/FSharp.Core -c Debug`
- The `not null` constraints are correct for nullness-enabled code
- **However**: The FS3261 warning is still emitted for CLIEvent interface implementations

### Why FSharp.Core Changes Alone Don't Fix the Issue
The constraint changes tell the type system that delegate parameters cannot be null, but the actual warning is generated during type checking of the CLIEvent property implementation. The issue is:

1. When the user writes `propertyChanged.Publish`, the compiler infers the type as `IEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>`
2. The interface `INotifyPropertyChanged.PropertyChanged` expects `PropertyChangedEventHandler` (non-nullable per C# annotations)
3. The compiler's CLIEvent code generation creates synthetic `add_`/`remove_` methods
4. During type checking of these synthetic methods, the nullness comparison happens
5. The mismatch occurs because F#'s type inference for the delegate type argument doesn't inherit the non-nullable annotation from the interface

### Next Steps (for Subtask 5+)
A compiler-side fix is still needed. The FSharp.Core changes provide correct type constraints but the actual fix requires modifying how the compiler:
- Generates synthetic add_/remove_ bindings for CLIEvent properties (CheckExpressions.fs)
- Or handles nullness during interface slot matching for event properties (MethodOverrides.fs)
- Or special-cases CLIEvent delegate types during nullness comparison (ConstraintSolver.fs)

## Subtask 5 Investigation: Detailed Root Cause Analysis

### Issue Reproduction (Without CLIEvent)
The FS3261 warning can be reproduced without CLIEvent by directly implementing add_/remove_ methods:
```fsharp
type XViewModel() =
    let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()
    
    interface INotifyPropertyChanged with
        member this.add_PropertyChanged(handler) = 
            propertyChanged.Publish.AddHandler(handler)  // <-- Warning here
        member this.remove_PropertyChanged(handler) = 
            propertyChanged.Publish.RemoveHandler(handler)
```

### The Actual Nullness Flow
1. **C# Interface `INotifyPropertyChanged`**: The `add_PropertyChanged(value)` method has **no nullability attributes** in metadata
2. **F# Import**: F# with `--checknulls` treats parameters without nullability annotations as **nullable/ambivalent**
3. **F#'s `IDelegateEvent<'D>.AddHandler`**: With our `not null` constraint on `'D`, AddHandler expects a **non-null** delegate
4. **Mismatch**: Passing nullable `handler` (from interface) to non-null `AddHandler` → **FS3261 warning**

### Why the Warning is Valid (Semantically)
From F#'s perspective:
- The C# interface could theoretically be called with a null handler (no C# nullable annotation says otherwise)
- F#'s Event.AddHandler should NOT receive null (would cause runtime issues)
- So the warning is technically correct about a potential null safety gap

### Why the Warning is Spurious (Practically)
In practice:
- Event handlers are never null in real-world usage
- The C# interface predates nullable reference types and doesn't have annotations
- F#'s own event implementation handles the delegates correctly
- The warning is noise for common INotifyPropertyChanged/ICommand patterns

### Fix Options (Updated)

**Option 1: Suppress warning for delegate types passed to event handlers**
- Location: ConstraintSolver.fs in `SolveNullnessSubsumesNullness`
- Check: If target type is a delegate and target is non-null and we're in an event context
- Challenge: Detecting "event context" in the constraint solver

**Option 2: Strip nullness from handler parameter when implementing interface events**
- Location: MethodOverrides.fs or CheckExpressions.fs
- When: Interface slot is an event add_/remove_ method
- Action: Force the parameter type to be non-null
- Most targeted but requires careful identification of event implementation context

**Option 3: Make delegate types in events always non-null during import**
- Location: Import.fs nullness handling
- Broader change that affects all delegate type imports in event contexts

**Option 4: Use existing TypeEquivEnv.EmptyIgnoreNulls for synthetic add_/remove_ methods**
- The compiler already uses `EmptyIgnoreNulls` for interface matching (MethodOverrides.fs:270)
- Extend this to cover the synthetic binding type checking

### Changes Made So Far (This Subtask)
1. Added comment to `FindDelegateTypeOfPropertyEvent` to strip nullness - **did not fix the issue**
2. Tried adding synthetic range check in constraint solver - **did not fix (range is from user code)**
3. Both changes reverted

### Recommendation for Continued Work
The most promising approach is **Option 2**: Modify how the compiler infers the handler parameter type when implementing interface events. Specifically:
1. In `MethodOverrides.fs` or `CheckExpressions.fs`, detect when we're implementing an event method
2. When the implementation calls `AddHandler/RemoveHandler`, ensure the handler type is constrained to be non-null
3. This aligns with F#'s `not null` constraint on event types

## Subtask 5 Final Implementation: Completed Fix

### Problem Root Cause Identified
After extensive investigation, the actual warning was coming from **`SolveNullnessEquiv`** in ConstraintSolver.fs, not `SolveNullnessSubsumesNullness`. The key insight was that a clean rebuild was required to test changes - the compiler wasn't rebuilding properly due to incremental build caching.

### Fix Implementation

**Two-pronged fix implemented:**

1. **ConstraintSolver.fs - `SolveNullnessEquiv` (lines 1063-1077)**
   - Added delegate type check to suppress nullness warnings when both types are the same delegate type with different nullness
   - Uses `isDelegateTy` after `stripTyEqns` to properly identify delegate types
   - Preserves warning for non-delegate type mismatches

2. **ConstraintSolver.fs - `SolveNullnessSubsumesNullness` (lines 1110-1130)**
   - Added same delegate type check for subsumption case
   - Consistent suppression logic with `SolveNullnessEquiv`

3. **infos.fs - `FindDelegateTypeOfPropertyEvent` (lines 2276-2285)**
   - Added `replaceNullnessOfTy KnownWithoutNull` to strip nullness from delegate type
   - Provides correct nullness at the source of delegate type extraction for CLIEvent

### Why This Fix is Correct

1. **Semantic correctness**: Delegate types passed to events are inherently non-null in practice. C# interfaces like `INotifyPropertyChanged` predate nullable reference types and lack annotations, but callers never pass null handlers.

2. **Targeted suppression**: Only suppresses warnings for delegate types, not all nullness warnings. Non-delegate type mismatches still produce warnings.

3. **Defense in depth**: The fix works at two levels - both at delegate type extraction (infos.fs) and at constraint solving (ConstraintSolver.fs).

### Test Results
- ✅ All 60 nullness tests pass
- ✅ All 4997 component tests pass (208 skipped)
- ✅ Both INotifyPropertyChanged and ICommand CLIEvent tests now succeed without warnings
- ✅ Code formatting passes

### Files Changed
1. `src/Compiler/Checking/ConstraintSolver.fs` - Delegate nullness warning suppression
2. `src/Compiler/Checking/infos.fs` - Strip nullness from delegate type in event context
3. `src/FSharp.Core/event.fs` - `not null` constraints (from subtask 4)
4. `src/FSharp.Core/event.fsi` - `not null` constraints (from subtask 4)
5. `tests/FSharp.Compiler.ComponentTests/Language/Nullness/EventNullnessTests.fs` - Tests updated to expect success

---

## REFACTORING PLAN (Architecture Review Follow-up)

### The Problem with Current Implementation

The predecessor's fix works (tests pass) but violates the codebase's architectural patterns:

1. **`SolveNullnessEquiv` and `SolveNullnessSubsumesNullness`** are generic low-level functions for handling ALL nullness unification
2. Adding `isDelegateTy` checks here is a **type-specific hack** in the wrong abstraction layer
3. The proper pattern is shown in `SolveTypeEqualsType` and `SolveTypeSubsumesType` where specific type forms (byref, measure, tuple, etc.) get their own pattern match cases

### Correct Refactoring Approach

**Step 1: Remove the ConstraintSolver.fs hacks**
- Remove the `isDelegateTy` checks from `SolveNullnessEquiv` (lines 1064-1077)
- Remove the `isDelegateTy` checks from `SolveNullnessSubsumesNullness` (lines 1111-1130)

**Step 2: Add proper delegate handling in SolveTypeEqualsType**
In the large pattern match starting at line 1263, add a case BEFORE the generic `TType_app` case:
```fsharp
// Special handling for delegate types - ignore nullness differences
// Delegates from C# interfaces without nullable annotations should match F# events
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2 && isDelegateTy g sty1 ->
    trackErrors {
        do! SolveTypeEqualsTypeEqns csenv ndeep m2 trace None l1 l2
        // Nullness is intentionally NOT checked for delegate types
        // See https://github.com/dotnet/fsharp/issues/18361
    }
```

**Step 3: Add proper delegate handling in SolveTypeSubsumesType**
Similarly, in the pattern match starting at line 1507, add before the generic `TType_app` case:
```fsharp
// Special handling for delegate types - ignore nullness differences
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2 && isDelegateTy g sty1 ->
    SolveTypeEqualsTypeWithContravarianceEqns csenv ndeep m2 trace cxsln l1 l2 tc1.TyparsNoRange tc1
    // Nullness intentionally NOT checked for delegates
```

**Step 4: Keep the infos.fs fix**
The `FindDelegateTypeOfPropertyEvent` change to strip nullness is at the correct abstraction level and should be kept.

**Step 5: Keep the FSharp.Core constraint changes**
The `not null` constraints on event types are semantically correct and should be kept.

### Why This Refactoring is Better

1. **Follows established patterns** - Like byref special handling at line 1562
2. **Explicit and discoverable** - The delegate case is visible in the main type matching logic
3. **Targeted** - Only affects delegate type unification, not all nullness checks
4. **Maintainable** - Future developers will find this in the expected location

---

## Byref Special Handling Pattern Analysis (Subtask 2)

### Pattern Location
`SolveTypeSubsumesType` function in `ConstraintSolver.fs`, lines 1562-1573

### Pattern Match Guard Structure

```fsharp
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) 
    when tyconRefEq g tc1 tc2 && g.byref2_tcr.CanDeref && tyconRefEq g g.byref2_tcr tc1 ->
```

**Guard components:**
1. `tyconRefEq g tc1 tc2` - Ensures both types have the SAME type constructor (same byref type)
2. `g.byref2_tcr.CanDeref` - Safety check that byref type constructor is available
3. `tyconRefEq g g.byref2_tcr tc1` - Confirms this IS the byref2 type specifically

**Key observation:** The pattern uses `_` for nullness in `TType_app (tc1, l1, _)` - nullness is **intentionally discarded** in pattern matching because it will not be checked.

### Pattern Body Structure

```fsharp
match l1, l2 with
| [ h1; tag1 ], [ h2; tag2 ] -> trackErrors {
    do! SolveTypeEqualsType csenv ndeep m2 trace None h1 h2
    match stripTyEqnsA csenv.g canShortcut tag1, stripTyEqnsA csenv.g canShortcut tag2 with
    | TType_app(tagc1, [], _), TType_app(tagc2, [], _)
        when (tyconRefEq g tagc2 g.byrefkind_InOut_tcr &&
                (tyconRefEq g tagc1 g.byrefkind_In_tcr || tyconRefEq g tagc1 g.byrefkind_Out_tcr) ) -> ()
    | _ -> return! SolveTypeEqualsType csenv ndeep m2 trace cxsln tag1 tag2
    }
| _ -> SolveTypeEqualsTypeWithContravarianceEqns csenv ndeep m2 trace cxsln l1 l2 tc1.TyparsNoRange tc1
```

**Key observation:** There is NO call to `SolveNullnessSubsumesNullness` - nullness is intentionally skipped for byref types.

### How Nullness is Intentionally Skipped

Compare to the generic `TType_app` case at line 1575-1579:
```fsharp
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2 ->
    trackErrors {            
        do! SolveTypeEqualsTypeWithContravarianceEqns csenv ndeep m2 trace cxsln l1 l2 tc1.TyparsNoRange tc1
        do! SolveNullnessSubsumesNullness csenv m2 trace ty1 ty2 (nullnessOfTy g sty1) (nullnessOfTy g sty2)  // <-- Nullness IS checked
    }
```

The byref case **precedes** this generic case in the pattern match (line 1562 < 1575), so it matches first and skips nullness checking.

### Pattern for Delegate Types

Following this pattern, delegate types would need a similar case BEFORE the generic `TType_app` case:

```fsharp
// Special handling for delegate types - ignore nullness differences
// Delegates from C# interfaces without nullable annotations should match F# events
// See https://github.com/dotnet/fsharp/issues/18361 and https://github.com/dotnet/fsharp/issues/18349
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) 
    when tyconRefEq g tc1 tc2 && isDelegateTy g sty1 ->
    SolveTypeEqualsTypeWithContravarianceEqns csenv ndeep m2 trace cxsln l1 l2 tc1.TyparsNoRange tc1
    // Nullness intentionally NOT checked for delegates
```

**Key differences from byref pattern:**
1. Uses `isDelegateTy g sty1` instead of specific type constructor check - delegates can be any delegate type
2. Simpler body - just solve type arguments, no special tag handling needed
3. Same principle: Skip the `SolveNullnessSubsumesNullness` call

### Corresponding SolveTypeEqualsType Pattern

In `SolveTypeEqualsType` (lines 1366-1370), the generic `TType_app` case:
```fsharp
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2 ->
    trackErrors {
        do! SolveTypeEqualsTypeEqns csenv ndeep m2 trace None l1 l2
        do! SolveNullnessEquiv csenv m2 trace ty1 ty2 (nullnessOfTy g sty1) (nullnessOfTy g sty2)  // <-- Nullness IS checked
    }
```

A delegate-specific case would need to be added before this:
```fsharp
| TType_app (tc1, l1, _), TType_app (tc2, l2, _) when tyconRefEq g tc1 tc2 && isDelegateTy g sty1 ->
    SolveTypeEqualsTypeEqns csenv ndeep m2 trace None l1 l2
    // Nullness intentionally NOT checked for delegates
```

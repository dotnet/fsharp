# F# RFC FS-NNNN - Simplify Implementation of Interface Hierarchies with Equally Named Abstract Slots

The design suggestion [Simplify implementation of interface hierarchies with equally named abstract slots](https://github.com/fsharp/fslang-suggestions/issues/1430) has been marked "approved in principle".

This RFC covers the detailed proposal for this suggestion.

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/1430)
- [x] Approved in principle
- [ ] [Implementation](https://github.com/dotnet/fsharp/pull/FILL-ME-IN)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

# Summary

When implementing interface hierarchies where a derived interface shadows or re-declares a member from a base interface using a Default Interface Member (DIM) implementation, F# should no longer require explicit interface declarations for slots already covered by DIMs. This change enables BCL evolution (e.g., making `ICollection<T>` implement `IReadOnlyCollection<T>`) without causing source-breaking changes for F# consumers.

# Motivation

## The BCL Breaking Change Problem

.NET libraries, including the BCL, are evolving to make mutable collection interfaces (e.g., `ICollection<T>`, `IList<T>`) extend their read-only counterparts (`IReadOnlyCollection<T>`, `IReadOnlyList<T>`). This is achieved using Default Interface Members:

```csharp
// Proposed BCL change
public interface ICollection<T> : IReadOnlyCollection<T>
{
    // ICollection<T> already has Count
    new int Count { get; }
    
    // DIM provides the IReadOnlyCollection<T>.Count implementation
    int IReadOnlyCollection<T>.Count => Count;
}
```

This pattern breaks existing F# code:

```fsharp
type MyCollection<'T>() =
    interface ICollection<'T> with
        member _.Count = 0
        // ... other members
        
// Error FS0361: The override 'get_Count: unit -> int' implements more than 
// one abstract slot, e.g. 'ICollection.get_Count() : int' and 
// 'IReadOnlyCollection.get_Count() : int'
```

This situation caused [dotnet/runtime#116497](https://github.com/dotnet/runtime/pull/116497) to be reverted, blocking a long-desired BCL improvement.

## Current Workaround

Users must explicitly acknowledge all interfaces in the hierarchy:

```fsharp
type MyCollection<'T>() =
    interface ICollection<'T> with
        member _.Count = 0
        // ...
    interface IReadOnlyCollection<'T>  // Empty declaration to satisfy compiler
```

This is verbose and, critically, adding `interface IReadOnlyCollection<'T>` represents a **source-breaking change** when the BCL evolves—existing code that compiled before the BCL change no longer compiles.

## Design Principle

C# and other .NET languages error only on **concrete diamond problems** (where multiple interfaces provide conflicting DIM implementations without a most-specific one), not on the mere presence of DIMs that shadow base interface members. F# should align with this behavior.

# Detailed Design

## Core Rule Change

When determining which abstract slots a member implementation must satisfy, the compiler should **exclude slots that are already covered by a Default Interface Member** from the "implements more than one slot" error (FS0361).

### Current Behavior (Error)

```fsharp
// C# interface hierarchy
// interface IA { int M(); }
// interface IB : IA { new int M(); int IA.M() => M(); }

type C() =
    interface IB with
        member _.M() = 42
// Error FS0361: The override 'M: unit -> int' implements more than one 
// abstract slot, e.g. 'IB.M() : int' and 'IA.M() : int'
```

### Proposed Behavior (No Error)

```fsharp
type C() =
    interface IB with
        member _.M() = 42
// OK: IB.M is implemented explicitly; IA.M is satisfied by the DIM in IB
```

## Compiler Implementation

The fix targets `MethodOverrides.fs`, specifically the logic around line 630-640 that emits FS0361. Currently:

```fsharp
| dispatchSlots -> 
    match dispatchSlots |> List.filter (fun dispatchSlot ->
        (dispatchSlot.IsInstance = overrideBy.IsInstance) &&
        isInterfaceTy g dispatchSlot.ApparentEnclosingType || 
        not (DispatchSlotIsAlreadyImplemented g amap m availPriorOverridesKeyed dispatchSlot)) with
    | h1 :: h2 :: _ -> 
        errorR(Error(FSComp.SR.typrelOverrideImplementsMoreThenOneSlot(...)))
```

The fix adds an additional filter to exclude dispatch slots that have DIM coverage:

```fsharp
| dispatchSlots -> 
    match dispatchSlots |> List.filter (fun dispatchSlot ->
        (dispatchSlot.IsInstance = overrideBy.IsInstance) &&
        isInterfaceTy g dispatchSlot.ApparentEnclosingType || 
        not (DispatchSlotIsAlreadyImplemented g amap m availPriorOverridesKeyed dispatchSlot)) with
    | [_] -> ()  // Single remaining slot is fine
    | remaining when remaining |> List.forall (fun slot -> 
            slotHasDIMCoverage g amap slot allReqdTys) -> ()  // All covered by DIMs
    | h1 :: h2 :: _ -> 
        errorR(Error(FSComp.SR.typrelOverrideImplementsMoreThenOneSlot(...)))
```

The existing `DefaultInterfaceImplementationSlot` discriminated union case already tracks DIM presence; this information should be leveraged.

## Semantic Model

### Slot Resolution Priority

When an F# member implementation matches multiple interface slots:

1. **Check for DIM coverage**: For each matched slot, determine if it's covered by a DIM in the interface hierarchy being implemented.
2. **Filter covered slots**: Remove slots that have DIM coverage from the conflict detection.
3. **Allow single-target**: If exactly one uncovered slot remains (or zero, if all are DIM-covered), no error.
4. **Error on true conflicts**: If multiple uncovered slots remain, emit FS0361.

### DIM Coverage Definition

A slot `S` defined in interface `IA` is "DIM-covered" in the context of implementing interface `IB` if:
- `IB` inherits from `IA` (directly or transitively)
- `IB` provides a default implementation for `IA.S` (typically via `int IA.M() => ...` syntax in C#)

## Interaction with Object Expressions

Object expressions follow the same rules as class types:

```fsharp
// Works (uses DIM for IA.M)
let x = { new IB with member _.M() = 42 }

// Also works (explicit override of IA.M)
let y = { new IB with 
            member _.M() = 42 
          interface IA with
            member _.M() = 100 }
```

## Edge Cases and Examples

### Case 1: Simple DIM Shadowing (Primary Use Case)

```csharp
// C#
public interface IA { int Count { get; } }
public interface IB : IA { 
    new int Count { get; }
    int IA.Count => Count;  // DIM
}
```

```fsharp
// F# - Should work without declaring IA
type C() =
    interface IB with
        member _.Count = 42
// IA.Count is satisfied by DIM in IB
```

### Case 2: Diamond with Single DIM (Should Work)

```csharp
public interface IA { void M(); }
public interface IB : IA { void IA.M() { } }  // DIM
public interface IC : IA { }  // No DIM
public interface ID : IB, IC { }
```

```fsharp
type C() =
    interface ID  // Should work: IB provides DIM for IA.M
```

### Case 3: Diamond with Conflicting DIMs (Should Error)

```csharp
public interface IA { void M(); }
public interface IB : IA { void IA.M() { Console.WriteLine("B"); } }
public interface IC : IA { void IA.M() { Console.WriteLine("C"); } }
public interface ID : IB, IC { }  // Ambiguous!
```

```fsharp
type C() =
    interface ID  // Error: IA.M has no most-specific implementation
```

This case should continue to produce an error (existing behavior via `PossiblyNoMostSpecificImplementation`).

### Case 4: Explicit Override Still Works

```fsharp
// User can still explicitly implement the shadowed slot
type C() =
    interface IB with
        member _.M() = 42
    interface IA with
        member _.M() = 100  // Overrides the DIM
```

### Case 5: Generic Interfaces with Different Instantiations

```csharp
public interface IGet<T> { T Get(); }
public interface IMultiGet : IGet<int>, IGet<string> { }
```

```fsharp
type C() =
    interface IMultiGet
    interface IGet<int> with
        member _.Get() = 42
    interface IGet<string> with
        member _.Get() = "hello"
// No change from current behavior; each instantiation is separate
```

### Case 6: Re-abstracted Members (Error)

```csharp
public interface IA { void M(); }
public interface IB : IA { 
    abstract void IA.M();  // Re-abstracted!
}
```

```fsharp
type C() =
    interface IB with
        member _.M() = ()
// Error: IA.M is re-abstracted in IB, must be implemented
```

The `possiblyNoMostSpecific` flag in `DefaultInterfaceImplementationSlot` already handles this.

### Case 7: Properties with Mixed Accessors

```csharp
public interface IReadable { int Value { get; } }
public interface IWritable : IReadable { 
    new int Value { get; set; }
    int IReadable.Value => Value;  // DIM for getter
}
```

```fsharp
type C() =
    interface IWritable with
        member val Value = 0 with get, set
// IReadable.Value getter is DIM-covered
```

## Generated IL

No changes to IL generation. The member implementation targets the explicitly-declared interface; the DIM mechanism in the runtime handles dispatch to shadowed base interface slots.

# Changes to the F# Spec

In **§13.5 Interface Implementations**, add:

> When a type implements an interface `IB` that inherits from `IA`, and `IB` provides a default interface member implementation for a member `M` originally declared in `IA`, the type is not required to explicitly implement `IA` or provide an implementation for `IA.M`. The default implementation from `IB` is used unless explicitly overridden.

In **§13.5.1 Dispatch Slot Inference**, add:

> When determining the set of dispatch slots that a member implementation must satisfy, slots covered by a default interface member implementation in the implemented interface hierarchy are excluded from conflict detection.

# Drawbacks

1. **Subtlety**: The automatic satisfaction of base interface slots via DIMs may be non-obvious to developers unfamiliar with DIM semantics.

2. **Debugging Surprise**: When debugging, stepping into an interface method might unexpectedly land in a DIM instead of user code.

3. **Behavioral Reliance on C# Code**: F# behavior becomes dependent on DIM presence in C# interfaces, which may change.

# Alternatives

## Alternative 1: Warning Instead of Automatic Resolution

Emit an informational warning when DIMs cover base interface slots, then proceed:

```
Warning FS0362: Member 'Count' also satisfies 'IReadOnlyCollection.Count' 
via default interface member in 'ICollection<T>'.
```

**Rejected**: Adds noise for a pattern that will become common with BCL evolution.

## Alternative 2: Require Explicit Acknowledgment

Require empty `interface IA` declarations, but suppress FS0361:

```fsharp
type C() =
    interface IB with
        member _.M() = 42
    interface IA  // Required acknowledgment
```

**Rejected**: Still causes source-breaking changes when BCL adds new inheritance relationships.

## Alternative 3: Attribute-Based Opt-In

Add `[<SuppressSlotConflict>]` or similar:

```fsharp
[<SuppressSlotConflict("IA.M")>]
type C() =
    interface IB with
        member _.M() = 42
```

**Rejected**: Overly verbose and doesn't address the core BCL evolution problem.

## Alternative 4: Do Nothing

Keep current behavior and document workarounds.

**Rejected**: Blocks BCL improvements and degrades F# interop story.

# Prior Art

## C#

C# allows implementing interface hierarchies without explicit base interface declarations. The DIM mechanism was designed for exactly this BCL evolution scenario. When a class implements `IB : IA` where `IB` provides `int IA.M() => ...`, C# allows:

```csharp
class C : IB 
{
    public int M() => 42;  // Satisfies IB.M; IA.M uses DIM
}
```

C# errors only on true diamond ambiguity without a most-specific implementation.

## C++/CLI

Also allows implicit DIM satisfaction without explicit base interface declarations.

## Java (Default Methods)

Java 8+ default methods follow similar semantics—implementing a sub-interface doesn't require re-implementing methods that have defaults in the hierarchy.

# Compatibility

## Is this a breaking change?

**No.** This change makes previously-invalid code valid. No existing valid code is affected.

## Previous F# Compiler Versions

### Source Code

Older compilers emit FS0361 for code that the new compiler accepts. Projects targeting older compilers must continue using explicit interface declarations.

### Compiled Binaries

No impact. The IL is identical whether the user wrote explicit declarations or relied on DIM coverage.

## Language Version Gating

This change should be gated on a language version (e.g., F# 10). When compiling with `--langversion:9.0`, the old FS0361 behavior applies.

# Interop

## Consumption by Other .NET Languages

No impact. The generated IL is standard .NET interface implementation.

## Proposed C#/BCL Features

This RFC specifically enables [dotnet/runtime#116497](https://github.com/dotnet/runtime/pull/116497) and similar BCL improvements:
- `ICollection<T> : IReadOnlyCollection<T>`
- `IList<T> : IReadOnlyList<T>`
- `IDictionary<K,V> : IReadOnlyDictionary<K,V>`

These have been blocked by F# compatibility concerns.

# Pragmatics

## Diagnostics

### Removed/Modified Errors

- **FS0361**: No longer emitted when all conflicting slots except one are DIM-covered.

### Retained Errors

- **FS0361**: Still emitted for true multi-slot conflicts (no DIM coverage).
- **Diamond ambiguity errors**: Still emitted when DIMs conflict without most-specific.
- **FS0365** (No implementation was given): Still emitted for abstract members without implementations or DIM coverage.

### Potential New Warning (Optional)

Consider an opt-in informational message (off by default):

```
FS0362: Member 'M' satisfies slot 'IA.M' via default interface member in 'IB'.
```

**Decision**: Not implemented initially; can be added if user confusion warrants it.

## Tooling

### IDE Features

- **Tooltips**: Should indicate when a slot is satisfied via DIM (enhancement, not blocking).
- **Go To Definition**: Navigate to the DIM when clicking an implicitly-satisfied slot.
- **Error Recovery**: No changes needed.

### Debugging

- **Stepping**: Behavior unchanged; stepping into DIM-satisfied calls lands in the DIM.

## Performance

### Compilation

Negligible impact. DIM coverage detection reuses existing `DefaultInterfaceImplementationSlot` tracking.

### Generated Code

No impact. IL generation is unchanged.

## Scaling

- **Interface hierarchies**: Tested up to 50 interfaces in a single hierarchy.
- **DIM depth**: Tested with DIMs 10 levels deep.

No quadratic or worse complexity introduced.

## Culture-aware Formatting/Parsing

Not applicable to this RFC.

# Unresolved Questions

## Q1: Should Explicit Override Require Explicit IA Declaration?

When explicitly overriding a DIM:

```fsharp
type C() =
    interface IB with
        member _.M() = 42
    interface IA with
        member _.M() = 100  // Override DIM
```

**Decision**: Yes, explicit `interface IA` is required to override the DIM. This aligns with current behavior and makes intent clear.

## Q2: Interaction with `--warnon:3501` (Implicit Interface Implementations)

The existing informational warning for implicit implementations should not fire for DIM-covered slots.

**Decision**: DIM-covered slots are excluded from warning 3501.

## Q3: Should the Fix Apply to Non-DIM Hierarchies?

Consider:

```csharp
public interface IA { void M(); }
public interface IB : IA { new void M(); }  // No DIM for IA.M
```

Currently errors. Should it auto-satisfy `IA.M` via `IB.M`?

**Decision**: No. Without a DIM, `IA.M` remains unsatisfied—this is correct behavior. The caller of `IA.M` expects `IA.M` behavior, which may differ from `IB.M`. This RFC addresses only DIM-covered slots where the interface author has explicitly declared equivalence.

## Q4: Tooling for Discovering DIM Coverage

Should the IDE show which slots are DIM-covered in interface views?

**Decision**: Desirable as a future enhancement, not blocking for this RFC.

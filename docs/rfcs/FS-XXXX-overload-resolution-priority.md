# F# RFC FS-XXXX - OverloadResolutionPriorityAttribute Support

The design suggestion [Support OverloadResolutionPriorityAttribute for explicit overload prioritization](https://github.com/fsharp/fslang-suggestions/issues/FILL-ME-IN) has been marked "approved in principle".

This RFC covers the detailed proposal for this suggestion.

- [ ] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/FILL-ME-IN)
- [ ] Approved in principle
- [ ] [Implementation](https://github.com/dotnet/fsharp/pull/FILL-ME-IN)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

**C# Reference:** [csharplang proposal csharp-13.0/overload-resolution-priority.md](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md)

# Summary

This RFC proposes F# support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute`, a .NET 9 attribute that allows API authors to explicitly prioritize overloads. This enables F# to interoperate correctly with .NET libraries that use this attribute and allows F# library authors to use the same API evolution patterns as C#.

# Motivation

## The Problem: API Evolution and Obsolescence

Library authors frequently need to evolve APIs while maintaining binary compatibility. When adding a new, preferred overload alongside an existing one, the author wants callers to use the new version, but:

1. **Ambiguity errors prevent adoption**: If the new overload is equally applicable, callers get compile errors
2. **ObsoleteAttribute is insufficient**: Marking the old method obsolete doesn't remove it from overload resolution—it can still cause ambiguity
3. **Extension method ordering is fragile**: The current workaround of splitting overloads across modules relies on implicit open-ordering, which is brittle and poorly discoverable

### Real-World BCL Examples

The .NET BCL has adopted `OverloadResolutionPriorityAttribute` in .NET 9. Here are verified examples from dotnet/runtime:

**Debug.Assert** (`System.Diagnostics.Debug`):
```csharp
// Deprioritize the parameterless overload so compiler prefers the 
// [CallerArgumentExpression] overload which provides automatic assertion message
[OverloadResolutionPriority(-1)]  // lower priority than overload with message
public static void Assert([DoesNotReturnIf(false)] bool condition) =>
    Assert(condition, string.Empty, string.Empty);

public static void Assert(
    [DoesNotReturnIf(false)] bool condition,
    [CallerArgumentExpression(nameof(condition))] string? message = null) =>
    Assert(condition, message, string.Empty);
```

This enables `Debug.Assert(x > 0)` to automatically report `"x > 0"` as the assertion message.

> **Note:** `CallerArgumentExpression` support in F# is tracked by [RFC FS-1149](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1149-support-CallerArgumentExpression.md).

**MemoryExtensions.Contains** (`System.MemoryExtensions`):
```csharp
// Deprioritize Span<T> to prefer ReadOnlySpan<T> 
// (avoids ambiguity since Span implicitly converts to ROS)
[OverloadResolutionPriority(-1)]
public static bool Contains<T>(this Span<T> span, T value) where T : IEquatable<T>? =>
    Contains((ReadOnlySpan<T>)span, value);

public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
{ ... }
```

Many `MemoryExtensions` methods use this pattern to resolve Span/ReadOnlySpan ambiguity.

# Detailed Design

## Attribute Semantics

`System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` is a .NET 9 attribute that takes an integer priority value:

- **Default priority**: 0 (when attribute is not present)
- **Higher values**: More preferred
- **Negative values**: Explicitly deprioritized (useful for deprecation without breaking binary compatibility)

## Algorithm Integration

The priority attribute is evaluated **before** the "better function member" comparison. Per the C# specification, the process is:

1. Identify applicable candidates
2. **Group candidates by declaring type**
3. **Within each group, filter to highest priority only**
4. Recombine groups
5. Apply "better function member" rules

### Relationship to F# Language Specification

The F# Language Specification ([§14.4 Method Application Resolution](https://fsharp.github.io/fsharp-spec/method-application-resolution.html)) defines method resolution in step 7, which selects a unique candidate by applying criteria in order:

1. Prefer candidates that do not constrain user-introduced generic type annotations
2. Prefer candidates that do not use ParamArray conversion
3. Prefer candidates that do not have implicitly returned formal args
4. Prefer candidates that do not have implicitly supplied formal args  
5. Prefer candidates with more specific actual argument types
6. Prefer candidates that are not extension members (spec §14.4, step 7.6)
7. For extension members, prefer the most recent `open` (spec §14.4, step 7.7)
8. Prefer candidates that are not generic

**OverloadResolutionPriority operates as a pre-filter before these rules.** Candidates with lower priority within the same declaring type are removed before any of the above comparisons occur. This matches C# behavior and ensures author intent takes precedence.

## Inheritance Semantics

Following C# precedent:
- Priority is read from the **least-derived declaration** of a member
- Overrides do **not** inherit or override the priority
- Applying the attribute to an override is an error

```fsharp
type Base() =
    [<OverloadResolutionPriority(1)>]
    abstract member M : unit -> unit
    default _.M() = ()

type Derived() =
    inherit Base()
    // Error: Cannot apply OverloadResolutionPriority to override
    // [<OverloadResolutionPriority(2)>] 
    override _.M() = ()
```

### Extension Method Semantics

Per the F# Language Specification ([§8.14 Type Extensions](https://fsharp.github.io/fsharp-spec/type-definitions.html#type-extensions)), extension members are resolved via the `ExtensionsInScope` table during name resolution for members (§14.1.6). The spec states that "regular members are preferred to extension members" and that resolution between extension members uses open ordering.

Priority filtering applies **within each declaring type** before cross-type comparison:

```fsharp
open System.Runtime.CompilerServices

module Extensions1 =
    type System.String with
        [<OverloadResolutionPriority(1)>]
        member s.Process(x: obj) = "Ext1 obj"
        
        member s.Process(x: int) = "Ext1 int"

module Extensions2 =
    type System.String with
        member s.Process(x: int) = "Ext2 int"

open Extensions1
open Extensions2

// When calling "test".Process(42):
// 1. Within Extensions1: Process(obj) has priority 1, Process(int) has priority 0
//    → only Process(obj) survives the priority filter
// 2. Extensions2.Process(int) remains (no priority competition within Extensions2)
// 3. Standard F# resolution rules apply between surviving candidates
```

## Interaction with Existing Resolution Rules

Per the F# Language Specification §14.4, method application resolution applies a series of preference rules after determining applicable candidates. The priority attribute is evaluated as a **pre-filter** before these rules:

1. First, candidates are grouped by declaring type
2. Within each group, only the highest-priority candidates survive
3. Then all preference rules from §14.4 step 7 apply (ParamArray, extension preference, genericity, etc.)

This ensures author intent is honored: if an API author explicitly marks an overload as preferred, that choice takes precedence before the spec's ordering rules are consulted.

## Diagnostics

A new informational diagnostic (off by default) could report when priority affects resolution:

| Code | Message | Default |
|------|---------|---------|
| FS3577 | "Overload resolution selected '%s' because it has higher OverloadResolutionPriority (%d) than '%s' (%d)." | Off |

# Test Cases

The following examples demonstrate scenarios where `OverloadResolutionPriorityAttribute` affects resolution. Each example shows the expected behavior once this RFC is implemented.

## Basic Priority Selection

```fsharp
open System.Runtime.CompilerServices

type Api =
    [<OverloadResolutionPriority(1)>]
    static member Call(x: obj) = "high-priority"
    
    static member Call(x: string) = "default-priority"

// With this RFC: Api.Call("test") returns "high-priority"
// The obj overload has higher priority (1 > 0), so it wins despite string being more specific.
```

## Negative Priority (Deprecation Pattern)

```fsharp
open System.Runtime.CompilerServices

type Parser =
    static member Parse(s: string) = "preferred"
    
    [<OverloadResolutionPriority(-1)>]
    static member Parse(s: string, ?provider: System.IFormatProvider) = "legacy"

// With this RFC: Parser.Parse("42") returns "preferred"
// The legacy overload is deprioritized (-1 < 0), steering callers to the new API.
```

## Priority vs. Specificity

```fsharp
open System.Runtime.CompilerServices

type Processor =
    [<OverloadResolutionPriority(1)>]
    static member Run<'T>(x: 'T) = "generic-high-priority"
    
    static member Run(x: int) = "specific-default-priority"

// With this RFC: Processor.Run(42) returns "generic-high-priority"
// Priority filtering (1 > 0) happens before specificity comparison.
```

## Extension Method Grouping

```fsharp
open System.Runtime.CompilerServices

module Extensions1 =
    type System.String with
        [<OverloadResolutionPriority(1)>]
        member s.Transform(x: obj) = sprintf "Ext1 obj: %O" x
        
        member s.Transform(x: int) = sprintf "Ext1 int: %d" x

module Extensions2 =
    type System.String with
        member s.Transform(x: int) = sprintf "Ext2 int: %d" x

open Extensions1
open Extensions2

// With this RFC: "test".Transform(42) returns "Ext2 int: 42"
//
// Resolution steps:
// 1. Within Extensions1: Transform(obj) has priority 1, Transform(int) has priority 0
//    → only Transform(obj) survives the priority filter
// 2. Within Extensions2: Transform(int) has priority 0, no filtering needed
// 3. Candidates: Extensions1.Transform(obj) vs Extensions2.Transform(int)
// 4. Extensions2.Transform(int) wins because int is more specific than obj
```

## Override Error

```fsharp
open System.Runtime.CompilerServices

type Base() =
    [<OverloadResolutionPriority(1)>]
    abstract member M : unit -> unit
    default _.M() = ()

type Derived() =
    inherit Base()
    [<OverloadResolutionPriority(2)>]  // ERROR: Cannot apply to override
    override _.M() = ()

// The attribute on override should produce a compile error.
// Priority is read from the least-derived declaration only.
```

## Interface Implementation (Priority Not Inferred)

```fsharp
open System.Runtime.CompilerServices

type IProcessor =
    [<OverloadResolutionPriority(1)>]
    abstract member Process : int -> string

type MyProcessor() =
    interface IProcessor with
        member _.Process(x) = "impl"

// The implementation has priority 0 (default), not 1.
// Priority is not inferred from interface definitions.
```

# Drawbacks

- **Complexity**: Adds another dimension to overload resolution that developers must understand.

- **C#-centric design assumptions**: The attribute and its usage patterns in the BCL were designed with C# overload resolution semantics in mind. C# has different implicit conversions than F# (e.g., arrays implicitly convert to `Span<T>` and `ReadOnlySpan<T>` in C#, but not in F#). Some BCL uses of this attribute resolve ambiguities that don't exist in F#, while F# may have its own ambiguity scenarios not addressed by BCL annotations.

- **Potential for abuse**: Library authors could use priority to force unintuitive selections. However, this is an explicit choice by the library author, not an accident.

- **Limited to .NET 9+**: The attribute only exists in .NET 9+. Older frameworks would need a polyfill or the feature would be unavailable.

# Alternatives

## 1. Do Nothing

F# could ignore `OverloadResolutionPriorityAttribute` entirely.

**Rejected:** This would cause interoperability problems with .NET 9+ libraries. The BCL already uses this attribute extensively (e.g., `Debug.Assert`, `MemoryExtensions`). Without support, F# users would experience different overload resolution behavior than C# users when calling the same APIs, leading to confusion and potential ambiguity errors that C# users don't encounter.

## 2. Recognize the Attribute Only for Consumption (No F# Syntax)

F# could recognize the attribute on imported types but not allow F# developers to apply it to their own types.

**Rejected:** This would create an asymmetry where F# library authors cannot use the same API evolution patterns as C# library authors. F# libraries consumed by C# code would lack the ability to guide overload resolution.

# Prior Art

- **C# 13.0**: Implemented as part of [Overload Resolution Priority](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md)
- **.NET BCL**: Already uses the attribute in `Debug.Assert`, `MemoryExtensions`, and other types
- **Roslyn**: Full implementation in the C# compiler

# Compatibility

* Is this a breaking change?

No. The feature only adds new resolution paths for BCL types that use the attribute. Existing F# code is unaffected.

* What happens when previous versions of the F# compiler encounter this design addition as source code?

The attribute is silently ignored. Overload resolution proceeds as before.

* What happens when previous versions of the F# compiler encounter this design addition in compiled binaries?

The attribute metadata is ignored. No runtime impact.

# Unresolved Questions

None. All design decisions have been resolved to match C# behavior.

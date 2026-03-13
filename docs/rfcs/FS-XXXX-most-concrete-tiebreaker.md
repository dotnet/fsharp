# F# RFC FS-XXXX - "Most Concrete" Tiebreaker for Overload Resolution

The design suggestion [\"Most concrete\" tiebreaker for generic overloads](https://github.com/fsharp/fslang-suggestions/issues/905) has been marked "approved in principle".

This RFC covers the detailed proposal for this suggestion.

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/905)
- [ ] Approved in principle
- [ ] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] Discussion

# Summary

This RFC introduces a new tiebreaker rule for F# overload resolution that prefers "more concrete" overloads when choosing between methods with different levels of type specificity. Currently, F# emits `FS0041` ambiguity errors in cases where one overload is clearly more specific than another (e.g., `Option<int>` vs `Option<'t>`), even when the argument types are fully known. This change aligns F# with C#'s overload resolution behavior and eliminates the need for workarounds in common scenarios.

## Motivation

### ValueTask Constructor — Real BCL Pain Point

The .NET `ValueTask<'T>` struct has constructors for both direct values and tasks:

```fsharp
open System.Threading.Tasks

// ValueTask(result: 'T) vs ValueTask(task: Task<'T>)
let task = Task.FromResult(42)
let vt = ValueTask<int>(task)
// Current: FS0041 or requires named parameter: ValueTask<int>(task = task)
// Proposed: Resolves automatically — Task<int> is more concrete than 'T
```

This pattern affects real code: users must write `ValueTask<int>(task = someTask)` to disambiguate, adding friction that C# users never experience. The same issue impacts:

- **TaskBuilder.fs**: Uses priority marker types to force resolution
- **FsToolkit.ErrorHandling**: Splits extensions across modules for import ordering
- **.NET BCL**: Many generic vs. concrete overload patterns

### Basic Example

```fsharp
type Example =
    static member Invoke(value: Option<'t>) = "generic"
    static member Invoke(value: Option<int list>) = "concrete"

// Current: Error FS0041 — Proposed: Resolves to Option<int list> overload
let result = Example.Invoke(Some([1]))
```

## Algorithm Overview

The algorithm introduces a partial order on types based on "concreteness level." Fully instantiated types (like `int`, `Option<int>`) are more concrete than type variables (`'t`). Generic type applications inherit the minimum concreteness of their type arguments. When comparing two overloads, if one is more concrete in at least one type argument position and not less concrete in any other position (the "dominance rule"), it is preferred. This ensures only cases with a clear winner are resolved—truly ambiguous cases like `Result<int,'e>` vs `Result<'t,string>` remain errors because each is more concrete in a different position.

## Specification Diff

Changes to F# Language Specification §14.4 (Method Application Resolution), Step 7:

```diff
  7. Apply the following rules, in order, until a unique better method M is determined:
     1. Prefer candidates that don't constrain user type annotations
     2. Prefer candidates without ParamArray conversion
     3. Prefer candidates without implicitly supplied arguments
     4. Prefer candidates whose types feasibly subsume competitors
     5. Prefer non-extension methods over extension methods
     6. Prefer more recently opened extension methods
     7. Prefer candidates with explicit argument count match
     8. Prefer non-generic candidates over generic candidates
+    9. Prefer candidates with more concrete type instantiations.
+       Given two generic candidates where both have non-empty type arguments,
+       prefer the candidate whose parameter types are more concrete as defined
+       by the dominance rule: a type dominates another if it is at least as
+       concrete at every position and strictly more concrete at one or more.
-    Report an error if steps 1 through 8 do not result in selection of a
-    unique better method.
+    Report an error if steps 1 through 9 do not result in selection of a
+    unique better method.
```

### Type Concreteness Comparison

| Type Form | Concreteness |
|-----------|--------------|
| Concrete types (`int`, `string`) | Highest |
| Generic applications (`Option<int>`) | Inherits from arguments |
| Type variables (`'t`) | Lowest |

Two types are comparable only if they have the same structural form (same type constructor with same arity). `Option<int>` and `List<int>` are incomparable regardless of concreteness.

## Diagnostics

| Code | Message | Default |
|------|---------|---------|
| FS3575 | "Overload resolution selected '%s' based on type concreteness. The more concrete type '%s' was preferred over '%s'. This is an informational message and can be enabled with --warnon:3575." | Off |
| FS3576 | "A more generic overload was bypassed: '%s'. The selected overload '%s' was chosen because it has more concrete type parameters." | Off |

Enable with `--warnon:3575` or `--warnon:3576` to audit resolution decisions during development.

### Enhanced Ambiguity Errors

When the tiebreaker cannot resolve (incomparable types), FS0041 is enhanced:

```
error FS0041: A unique overload for method 'Invoke' could not be determined.
Neither candidate is strictly more concrete than the other:
  - Invoke(x: Result<int, 'e>) is more concrete at position 1
  - Invoke(x: Result<'t, string>) is more concrete at position 2
```

## Compatibility

**Non-breaking change.** The tiebreaker only applies when:
1. Multiple overloads remain after all existing tiebreakers
2. Current behavior would produce an `FS0041` ambiguity error

| Aspect | Impact |
|--------|--------|
| Existing code | Compiles identically |
| Previous FS0041 errors | May now compile successfully |
| Binary/IL | No change |
| Feature gate | F# 10.0 / `LangVersion preview` |

### Portability

```fsharp
// Works on new compiler:
let result = Api.Call(Some 42)

// Portable to all versions (add type annotation):
let result = Api.Call(Some 42 : Option<int>)
```

## C# Alignment

This change brings F# closer to C#'s "better function member" rules (ECMA-334 §12.6.4). In C#, after type inference, a generic method with inferred concrete types is compared as if it were a concrete overload. The F# tiebreaker produces the same resolution as C# in common cases, improving interoperability with .NET libraries that rely on overloading patterns.

## Drawbacks

- **Silent behavior change**: Code that previously failed with `FS0041` will now compile. Developers who relied on this error as a guardrail forcing explicit annotations may find overload selection happens implicitly.

- **Adding generic overloads can change resolution**: When a library adds a new, more generic overload, existing call sites may switch to different (now "more concrete" by comparison) overloads.

- **Learning curve for partial order semantics**: Developers must understand why `Result<int,'e>` vs `Result<'t,string>` remains ambiguous (neither dominates). The dominance rule is mathematically clean but may require explanation.

# Alternatives

1. **Do nothing**: Continue requiring explicit type annotations or named arguments for disambiguation. This is the status quo but creates friction, especially when consuming .NET libraries designed with C#'s resolution rules in mind.

2. **Full C# semantics adoption**: Implement all of C#'s "better function member" rules. This would be a larger change with more risk of breaking existing F# code. The tiebreaker approach is more conservative.

3. **Attribute-based explicit priority**: Allow library authors to mark overloads with explicit priority (see related RFC for `OverloadResolutionPriorityAttribute`). This is complementary—explicit priority could override implicit concreteness when needed.

# Prior Art

- **C# "better function member"** (ECMA-334 §12.6.4): C# prefers more specific overloads after type inference. Our tiebreaker aligns with this for the common cases.

- **Scala overload resolution**: Scala has similar specificity rules preferring more specific signatures.

- **Haskell type class resolution**: Uses specificity ordering for instance selection, though the mechanism is different.

# SRTP Exclusion

Methods involving statically resolved type parameters (`^T`) are **entirely excluded** from concreteness comparison. If either candidate has SRTP type parameters, SRTP type arguments, or parameter types containing SRTP type variables, the tiebreaker returns 0 (no preference) and defers to existing resolution rules. SRTP uses constraint solving, not type-parameter specificity, and mixing the two would produce incorrect results.

# Unresolved Questions

1. **Interaction with OverloadResolutionPriorityAttribute**: When ORPA removes candidates before type-checking, the surviving candidates may have different concreteness relationships than the original set. Should the tiebreaker's concreteness warnings account for ORPA-filtered candidates?

2. **Rule ordering relative to NullableOptionalInterop**: The concreteness tiebreaker fires before the F# 5.0 NullableOptionalInterop rule (which compares all args including optional/named). A case where concreteness decides before nullable interop gets a chance could produce surprising results for `Nullable<T>` overloads.

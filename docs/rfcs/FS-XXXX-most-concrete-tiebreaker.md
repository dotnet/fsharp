# F# RFC FS-XXXX - "Most Concrete" Tiebreaker for Overload Resolution

The design suggestion [\"Most concrete\" tiebreaker for generic overloads](https://github.com/fsharp/fslang-suggestions/issues/905) has been marked "approved in principle".

This RFC covers the detailed proposal for this suggestion.

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/905)
- [x] Approved in principle
- [x] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] Discussion

# Summary

This RFC introduces a new tiebreaker rule for F# overload resolution that prefers "more concrete" overloads when choosing between methods with different levels of type specificity. Currently, F# emits `FS0041` ambiguity errors in cases where one overload is clearly more specific than another (e.g., a parameter typed `'T option` versus one typed `'T`), even when the argument types are fully known. This change aligns F# with C#'s overload resolution behavior and eliminates the need for workarounds in common scenarios.

## Motivation

### Generic wrapper "smart constructors" — the constructor pain point

A very common F# pattern is a generic wrapper type that offers several
constructors: one taking a bare value and others taking already-wrapped forms.
A constructor's type parameters live on the *enclosing type*, and F# infers them
from the arguments, so more than one constructor can be applicable. Today F#
reports an ambiguity even when one parameter type is strictly more concrete:

```fsharp
type Wrapper<'T>(tag: string) =
    new(x: 'T)        = Wrapper<'T>("value")
    new(x: 'T option) = Wrapper<'T>("option")
    member _.Tag = tag

// Current: FS0041 (ambiguous) — Proposed: resolves to new('T option), Tag = "option"
let w = Wrapper(Some 5)
```

The .NET BCL is full of the same shape — most famously `ValueTask<'T>`, whose
`ValueTask(result: 'T)` / `ValueTask(task: Task<'T>)` pair motivated the original
suggestion — as are widely used libraries:

- **TaskBuilder.fs**: uses priority marker types to force resolution
- **FsToolkit.ErrorHandling**: splits extensions across modules for import ordering

### Basic Example (both candidates generic)

When neither of the pre-existing tiebreakers can separate two **equally generic**
overloads, F# still errors even though one parameter is more concrete:

```fsharp
type Api =
    static member Call(x: 'T)        = "generic"
    static member Call(x: 'T option) = "concrete"

// Current: FS0041 — Proposed: resolves to Call('T option), "concrete"
let result = Api.Call(Some 42)
```

> Note: an overload set like `Invoke(Option<'t>)` vs `Invoke(Option<int list>)`
> — one fully generic, one fully concrete — already compiles *without* this
> feature, because the pre-existing *prefer non-generic over generic* rule
> (§14.4 step 7 rule 8) selects the concrete overload before this tiebreaker is
> reached. The new tiebreaker is only needed when both candidates are generic,
> as in the example above.

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
+    9. Prefer candidates with more concrete parameter types.
+       When both candidates have formal parameter types that mention a
+       comparable (non-SRTP) type variable — from a method type parameter or
+       from the enclosing generic type (so constructors and generic-type
+       members participate too) — prefer the candidate whose parameter types are
+       more concrete as defined by the dominance rule: a type dominates another
+       if it is at least as concrete at every position and strictly more concrete
+       at one or more.
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

## Scope

The tiebreaker applies uniformly to any overloadable member whose formal
parameter types mention a comparable type variable, regardless of where that
type variable is bound:

- **Methods** — the type variable is a method type parameter.
- **Constructors** and **generic-type members** (static or instance) — the type
  variable belongs to the enclosing generic type and is inferred from the
  arguments. Because `compareTypeConcreteness` treats an enclosing-type
  `'T` identically to a method-level `'T`, these participate on exactly the same
  footing as methods once the firing condition is broadened from "the method has
  type arguments" to "a formal parameter mentions a comparable type variable".

Extension methods, `params`/optional-argument overloads, tuples, and nested
generics are all in scope whenever the applicability step leaves more than one
candidate and their parameter types mention a comparable type variable.

Two consequences are worth calling out explicitly:

1. **Constructor type-argument inference is F#-only.** C# has no way to infer a
   constructor's type arguments from its call arguments, so the constructor case
   has no C# analogue — it is an intentional F# *superset*, not a parity gap.
2. **The tiebreak can determine the expression's type, not just the
   implementation.** Choosing `new(x: 'T option)` over `new(x: 'T)` for
   `Wrapper(Some 5)` fixes the value's type at `Wrapper<int>`. This is not new
   behavior introduced by constructors: the tiebreaker already determines the
   result type for the method form (e.g. two `static member Make` overloads
   returning different types), and constructors behave identically.

SRTP members remain **entirely excluded** (see [SRTP Exclusion](#srtp-exclusion)),
including when the SRTP type variable comes from the enclosing type.

## Diagnostics

| Code | Message | Default |
|------|---------|---------|
| FS3575 | "Overload resolution preferred the more concrete overload '%s' over '%s' based on parameter type concreteness. This is an informational message and can be enabled with --warnon:3575." | Off |
| FS3576 | "A more generic overload was bypassed: '%s'. The selected overload '%s' was chosen because it has more concrete type parameters." | Off |

Enable with `--warnon:3575` or `--warnon:3576` to audit resolution decisions during development. The `%s` placeholders are rendered as full method signatures (e.g. `static member Api.Call: x: Result<'t,string> -> string`), so the two candidates are always distinguishable even when they share a name.

### Enhanced Ambiguity Errors

When the tiebreaker cannot resolve (incomparable types), the standard FS0041 is extended with a per-position concreteness breakdown:

```
error FS0041: A unique overload for method 'Call' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: Result<int,'a>

Candidates:
 - static member Api.Call: x: Result<'t,string> -> string
 - static member Api.Call: x: Result<int,'e> -> string
Neither candidate is strictly more concrete than the other:
 - static member Api.Call: x: Result<'t,string> -> string is more concrete at position 1
 - static member Api.Call: x: Result<int,'e> -> string is more concrete at position 2
```

The extra detail lines are gated behind the same language feature as the tiebreaker and are localized.

## Compatibility

**Non-breaking change.** The tiebreaker only applies when:
1. Multiple overloads remain after all existing tiebreakers
2. Current behavior would produce an `FS0041` ambiguity error

| Aspect | Impact |
|--------|--------|
| Existing code | Compiles identically |
| Previous FS0041 errors | May now compile successfully |
| Binary/IL | No change |
| Feature gate | `LangVersion preview` |

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

# Resolved Questions

1. **Interaction with OverloadResolutionPriorityAttribute**: `OverloadResolutionPriorityAttribute` is applied *before* betterness — it prunes the candidate set among the applicable members of each declaring type — so the concreteness tiebreaker only ever sees the post-ORPA survivors. Priority therefore always wins over concreteness (a high-priority overload is kept even if a lower-priority one is more concrete), and the concreteness diagnostics naturally describe only the surviving candidates.

2. **Rule ordering relative to NullableOptionalInterop**: The concreteness tiebreaker is the **last** rule consulted — it fires only after every pre-existing rule, including the F# 5.0 NullableOptionalInterop rule and the property/override rule, has tied. This guarantees the new rule can only convert a former `FS0041` ambiguity into a success; it can never re-decide a resolution that an already-shipping rule settled, so no existing `Nullable<T>` / optional-argument resolution changes.

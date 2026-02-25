# Implementation Gaps — Improved Byref-Like Escape Analysis

> **Branch:** `feature-ref-fields`
> **RFC:** [`docs/rfc/FS-XXXX-improved-byreflike-escape-analysis.md`](rfc/FS-XXXX-improved-byreflike-escape-analysis.md)
> **Suggestion:** [fsharp/fslang-suggestions#1143](https://github.com/fsharp/fslang-suggestions/issues/1143) (approved by dsyme)

## What this branch delivers

The branch implements improved escape analysis for byref-like types, preventing
`Span<int>(&local)` from compiling when the span escapes the local's scope.
Gated behind `--langversion:preview`.

**Fully implemented:**

| Feature | Files | Tests |
|---------|-------|-------|
| Core rule (byref-to-local limits span-like return) | `PostInferenceChecks.fs` L1097–1103 | `ByrefSafetyAnalysis.fs` T1–T10 |
| `[<ScopedRef>]` consumption (IL + same-assembly) | `PostInferenceChecks.fs` L878–898 | T11a–T11e |
| `[UnscopedRef]` on struct receivers (cross-assembly) | `PostInferenceChecks.fs` L901–908 | T12a–T12c |
| `RefSafetyRulesAttribute` read | `import.fs` `GetRefSafetyRulesVersion` | T13a–T13f |
| `RefSafetyRulesAttribute` emit | `IlxGen.fs` L12143 | `FSharp assembly does not emit...` |
| Implicit scoping (`out`, `ref`-to-refstruct) | `PostInferenceChecks.fs` L910–925 | T13d–T13f |
| Generic method guard relaxation | `PostInferenceChecks.fs` L930 | T13e |
| `not isCtor` bugfix (all lang versions) | `PostInferenceChecks.fs` L1765 | backward-compat twins |
| `tyOfExpr g expr` bugfix (all lang versions) | `PostInferenceChecks.fs` L1523,1566,1768 | backward-compat twins |
| Feature gate | `LanguageFeatures.fs` | T8 backward-compat tests |

## Gaps

### GAP-1: `[<ScopedRef>]` body enforcement *(soundness)*

**Status:** Not implemented.

**Problem:** `[<ScopedRef>]` is enforced at the **call site** but not in the
**function body**. The author can violate their own scoped contract:

```fsharp
let leak ([<ScopedRef>] x: byref<int>) : Span<int> =
    Span<int>(&x)  // Compiles. Body treats x as scope=0 (parameter).

let test () : Span<int> =
    let mutable local = 42
    leak &local  // Caller trusts ScopedRef → allows &local → dangling span.
```

C# prevents this via [CS9075](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#rules-scoped):
a `scoped` parameter has `ref-safe-to-escape = function-member`, so returning
it is an error.

**Impact:** An F# author who writes `[<ScopedRef>]` and accidentally returns
the parameter creates an unsound API. Callers are protected (they see scoped),
but the contract is not enforced on the author.

**Fix:** In `CheckLambdas` or `BindArgVals` in `PostInferenceChecks.fs`, when
a parameter has `ScopedRefAttribute` in its `ArgReprInfo.Attribs`, call
`LimitVal cenv arg { scope = 1; flags = LimitFlags.ByRef }` to mark it as
non-returnable inside the function body.

**Verification tests:**

```fsharp
// Must error (body violates scoped contract)
let leak ([<ScopedRef>] x: byref<int>) : Span<int> = Span<int>(&x)
// → shouldFail |> withErrorCodes [3235]

// Must error (byref return of scoped param)
let leak2 ([<ScopedRef>] x: byref<int>) : byref<int> = &x
// → shouldFail |> withErrorCodes [3228]

// Must succeed (body does not use scoped param in return)
let safe ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> = Span(arr)
// → shouldSucceed

// Backward compat: same code without --langversion:preview → shouldSucceed
```

---

### GAP-2: `[<ScopedRef>]` emission to IL *(interop)*

**Status:** Likely works via standard attribute emit path, not verified.

**Problem:** The RFC claims F# parameters annotated with `[<ScopedRef>]` emit
`ScopedRefAttribute` into IL. This is critical for cross-assembly F#→F#
consumption. The standard F# attribute emit path should handle this
automatically, but there is no test verifying the IL output.

**Verification test:**

```fsharp
// Compile F# library with [<ScopedRef>] param, verify IL contains the attribute
let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> = Span(arr)
// → compile |> verifyIL [""".custom instance void [System.Runtime]System.Runtime.CompilerServices.ScopedRefAttribute"""]
```

---

### GAP-3: Scope variance in overrides *(diagnostic)*

**Status:** Not validated.

**Problem:** C# validates that overrides don't widen scoping (removing
`[<ScopedRef>]` from a parameter that the base declares as scoped). F# does
not check parameter attributes during override validation.

**Impact:** A C# caller of an F# override that drops `[<ScopedRef>]` would see
the base's scoped annotation but the derived's wider behavior.

**Verification test:**

```csharp
// C# base class
public abstract class Base {
    public abstract Span<int> M(scoped ref int x, int[] arr);
}
```

```fsharp
// F# override that drops scoped — should warn or error
type Derived() =
    inherit Base()
    override _.M(x: byref<int>, arr: int[]) = Span<int>(&x)
// → shouldFail or shouldWarn
```

---

### GAP-4: `[UnscopedRef]` on F# struct methods same-assembly *(expressiveness)*

**Status:** Not implemented.

**Problem:** `[UnscopedRef]` on struct instance methods is honored cross-assembly
(IL path reads the attribute). Same-assembly F# calls always treat struct `this`
as implicitly scoped because `CheckApplication` does not read `UnscopedRefAttribute`
from `ArgReprInfo.Attribs`.

**Impact:** An F# struct author who manually applies `[<UnscopedRef>]` to a method
gets correct behavior when consumed from another assembly, but same-assembly
callers see `this` as scoped (potentially over-rejecting).

**Verification test:**

```fsharp
[<Struct; IsByRefLike>]
type S =
    val mutable x: int
    [<UnscopedRef>]
    member this.GetRef() : byref<int> = &this.x

let test (s: byref<S>) =
    let r = s.GetRef()  // Same-assembly: should allow (UnscopedRef), currently may over-reject
    r
```

---

### GAP-5: `hasUnscopedRef` default on resolution failure *(edge case)*

**Status:** Defaults to `false` (optimistic for receiver, conservative for args).

**Problem:** When `tryResolveILMethodContext` returns `None`, `hasUnscopedRef`
defaults to `false`. If the method actually has `[UnscopedRef]` but resolution
failed, the receiver is treated as scoped (excluded from limit).

**Impact:** Near-zero. Resolution failure for a method being actively called is
extremely rare (assembly is loaded). Not a regression — FS-1053 never checked
receivers for `[UnscopedRef]` at all. Defaulting to `true` would cause false
positives on every struct method call where resolution fails.

**Decision:** Accepted risk. Document only, no code change needed.

---

## Gap priority

| Gap | Severity | Blocks shipping? |
|-----|----------|-----------------|
| GAP-1 | Soundness (author-side only) | Discuss — callers are safe |
| GAP-2 | Verification gap | No — add IL test |
| GAP-3 | Diagnostic | No — independent concern |
| GAP-4 | Expressiveness | No — over-rejects, never under-rejects |
| GAP-5 | Edge case | No — accepted risk |

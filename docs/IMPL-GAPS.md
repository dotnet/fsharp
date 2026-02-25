# Implementation Gaps — Improved Byref-Like Escape Analysis

> **Branch:** `feature-ref-fields`
> **RFC:** [`docs/rfc/FS-XXXX-improved-byreflike-escape-analysis.md`](rfc/FS-XXXX-improved-byreflike-escape-analysis.md)
> **Suggestion:** [fsharp/fslang-suggestions#1143](https://github.com/fsharp/fslang-suggestions/issues/1143) (approved by dsyme)
> **C# spec:** [C# 11 low-level struct improvements](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md)
> **Predecessor:** [FS-1053 — Span, byref, IsByRefLike](https://github.com/fsharp/fslang-design/blob/main/FSharp-4.5/FS-1053-span.md)

## Background

.NET 7 introduced [ref fields](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/ref-struct#ref-fields), allowing `Span<int>(ref x)` to capture a byref as a field. FS-1053's escape analysis did not account for this — `Span<int>(&local)` compiled and produced a dangling span.

This branch adds a new escape check: when a call returns a byref-like type (e.g. `Span<T>`), and any non-scoped argument is a byref pointing to a local, the return is **limited** (cannot escape). This is gated behind `--langversion:preview` via `LanguageFeature.ImprovedByRefLikeEscapeAnalysis`.

The branch also reads C# 11 attributes (`ScopedRefAttribute`, `UnscopedRefAttribute`, `RefSafetyRulesAttribute`) to interoperate correctly with C# libraries that use `scoped`, `[UnscopedRef]`, and implicit parameter scoping.

## Key files

| File | Role |
|------|------|
| `src/Compiler/Checking/PostInferenceChecks.fs` | All escape analysis logic. Walks the typed tree after inference, tracks `Limit` per expression. |
| `src/Compiler/Checking/import.fs` | `GetRefSafetyRulesVersion` — reads `[module: RefSafetyRules(N)]` from IL. |
| `src/Compiler/CodeGen/IlxGen.fs` | Emits `[module: RefSafetyRules(11)]` when feature is enabled. |
| `src/Compiler/TypedTree/TcGlobals.fs` | `attrib_ScopedRefAttribute_opt`, `attrib_UnscopedRefAttribute_opt`, `attrib_RefSafetyRulesAttribute_opt` — attribute lookups via `tryFindSysAttrib` (returns `Option`, graceful on older TFMs). |
| `src/Compiler/TypedTree/TypedTree.fs` | `CcuData.RefSafetyRulesVersion` field. |
| `src/Compiler/Driver/CompilerImports.fs` | Wires `RefSafetyRulesVersion` into `CcuData`. |
| `tests/.../ByrefSafetyAnalysis/ByrefSafetyAnalysis.fs` | 211 tests covering all escape scenarios. |

## Key concepts

- **`Limit`**: A `{ scope: int; flags: LimitFlags }` attached to each expression. `scope >= 1` means the value is local (must not escape). `scope = 0` means safe to return.
- **`LimitFlags`**: `ByRef`, `SpanLike`, `StackReferringSpanLike`, `ByRefOfSpanLike`, `ByRefOfStackReferringSpanLike` — bit flags describing what kind of reference the expression is.
- **Scoped mask**: A `bool array option` aligned 1:1 with non-receiver arguments. `true` = scoped (excluded from limit). `None` = all params non-scoped (conservative).
- **`CheckCallLimitArgs`** (line ~1083): Central function that decides if a return is limited based on argument limits.
- **`CheckCallWithReceiver`** (line ~1172): Splits receiver from args, applies scoped mask, handles `[UnscopedRef]` on receiver.
- **`computeScopedMask`** (line ~928): Builds the scoped mask for IL calls: explicit `ScopedRef` → implicit scoping → `UnscopedRef` negation.

## How to build and test

```bash
# Build the compiler
BUILDING_USING_DOTNET=true dotnet build src/Compiler/FSharp.Compiler.Service.fsproj -c Debug

# Run escape analysis tests
dotnet test tests/FSharp.Compiler.ComponentTests -c Release --no-build \
    -- --filter-class "*ByrefSafetyAnalysis*"

# Run all component tests
dotnet test tests/FSharp.Compiler.ComponentTests -c Release
```

## What this branch delivers

**Fully implemented:**

| Feature | Where in PostInferenceChecks.fs | Tests |
|---------|-------------------------------|-------|
| Core rule (byref-to-local limits span-like return) | `CheckCallLimitArgs` L1097–1103 | T1–T10 |
| `[<ScopedRef>]` consumption from IL | `tryGetScopedParamMask` L878–882 | T11a–T11e |
| `[<ScopedRef>]` consumption same-assembly | `tryGetScopedParamMaskFromFSharpAttribs` L885–898 | T11c |
| `[UnscopedRef]` on struct receivers (cross-assembly) | `hasUnscopedRefAttribute` L901–908 | T12a–T12c |
| `RefSafetyRulesAttribute` read | `import.fs` `GetRefSafetyRulesVersion` | T13a–T13f |
| `RefSafetyRulesAttribute` emit | `IlxGen.fs` L12143 | `FSharp assembly does not emit...` |
| Implicit scoping (`out`, `ref`-to-refstruct when version ≥ 11) | `isImplicitlyScopedParam` L910–925 | T13d–T13f |
| Generic method guard relaxation (version ≥ 11) | `computeScopedMask` L930 | T13e |
| `not isCtor` bugfix (all lang versions) | L1765 | backward-compat twins |
| `tyOfExpr g expr` bugfix (all lang versions) | L1523, 1566, 1768 | backward-compat twins |
| Feature gate | `LanguageFeatures.fs` | T8 backward-compat tests |

## Gaps

### GAP-1: `[<ScopedRef>]` body enforcement *(soundness)*

**Status:** Not implemented.

**Problem:** When an F# function author annotates a parameter with `[<ScopedRef>]`,
the compiler enforces this at the **call site** (callers can pass `&local` because the
parameter is excluded from the escape limit). But the **function body** is not
constrained — the author can still return the scoped parameter, violating the contract.

```fsharp
open System
open System.Runtime.CompilerServices

// The attribute promises: "I will NOT capture x in the return value."
let leak ([<ScopedRef>] x: byref<int>) : Span<int> =
    Span<int>(&x)  // BUG: Compiles. Body treats x as scope=0 (parameter = safe to return).

let test () : Span<int> =
    let mutable local = 42
    leak &local  // Caller trusts ScopedRef → allows &local → dangling span returned.
```

C# prevents this: a `scoped` parameter has `ref-safe-to-escape = function-member`,
so `return ref x;` is error [CS9075](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#rules-scoped).

**Impact:** An F# author who writes `[<ScopedRef>]` and accidentally returns
the parameter creates an unsound API. **Callers are protected** (they see scoped
and pass locals safely), but the **contract is not enforced on the author**.

**Where to fix:** `PostInferenceChecks.fs`, in the function that binds argument
symbols before checking the body. The entry point is `CheckLambdas` (line ~2019)
which calls `BindArgVals`. When binding, check if the parameter has
`ScopedRefAttribute` in its `ArgReprInfo.Attribs` (accessed via the function's
`ValReprInfo`). If so, call `LimitVal cenv arg { scope = 1; flags = LimitFlags.ByRef }`
to mark it as non-returnable (scope ≥ 1 = local) inside the function body.

The attribute check pattern already exists in `tryGetScopedParamMaskFromFSharpAttribs`
(line ~885) — reuse the same `TryFindFSharpAttribute g g.attrib_ScopedRefAttribute_opt`
logic.

**Verification tests** (add to `ByrefSafetyAnalysis.fs`):

```fsharp
// Must error: body returns scoped param via span ctor
[<Fact>]
let ``ScopedRef param cannot escape via Span ctor in body`` () =
    FSharp """
module Test
open System
open System.Runtime.CompilerServices
let leak ([<ScopedRef>] x: byref<int>) : Span<int> = Span<int>(&x)
"""
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withErrorCodes [3235]

// Must error: body returns scoped param as byref
[<Fact>]
let ``ScopedRef param cannot be returned as byref`` () =
    FSharp """
module Test
open System.Runtime.CompilerServices
let leak ([<ScopedRef>] x: byref<int>) : byref<int> = &x
"""
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withErrorCodes [3228]

// Must succeed: body does not use scoped param in return
[<Fact>]
let ``ScopedRef param not returned is OK`` () =
    FSharp """
module Test
open System
open System.Runtime.CompilerServices
let safe ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> = Span<int>(arr)
"""
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed

// Backward compat: without preview, body enforcement does not apply
[<Fact>]
let ``ScopedRef body enforcement backward compat`` () =
    FSharp """
module Test
open System
open System.Runtime.CompilerServices
let leak ([<ScopedRef>] x: byref<int>) : Span<int> = Span<int>(&x)
"""
    |> asLibrary
    |> compile
    |> shouldSucceed
```

---

### GAP-2: `[<ScopedRef>]` emission to IL *(interop)*

**Status:** Likely works via standard attribute emit path, not verified.

**Problem:** The RFC claims F# parameters annotated with `[<ScopedRef>]` emit
`ScopedRefAttribute` into IL. This is critical for **cross-assembly F#→F#**
consumption: if library A is compiled with `[<ScopedRef>]` and library B
references A's DLL, B's compiler reads `ScopedRefAttribute` from IL metadata
(`ILParameter.CustomAttrs`) to build the scoped mask.

F# emits user-specified parameter attributes via the standard attribute path in
`IlxGen.fs`. Since `ScopedRefAttribute` is a normal `System.Runtime.CompilerServices`
attribute, it should be emitted automatically. But there is no test that verifies
the IL output actually contains it.

**Where to verify:** Compile an F# library with `[<ScopedRef>]`, then use
`verifyIL` in a test to check the IL contains the attribute on the parameter.

**Verification test** (add to `ByrefSafetyAnalysis.fs`):

```fsharp
[<Fact>]
let ``ScopedRef attribute is emitted to IL`` () =
    FSharp """
module Test
open System
open System.Runtime.CompilerServices
let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> = Span<int>(arr)
"""
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed
    |> verifyIL ["""
.custom instance void [System.Runtime]System.Runtime.CompilerServices.ScopedRefAttribute::.ctor()
"""]
```

---

### GAP-3: Scope variance in overrides *(diagnostic)*

**Status:** Not validated.

**Problem:** C# validates that when a derived class overrides a method, it cannot
**widen** scoping — i.e., if the base declares a parameter as `scoped`, the
override must also declare it as `scoped`. This prevents a caller using the base
type from observing wider escaping through the derived implementation.

F# override checking compares types and return types, but does not compare
parameter attributes. An F# class can override a C# base method and silently
drop `[<ScopedRef>]`.

See [C# spec §scoped-mismatch](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#rules-scoped)
for C#'s validation rules.

**Impact:** A C# caller holding a `Base` reference that points to the F# `Derived`
would see the base's scoped annotation but get the derived's wider behavior.
The caller might pass `&local` (trusting scoped) but the derived returns it.

**Where to fix:** Override validation happens in `CheckDeclarations.fs` /
`MethodOverrides.fs`. Parameter attribute comparison would need to be added
alongside the existing type comparison.

**Verification test** (add to `ByrefSafetyAnalysis.fs`):

```fsharp
[<Fact>]
let ``Override cannot widen scoped parameter`` () =
    let csharpLib =
        CSharp """
using System;
public abstract class Base {
    public abstract Span<int> M(scoped ref int x, int[] arr);
}
"""     |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    FSharp """
module Test
open System

type Derived() =
    inherit Base()
    override _.M(x: byref<int>, arr: int[]) = Span<int>(&x)
"""
    |> withReferences [csharpLib]
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldFail  // or shouldWarn — decide which diagnostic level
```

---

### GAP-4: `[UnscopedRef]` on F# struct methods same-assembly *(expressiveness)*

**Status:** Not implemented.

**Problem:** C# 11 struct instance methods have `this` implicitly scoped.
`[UnscopedRef]` on a method opts out, allowing `this` to escape (e.g., returning
`ref this.field`). See [C# spec §unscoped-ref](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#rules-unscoped).

F# honors this **cross-assembly** via the IL path: `CheckExprOp` handles
`TOp.ILCall` and reads `UnscopedRefAttribute` from `ILMethodDef.CustomAttrs`
via `hasUnscopedRefAttribute` (line ~901).

But **same-assembly** F# calls go through `CheckApplication`, which reads
scoped masks from `ArgReprInfo.Attribs`. `CheckApplication` always passes
`hasUnscopedRef = false` (line ~1626), meaning struct `this` is always treated
as scoped for same-assembly calls, even if the author manually applied
`[<UnscopedRef>]` to the method.

**Impact:** An F# struct author who writes `[<UnscopedRef>]` on a method gets
correct behavior when consumed from another assembly, but same-assembly callers
see `this` as scoped. This may **over-reject** valid code but never under-rejects.

**Where to fix:** In `CheckApplication` (line ~1620), when `hasReceiver` is true
and the function is a struct instance member, check if the method's attributes
contain `UnscopedRefAttribute`. If so, pass `hasUnscopedRef = true` to
`CheckCallWithReceiver` instead of `false`.

**Verification test** (add to `ByrefSafetyAnalysis.fs`):

```fsharp
[<Fact>]
let ``UnscopedRef on F# struct method allows this to escape same-assembly`` () =
    FSharp """
module Test
open System
open System.Diagnostics.CodeAnalysis
open System.Runtime.CompilerServices

[<Struct; IsByRefLike>]
type S =
    val mutable X: int
    [<UnscopedRef>]
    member this.GetRef() : byref<int> = &this.X

let test (s: byref<S>) : byref<int> =
    s.GetRef()
"""
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed
```

---

### GAP-5: `hasUnscopedRef` default on resolution failure *(edge case)*

**Status:** Defaults to `false` (optimistic for receiver, conservative for args).

**Problem:** When `tryResolveILMethodContext` (line ~841) returns `None`
(e.g., malformed metadata), `hasUnscopedRef` defaults to `false`
(line ~1793: `| None -> None, false`). If the method actually has
`[UnscopedRef]` but resolution failed, the receiver is treated as scoped
(excluded from limit) — potentially allowing a dangling ref from a struct
receiver.

Meanwhile, `scopedMask` also defaults to `None` (all params non-scoped), which
is **conservative** for regular arguments.

**Impact:** Near-zero. Resolution failure for a method being actively called is
extremely rare (the assembly must be loaded for the call to type-check). This is
**not a regression** — FS-1053 never checked receivers for `[UnscopedRef]` at all.
Defaulting `hasUnscopedRef` to `true` would cause false positives on every struct
method call where resolution fails.

**Decision:** Accepted risk. Document only, no code change needed.

If this is revisited, the fix would be in `PostInferenceChecks.fs` line ~1793:
change `| None -> None, false` to `| None -> None, true`. But measure the
false-positive impact on real codebases first.

---

## Gap priority

| Gap | Severity | Blocks shipping? |
|-----|----------|-----------------|
| GAP-1 | Soundness (author-side only) | Discuss — callers are safe |
| GAP-2 | Verification gap | No — add IL test |
| GAP-3 | Diagnostic | No — independent concern |
| GAP-4 | Expressiveness | No — over-rejects, never under-rejects |
| GAP-5 | Edge case | No — accepted risk |

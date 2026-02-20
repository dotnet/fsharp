# F# RFC FS-XXXX — Improved Escape Analysis for Byref-Like Types

* [ ] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/1143)
* [ ] Implementation: In progress
* [ ] [C# spec: Low-level struct improvements](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md)

Extends FS-1053 escape analysis. All existing behavior preserved when feature flag is off.

## Motivation

.NET 7 ref fields let `Span<int>(ref x)` capture `x`. Old F# analysis didn't track `ByRef` args for span-like returns:

```fsharp
let f () =
    let mutable x = 1
    Span<int>(&x) // Compiled. Dangling span — use-after-free.
```

## Rule

Gated: `--langversion:preview` / `LanguageFeature.ImprovedByRefLikeEscapeAnalysis`.

A call returning a byref-like type is limited (cannot escape) if any non-scoped argument
is a `ByRef` whose referent does not outlive the current scope.

### Limit flags that restrict a span-like return

| Argument limit | Before (FS-1053) | After (this RFC) |
|---|---|---|
| `StackReferringSpanLike` | limited | limited |
| `ByRefOfStackReferringSpanLike` | limited | limited |
| `ByRef` to local | **ignored** | **limited** |
| `ByRef` to param | ignored | ignored |

### Struct receiver rule

Struct instance methods returning byref-like: receiver with `ByRef` flag is included in limit computation
only when `[UnscopedRef]` is present on the method.
Follows [C# `scoped` on `this`](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#implicitly-scoped):
C# struct `this` is implicitly `scoped ref`. `[UnscopedRef]` opts out, allowing `this` to escape.
F# reads `UnscopedRefAttribute`: if present, receiver is included in limit (can escape).
If absent, receiver is excluded (cannot escape — consistent with C# default).

### ScopedRefAttribute consumption and authoring

**Consumption:** Reads `System.Runtime.CompilerServices.ScopedRefAttribute` from IL metadata on parameters.
Scoped parameters are excluded from the escape limit.
Follows [C# `scoped` modifier](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#rules-scoped).

**Authoring:** F# parameters can be annotated with `[<ScopedRef>]` (attribute-only, no keyword).
The compiler emits `ScopedRefAttribute` and honors it in escape analysis.

```fsharp
open System.Runtime.CompilerServices

let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> =
    x <- 42         // can use x
    Span<int>(arr)   // but x is scoped — not captured, callers with locals OK
```

Applied to: IL calls returning byref-like types in returnable context, and F# calls where the callee has `[<ScopedRef>]`.

## What Compiles, What Doesn't

| # | Code | Result | Why |
|---|------|--------|-----|
| 1 | `let f () = let mutable x = 1 in Span<int>(&x)` | **FS3235** | byref to local, may be captured as ref field |
| 2 | `let f (x: byref<int>) = Span<int>(&x)` | OK | byref to param outlives callee |
| 3 | `let f () = Span<int>([| 1;2;3 |])` | OK | array arg, no byref |
| 4 | `let f () = let mutable x = 1 in ReadOnlySpan<int>(&x)` | **FS3235** | same rule, inref |
| 5 | `let f () = let mutable x = 1 in let s = Span<int>(&x) in s[0]` (no return) | OK | local use, span doesn't escape |
| 6 | `let wrap (x: byref<int>) : Span<int> = Span<int>(&x)` then `let g () = let mutable x = 1 in wrap &x` | **FS3235** | propagated: `wrap` returns span-like taking byref |
| 7 | `let f () = let mutable x = 1 in passThrough(Span<int>(&x))` | **FS3235** | nested: inner span limited, outer inherits |
| 8 | `let f () = let mutable x = 1 in MemoryMarshal.CreateSpan(&x, 1)` | OK | `scoped ref T` parameter (V2: RefSafetyRulesAttribute from System.Runtime) |
| 9 | `if flag then Span<int>(&local) else Span<int>(arr)` | **FS3235** | branches combined, worst wins |
| 10 | Same code without `--langversion:preview` | old behavior | feature-gated |

### Struct receiver examples

| # | Code | Result | Why |
|---|------|--------|-----|
| 11 | `let f () = let mutable s = Evil() in s.AsSpan()` (`[UnscopedRef]` on AsSpan) | **FS3235** | `[UnscopedRef]` → receiver escapes → local struct limited |
| 12 | `let f (s: byref<Evil>) = s.AsSpan()` | OK | param receiver outlives callee |
| 13 | `let f () = let mutable s = SafeStruct() in s.GetSpan([| 1 |])` (no `[UnscopedRef]`) | OK | no `[UnscopedRef]` → receiver is scoped → excluded |

### C# interop examples

| # | C# signature | F# call with local byref | Result | Why |
|---|---|---|---|---|
| 14 | `Span<int> M(ref int x)` | `M(&local)` | **FS3235** | non-scoped ref, captured |
| 15 | `Span<int> M(scoped ref int x)` | `M(&local)` | OK | ScopedRefAttribute → excluded |
| 16 | `Span<int> M(scoped in int x)` | `M(&local)` | OK | ScopedRefAttribute → excluded |
| 17 | `Span<int> M(in int x)` | `M(&local)` | **FS3235** | non-scoped in, same as ref |
| 18 | `Span<int> M(scoped ref int a, ref int b)` | `M(&s, &u)` | **FS3235** | `b` non-scoped, limits return |
| 19 | `ScopedRefStruct(scoped ref int x, int[] arr)` (ctor) | `ScopedRefStruct(&local, arr)` | OK | scoped ctor param |
| 20 | `UnscopedRefStruct(ref int x)` (ctor) | `UnscopedRefStruct(&local)` | **FS3235** | non-scoped ctor param |
| 21 | `Span<int> M(scoped Span<int> s)` (scoped value) | `M(Span(&local))` | OK | ScopedRefAttribute on value param → excluded |
| 22 | `Span<int> M([UnscopedRef] out int x)` | `M(&local)` | **FS3235** | UnscopedRef overrides out's implicit scoped |
| 23 | `ref int M(ref int x)` then `Span(ref M(ref local))` | chained | **FS3235** | ref return carries local origin; span captures it |

## C# Interop Mapping

| C# parameter form | IL attribute | F# behavior under this RFC |
|---|---|---|
| `ref T` | bare byref | Limits span-like return (follows C# default) |
| `in T` | `[In]` + `IsReadOnlyAttribute` | Same as `ref` — limits return |
| `out T` | `[Out]` | **V2**: When `RefSafetyRulesVersion >= 11`, `out` is implicitly scoped (excluded from limit). Otherwise, limits span-like return (conservative). |
| `scoped ref T` | `ScopedRefAttribute` | Excluded from limit computation |
| `scoped in T` | `ScopedRefAttribute` | Excluded from limit computation |
| `scoped Span<T>` (value) | `ScopedRefAttribute` | Excluded from limit computation (mask reads all params, not just byrefs) |
| `[UnscopedRef]` on member | `UnscopedRefAttribute` | Read: receiver NOT excluded from limit (escape allowed) |
| `[UnscopedRef]` on param | `UnscopedRefAttribute` | Read: param NOT excluded from limit (escape allowed) |
| `[UnscopedRef] out T` | `UnscopedRefAttribute` + `[Out]` | Read: `out` no longer implicitly scoped — limits return |
| `ref` return chaining | byref return → byref arg | Handled: `Span(ref M(ref local))` → inner byref propagates limit via FS-1053, outer span captures it |
| `RefSafetyRulesAttribute` on assembly | version negotiation | **V2**: Read from referenced assemblies; emitted on F# assemblies. Gates implicit scoping and generic method mask. |

### Decisions following C#

| Decision | C# rule | F# follows? | Notes |
|---|---|---|---|
| `out` implicitly scoped | [C# spec](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#out-compat-change): `out` has ref-safe-context of function-member | **Yes (V2)** — when `RefSafetyRulesVersion >= 11`, `out` is implicitly scoped (excluded from limit). Without `RefSafetyRulesAttribute`, treated conservatively as non-scoped. | Reads `RefSafetyRulesAttribute` from assembly; `[UnscopedRef]` overrides implicit scoping |
| `this` on struct implicitly scoped | [C# spec](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#implicitly-scoped): struct `this` is implicitly `scoped ref` | Yes: receiver excluded unless `[UnscopedRef]` | Reads `UnscopedRefAttribute` |
| `[UnscopedRef]` opts out of scoped | C# `[UnscopedRef]` allows `this`/param to escape | Yes: reads attribute, includes in limit | — |
| `[UnscopedRef]` on `out` param | C# `[UnscopedRef] out T` makes `out` escapable again | Yes: read attribute, treat as non-scoped `ref` | Falls out from UnscopedRef reading |
| `in` same as `ref` for escape | C# `in` has ref-safe-context of caller-context (same as `ref`) | Yes | Both limit span-like returns |
| `scoped` on value params | C# `scoped Span<T>` emits `ScopedRefAttribute` on value param | Yes: mask reads all params, not just byrefs | Working — mask applies to all IL params |
| `ref` return chaining | C# ref return carries ref-safe-context of argument | Yes: FS-1053 byref return propagation, this RFC adds span capture on top | `Span(ref M(ref local))` errors |
| Resolution failure → conservative | — | Yes: mask = None → all params treated as non-scoped | May cause false positives, never false negatives |
| `RefSafetyRulesAttribute` version | C# checks assembly-level attribute to select C#7.2 vs C#11 rules | **Yes (V2)** — reads and emits `[module: RefSafetyRules(11)]` | Used to gate implicit scoping and generic method mask |
| `scoped` variance in overrides | C# allows adding `scoped` (narrowing) but not removing (widening) in overrides | **No** — see Out of Scope | Independent diagnostic concern |
| `[UnscopedRef]` on F# struct methods | C# reads from IL metadata | **Partial** — honored cross-assembly (IL path), not same-assembly (F# path) | F# has no syntax for this; manual attribute application is rare |
| Generic method scoped mask | C# reads `ScopedRefAttribute` on all methods | **Yes (V2)** — when `RefSafetyRulesVersion >= 11`, scoped mask is read even on generic methods | V2 reads `RefSafetyRulesAttribute`; when version >= 11, attributes are authoritative (not implicit) |
| `RefSafetyRulesAttribute` reading | C# checks assembly-level attribute to select C#7.2 vs C#11 rules | **Yes (V2)** — reads `[module: RefSafetyRules(11)]` from referenced assemblies | Stored in `CcuData.RefSafetyRulesVersion`; gates implicit `out`/`ref`-to-refstruct scoping and generic method mask |
| `RefSafetyRulesAttribute` emission | C# emits `[module: RefSafetyRules(11)]` on every C#11+ assembly | **Yes (V2)** — F# emits `[module: RefSafetyRules(11)]` when `--langversion:preview` and attribute type is available | C# consumers apply C#11 rules to F# assemblies |
| Implicit `out` / `ref`-to-refstruct scoping | C# `out` and `ref`/`in` to ref struct params are implicitly scoped in C#11 | **Yes (V2)** — when `RefSafetyRulesVersion >= 11`, `out T` and `ref`/`in` to byref-like are implicitly scoped | Merged with explicit `ScopedRefAttribute`; `UnscopedRefAttribute` can override |

## Errors

| Code | Fires when | Fix |
|---|---|---|
| FS3234 | Named span variable in return position with local-byref origin | Move byref source to parameter |
| FS3235 | Span-returning expression in return position with local-byref arg | Same; or use array/pinned memory |

## Feature Interactions

| Feature | Interaction |
|---|---|
| `task { }` / `async { }` | FS0412 (byref in closure) fires first; no new interaction |
| `inline` | Analysis on typed tree; escape caught at call site |
| `match` / `if-else` / `try-with` | Branches independent; limits combined (worst wins) |
| Active patterns | Can't return byref-like; no interaction |
| `allows ref struct` (C# 13) | Generic return type resolved at call site; checked if byref-like |
| F#-to-F# calls | Scoped mask read from `[<ScopedRef>]` on callee params; analysis applies via limit propagation |
| Property getters / indexers | Go through IL call path; same rules apply |

## Breaking Changes

Under `--langversion:preview` only. Unchanged without the flag.

```fsharp
// NOW ERRORS — pass byref from caller instead:
let f () =
    let mutable x = 1
    Span<int>(&x)            // FS3235

let f (x: byref<int>) =     // Workaround
    Span<int>(&x)            // OK
```

### Migration

1. Compile with `--langversion:preview` to discover errors
2. Move byref source to parameter (callee receives param-byref instead of local)
3. For struct receiver false positives: bind data to array before calling span-returning method

## Required Work (before shipping)

| Item | Status | Notes |
|---|---|---|
| MemoryMarshal.CreateSpan soundness | ✅ Done | Fixed generic IL return type resolution using `tyOfExpr g expr` |
| `[UnscopedRef]` reading | ✅ Done | Method-level for struct `this`, per-parameter for `[UnscopedRef] out T` |
| `[<ScopedRef>]` on F# params | ✅ Done | Emit via GenAttrs, honor in CheckApplication for same-assembly F#-to-F# calls |

## Non-gated fixes

These bugfixes apply at **all language versions** (not gated by `--langversion:preview`):

| Fix | Commit | Impact |
|---|---|---|
| Constructor exclusion from receiver | `not isCtor` guard | `.ctor` calls no longer incorrectly treated as instance method calls with `this` receiver |
| Return type resolution | `tyOfExpr g expr` instead of raw IL return type | Correctly identifies `Span<T>` when generic type parameters are instantiated |

### Out of scope (separate RFC)

| Item | Why separate |
|---|---|
| `ref` field declaration in F# | Different feature, different syntax design |
| `stackalloc` improvements | Consistent with FS-1053 approach |
| Scope variance validation | F# override checking compares only types, not parameter attributes. An F# override removing `[<ScopedRef>]` doesn't create unsoundness for F# callers. Risk is narrow: C# callers of the F# override only. Independent diagnostic concern. |

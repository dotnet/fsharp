# F# RFC FS-XXXX — Improved Escape Analysis for Byref-Like Types

* [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/1143)
* [ ] Approved in principle
* [x] Implementation: [PR](TBD)
* [ ] [Discussion](TBD)

Links: [FS-1053 (Span/byref)](https://github.com/fsharp/fslang-design/blob/main/FSharp-4.5/FS-1053-span.md) · [C# 11 low-level struct improvements](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md)

## Summary

Extends [FS-1053](https://github.com/fsharp/fslang-design/blob/main/FSharp-4.5/FS-1053-span.md) escape analysis so that a call returning a byref-like type is **limited** (cannot escape) if any non-scoped argument is a `ByRef` whose referent does not outlive the current scope. Gated behind `--langversion:preview`. All existing behavior preserved when the flag is off.

## Motivation

.NET 7 [ref fields](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/ref-struct#ref-fields) let `Span<int>(ref x)` capture `x`. FS-1053 didn't track `ByRef` args for span-like returns:

```fsharp
let f () =
    let mutable x = 1
    Span<int>(&x) // Compiles today. Dangling span.
```

## Detailed design

Gated: `--langversion:preview` / `LanguageFeature.ImprovedByRefLikeEscapeAnalysis`.

### The rule

| Argument limit | Before (FS-1053) | After (this RFC) |
|---|---|---|
| `StackReferringSpanLike` | limited | limited |
| `ByRefOfStackReferringSpanLike` | limited | limited |
| `ByRef` to local | **ignored** | **limited** |
| `ByRef` to param | ignored | ignored |

### Scoped parameters (`[<ScopedRef>]`)

**Consumption:** Reads `ScopedRefAttribute` from IL metadata. Scoped parameters are excluded from the escape limit. Follows [C# `scoped`](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#rules-scoped).

**Authoring:** F# parameters annotated with `[<ScopedRef>]` emit `ScopedRefAttribute` and are honored in escape analysis for same-assembly calls.

```fsharp
open System.Runtime.CompilerServices

let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> =
    Span<int>(arr) // x is scoped — excluded from limit, callers with locals OK
```

### Struct receiver rule

Struct `this` is implicitly scoped ([C# 11 rule](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md#implicitly-scoped)). The receiver is included in the limit only when `[UnscopedRef]` is present on the method.

### RefSafetyRulesAttribute

F# reads `[module: RefSafetyRules(N)]` from referenced assemblies and emits it when `--langversion:preview` and the attribute type is available. When version ≥ 11:

- `out T` is implicitly scoped (excluded from limit)
- `ref`/`in T` where `T` is byref-like is implicitly scoped
- `ScopedRefAttribute` is trusted on generic methods

`[UnscopedRef]` on a parameter overrides implicit scoping. Without `RefSafetyRulesAttribute`, all parameters are treated conservatively as non-scoped.

### Resolution algorithm

For each IL call returning a byref-like type:

1. Read explicit `ScopedRefAttribute` on each parameter
2. If `RefSafetyRulesVersion >= 11`, merge implicit scoping (`out T`, `ref`/`in` to byref-like)
3. Negate scoped status where `UnscopedRefAttribute` is present
4. Apply mask to argument limits; combine remaining limits
5. If any remaining argument has `ByRef` to local → return is limited

On resolution failure, mask = None → all params treated as non-scoped. May cause false positives, never false negatives.

## C# interop

| C# parameter form | IL representation | F# behavior | Diverges? |
|---|---|---|---|
| `ref T` | bare byref | Limits span-like return | — |
| `in T` | `[In]` + `IsReadOnlyAttribute` | Same as `ref` — limits return | — |
| `out T` | `[Out]` | Implicitly scoped when `RefSafetyRulesVersion >= 11`; conservative otherwise | — |
| `scoped ref T` / `scoped in T` | `ScopedRefAttribute` | Excluded from limit | — |
| `scoped Span<T>` (value) | `ScopedRefAttribute` | Excluded from limit | — |
| `[UnscopedRef]` on member | `UnscopedRefAttribute` | Receiver NOT excluded (escape allowed) | — |
| `[UnscopedRef]` on param | `UnscopedRefAttribute` | Param NOT excluded (escape allowed) | — |
| `[UnscopedRef] out T` | both attributes | `out` no longer implicitly scoped — limits return | — |
| `ref` return → span capture | byref return → byref arg | Propagated via FS-1053; span captures it | — |
| `scoped` variance in overrides | — | **Not validated** — see Limitations | Yes |
| `[UnscopedRef]` on F# struct methods | `UnscopedRefAttribute` | **Partial** — honored cross-assembly (IL path), not same-assembly | Yes |
| Generic method `ScopedRefAttribute` | — | Read when `RefSafetyRulesVersion >= 11`; skipped otherwise (conservative) | — |

## Examples

| # | Code | Result | Why |
|---|------|--------|-----|
| 1 | `let f () = let mutable x = 1 in Span<int>(&x)` | **FS3235** | local byref |
| 2 | `let f (x: byref<int>) = Span<int>(&x)` | OK | param byref |
| 3 | `let f () = Span<int>([| 1;2;3 |])` | OK | no byref |
| 4 | `let f () = let mutable x = 1 in let s = Span<int>(&x) in s[0]` | OK | span doesn't escape |
| 5 | `let wrap (x: byref<int>) = Span<int>(&x)` then `wrap &local` | **FS3235** | propagated |
| 6 | `if flag then Span<int>(&local) else Span<int>(arr)` | **FS3235** | worst branch wins |
| 7 | `MemoryMarshal.CreateSpan(&local, 1)` | OK | `scoped ref T` in System.Runtime |
| 8 | Same code without `--langversion:preview` | old behavior | feature-gated |

### Struct receivers

| # | Code | Result | Why |
|---|------|--------|-----|
| 9 | `s.AsSpan()` where `s` is local, `[UnscopedRef]` on `AsSpan` | **FS3235** | receiver escapes |
| 10 | `s.AsSpan()` where `s` is `byref<_>` param | OK | param receiver |
| 11 | `s.GetSpan(arr)` where `s` is local, no `[UnscopedRef]` | OK | receiver is scoped |

### C# interop

| # | C# signature | F# call | Result |
|---|---|---|---|
| 12 | `Span<int> M(ref int x)` | `M(&local)` | **FS3235** |
| 13 | `Span<int> M(scoped ref int x)` | `M(&local)` | OK |
| 14 | `Span<int> M(in int x)` | `M(&local)` | **FS3235** |
| 15 | `Span<int> M(scoped ref int a, ref int b)` | `M(&s, &u)` | **FS3235** (`b`) |
| 16 | `Span<int> M(scoped Span<int> s)` | `M(Span(&local))` | OK |
| 17 | `Span<int> M([UnscopedRef] out int x)` | `M(&local)` | **FS3235** |
| 18 | `ref int M(ref int x)` then `Span(ref M(ref local))` | chained | **FS3235** |

## Errors

| Code | When | Fix |
|---|---|---|
| FS3234 | Named span variable escapes (e.g., `let s = Span(&local) in s`) | Move byref to parameter |
| FS3235 | Span-returning expression escapes (e.g., `Span(&local)`) | Same; or use array/pinned memory |

## Feature interactions

| Feature | Interaction |
|---|---|
| `task { }` / `async { }` | FS0412 (byref in closure) fires first; no new interaction |
| `inline` | Escape caught at call site on typed tree |
| `match` / `if-else` / `try-with` | Branches combined; worst wins |
| Active patterns | Can't return byref-like; no interaction |
| [`allows ref struct`](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-13.0/ref-struct-interfaces) (C# 13) | Generic return type resolved at call site; checked if byref-like |
| F#→F# calls | `[<ScopedRef>]` mask read from callee params |
| Property getters / indexers | IL call path; same rules |

## Drawbacks

- **False positives.** Resolution failure (e.g., malformed IL, unresolvable type) falls back to treating all params as non-scoped. Correct code may be rejected until the referenced assembly is updated.
- **Attribute-only authoring.** `[<ScopedRef>]` is verbose compared to C#'s `scoped` keyword. Users may expect a keyword.
- **No same-assembly `[UnscopedRef]` on struct methods.** F# has no syntax for this; manual attribute application works cross-assembly but not same-assembly.

## Alternatives

| Alternative | Why not |
|---|---|
| Do nothing | Unsound: `Span<int>(&local)` compiles and crashes at runtime |
| `scoped` keyword | Language design scope — can be added later as sugar for `[<ScopedRef>]` without breaking changes |
| Conservative-only (no attribute reading) | Too many false positives on real C# libraries (`MemoryMarshal`, `CollectionsMarshal`) |

## Compatibility

**Is this a breaking change for existing source?** Yes, under `--langversion:preview` only. Code that returns `Span<int>(&local)` will error with FS3235.

**Is this a breaking change for existing binaries?** No. No IL changes; only new compile-time checks.

**Is this a breaking change for existing packages?** No.

**Does this affect FSharp.Core?** No. FSharp.Core does not expose byref-like return types from local byrefs.

### Non-gated bugfixes (all language versions)

| Fix | Impact |
|---|---|
| Constructor exclusion from receiver (`not isCtor` guard) | `.ctor` calls no longer incorrectly treated as instance methods with `this` receiver |
| Return type resolution (`tyOfExpr g expr`) | Correctly identifies `Span<T>` when generic type parameters are instantiated |

These are correctness fixes for pre-existing bugs. They may allow previously over-rejected code to compile.

### Migration

1. Compile with `--langversion:preview` to discover errors
2. Move byref source to parameter
3. For struct receiver false positives: bind data to array before calling span-returning method

## Limitations and deferrals

Everything in this section is **not implemented** in this RFC. Each item is either explicitly out of scope or a known gap.

| Item | Status | Why |
|---|---|---|
| `ref` field declaration in F# | **Out of scope** | Different feature, different syntax design. Separate RFC. |
| `stackalloc` improvements | **Out of scope** | Consistent with FS-1053 approach. Separate RFC. |
| `scoped` keyword for F# | **Not implemented** | Can be added later as sugar for `[<ScopedRef>]`. No breaking change needed — the attribute is the underlying mechanism. |
| Scope variance validation in overrides | **Not implemented** | C# validates that overrides don't widen scoping. F# override checking compares types only, not parameter attributes. Risk is narrow: only C# callers of an F# override that removes `[<ScopedRef>]`. Independent diagnostic concern. |
| `[UnscopedRef]` on F# struct methods (same-assembly) | **Not implemented** | Honored cross-assembly via IL path. Same-assembly F# calls always treat struct `this` as scoped. F# has no syntax for `[UnscopedRef]` on methods; manual attribute application would require checking `ArgReprInfo.Attribs` in `CheckApplication`. |
| `[<ScopedRef>]` on curried partial applications | **Conservative fallback** | When arg count doesn't match (partial application), scoped mask is `None` — all params treated as non-scoped. May over-reject. |
| `ILType.TypeVar` implicit scoping | **Conservative fallback** | For `ref T` where `T` is a type variable, can't determine at the call site whether `T` is byref-like. Falls back to non-scoped. |
| Multi-TFM cross-reference testing | **Not tested** | E.g., `netstandard2.0` (LangVersion=8) referencing `net9.0` (LangVersion=preview) in the same solution. The design handles this (conditional on `RefSafetyRulesVersion` per assembly), but no integration test exists. |

## Unresolved questions

- Should F# add a `scoped` keyword in a future RFC? The attribute works but is verbose.
- Should F# validate scope variance in overrides? The risk surface is narrow (C# callers only), but it's a gap relative to C# 11.

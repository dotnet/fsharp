---
applyTo:
  - "src/Compiler/Checking/PostInferenceChecks.{fs,fsi}"
  - "tests/**/ByrefSafetyAnalysis/**"
---

# Byref & Span Escape Analysis

## Background: what "escape" means and why it matters

A **byref** (`&x` in F#, `ref` in C#) is a managed pointer to a stack location. A **span** (`Span<T>`, `ReadOnlySpan<T>`) is a byref-like struct that may contain a ref field pointing to stack memory. If either is returned from the function that owns the stack frame, the caller gets a dangling pointer — a use-after-free bug with no GC safety net.

"Escape" means a value leaves the scope where its target memory is valid. The compiler must prove, at compile time, that no byref or byref-like value "escapes" the lifetime of the data it points to.

### The .NET evolution

| Era | What changed | Why it matters |
|-----|-------------|----------------|
| .NET Core 2.1 | `Span<T>` introduced as `ref struct` | Stack-only types need escape rules |
| C# 7.2 / F# 4.5 | `Span<T>` support, basic byref returns | F# added `PostInferenceChecks` limit tracking |
| C# 11 / .NET 7 | `ref` fields in structs, `scoped` keyword, `[UnscopedRef]` | `Span<int>(ref x)` can now capture a local — old rules insufficient |
| C# 11 | `[module: RefSafetyRules(11)]` emitted | Signals new implicit scoping rules to consumers |
| F# (this feature) | Improved escape analysis | Detects `Span<int>(&local)` dangling refs; reads/emits C# 11 attributes |

### Key .NET / C# design references

- [C# low-level struct improvements](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md) — the C# 11 spec defining `scoped`, `UnscopedRef`, implicit scoping rules, `RefSafetyRulesAttribute`
- [F# language suggestion #1143](https://github.com/fsharp/fslang-suggestions/issues/1143) — the original request
- F# RFC: `docs/rfc/FS-XXXX-improved-byreflike-escape-analysis.md`

### Types and syntax involved

| F# syntax | What it is | Escape relevance |
|-----------|-----------|------------------|
| `&x` | Address-of a mutable local or field (`byref<T>`) | Creates a byref — the core thing that must not escape |
| `&&x` | Native pointer address-of (`nativeptr<T>`) | Similar concern but handled separately via `NativePtr` |
| `byref<T>` | Managed pointer (IL `T&`) | Can be returned if pointing to caller-owned memory |
| `inref<T>` | Read-only managed pointer (IL `T&` + `IsReadOnly`) | Same escape rules as `byref` |
| `outref<T>` | Out parameter (IL `T&` + `IsOut`) | Implicitly scoped in C# 11+; safe (callee writes, doesn't capture) |
| `Span<T>` | Byref-like struct, may contain `ref T` field | If constructed from `&local`, the span is "stack-referring" |
| `ReadOnlySpan<T>` | Read-only span | Same escape rules as `Span<T>` |
| `[<IsByRefLike>]` | Marks any ref struct | All ref structs follow span-like escape rules |
| `scoped` (C#) / `[<ScopedRef>]` (F#) | Parameter cannot escape | Excluded from return-limit computation |
| `[UnscopedRef]` (C#) | Opts out of implicit scoping | struct `this` or `out` param CAN escape |

## Architecture of PostInferenceChecks.fs

`PostInferenceChecks.fs` walks every expression in the typed tree **after** type inference is complete. It tracks a `Limit` for each expression — a pair of `(scope: int, flags: LimitFlags)` describing whether the expression refers to stack memory and how.

### Limit flags

```
LimitFlags.ByRef                         — expression is a byref (&x)
LimitFlags.ByRefOfSpanLike               — byref pointing into a span
LimitFlags.ByRefOfStackReferringSpanLike — byref pointing into a stack-referring span
LimitFlags.SpanLike                      — expression is a span-like type
LimitFlags.StackReferringSpanLike        — span that refers to stack memory
```

A `scope >= 1` means the value is **local** (stack-bound). The checker prevents returning local-scoped limited values.

### Call checking flow

Two call paths, both ending at `CheckCallLimitArgs`:

```
Expr.App → CheckApplication → CheckCall / CheckCallWithReceiver
  ↑ F#-to-F# same-assembly calls
  Scoped mask from ArgReprInfo.Attribs ([<ScopedRef>])

TOp.ILCall → CheckExprOp → CheckCall / CheckCallWithReceiver
  ↑ IL calls (C# interop + cross-assembly F#)
  Scoped mask from ILParameter.CustomAttrs (ScopedRefAttribute)
  Implicit scoping from RefSafetyRulesVersion >= 11 (out T, ref/in T to byref-like)
  UnscopedRef from ILMethodDef.CustomAttrs (struct this) + ILParameter.CustomAttrs (param)
```

`CheckCallLimitArgs` decides whether the combined argument limits make the return value unsafe:
- **Byref return**: limited if any arg is a local byref or stack-referring span-like
- **Span-like return**: limited if any arg is a stack-referring span-like, a byref-of-stack-referring-span-like, or (with improved analysis) a local byref — because `Span<T>(ref local)` captures `&local` via a ref field

### Scoped parameter masking

A "scoped" parameter is one whose value the callee promises not to capture in its return. Scoped parameters are **excluded** from the combined limit — they don't infect the return.

The mask is a `bool[]` aligned 1:1 with non-receiver arguments. `true` = scoped (zeroed out). The mask is computed differently per call path, then applied by `ApplyScopedMask`.

### Address-of expressions

`TOp.LValueOp(LAddrOf, vref)` is the typed-tree node for `&x`. `GetLimitValByRef` computes the byref limit: if `vref` is a local mutable, the limit has `scope >= 1` and `LimitFlags.ByRef`, meaning it must not escape. If it's a parameter or module-level mutable, it has `scope = 0` (safe to return).

## Key invariants

1. **Mask alignment**: `scopedMask` length must equal the number of non-receiver arguments. `CheckCallWithReceiver` splits the receiver off — mask indices are 0-based on remaining args.

2. **Conservative default**: When mask computation fails (resolution error, partial application, generic methods), return `None`. All params treated as non-scoped — may over-reject, never under-reject.

3. **Feature gating**: The `ByRef + IsLocal` check in `CheckCallLimitArgs` is gated by `cenv.improvedByRefLikeEscapeAnalysis`. The `not isCtor` and `tyOfExpr g expr` fixes are NOT gated (bugfixes applying at all lang versions).

4. **Generic method guard**: `methInst.IsEmpty || refSafetyVersion >= 11` — skip mask for generic IL methods unless the assembly declares `RefSafetyRules`, because C# may emit implicit `ScopedRefAttribute` via `RefSafetyRulesAttribute` which F# cannot interpret without that context.

## Helpers (PostInferenceChecks.fs)

- `tryResolveILMethodContext amap m ilMethRef` — resolve ILMethodRef → `(ILMethodDef * refSafetyVersion) option`
- `tryGetILParamAttrMask attribOpt methDef` — generic IL parameter attribute mask builder
- `tryGetScopedParamMask g methDef` / `tryGetUnscopedRefParamMask g methDef` — aliases for specific attributes
- `tryGetScopedParamMaskFromFSharpAttribs g argInfos` — ScopedRefAttribute mask from F# `ArgReprInfo.Attribs`
- `hasUnscopedRefAttribute g methDef` — method-level UnscopedRef check (struct `this`)
- `isImplicitlyScopedParam g amap m p` — implicit scoping: `out T` or `ref/in T` where T is byref-like
- `computeScopedMask cenv m methDef methInst refSafetyVersion` — full pipeline: explicit → implicit → UnscopedRef negation
- `ApplyScopedMask mask limits` — zero out limits for scoped params; falls back to identity on length mismatch

## Plumbing across files

- `TcGlobals`: `attrib_ScopedRefAttribute_opt`, `attrib_UnscopedRefAttribute_opt`, `attrib_RefSafetyRulesAttribute_opt`
- `TypedTree`: `CcuData.RefSafetyRulesVersion` (0 = absent, 11 = C# 11+)
- `import.fs`: `GetRefSafetyRulesVersion` reads `[module: RefSafetyRules(N)]` from ILModuleDef
- `IlxGen.fs`: emits `[module: RefSafetyRules(11)]` when feature is enabled

## Diagnostics

| Code | Name | When |
|------|------|------|
| FS3232 | `chkStructsMayNotReturnAddressesOfContents` | Struct member returns `&this.field` |
| FS3233 | `chkNoByrefLikeFunctionCall` | Call mixes byref-of-span with stack-referring span |
| FS3234 | `chkNoSpanLikeVariable` | Named span variable used in escape position |
| FS3235 | `chkNoSpanLikeValueFromExpression` | Span-like expression result escapes (the new path) |
| FS3236 | `tastCantTakeAddressOfExpression` | Address of non-addressable expression |

## Test conventions

Every error test (`shouldFail |> withErrorCodes [3235]`) must have a backward-compat twin that compiles the same code without `--langversion:preview` and expects `shouldSucceed`. This ensures the feature gate works.

C# interop tests use inline `CSharp """..."""` with `withCSharpLanguageVersion CSharpLanguageVersion.CSharp11`. Place inside `#if NET7_0_OR_GREATER` block.

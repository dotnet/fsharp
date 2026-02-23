# Hypothesis Investigation: ByRefLike Escape Analysis Soundness

## Issue Summary
Investigating potential soundness gaps in `PostInferenceChecks.fs` implementation of Improved ByRefLike Escape Analysis. Focus on false negatives where unsafe code might compile without error.

## Hypotheses

### Hypothesis 1: IL Resolution Failure Defaults to Unsafe (Struct Receivers)
**Theory**: `tryResolveILMethodDef` returns `None` on failure. `CheckExprOp` handles `None` by setting `scopedMask` to `None`. `CheckCall` with `None` mask falls back to `CheckExprs` (standard analysis).
However, for struct receivers, `hasUnscopedRef` defaults to `false`.
If a struct method is `[UnscopedRef]` but resolution fails, `hasUnscopedRef` is false.
In `CheckCallWithReceiver`:
```fsharp
elif improvedEscapeAnalysis && hasUnscopedRef && HasLimitFlag LimitFlags.ByRef receiverLimit then
    CombineTwoLimits limitArgs receiverLimit
else
    limitArgs
```
If `hasUnscopedRef` is false, the receiver limit is NOT combined (unless it's a stack-referring span-like).
**Implication**: If I have a struct `ref` local, and I call a method on it that *should* let it escape (marked `[UnscopedRef]`), but IL resolution fails (e.g. type forwarder, missing dep), the compiler treats it as scoped (safe). The local escapes.
**Risk**: High. Resolution failures happen.

### Hypothesis 2: Version Forgery / Mismatch
**Theory**: `getRefSafetyRulesVersion` reads the version from the assembly.
If an assembly claims `RefSafetyRulesVersion = 11` (C# 11 rules), the compiler assumes `out` parameters are implicitly scoped.
```fsharp
// V2-3: out T → always implicitly scoped when version ≥ 11
(p.IsOut && not p.IsIn)
```
If an assembly lies (says v11 but was compiled by old compiler), it might have an `out` param that *does* escape (e.g. old C# semantics or custom IL), but F# treats it as scoped.
**Counter-argument**: `out` parameters in C# 11 are scoped *by default*. If they want to escape, they need `[UnscopedRef]`. If an old compiler compiled it, it wouldn't have `[UnscopedRef]`.
So if we treat it as scoped, we are being *more permissive* than the old compiler intended?
Old C#: `out` is like `ref`. It can be returned.
`ref int M(out int x) { x = ...; return ref x; }`
If we treat `out` as scoped, we assume it *cannot* be returned/captured.
So if we don't limit the return of `M` by `x`.
But `M` *does* return `x`.
So we have a use-after-free.
**Verification**: Create an IL assembly with `RefSafetyRules(11)` but a method `ref int M(out int x)` that returns `x`. Call it from F#. Pass local. Check if error.

### Hypothesis 3: Generic Method Resolution Failure
**Theory**: `tryResolveILMethodDef` fails for some generic method instantiations or complex types.
Code:
```fsharp
let methDefOpt = ... tryResolveILMethodDef ...
// ...
match methDefOpt with
| Some methDef -> ...
| None -> None, false
```
If `None`, `scopedMask` is `None`. `CheckCall` falls back to `CheckExprs`.
`CheckExprs` combines all limits.
This is **safe** (conservative). It treats everything as non-scoped (escaping).
Exception: The struct receiver `hasUnscopedRef` logic (H1).

### Hypothesis 4: `tyOfExpr` vs Raw IL Return Type
**Theory**: `CheckExprOp` now uses `tyOfExpr g expr` instead of `retTypes` from `TOp.ILCall`.
`let returnTy = tyOfExpr g expr`
`TOp.ILCall` contains the substituted return type `retTypes`.
`tyOfExpr` re-computes type from expression.
If `expr` is the `ILCall` itself, `tyOfExpr` should match.
However, `MemoryMarshal.CreateSpan` soundness fix relied on this.
Risk: `tyOfExpr` might be wrong if the expression has been wrapped or if type inference did something weird?
Likely safe/correct as it's the F# view of the type.

### Hypothesis 5: `limitArgs.IsLocal` calculation
**Theory**: The `IsLocal` flag is computed during `CheckExpr`.
We checked `CombineTwoLimits`.
- `GetLimitValByRef` returns `scope=1` and `LimitFlags.ByRef` for locals. `isLimited` becomes true.
- `CombineTwoLimits` preserves scope of the limited item.
- If multiple items are limited, it takes `Math.Max` of scopes.
- If one item is limited and another is not (e.g. constant), it takes the limited one.
**Conclusion**: `IsLocal` (scope >= 1) seems correctly preserved during combination.
**Status**: DISPROVED (Code looks correct).

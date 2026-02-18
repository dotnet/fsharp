---
applyTo:
  - "src/Compiler/Checking/PostInferenceChecks.{fs,fsi}"
  - "tests/**/ByrefSafetyAnalysis/**"
---

# Byref-Like Escape Analysis

Read `docs/rfc/FS-XXXX-improved-byreflike-escape-analysis.md` for the full spec.

## Architecture

Two call paths for escape analysis, both ending at `CheckCallLimitArgs`:

```
Expr.App → CheckApplication → CheckCall/CheckCallWithReceiver
  ↑ F#-to-F# same-assembly calls
  Scoped mask from ArgReprInfo.Attribs ([<ScopedRef>])

TOp.ILCall → CheckExprOp → CheckCall/CheckCallWithReceiver
  ↑ IL calls (C# interop + cross-assembly F#)
  Scoped mask from ILParameter.CustomAttrs (ScopedRefAttribute)
  UnscopedRef from ILMethodDef.CustomAttrs (struct this) + ILParameter.CustomAttrs (param)
```

## Key invariants

1. **Mask alignment**: `scopedMask` length must equal the number of non-receiver arguments. `CheckCallWithReceiver` splits the receiver off — mask indices are 0-based on remaining args.

2. **Conservative default**: When mask computation fails (resolution error, partial application, generic methods), return `None`. This means all params treated as non-scoped — may over-reject, never under-rejects.

3. **Feature gating**: The new `ByRef + IsLocal` check in `CheckCallLimitArgs` is gated by `cenv.improvedByRefLikeEscapeAnalysis`. The `not isCtor` fix and `tyOfExpr g expr` fix are NOT gated (they're bugfixes).

4. **methInst.IsEmpty guard**: Skip scoped mask reading for generic IL methods. C# may emit implicit `ScopedRefAttribute` via `RefSafetyRulesAttribute` which F# does not read. Without this guard, generic methods could be wrongly treated as having scoped params.

## Helpers (current)

- `tryGetScopedParamMask g methDef` — ScopedRefAttribute mask (IL)
- `tryGetUnscopedRefParamMask g methDef` — UnscopedRefAttribute mask (IL)
- `tryGetScopedParamMaskFromFSharpAttribs g argInfos` — ScopedRefAttribute mask (F# Attribs)
- `hasUnscopedRefAttribute g methDef` — method-level UnscopedRef check (struct `this`)
- `ApplyScopedMask mask limits` — zero out limits for scoped params (lengths must match)

Note: `tryGetScopedParamMask` and `tryGetUnscopedRefParamMask` are near-identical and should be refactored into a shared `tryGetILParamAttrMask` (see Refactors below).

## Test conventions

Every error test (`shouldFail |> withErrorCodes [3235]`) must have a backward-compat twin that compiles the same code without `--langversion:preview` and expects `shouldSucceed`. This ensures the feature gate works.

C# interop tests use inline `CSharp """..."""` with `withCSharpLanguageVersion CSharpLanguageVersion.CSharp11`. Place inside `#if NET7_0_OR_GREATER` block.

## Pending work (review council round 2)

### Tests to add

1. **Cross-assembly F#→F# `[<ScopedRef>]`**: Compile F# lib with `[<ScopedRef>]` param as separate assembly, reference from consumer F# project, verify scoped mask works via IL path. `GenAttrs` already emits the attribute (verified — not in `GenParamAttribs` exclusion list).

2. **Backward-compat twins**: `Non-scoped ref param still triggers escape error` and `Mixed scoped and non-scoped params` lack `_without_preview` twins.

3. **Inline function escape**: `let inline f (x: byref<int>) : Span<int> = Span(&x)` called with local — verify FS3235 at call site.

4. **`[UnscopedRef]` on non-out ref param**: C# `Span<int> M([UnscopedRef] scoped ref int x, int[] arr)` — `[UnscopedRef]` negates `scoped`, F# call with local should error.

### Refactors to apply

1. **Dedup mask helpers**: `tryGetScopedParamMask` and `tryGetUnscopedRefParamMask` are near-identical — extract `tryGetILParamAttrMask (attribOpt) (methDef)` parameterized by attribute.

2. **Flatten TOp.ILCall nesting**: Extract `computeEffectiveScopedMask g methDef methInst hasReceiver` using `Option.bind` to collapse 5-level nested match into a pipeline.

3. **Flatten CheckApplication nesting**: Extract `tryGetFSharpScopedMask g f hasReceiver argslLength` to collapse 4-level nested match.

4. **Share IL method resolution**: The `TOp.ILCall` arm resolves the same `ilMethRef` twice (once for `HasAllowsRefStruct`, once for scoped mask). Resolve once and reuse.

5. **Remove dead guard in `ApplyScopedMask`**: The `i < scopedMask.Length` check at line 197 is unreachable after the `failwith` precondition at line 192.

6. **Upgrade test assertions**: ~15 tests use `withErrorCodes [3235]` instead of full `withDiagnostics` with line/col/message. At minimum upgrade the control-flow tests (IfElse, Match, TryWith) where error location matters.

### RFC updates

1. Document `methInst.IsEmpty` guard in Decisions table.
2. Document `hasUnscopedRef=false` for same-assembly F# struct methods in Decisions table.

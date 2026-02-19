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

- `tryGetILParamAttrMask attribOpt methDef` — generic IL parameter attribute mask builder
- `tryGetScopedParamMask g methDef` — ScopedRefAttribute mask (IL), alias of tryGetILParamAttrMask
- `tryGetUnscopedRefParamMask g methDef` — UnscopedRefAttribute mask (IL), alias of tryGetILParamAttrMask
- `tryGetScopedParamMaskFromFSharpAttribs g argInfos` — ScopedRefAttribute mask (F# Attribs)
- `hasUnscopedRefAttribute g methDef` — method-level UnscopedRef check (struct `this`)
- `ApplyScopedMask mask limits` — zero out limits for scoped params (lengths must match)

## Test conventions

Every error test (`shouldFail |> withErrorCodes [3235]`) must have a backward-compat twin that compiles the same code without `--langversion:preview` and expects `shouldSucceed`. This ensures the feature gate works.

C# interop tests use inline `CSharp """..."""` with `withCSharpLanguageVersion CSharpLanguageVersion.CSharp11`. Place inside `#if NET7_0_OR_GREATER` block.

## Resolved work (review council round 2)

All items from the review council round 2 have been implemented:

- ✅ Cross-assembly F#→F# `[<ScopedRef>]` test
- ✅ Backward-compat twins for C# interop error tests
- ✅ Inline function escape test
- ✅ `[UnscopedRef]` on out param in mixed params test
- ✅ RFC decisions: `hasUnscopedRef=false`, `methInst.IsEmpty` guard
- ✅ Extracted `tryGetILParamAttrMask` (deduped mask helpers)
- ✅ Flattened TOp.ILCall and CheckApplication nesting with `Option.bind`
- ✅ Removed dead guard in `ApplyScopedMask`
- ✅ Upgraded control-flow test assertions to `withDiagnostics`

## V2: RefSafetyRulesAttribute support

V2 adds reading and emission of `[module: RefSafetyRules(11)]`:

### New plumbing
- `TcGlobals.attrib_RefSafetyRulesAttribute_opt` — attribute type lookup
- `CcuData.RefSafetyRulesVersion` — stored per-assembly (0 = absent, 11 = C# 11+)
- `GetRefSafetyRulesVersion` (import.fs) — reads attribute from ILModuleDef
- `getRefSafetyRulesVersion` (PostInferenceChecks.fs) — resolves version from ILMethodRef

### TOp.ILCall arm changes
- `refSafetyVersion` computed before mask pipeline
- Guard relaxed: `methInst.IsEmpty || refSafetyVersion >= 11`
- Implicit mask merged between `tryGetScopedParamMask` and `tryGetUnscopedRefParamMask`:
  - `out T` (IsOut && not IsIn) → implicitly scoped
  - `ref`/`in` to ref struct (ILType.Byref inner where inner is byref-like) → implicitly scoped
- `UnscopedRefAttribute` negation still applies after merge

### Emission
- `IlxGen.fs`: `[module: RefSafetyRules(11)]` added to `ilNetModuleAttrs` when feature is enabled

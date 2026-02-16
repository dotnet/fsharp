# F# RFC FS-XXXX — Improved Escape Analysis for Byref-Like Types

## Summary

This RFC improves the escape analysis for byref-like types (`Span<T>`, `ReadOnlySpan<T>`, custom `[<IsByRefLike>]` structs) to account for ref fields introduced in .NET 7. Under the new rules, a byref argument to a method returning a byref-like type is treated as potentially captured, preventing unsound escapes.

## Motivation

Since .NET 7, byref-like structs can contain ref fields. This means a constructor like `Span<int>(ref x)` can store a reference to `x` inside the returned `Span`. The previous F# escape analysis did not account for this — it only tracked `StackReferringSpanLike` and `ByRefOfStackReferringSpanLike` arguments, not plain `ByRef` arguments. As a result, the following unsound code compiled without error:

```fsharp
let f () =
    let mutable x = 1
    Span<int>(&x)  // Returns a span pointing to a local — use-after-free!
```

## Detailed Design

### Feature Flag

This change is gated behind `LanguageFeature.ImprovedByRefLikeEscapeAnalysis` and requires `--langversion:preview`.

### Core Rule Change

In `CheckCallLimitArgs` (PostInferenceChecks.fs), the condition for a span-like return being "limited" is extended:

**Before (F# 8 and earlier):**
A span-like return is limited if arguments contain `StackReferringSpanLike` or `ByRefOfStackReferringSpanLike`.

**After (with this feature):**
A span-like return is limited if arguments contain any of:
- `StackReferringSpanLike`
- `ByRefOfStackReferringSpanLike`
- `ByRef` with `scope >= 1` (i.e., the byref refers to a local)

### Struct Receiver Tracking

When improved analysis is enabled, `CheckCallWithReceiver` also includes a receiver with `LimitFlags.ByRef` in the limit computation. This handles C# structs with `[UnscopedRef]` methods that can return spans pointing into `this`:

```csharp
public struct Evil {
    public int Field;
    [UnscopedRef]
    public Span<int> AsSpan() => new Span<int>(ref Field);
}
```

Without tracking the receiver, F# would allow:
```fsharp
let f () =
    let mutable s = Evil()
    s.AsSpan()  // Unsound: span points into stack-local struct
```

This is conservative — it also prevents safe struct methods from returning spans when called on local structs. Future work can read `[UnscopedRef]` to reduce false positives.

### ScopedRefAttribute Consumption

C# methods can mark parameters as `scoped ref` or `scoped in`, indicating the parameter cannot escape. The compiler reads `System.Runtime.CompilerServices.ScopedRefAttribute` from IL metadata and excludes scoped parameters from the escape limit computation.

This is implemented in `tryGetScopedParamMask`, which resolves an `ILMethodRef` to its `ILMethodDef`, reads `ScopedRefAttribute` on each parameter, and returns a boolean mask. `ApplyScopedMask` zeros out limits for scoped parameters.

**Scope**: Only applied for IL calls (`TOp.ILCall`) returning byref-like types in returnable context. F#-declared methods cannot currently be marked as `scoped` (no F# syntax exists for this).

### Error Messages

No new error messages are introduced. The feature reuses existing error codes:
- **FS3234**: "The Span or IsByRefLike variable '%s' cannot be used at this point..."
- **FS3235**: "A Span or IsByRefLike value returned from the expression cannot be used at this point..."

## Breaking Changes

Under `--langversion:preview`, code that previously compiled will now be rejected:

```fsharp
// Previously compiled, now errors with FS3234/FS3235:
let f () =
    let mutable x = 1
    Span<int>(&x)

// Workaround: pass byref from caller
let f (x: byref<int>) =
    Span<int>(&x)
```

Without `--langversion:preview`, behavior is unchanged.

## Known Limitations

1. **No UnscopedRef reading**: The compiler does not read `[UnscopedRef]` on struct methods. All struct method receivers with `ByRef` flag are treated conservatively. Future work.
2. **F# `scoped` keyword**: F# has no `scoped` parameter keyword. Scoped parameter semantics are only consumed from C# metadata, not expressible in F#. Future work.
3. **ScopedRef IL-only**: The scoped mask is only computed for IL calls. F#-to-F# calls through `CheckApplication` always pass `None` for the scoped mask. This is correct because F# cannot declare scoped parameters.
4. **stackalloc**: Not addressed in this change. Future work per fslang-suggestions#1143.
5. **ref field declaration**: F# cannot declare ref fields. This feature only improves consumption of types with ref fields.

## Interaction with FS-1053

This feature extends the escape analysis rules from FS-1053 (Span, byref, inref, outref, IsByRefLike). It does not change any of the existing rules — it adds a new condition that triggers the existing error reporting. All FS-1053 behavior is preserved when the feature flag is off.

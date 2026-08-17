# Regression: Legacy inline metadata decoded as non-inline, breaking cross-assembly SRTP

## Summary

Adding a new `ValInline.InlinedDefinition` case reused the serialized inline-flag bit pattern `0x00`, which already meant "required inline" in assemblies compiled by F# 5.0 and earlier. A newer compiler reading those older assemblies decoded the value as *not* inlined, dropped the inline body at the call site, and emitted a direct call to a dynamic-invocation stub. Cross-assembly SRTP APIs such as Aether 8.3.1 then threw `System.NotSupportedException` at runtime. Shipped in .NET SDK 10.0.400.

## Error Manifestation

A program that consumes an inline SRTP API from a pre-F#6 library compiles cleanly but throws at runtime, including in optimized Release builds:

```text
Unhandled exception. System.NotSupportedException:
Dynamic invocation of op_HatEquals is not supported
   at Aether.Optic.set[a,b,c](a optic, b value)
```

The same source built with SDK 10.0.303 prints the expected result. `--always-inline+` does not help; rebuilding the referenced library with a current compiler does.

## Root Cause

`ValFlags` packs a value's inline declaration into two bits of an `int64` that is serialized verbatim into assembly metadata. The bit patterns are a permanent on-disk contract.

In F# 5.0 and earlier the field had a `PseudoVal` case — "must always be inlined, no IL body needed" — encoded as `0x00` with `ShouldInline = true`:

```fsharp
match (flags &&& 0b110000L) with
| 0b000000L -> ValInline.PseudoVal   // ShouldInline = true
| 0b010000L -> ValInline.Always
| ...
```

PR #6811 (July 2021, F# 6) removed `PseudoVal` and folded it into `Always`. Crucially, `0x00` kept decoding to a `ShouldInline = true` value, so libraries built before the removal continued to import correctly.

PR #19548 introduced `ValInline.InlinedDefinition` and reused the now-"free-looking" `0x00` bit pattern for it — but with the *opposite* semantics, `ShouldInline = false`. The reader was changed so `0x00` decoded to `InlinedDefinition`. That silently reinterpreted every `0x00` inline value already sitting in shipped DLLs: a required-inline definition from an old library now imported as non-inline, so the consuming compiler emitted a direct call to the SRTP dynamic-invocation stub instead of inlining the resolved witness.

The violated assumption is the "tag values are forever" rule: a bit pattern that already has a meaning in shipped metadata cannot be given a new, incompatible meaning.

## Why It Escaped

PR #19548 *did* add write-side normalization so a current compiler serializes `InlinedDefinition` as `Always` (`0x10`), keeping fresh round-trips correct. That protection is exactly what hid the bug:

- Any in-repo test compiles the producer library **with the new compiler**, which never writes `0x00` for an inline value. So no test that builds its own fixtures could reproduce it — the poisoned byte only exists in binaries produced by an F# 5.0-or-earlier compiler.
- The `CompilerCompat` cross-version suite exercises recent SDKs (9 ↔ current), not pre-2021 F# 5 binaries, so the format generation that still emits `0x00` inline bits was outside its matrix.

The gap was read-side: the new meaning was applied to old bytes, and nothing in CI reads bytes written by a 2021-era compiler.

## Fix

PR #20260 adds `ValFlags.OfPickledBits`, used by `u_ValData` when importing metadata. Because the write side always normalizes `InlinedDefinition` to `Always`, a serialized `0x00` inline field can only originate from a legacy compiler, where it meant required inline. `OfPickledBits` therefore maps legacy `0x00` back to `Always` on import. The serialized byte layout is unchanged; only interpretation of the legacy pattern is restored.

## Timeline

| Date | Event |
|---|---|
| ≤ 2021 | F# 5.0 and earlier encode required-inline values (`PseudoVal`) as inline bits `0x00`, `ShouldInline = true`. |
| 2021-07-19 | PR #6811 removes `PseudoVal`; `0x00` still decodes to a `ShouldInline = true` value. Old libraries keep working. |
| 2026-04-16 | Commit `761c8635b8` adds write-side normalization for the upcoming `InlinedDefinition` (`0x00` → `0x10` on pickle). |
| 2026-07-02 | PR #19548 merges: `InlinedDefinition` reuses `0x00` with `ShouldInline = false`; reader decodes `0x00` → `InlinedDefinition`. Latent regression for legacy binaries. |
| ~2026-08 | Ships in .NET SDK 10.0.400. |
| 2026-08-13 | Issue #20253 filed: Aether 8.3.1 SRTP call throws `NotSupportedException` under 10.0.400. |
| — | PR #20260 adds read-side normalization (`OfPickledBits`), restoring the invariant. |

## Prevention

The generalized rule — flag bit patterns baked into pickled metadata are permanent, and a pattern that already has a meaning in shipped DLLs must never be reinterpreted — is encoded in [`.github/instructions/TypedTreePickle.instructions.md`](../../.github/instructions/TypedTreePickle.instructions.md), whose `applyTo` covers the flag-encoding types in `src/Compiler/TypedTree/TypedTree.{fs,fsi}` and the pickle path. When adding a case to a serialized flag enum, either allocate an unused bit pattern or add read-side normalization that maps legacy patterns to their original semantics — write-side normalization alone only protects future binaries, never the ones already in the wild.

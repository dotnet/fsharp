# FSharpPlus curryN Regression Fix - Executive Summary

## The Bug

FSharpPlus `curryN` pattern worked in F# SDK 8.0 but failed in SDK 9.0+ with error FS0030 (value restriction):

```fsharp
let _x1 = curryN f1 100  // SDK 8: OK | SDK 9+: FS0030 error
```

## Root Cause

In `CheckDeclarations.fs`, the `ApplyDefaults` function processes unsolved type variables at the end of type checking. The existing code only solved typars with `StaticReq <> None`:

```fsharp
if (tp.StaticReq <> TyparStaticReq.None) then
    ChooseTyparSolutionAndSolve cenv.css denvAtEnd tp
```

**The problem**: Some SRTP (Statically Resolved Type Parameter) typars have `MayResolveMember` constraints but `StaticReq=None`. These typars were being skipped, leaving them unsolved. When `CheckValueRestriction` ran next, it found unsolved typars and reported FS0030.

## Why This Is a Bug

The `ApplyDefaults` code checks `StaticReq <> None` to identify SRTP typars that need solving. However, a typar may participate in an SRTP constraint (having a `MayResolveMember` constraint) without having `StaticReq` set. This can happen when:

1. The typar is the **result type** of an SRTP method call, not the head type
2. The typar is constrained through SRTP constraints but isn't directly marked with `^`

**Key insight from instrumentation:**
```
  - ? (StaticReq=None, Constraints=[MayResolveMember, CoercesTo])  ← HAS SRTP CONSTRAINT!
[ApplyDefaults] After processing: 17 still unsolved  ← SRTP typar SKIPPED because StaticReq=None
```

The condition `tp.StaticReq <> None` was too narrow - it missed typars that have SRTP constraints but no explicit static requirement.

## Regression Analysis - Git Blame Evidence

**Root Cause: PR #15181 (commit `b73be1584`) - Nullness Checking Feature**

The regression was introduced by `FreshenTypar` added in PR #15181:

```fsharp
// src/Compiler/Checking/NameResolution.fs:1600-1604
let FreshenTypar (g: TcGlobals) rigid (tp: Typar) =
    let clearStaticReq = g.langVersion.SupportsFeature LanguageFeature.InterfacesWithAbstractStaticMembers
    let staticReq = if clearStaticReq then TyparStaticReq.None else tp.StaticReq  // ← BUG!
    ...
```

**The Mechanism:**

1. **SDK 8**: `FreshenTypar` did not exist. When typars were freshened, `StaticReq` was preserved from the original typar.

2. **SDK 9+**: When `InterfacesWithAbstractStaticMembers` is enabled (always on), `FreshenTypar` **clears `StaticReq` to `None`** unconditionally.

3. **Effect**: SRTP typars still have `MayResolveMember` constraints, but lose their `StaticReq` marker.

4. **Consequence**: `ApplyDefaults` checks `if tp.StaticReq <> None` → returns false → typar never solved → FS0030 error.

**The fix** adds an alternative check for `MayResolveMember` constraints directly, making `ApplyDefaults` robust against this `StaticReq` clearing.

## The Fix

Added a check for `MayResolveMember` constraints in addition to `StaticReq`:

```fsharp
let hasSRTPConstraint = tp.Constraints |> List.exists (function TyparConstraint.MayResolveMember _ -> true | _ -> false)
if (tp.StaticReq <> TyparStaticReq.None) || hasSRTPConstraint then
    ChooseTyparSolutionAndSolve cenv.css denvAtEnd tp
```

## Verification

- ✅ New curryN-style regression test passes
- ✅ FSharpPlus `curryN` pattern compiles without type annotations
- ✅ No regressions introduced in SRTP-related tests

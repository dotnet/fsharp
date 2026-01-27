# Replan Request for Issue #16292

## Current Status
The investigation of Issue #16292 (Debug SRTP mutable struct incorrect codegen) has revealed that the fix is more complex than initially anticipated.

## Investigation Summary

### Root Cause Analysis
1. In Debug mode with `--optimize-`, when a SRTP trait call is made on a mutable struct local (`iter.MoveNext()`), the compiler creates a defensive copy of the struct.

2. The defensive copy logic is in `mkExprAddrOfExprAux` in `TypedTreeOps.fs`. When the function can't recognize that an expression is a mutable local that can be addressed directly, it falls through to the catch-all case that creates `copyOfStruct`.

3. Initially suspected: `Expr.DebugPoint` wrappers around expressions prevent pattern matching on `Expr.Val`.

4. Fix attempted: Call `stripDebugPoints` before pattern matching in `mkExprAddrOfExprAux`.

5. Result: Test still hangs - fix not working.

### Hypotheses for Why Fix Isn't Working

1. **Expression Structure**: The receiver expression may not be a simple `Expr.Val` - it could be wrapped in additional layers (Let bindings, applications, etc.) that my fix doesn't handle.

2. **Variable Mutation During Inlining**: The inline function's local variable may be copied/renamed during inlining in a way that changes its `IsMutable` property.

3. **Different Code Path**: The defensive copy may be created in a different location, not in `mkExprAddrOfExprAux`.

4. **Test Infrastructure**: The test may be using a cached/old version of the compiler.

## Proposed Next Steps

### Option A: Deeper Investigation
1. Add diagnostic logging to `mkExprAddrOfExprAux` to see what expressions are being passed.
2. Check the exact expression structure after inlining.
3. Trace through `MustTakeAddressOfVal` to verify it returns `true` for the receiver.

### Option B: Alternative Fix Location
1. Look at IlxGen.fs more closely for where the trait call is generated.
2. Check if there's a way to fix this at the IL generation level.
3. Look at how the optimizer handles this (it works in Release mode).

### Option C: Mark as KNOWN_LIMITATION
If after further investigation the fix proves too risky or complex:
1. Document the issue as a known limitation
2. Provide workaround: Use `--optimize+` or avoid SRTP with mutable structs in Debug builds

## Recommendation
Continue investigation with Option A to understand the exact expression structure. If that doesn't reveal the issue within 1-2 more sprints, consider Option C.

## Time Spent So Far
- Initial investigation: ~1 hour
- First fix attempt: ~30 minutes
- Debugging fix attempt: ~1 hour
- Documentation: ~15 minutes

Total: ~3 hours

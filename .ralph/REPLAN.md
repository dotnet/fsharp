# Sprint 5 REPLAN: Issue #12136 - Fixed Unpin

## Investigation Summary

Issue #12136 requires emitting cleanup code (`ldc.i4.0; conv.u; stloc`) at the end of `use fixed` scopes to zero out pinned locals, allowing the GC to move previously pinned objects.

## Root Cause Analysis

The `use fixed` expression is elaborated by the type checker as:
```fsharp
use pin = fixed &array.[0]
// becomes:
let pin =
    let pinnedByref = &array.[0]  // inner binding with IsFixed = true
    conv.i pinnedByref            // inner body
// outer body continues here
```

The `IsFixed` flag is set on the **inner** `pinnedByref` variable, not on the outer `pin` variable.

## Fix Attempt

The attempted fix:
1. Extended `EndLocalScope` sequel to carry an optional pinned local index
2. Modified `GenSequelEndScopes` to emit unpin instructions
3. Checked `v.IsFixed` when processing let bindings

**Problem**: The check `v.IsFixed` in `Expr.Let` only detects the inner binding (`pinnedByref`), not the outer binding (`pin`). This causes:
1. Unpin code to be emitted at the end of the **inner** scope (too early)
2. Dead code generation in conditional branches
3. Breaking 23 existing `EmittedIL.FixedBindings` tests

## Why This Is Complex

The proper fix requires one of these approaches:

### Approach 1: Track Pinned Locals Across Scopes
- When processing RHS of a binding, track any pinned locals allocated
- Associate those pinned locals with the outer scope
- Emit unpin at the outer scope end
- **Complexity**: Requires modifying `GenBindingRhs` to return allocated pinned locals

### Approach 2: Modify Type Checker Elaboration
- Change how `use fixed` is elaborated so the outer binding has `IsFixed = true`
- Would require changes in `CheckExpressions.fs`
- **Complexity**: Could affect semantics and other parts of the compiler

### Approach 3: Post-Processing IL
- After generating IL, scan for pinned locals that aren't zeroed at scope end
- Insert cleanup instructions
- **Complexity**: Significant changes to IL emission pipeline

## Recommendation

This fix requires 2-3 sprints to implement properly:

**Sprint A**: Modify `GenBindingRhs` and `GenBindingAfterDebugPoint` to return a list of pinned local indices allocated during RHS generation. Thread this information to the outer scope.

**Sprint B**: Emit unpin code for all pinned locals when their associated outer scope ends. Handle multiple exit points (returns, branches).

**Sprint C**: Update all ~23 FixedBindings baseline tests, verify no regressions.

## Current State

- The `EndLocalScope` sequel type has been extended but the detection logic is incomplete
- Existing tests are broken
- The test for #12136 passes because it doesn't verify IL patterns

## Temporary Workaround

Users can work around this issue by placing the fixed block in a separate function:
```fsharp
let testFixed (array: int[]) : unit =
    let doBlock() =
        use pin = fixed &array.[0]
        used pin
    doBlock()  // Function returns, so pinned local is implicitly cleaned
    used 1     // Array is now unpinned
```

## Request

Mark issue #12136 as KNOWN_LIMITATION and schedule follow-up sprints to implement the proper fix.

# Fix Nullness Flow for Match Expressions (Issue #18488)

## High-level Goal
Fix the nullness flow analysis so that variables in non-null branches of match expressions are correctly refined to non-null types, while preserving type aliases.

## The Problem
When using `--checknulls` with F# 9+, variables matched after a `| null ->` case should be inferred as non-null. Currently:

```fsharp
let getEnvironmentVariable : string -> _ = failwith ""

match "ENVVAR" |> getEnvironmentVariable with
| null -> failwith ""
| x -> x // x is incorrectly inferred as `obj | null` rather than `obj`
```

The user @Smaug123 reports that `x` should be locally provable as non-null after the `| null` case is handled.

## Root Cause Analysis

The current `removeNull` function in `TcMatchClause` does:
```fsharp
let removeNull t =
    let stripped = stripTyEqns cenv.g t
    replaceNullnessOfTy KnownWithoutNull stripped
```

This correctly strips nullness but has two issues:
1. It uses `stripTyEqns` which removes type abbreviations (like `objnull = obj | null`), causing loss of type alias information
2. The current PR's fix only handles the type abbreviation case but not the general case

## PR #18852 Analysis (Copilot's Attempt)
The PR attempted to fix this by checking `IsTypeAbbrev`:
```fsharp
match t with
| TType_app (tcref, tinst, _) when tcref.Deref.IsTypeAbbrev ->
    TType_app (tcref, tinst, KnownWithoutNull)
| _ ->
    let stripped = stripTyEqns cenv.g t
    replaceNullnessOfTy KnownWithoutNull stripped
```

**Problems with PR #18852:**
- It only fixes the case for type abbreviations, not the general case
- Comments from T-Gro indicate the change initially broke the compiler (made null elimination worse)
- The fix is incomplete - needs tests and release notes

## Solution Approach

The fix needs to:
1. **Preserve type alias structure** when removing nullness - don't strip type equations for type abbreviations
2. **Apply nullness refinement** to the underlying type structure correctly
3. **Handle all cases** - type apps, type vars, and other type forms
4. **Add comprehensive tests** covering:
   - Basic `match x with | null -> ... | x -> ...` pattern  
   - Type aliases like `type objnull = obj | null`
   - Real-world scenarios like `System.Environment.GetEnvironmentVariable`
   - Ensure no regressions in TypeProviders tests

## Key Design Decisions

1. **Don't break existing null elimination**: The original code works for direct types; we should only add handling for abbreviations
2. **Use `replaceNullnessOfTy` on original type**: Instead of stripping first, apply nullness replacement to the original type structure
3. **Preserve backwards compatibility**: All existing nullness tests must pass

## Constraints/Gotchas

- The F# compiler compiles itself (bootstrap) - any broken code affects subsequent builds
- Type providers have complex nullness handling that we must not regress
- Need to add release notes to `docs/release-notes/.FSharp.Compiler.Service/`
- Tests should go in `tests/FSharp.Compiler.ComponentTests/Language/Nullness/`

## Test Strategy

1. Write a failing test for the exact issue from #18488
2. Verify it fails on main branch
3. Implement fix
4. Ensure test passes
5. Run full nullness test suite
6. Run TypeProvider tests to check for regressions

## Lessons from Previous Attempts

- Previous iterations tried more complex approaches that broke other cases
- The final PR approach (checking `IsTypeAbbrev`) is targeted but needs verification
- Need to understand what cases `stripTyEqns` is actually needed for

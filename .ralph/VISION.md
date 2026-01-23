# Vision: CodeGen Regression Bugfix Campaign

## High-Level Goal

Fix all 62 documented codegen bugs in the F# compiler, one at a time, enabling all commented-out tests in `CodeGenRegressions.fs` to pass with their `[<Fact>]` attributes uncommented.

## Current State

- **62 tests** exist in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs`
- **1 test fixed** (Issue #18319 - Literal upcast missing box instruction) - Sprint 1 complete
- **61 tests** still have `// [<Fact>]` (commented out) to avoid CI failure
- Documentation for each issue is in `CODEGEN_REGRESSIONS.md`
- 5 tests are Feature Requests (OUT_OF_SCOPE) - these need tests rewritten to document limitations rather than expect fixes

### Fixed Issues
| Sprint | Issue | Description | Status |
|--------|-------|-------------|--------|
| 1 | #18319 | Literal upcast missing box instruction | ✅ Fixed |

## Approach

### Prioritization Strategy

1. **Low Risk + Simple Fix Location** → Fix first
2. **Medium Risk** → Fix with extra care
3. **Feature Requests** → Convert tests to document limitation
4. **Cross-module changes** → Defer or document as unfixable

### Categories of Tests

| Category | Count | Action |
|----------|-------|--------|
| Invalid IL | 6 | Fix codegen to emit valid IL |
| Compile Error | 12 | Fix to allow compilation |
| Runtime Crash | 4 | Fix to prevent crash |
| Wrong Behavior | 13 | Fix to produce correct behavior |
| Performance | 12 | Fix to produce optimal IL (may skip some) |
| Feature Request | 5 | Rewrite test to pass as "documented" |
| Interop | 3 | Fix for C# compatibility |
| Other | 7 | Case-by-case |

### Fix Validation Process

For each fix:
1. Understand the issue from CODEGEN_REGRESSIONS.md
2. Locate the fix location in compiler source
3. Make minimal surgical change
4. Uncomment the `[<Fact>]` attribute
5. Run the specific test to verify it passes
6. Run full test suite to ensure no regression
7. Update CODEGEN_REGRESSIONS.md with fix notes

## Key Constraints

1. **Surgical Changes Only**: Each fix must be minimal and non-invasive
2. **No Breaking Changes**: Existing working code must continue to work
3. **One Issue Per Sprint**: Keeps work manageable and reduces risk
4. **Full Test Suite Must Pass**: After each sprint

## Known Unfixable Cases

Some issues may be determined to be unfixable. Document in CODEGEN_REGRESSIONS.md with:
- Multiple approaches tried
- Why each approach failed
- Evidence of conflicts with existing features

## Sprint Cadence

Each sprint:
1. Pick next issue by priority
2. Implement fix
3. Uncomment test
4. Verify test passes
5. Run full suite
6. Update documentation
7. Mark sprint complete

## Success Criteria

- All 62 tests have uncommented `[<Fact>]` attributes
- Full test suite passes
- CODEGEN_REGRESSIONS.md updated with fix notes for each issue
- No regressions in existing functionality

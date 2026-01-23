# Vision: CodeGen Regression Bugfix Campaign - Phase 2

## High-Level Goal

Fix all 62 documented codegen bugs in the F# compiler, enabling all tests in `CodeGenRegressions.fs` to pass with their `[<Fact>]` attributes uncommented.

## Current State (Phase 2 Start)

- **62 tests** exist in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs`
- **1 test fixed** (Issue #18319 - Literal upcast missing box instruction) - Uncommented and passing
- **61 tests** have `// [<Fact>]` (commented out) to avoid CI failure
- **5 tests** are Feature Requests (OUT_OF_SCOPE) - need rewritten tests documenting limitations
- All issues documented in `CODEGEN_REGRESSIONS.md`

### Fixed Issues (1 of 62)
| Issue | Description | Status |
|-------|-------------|--------|
| #18319 | Literal upcast missing box instruction | ✅ Fixed |

### OUT_OF_SCOPE Issues (5 - Feature Requests)
| Issue | Description | Action Needed |
|-------|-------------|---------------|
| #15467 | Include language version in metadata | Rewrite test to pass |
| #15092 | DebuggerProxies in release builds | Rewrite test to pass |
| #14392 | OpenApi Swashbuckle support | Rewrite test to pass |
| #13223 | FSharp.Build reference assemblies | Rewrite test to pass |
| #9176 | Decorate inline function with attribute | Rewrite test to pass |

## Approach for Bug Fixes

### Prioritization Strategy

1. **Low Risk + Simple Fix Location** → Fix first
2. **Invalid IL issues** → High priority (cause runtime failures)
3. **Compile Error issues** → Medium priority
4. **Performance issues** → Lower priority
5. **Cross-module changes** → Most complex, fix carefully

### Category Breakdown

| Category | Count | Priority |
|----------|-------|----------|
| Invalid IL | 6 | HIGH - Runtime failures |
| Runtime Crash/Error | 4 | HIGH - Program crashes |
| Compile Error/Warning | 12 | MEDIUM - Blocks compilation |
| Wrong Behavior | 13 | MEDIUM - Incorrect results |
| Performance | 12 | LOW - Optimization only |
| Feature Request | 5 | N/A - OUT_OF_SCOPE |
| Interop (C#/Other) | 3 | MEDIUM |
| Other | 7 | Case-by-case |

### Sprint Strategy

Each sprint:
1. Pick ONE issue from highest priority unfixed category
2. Analyze the issue deeply
3. Implement minimal surgical fix
4. Uncomment `[<Fact>]` attribute
5. Run test to verify it passes
6. Run full test suite to check regressions
7. Update CODEGEN_REGRESSIONS.md with UPDATE note
8. Document approach for future reference

## Constraints

1. **Surgical Changes Only**: Each fix must be minimal and non-invasive
2. **No Breaking Changes**: Existing working code must continue to work
3. **One Issue Per Sprint**: Reduces risk and keeps work manageable
4. **Full Test Suite Must Pass**: After each sprint

## Proven-to-Work Fix: Issue #18319

The fix for #18319 shows the pattern to follow:
- Location: `src/Compiler/CodeGen/IlxGen.fs` in `GenConstant`
- Pattern: Track underlying IL type, emit `box` instruction when needed
- Test: Uncommented, passing in CI

## Next Priority Issues

Based on category and risk assessment, the next issues to tackle are:

### Invalid IL / Runtime Crash Issues (6+4 = 10)
- #18956 - Decimal constant InvalidProgramException (Low risk)
- #18140 - Callvirt on value type ILVerify error (Low risk)  
- #17692 - Mutual recursion duplicate param name (Low risk)
- #19068 - Struct object expression byref field (Low risk)
- #19075 - Constrained calls crash (Medium risk)
- #14492 - Release config TypeLoadException (Medium risk)

### Compile Error Issues (12)
- #18868 - CallerFilePath in delegates (Low risk)
- #18815 - Duplicate extension method names (Low risk)
- #16565 - DefaultAugmentation duplicate entry (Low risk)
- #18263 - DU .Is* properties duplicate (Medium risk)

## Lessons Learned

1. Box instruction fix was surgical - added ~30 lines to GenConstant
2. Fix pattern: Identify exact codegen site, add conditional logic
3. Tests verify the fix works without regression
4. Update doc with UPDATE note documenting the fix

## Unfixable Criteria

An issue is only unfixable if:
1. 5+ different approaches have been tried and documented
2. Each approach causes regressions in existing tests
3. The fix conflicts with fundamental F# semantics
4. Clear evidence provided in CODEGEN_REGRESSIONS.md

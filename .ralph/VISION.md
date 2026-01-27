# Vision: CodeGen Regression Bugfix Campaign - Phase 3

## High-Level Goal

Fix all remaining 47 pending codegen bugs (of 62 total) in the F# compiler, enabling all tests in `CodeGenRegressions.fs` to pass with their `[<Fact>]` attributes uncommented.

## Current State (Phase 3 Start - 2026-01-27)

- **62 tests** in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs`
- **15 tests PASSING** with `[<Fact>]` uncommented (24% complete):
  - 10 actual bug fixes
  - 5 Feature Request tests marked OUT_OF_SCOPE (documentation tests)
- **47 tests PENDING** with `// [<Fact>]` commented out (76% remaining)

### Fixed Issues (10 of 62 bugs)
| Issue | Description | Fix Type |
|-------|-------------|----------|
| #18956 | Decimal InvalidProgram in Debug | IlxGen.fs - exclude literals from shadow local |
| #18868 | CallerFilePath Delegates | Already fixed in compiler |
| #18815 | Duplicate Extension Names | IlxGen.fs - fully qualified type prefix |
| #18319 | Literal upcast missing box | IlxGen.fs - box instruction |
| #18140 | Callvirt on value type | IlxGen.fs - constrained.callvirt |
| #17692 | Mutual recursion duplicate param | EraseClosures.fs - unique param names |
| #16565 | DefaultAugmentation duplicate entry | IlxGen.fs - method table dedup |
| #12384 | Mutually recursive values init | Fixed initialization order |
| #5834 | Obsolete SpecialName | IlxGen.fs - SpecialName for events |
| #878 | Exception serialization | IlxGen.fs - serialize exception fields |

### OUT_OF_SCOPE Issues (5 - Feature Requests)
These are properly tested as "documents current behavior" - not bugs:
- #15467, #15092, #14392, #13223, #9176

### Pending Issues by Category (47 remaining)

| Category | Issues | Priority |
|----------|--------|----------|
| **Runtime Crash/Invalid IL** (5) | #19075, #19068, #14508, #14492, #13447 | CRITICAL |
| **Compile Error** (7) | #18263, #18135, #14321, #7861, #6379, #14707, #14706 | HIGH |
| **Wrong Behavior** (10) | #18953, #18672, #18374, #16546, #16292, #15627, #13468, #13100, #12136, #6750 | MEDIUM |
| **Performance** (14) | #18753, #16378, #16245, #16037, #15326, #13218, #12546, #12416, #12366, #12139, #12137, #11556, #9348 | LOW |
| **Interop/Cosmetic/Other** (11) | #19020, #18125, #17641, #16362, #15352, #14712, #13108, #12460, #11935, #11132, #11114, #5464 | CASE-BY-CASE |

## Sprint Strategy

1. **ONE issue per sprint** - keeps risk manageable
2. **Prioritize by severity**: Runtime crashes > Compile errors > Wrong behavior > Performance
3. **Surgical fixes only**: Minimal changes, no refactoring
4. **Full test suite verification** after each fix
5. **Document in CODEGEN_REGRESSIONS.md** with UPDATE note

## External Code Auditor Responsibilities

After each bugfix sprint, verify:
1. **Code duplication audit**: No copy-paste patterns, reuse existing helpers
2. **Reinventing the wheel audit**: Use existing compiler infrastructure
3. **Proper layer placement audit**: Fix goes in correct module (IlxGen, Optimizer, etc.)
4. **Fix-feels-like-a-hack audit**: Fix addresses root cause, not symptoms
5. **Unnecessary allocations audit**: No new allocations in hot paths

## Lessons Learned from Previous Fixes

| Fix | Pattern Used | Key Insight |
|-----|-------------|-------------|
| #18956 | Exclude from condition | Literal values shouldn't get shadow locals |
| #18815 | Qualify names | Extension methods need fully qualified type prefix |
| #18319 | Add missing IL instruction | Box instruction for value-to-ref conversion |
| #18140 | Use constrained prefix | Value type method calls need constrained.callvirt |
| #17692 | Unique naming | Closures need globally unique parameter names |

## Unfixable Criteria

An issue is only declared unfixable if:
1. **5+ different approaches** have been tried and documented
2. Each approach causes **regressions in existing tests**
3. The fix **conflicts with fundamental F# semantics** (e.g., language spec)
4. Clear evidence provided in CODEGEN_REGRESSIONS.md with full reasoning
5. Issue may be reclassified as "design limitation" with documentation

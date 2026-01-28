# Vision: CodeGen Regression Bugfix Campaign - Phase 5 (Replan)

## High-Level Goal

Fix remaining 36 pending codegen bugs (of 62 total) in the F# compiler, enabling all tests in `CodeGenRegressions.fs` to pass with their `[<Fact>]` attributes uncommented.

## Current State (Phase 5 Start - 2026-01-27)

- **62 tests** in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs`
- **26 tests PASSING** with `[<Fact>]` uncommented (42% complete):
  - 21 actual bug fixes
  - 5 Feature Request tests marked OUT_OF_SCOPE (documentation tests)
- **36 tests PENDING** with `// [<Fact>]` commented out (58% remaining)

### Fixed Issues (21 of 62 bugs)
| Issue | Description | Fix Type |
|-------|-------------|----------|
| #19075 | CLR crash constrained calls | IlxGen.fs - skip constrained for reference types |
| #19068 | Struct object expr byref field | IlxGen.fs - deref byref for closure |
| #18956 | Decimal InvalidProgram in Debug | IlxGen.fs - exclude literals from shadow local |
| #18953 | Action/Func captures extra | MethodCalls.fs - bind expression result once |
| #18868 | CallerFilePath Delegates | Already fixed in compiler |
| #18815 | Duplicate Extension Names | IlxGen.fs - fully qualified type prefix |
| #18672 | Resumable code top-level null | LowerStateMachines.fs - removed top-level restriction |
| #18374 | RuntimeWrappedException catch | IlxGen.fs - proper exception wrapping |
| #18319 | Literal upcast missing box | IlxGen.fs - box instruction |
| #18263 | DU Is* duplicate method | Already fixed in compiler |
| #18140 | Callvirt on value type | IlxGen.fs - constrained.callvirt |
| #18135 | Static abstract byref params | ilwrite.fs - compareILTypes for Modified |
| #17692 | Mutual recursion duplicate param | EraseClosures.fs - unique param names |
| #16565 | DefaultAugmentation duplicate entry | IlxGen.fs - method table dedup |
| #14508 | nativeptr in interfaces TypeLoad | IlxGen.fs - preserve nativeptr in GenActualSlotsig |
| #14492 | Release config TypeLoadException | EraseClosures.fs - strip constraints from Specialize |
| #14321 | DU and IWSAM names conflict | IlxGen.fs - tdefDiscards for nullary cases |
| #13447 | Tail instruction corruption | IlxGen.fs - fixed tail emission |
| #12384 | Mutually recursive values init | Fixed initialization order |
| #5834 | Obsolete SpecialName | IlxGen.fs - SpecialName for events |
| #878 | Exception serialization | IlxGen.fs - serialize exception fields |

### OUT_OF_SCOPE Issues (5 - Feature Requests)
These are properly tested as "documents current behavior" - not bugs:
- #15467, #15092, #14392, #13223, #9176

### KNOWN_LIMITATION Issues (3)
- #16546 - Debug recursive reference null (requires type checker changes in EliminateInitializationGraphs)
- #16292 - Debug SRTP mutable struct incorrect codegen (requires deeper investigation of defensive copy suppression after inlining)
- #15627 - Async before EntryPoint hangs (CLR type initializer lock deadlock; requires rearchitecting module initialization)

### Pending Issues by Category (35 remaining)

| Category | Count | Issues |
|----------|-------|--------|
| **Wrong Behavior** | 4 | #13468, #13100, #12136, #6750 |
| **Performance** | 15 | #18753, #16378, #16245, #16037, #15326, #13218, #12546, #12416, #12366, #12139, #12137, #11556, #9348 |
| **Compile Error/Warning** | 5 | #7861, #6379, #14707, #14706, #13108 |
| **Runtime Error** | 2 | #11132, #11114 |
| **Interop/Metadata** | 6 | #18125, #17641, #16362, #15352, #12460, #11935, #5464 |
| **Signature Gen/Cosmetic** | 3 | #14712, #19020 |
| **KNOWN_LIMITATION** | 3 | #16546, #16292, #15627 |

## Sprint Strategy for Phase 4

1. **ONE issue per sprint** - keeps risk manageable
2. **Prioritize by severity**: Runtime crashes > Wrong behavior > Compile errors > Performance
3. **Surgical fixes only**: Minimal changes, no refactoring
4. **Full test suite verification** after each fix
5. **Document in CODEGEN_REGRESSIONS.md** with UPDATE note
6. **Mark KNOWN_LIMITATION** for issues requiring major architectural changes (like #16546)

## Key Insight from #16546 Investigation

Issue #16546 taught us that some bugs are caused by earlier compiler phases (type checker), not IlxGen. When IlxGen sees the code, the damage is already done. These require:
- Analysis of the entire compilation pipeline
- Changes to CheckExpressions.fs (EliminateInitializationGraphs)
- Extensive testing of mutual recursion scenarios

Such fixes are beyond "surgical bugfix" scope and should be marked KNOWN_LIMITATION with documented workarounds.

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
| #19068 | Deref byref for closure | Byref fields not allowed in classes; copy value instead |
| #18956 | Exclude from condition | Literal values shouldn't get shadow locals |
| #18815 | Qualify names | Extension methods need fully qualified type prefix |
| #18319 | Add missing IL instruction | Box instruction for value-to-ref conversion |
| #18140 | Use constrained prefix | Value type method calls need constrained.callvirt |
| #18135 | Recursive type comparison | ILType.Modified wrappers need unwrapping |
| #17692 | Unique naming | Closures need globally unique parameter names |
| #14321 | Discard duplicates | DU nullary case properties shadow IWSAM implementations |
| #16546 | KNOWN_LIMITATION | Some fixes require type checker changes, not IlxGen |

## Unfixable Criteria

An issue is only declared unfixable if:
1. **5+ different approaches** have been tried and documented
2. Each approach causes **regressions in existing tests**
3. The fix **conflicts with fundamental F# semantics** (e.g., language spec)
4. Clear evidence provided in CODEGEN_REGRESSIONS.md with full reasoning
5. Issue reclassified as KNOWN_LIMITATION with documented workaround

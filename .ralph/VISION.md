# Vision: CodeGen Regression Test Suite

## High-Level Goal

Create a comprehensive test suite documenting all 62 open `Area-Compiler-CodeGen` bugs in the F# compiler. Each issue will have:
1. A failing test in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/` 
2. An entry in `CODEGEN_REGRESSIONS.md` with analysis
3. The test will have its `[<Fact>]` attribute commented out to keep the suite green

## Current Status (as of Final Iteration)

**Completed:**
- All 62 issues have tests in `CodeGenRegressions.fs` (63 commented `[<Fact>]` tests)  
- All 62 issues documented in `CODEGEN_REGRESSIONS.md`
- Build succeeds with 0 errors
- ToC and summary statistics present

**Remaining Gaps (FINAL_REPORT findings):**

### CRITICAL: 8 Stub Tests Need Real Reproductions
These tests just `shouldSucceed` without demonstrating actual failure:

| Issue | Problem | Required Fix |
|-------|---------|--------------|
| #878 | Just defines exception, no serialization roundtrip | Add `BinaryFormatter` serialize/deserialize showing fields lost |
| #5464 | Just `let f x = x + 1`, no C# modreq/modopt | Demonstrate IL level that modifiers are stripped |
| #5834 | Missing `[<Obsolete>]` attribute, no specialname verification | Add `[<Obsolete>]` + event accessor + IL check for specialname |
| #9176 | Just `inline f`, no attribute tracking | Mark as Feature Request clearly - not a bug |
| #11556 | Simple type, no IL comparison | Add IL check showing locals vs no-locals pattern |
| #12137 | Explanation only, no actual cross-assembly test | Document as IL-level issue (hard to repro in single test) |
| #12139 | Explanation only, no IL comparison | Add IL check showing `String.Equals` call |
| #12366 | Explanation only, no IL type name check | Add IL check for closure type names |

### CRITICAL: OUT_OF_SCOPE Markers Not in Test File
CODEGEN_REGRESSIONS.md mentions Feature Requests but CodeGenRegressions.fs has no markers:
- #14392, #13223, #9176, #15467, #15092 should be clearly marked as OUT_OF_SCOPE

### CRITICAL: 10+ Tests Need Verification  
Need to verify representative tests actually fail when uncommented.

## Key Design Decisions

1. **Test Commenting Strategy**: Use `// [<Fact>]` (commented attribute) rather than `[<Fact(Skip="...")>]` because:
   - Some tests may cause crashes that Skip doesn't prevent
   - Clearer visual indication of "known failing"
   - Easy to uncomment when fixing

2. **Test Naming**: `Issue_NNNNN_ShortDescription` format for easy grep

3. **Standalone Repros**: Each test must be self-contained (no external files) using inline F# code strings

4. **Verification Method**: Tests verify the bug exists by:
   - Compile-time: `shouldFail`, `withErrorCode`, or crash detection
   - Runtime: `compileAndRun` with expected failure/wrong output
   - IL inspection: `verifyIL` for IL-level bugs

5. **Feature Requests**: Mark clearly with `[OUT_OF_SCOPE: Feature Request]` in both test comments and docs

## Final Sprint Focus

This is the CLOSING iteration. All gaps identified must be addressed:
1. Fix the 8 stub tests to be actual reproductions  
2. Add OUT_OF_SCOPE markers to test file comments
3. Verify 10 representative tests compile and can demonstrate failure
4. Update CODEGEN_REGRESSIONS.md with any corrections
5. Clean up any inconsistencies between test file and documentation

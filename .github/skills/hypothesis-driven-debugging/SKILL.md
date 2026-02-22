---
name: hypothesis-driven-debugging
description: Investigate compiler failures, test errors, or unexpected behavior through systematic minimal reproduction, 3-hypothesis testing, and verification. Always re-run builds and tests after changes.
---

# Hypothesis-Driven Debugging

A systematic, rigorous approach to debugging failures in the F# compiler codebase.

## When to Use This Skill

Use this skill when:
- Investigating test failures (unit tests, integration tests, end-to-end tests)
- Debugging build errors or compilation failures
- Analyzing unexpected runtime behavior
- Troubleshooting performance regressions
- Examining warning/error message issues

## Core Principles

1. **Always start with a minimal reproduction**
2. **Form multiple competing hypotheses**
3. **Design verification for each hypothesis**
4. **Document findings rigorously**
5. **Re-run builds and tests after every change**

## Process

### Step 1: Create Minimal Reproduction

Before forming hypotheses, create the smallest possible reproduction:

1. **Extract the failure**:
   ```bash
   # For test failures - run just the failing test
   dotnet test -- --filter-method "*YourTest*"
   
   # For build failures - try to isolate the problematic file
   # Create a minimal .fs file that reproduces the issue
   ```

2. **Reduce to essentials**:
   - Remove unrelated code
   - Simplify to the core issue
   - Verify the minimal case still fails

3. **Document the repro**:
   ```markdown
   ## Minimal Reproduction
   
   File: test-case.fs
   Command: dotnet test -- --filter-method "*TestName*"
   Expected: <expected behavior>
   Actual: <actual behavior>
   ```

### Step 2: Form 3 Hypotheses

Always form **at least 3 competing hypotheses** about the root cause:

```markdown
## Hypothesis 1: [Brief description]
**Theory**: The failure occurs because...
**How to verify**: Run/change X and observe Y
**Verification result**: [To be filled]
**Implications**: If true, this means...

## Hypothesis 2: [Brief description]
**Theory**: The failure occurs because...
**How to verify**: Add instrumentation/logging at point Z
**Verification result**: [To be filled]
**Implications**: If true, this means...

## Hypothesis 3: [Brief description]
**Theory**: The failure occurs because...
**How to verify**: Check assumption A by running test B
**Verification result**: [To be filled]
**Implications**: If true, this means...
```

### Step 3: Verification Methods

For each hypothesis, use one or more verification methods:

#### Code Instrumentation
```fsharp
// Add temporary debugging output
printfn "DEBUG: Value at checkpoint: %A" someValue
printfn "DEBUG: Entering function X with args: %A %A" arg1 arg2
```

#### Minimal Test Cases
```fsharp
// Create focused test to verify specific behavior
[<Test>]
let ``Hypothesis 1 verification test`` () =
    let result = functionUnderTest input
    result |> should equal expectedValue
```

#### Build with Different Flags
```bash
# Try different configurations
./build.sh -c Debug
./build.sh -c Release

# Compare outputs
diff debug-output.log release-output.log
```

#### Targeted Logging
```bash
# Enable verbose logging for specific component
export FSHARP_COMPILER_VERBOSE=1
dotnet build
```

### Step 4: Document Findings

Maintain a `HYPOTHESIS.md` file in the working directory:

```markdown
# Hypothesis Investigation

## Issue Summary
Brief description of the failure/bug being investigated.

## Minimal Reproduction
[Code/commands to reproduce]

## Hypotheses

### Hypothesis 1: Token position tracking issue
**Theory**: The warning check compares line numbers but lastNonCommentTokenLine is not being updated correctly.
**How to verify**: Add printfn debugging in LexFilter.fs to log every token and its line number.
**Verification result**: ✅ CONFIRMED - Logging showed LBRACE tokens were updating the tracking when they shouldn't.
**Implications**: Need to exclude LBRACE and potentially other structural tokens from tracking.

### Hypothesis 2: Lexer pattern matching order
**Theory**: The /// pattern might be matched after other patterns, losing context.
**How to verify**: Check lex.fsl pattern order and add logging in the /// rule.
**Verification result**: ❌ DENIED - Pattern order is correct; /// is matched specifically.
**Implications**: Issue is not in the lexer pattern matching.

### Hypothesis 3: Test expectations wrong
**Theory**: The test expectations might not match actual compiler behavior.
**How to verify**: Manually compile test code and check actual warning positions.
**Verification result**: ⚠️ PARTIAL - Some tests had wrong expectations, but underlying issue still exists.
**Implications**: Fixed test expectations, but still need to address token tracking.

## Resolution
[Final solution and verification]

## Lessons Learned
- What worked well
- What to do differently next time
- Patterns to remember
```

### Step 5: Critical - Always Re-run Tests

**ABSOLUTELY REQUIRED**: After implementing any fix:

1. **Build from scratch**:
   ```bash
   ./build.sh -c Release
   # Record: Time, exit code, number of errors
   ```

2. **Run affected tests**:
   ```bash
   # For targeted testing
   dotnet test -- --filter-class "*AffectedTestSuite*"
   
   # Record: Passed, Failed, Skipped, Time
   ```

3. **Verify the fix**:
   - Run the minimal reproduction - confirm it passes
   - Run related tests - confirm no regressions
   - Build the full project - confirm no new errors

4. **Document results**:
   ```markdown
   ## Verification Results
   
   Build:
   - Command: ./build.sh -c Release
   - Time: 4m 23s
   - Errors: 0
   
   Tests:
   - Command: dotnet test -- --filter-class "*XmlDocTests*"
   - Total: 61
   - Passed: 56
   - Failed: 0
   - Skipped: 5
   - Time: 2.1s
   
   Minimal Repro:
   - Status: ✅ PASSING
   ```

## Example Workflow

```bash
# 1. Observe failure
dotnet test -- --filter-class "*XmlDocTests*"
# Result: 15 tests failing

# 2. Create minimal repro
cat > test-case.fs <<EOF
type R = { /// field doc
    Field: int
}
EOF
dotnet fsc test-case.fs
# Observe: Warning FS3879 incorrectly triggered

# 3. Form hypotheses (in HYPOTHESIS.md)
# - H1: LBRACE token incorrectly tracked
# - H2: Lexer pattern issue
# - H3: Test expectations wrong

# 4. Verify H1
# Add: printfn "DEBUG: Token %A at line %d" token lineNum
./build.sh -c Release && dotnet test ...
# Result: Confirms LBRACE is being tracked

# 5. Implement fix
# Exclude LBRACE from tracking in LexFilter.fs

# 6. CRITICAL: Re-run everything
./build.sh -c Release
# 4m 44.9s, 0 errors

dotnet test -- --filter-class "*XmlDocTests*"
# 61 total, 56 passed, 0 failed, 5 skipped, 2s

# 7. Verify minimal repro
dotnet fsc test-case.fs
# No warning - ✅ FIXED

# 8. Update HYPOTHESIS.md with results
# 9. Commit with evidence
```

## Anti-Patterns to Avoid

❌ **Don't**:
- Skip the minimal reproduction
- Form only one hypothesis
- Make changes without verification
- Forget to re-run tests after fixes
- Claim "fixed" without build evidence

✅ **Do**:
- Start with smallest possible repro
- Consider multiple explanations
- Verify each hypothesis systematically
- Always re-run build and tests
- Document commands, timings, and results

## Integration with Development Workflow

After using this skill:

1. Clean up temporary debugging code
2. Remove or archive `HYPOTHESIS.md`
3. Update documentation with lessons learned
4. Add regression tests if appropriate
5. Consider whether findings reveal deeper issues

## References

- [Software Debugging Techniques](https://en.wikipedia.org/wiki/Debugging#Techniques)
- [Scientific Method Applied to Software](https://en.wikipedia.org/wiki/Scientific_method)
- F# Compiler build guide: `docs/DEVGUIDE.md`
- F# Compiler testing guide: `docs/testing.md`

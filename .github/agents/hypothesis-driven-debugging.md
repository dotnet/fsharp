# Hypothesis-Driven Debugging Skill

## When to Use
Use this approach when:
- Multiple tests are failing and the root cause is unclear
- A fix attempt didn't work as expected
- You need to systematically investigate a complex issue
- You're debugging compiler/lexer/parser behavior

## Process

### 1. Document Initial State
Create a `HYPOTHESIS.md` file in the repository root with:
```markdown
# Investigation: [Brief Description]

## Initial Observations
- Number of failing tests: X
- Pattern of failures: [describe]
- Error messages: [key snippets]

## Hypotheses
[List will be populated as you investigate]
```

### 2. Form Hypotheses
For each potential root cause, add to HYPOTHESIS.md:
```markdown
### Hypothesis N: [Short title]
**Theory**: [What you think is wrong]
**Evidence**: [What makes you think this]
**Test**: [How to verify this hypothesis]
**Expected if true**: [What should happen]
**Expected if false**: [What should happen]
```

### 3. Test Each Hypothesis
For each hypothesis:
1. Add temporary instrumentation/logging if needed
2. Make minimal changes to test the hypothesis
3. Run tests and record results
4. Update HYPOTHESIS.md with:
   ```markdown
   **Result**: ✓ Confirmed / ✗ Rejected
   **Findings**: [What you learned]
   ```

### 4. Iterate
- If hypothesis confirmed: Implement proper fix
- If hypothesis rejected: Move to next hypothesis
- If all rejected: Form new hypotheses based on findings

### 5. Final Verification
After implementing fix:
1. Run all affected tests
2. Record final test results in HYPOTHESIS.md
3. Document the root cause and solution
4. Keep HYPOTHESIS.md for future reference (don't commit to main branch)

## Example Template

```markdown
# Investigation: FS3879 Warning Implementation

## Initial Observations
- 15 XmlDocTests failing
- Pattern: Tests expecting no warnings are getting FS3879
- All failures involve /// comments

## Hypotheses

### Hypothesis 1: Warning check in wrong location
**Theory**: LexFilter can't distinguish /// from //
**Evidence**: LINE_COMMENT token doesn't contain comment text
**Test**: Move warning check to lex.fsl where /// pattern is matched
**Expected if true**: Tests should pass
**Expected if false**: Same failures
**Result**: ✓ Confirmed
**Findings**: lex.fsl has direct access to /// pattern, LexFilter only sees LINE_COMMENT token

### Hypothesis 2: [Next hypothesis if needed]
...

## Resolution
- Root cause: Warning was checked in LexFilter.fs instead of lex.fsl
- Solution: Moved check to lex.fsl line 744-747
- Test results: 55 passed, 0 failed (was 40 passed, 15 failed)
```

## Best Practices
1. **Be specific**: Each hypothesis should test one thing
2. **Be measurable**: Define clear success/failure criteria
3. **Document everything**: Record all findings, even negative results
4. **Iterate quickly**: Small tests, fast feedback
5. **Clean up**: Remove temporary instrumentation before final commit

## Integration with Workflow
1. When facing complex failures, create HYPOTHESIS.md
2. Work through hypotheses systematically
3. Run tests after each change
4. Keep HYPOTHESIS.md updated
5. Include final test metrics in commit message
6. Add HYPOTHESIS.md to .gitignore (ephemeral file)

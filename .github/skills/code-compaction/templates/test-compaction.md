# Test Compaction

Mode boundary: **same behavioral coverage, less scaffolding.** Distinct
scenarios stay the same or grow.

## Dispatch

Use SKILL.md "Reviewer Diversity".

```
You are a test-compaction reviewer for the test file(s) below.

INPUTS:
1. <full test file diff (or full test file if rewriting from scratch)>
2. <list of existing test helpers / fixtures already in the test project>
3. <repo's NoBloat / style instruction file if present — AUTHORITATIVE on
   test parametrization, shared constants, setup helpers>

GOAL: apply the style file's test rules to compact equivalent coverage.

CONSTRAINTS:
- coverage MUST NOT decrease: every distinct input/output pair before
  compaction must still be exercised after;
- failure messages must still point to the scenario (not just a
  parametrized row) — prefer named cases over anonymous tuples;
- do not add new test infrastructure modules without justification;
  prefer extending an existing helper module if one exists;
- delete `open` / `using` / imports unused by the compacted form.

DELIVERABLES:
1. Unified diff of the rewritten tests.
2. Before/after row count: distinct scenarios (must be ≥), distinct
   test methods (should be ≪), file LOC (should be ≪).
3. List of hoisted constants and helpers, with file:line.
4. List of any helper REUSED from the codebase, file:line.
5. Test run command + result (must be green).
```

## After compaction

- Run the full test class to catch collateral breakage from hoisted constants.
- Confirm distinct scenario count is unchanged or higher.
- If still bloated, hand off to Logic Compaction.

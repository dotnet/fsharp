---
---
# Sprint: Fixup 1

## Context

PR #19243 (branch: bugfix-queries) completed sprints 1 (Add Ident.MakeSynthetic) and 2 (Fix Query Shadowing FS1182). Post-implementation verification flagged issues in three categories: NO-LEFTOVERS, TEST-CODE-QUALITY, and TEST-COVERAGE. This sprint addresses those verifier findings with targeted fixes.

## Description

### Verifier Findings

1. **NO-LEFTOVERS**: Redundant inline comments restating test names/assertions in QueryTests.fs and ComputationExpressionTests.fs.
2. **CODE-QUALITY**: A 6-line `syntheticPat` block in CheckComputationExpressions.fs was dead code (Val ranges derive from Ident.idRange, not pattern range). An unrelated `.gitignore` `*.exe` addition was also flagged.
3. **TEST-CODE-QUALITY**: All 11 FS1182 query tests used `withOptions ["--warnon:FS1182"]` instead of the existing `withWarnOn 1182` helper. The 11 individual `[<Fact>]` tests should be parameterized into `[<Theory>]`/`[<MemberData>]` tests following the ObjInference.fs pattern.
4. **TEST-COVERAGE**: One test chain was missing `|> ignore` at the end, inconsistent with the rest.

### Files Modified

1. `src/Compiler/Checking/Expressions/CheckComputationExpressions.fs` -- Remove syntheticPat dead code
2. `.gitignore` -- Revert unrelated `*.exe` addition
3. `tests/FSharp.Compiler.ComponentTests/Language/ComputationExpressionTests.fs` -- Parameterize tests, use withWarnOn helper
4. `tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/QueryTests.fs` -- Remove redundant comments

### Implementation Steps

#### Step 1: Remove syntheticPat dead code

In CheckComputationExpressions.fs, the `syntheticPat` block marked the pattern range as synthetic, but Val ranges derive from `Ident.idRange` not the pattern range `m`, so this had no effect. Removed the 6-line block.

#### Step 2: Revert .gitignore change

Removed the unrelated `*.exe` line that was added to `.gitignore`.

#### Step 3: Parameterize FS1182 query tests

Replaced 11 individual `[<Fact>]` tests with 2 parameterized `[<Theory>]`/`[<MemberData>]` tests:
- `queryFS1182NoWarnCases`: 6 cases where no FS1182 warning should appear
- `queryFS1182WarnCases`: 5 cases with exact diagnostic line/col/message assertions

All tests now use `withWarnOn 1182` instead of `withOptions ["--warnon:FS1182"]`.

#### Step 4: Remove redundant comments

Removed ~29 redundant inline comments from QueryTests.fs that restated what test names and assertions already expressed. Preserved all issue references and WHY-explanations.

### What to Avoid
- Do NOT change any production logic
- Do NOT remove issue references or explanatory comments
- Do NOT alter test coverage (same cases, just restructured)

## Definition of Done
- All previously passing tests still pass
- Fixes address the specific issues flagged by verifiers
- No new regressions introduced
- NO-LEFTOVERS verifier passes
- TEST-CODE-QUALITY verifier passes
- TEST-COVERAGE verifier passes

# BACKLOG

## Original Request

Repair the existing PR #20562 for https://github.com/dotnet/fsharp/issues/20211 on branch `fix/issue-20211`.

The failing checks are `fsharp-ci (Build Linux)`, `fsharp-ci (Build MacOS Batch2)`,
`fsharp-ci (Build WindowsCompressedMetadata coreclr_release)`,
`fsharp-ci (Build WindowsCompressedMetadata transparent_compiler_release)`,
`fsharp-ci (Build WindowsCompressedMetadata_Desktop Batch2)`, and
`fsharp-ci (Build WindowsNoRealsig_testCoreclr)`.

First run `git fetch origin fix/issue-20211`, then `git diff origin/main...origin/fix/issue-20211`. Build on the existing changes.

Diagnose every failed check before editing. Distinguish build errors, test assertions, and `EmittedIL/*.bsl` mismatches. Regenerate and commit baselines only when evidence requires them.

Validate affected tests in the failing configuration. A Debug-only pass is insufficient. Use minimal, surgical changes. Do not push or create another PR. Only commit.

This recovery task is an architecture handoff. Replace the old sprint files and update this backlog. Implementation follows the replacement sprint.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20211.

Make the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality matters more than time. Take all the time needed. Smaller is better, but not at the cost of correctness. Preserve progress and evidence across execution windows rather than truncate the work.

**Verified root cause.** At main `b5c530ed6bc42937de6363e3dcc104ebb833893d`, recursive declaration checking evaluates `not null` constraints while union case tables are provisional and empty. `IsUnionTypeWithNullAsTrueValue` accepts every empty table before checking `UseNullAsTrueValue`. An ordinary unfinished union therefore receives the false FS3261 representation warning. This affects ordinary unions as well as structs. The payload, `Value` member, and imported Dictionary metadata are not the cause. Twenty-five isolated current-main compilations confirm the timing and controls. Six intended regression scenarios each fail with the false FS3261.

**Surgical candidate.** In `src/Compiler/TypedTree/TypedTreeOps.Attributes.fs:1319-1327`, require `TyconHasUseNullAsTrueValueAttribute` before the empty-table alternative. Keep the provisional alternative for genuinely attributed unions. Keep `CanHaveUseNullAsTrueValueAttribute`, the `unit` special case, and constraint behavior unchanged. Do not suppress warnings, special-case Dictionary or structs, inspect payload nullness, or defer all constraints. The candidate is source-grounded but has not been applied or proven GREEN.

The declaration-phase evidence is in `src/Compiler/Checking/CheckDeclarations.fs:2950-3036,3301-3408,4413-4530`. Attributes are published before the failing checked-abbreviation pass. The caller is `ConstraintSolver.fs:2940-2965`, through `TypedTreeOps.Transforms.fs:496-501`. Reconfirm this timing for any new counterexample before expanding the change.

**RED-first test plan.** Use `tests/FSharp.Compiler.ComponentTests/Language/Nullness/NullableRegressionTests.fs` and its existing helpers. Use one compact parameterized test for the valid regression sources. Each must execute the compiler and assert success without diagnostics. Promoting warnings is useful; ignoring them is not.

| Scenario | Required assertion |
|---|---|
| 1. Exact issue: `module rec M`, struct `Hole = Hole of string`, original `Value` member, then `Substitution = Dictionary&lt;Hole,obj&gt;` | No diagnostics. Current main reports false FS3261. |
| 2. Exact comment: non-recursive module with `and Substitution` in the same type group | No diagnostics. Current main reports false FS3261. |
| 3. Ordinary reference union in the recursive group, without the member | No diagnostics. Prevent a struct-only workaround. |
| 4. Generic struct union with a nullable string payload | No diagnostics. Nullable contents do not make the enclosing struct nullable or require a new payload constraint. |
| 5. A record field containing `Dictionary&lt;Hole,obj&gt;` before the struct union declaration | No diagnostics. Cover the early representation-check path beyond aliases. |
| 6. An explicit local F# `not null` constraint with `--checknulls-` | No false representation warning. Do not fix only imported Dictionary constraints. |
| 7. Non-recursive original source and a completed union from a preceding file or referenced assembly | Remain valid. Reuse existing completed-type controls rather than duplicate setup. |
| 8. Same-group `UseNullAsTrueValue` union, with its constrained alias before the union | Keep the genuine representation FS3261. This protects the provisional attributed-union case. |
| 9. An option used as a constrained key | Keep the genuine representation FS3261. |
| 10. A nullable string key | Keep the genuine supports-null FS3261. |

Rows 1-6 are real RED cases, already observed on current main. Rows 7-10 are controls and need not fail before the fix. Check full diagnostic lists for invalid cases, using the existing helper's severity convention consistently. Reuse an equivalent existing negative test when it exercises the same path.

Get GREEN without weakening those tests. Then run the neighboring nullness constraints and existing null-as-true-value runtime, signature, invalid-attribute, and metadata tests. The starting baseline passed five selected tests under `*UseNullAsTrueValue*` and `*Nullable attr for Option clones*`. Keep the shared predicate's completed-union behavior unchanged.

Format only changed F# files. Invoke `fsharp-diagnostics` after compiler edits and invoke the expert-review skill on the final work. Remove noisy comments, duplicate setup, and redundant helpers. Reuse existing compiler and test mechanisms. Leave a clean, compact test suite and the required concise release note. No public API, new diagnostic, new traversal, broad refactor, or unrelated baseline update is expected.

Sources: [issue and follow-up](https://github.com/dotnet/fsharp/issues/20211), [faulty predicate](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/TypedTree/TypedTreeOps.Attributes.fs#L1319-L1327), [declaration ordering](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/CheckDeclarations.fs#L4413-L4530), [constraint caller](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/ConstraintSolver.fs#L2940-L2965).

## Analysis

### Decision: preserve the implementation and repair the merged-state IL fixture

Recovery ran the required fetch and branch diff on September 16, 2026.
It also read the previous sprint, execution log, final verifier evidence, and all six current failed-task logs.

| Reference | Revision |
|---|---|
| Existing PR and branch | #20562, `fix/issue-20211` |
| Local HEAD and fetched PR head | `d7af5d9f109806cac12ae9d47da48ce416cd2cb7` |
| Original feature base | `b5c530ed6bc42937de6363e3dcc104ebb833893d` |
| Current CI main parent | `c9213bc6f2ebae94a59d563319483c6ce8c527c7` |
| Current CI merge | `53091b2e594a7d0be25ab037bedff06243318a01` |
| Current Azure build | `1599006` |

The fetched merge has exactly the listed main and PR parents.
The PR head lacks three main commits, including `ec437d5ac2f` (#20422).
That commit makes `List.forall2` inline and introduces opaque-callback adaptation.

The existing compiler fix and 42 issue rows remain implemented.
Commit `d7af5d9f109` also already applies the three `TheoryForNETCOREAPP` guards.
Do not repeat that repair or return to the original two-line compiler candidate.
The supplied orchestration diff is not the product diff. Leave the runner unchanged.

### What the previous run actually missed

The execution log ends with successful verification, not a documented crash.
Earlier iterations addressed real deferred-attribute and performance findings.
The last recovery then spent several iterations obtaining a compatible Windows MSBuild host.

The exact Desktop Batch2 command eventually passed 12,422 tests on the unmerged PR head.
Those results do not validate the later CI merge with the changed FSharp.Core.
Repeated source reviews and nullness selections cannot detect a changed dependency in an IL fixture.

The previous statement that all remote failures concern an old, unpushed repair is now false.
Current CI includes `d7af5d9f109`. It no longer reports the 13 Dictionary diagnostic failures.
Always record both PR and merge revisions before reusing validation evidence.

### Diagnosis of every current failure

Every listed job reports seven failures in `EmittedIL.InlineIfLambdaClosureForms+DoesNotAllocate`.
The exact assertion is `Found in actual IL: 'newobj'`.

| Job | Configuration | Log | Component passed / failed / skipped |
|---|---|---:|---|
| Linux | Release, net11.0 | 162 | 8827 / 7 / 259 |
| MacOS Batch2 | Release, net11.0 | 147 | 2953 / 7 / 10 |
| WindowsCompressedMetadata coreclr_release | Release, compressed, net11.0 | 773 | 8839 / 7 / 247 |
| WindowsCompressedMetadata transparent_compiler_release | Same, transparent compiler enabled | 772 | 8839 / 7 / 247 |
| WindowsCompressedMetadata_Desktop Batch2 | Release, compressed, net472 on net48 x64 | 267 | 2649 / 7 / 289 |
| WindowsNoRealsig_testCoreclr | Release, compressed, BuildNoRealsig, net11.0 | 798 | 8839 / 7 / 247 |

All six logs contain successful build summaries.
These are IL fragment assertions, not build errors or `.bsl` comparisons.
No baseline regeneration is indicated by the collected failures.

`InlineIfLambdaClosureForms.fs:14-42` compiles the same prelude into every test.
That prelude includes `forall2Forward`, which calls the newly inline `List.forall2`.
`ILChecker.fs:204-221` searches the entire assembly, not just `Test.test`.

All six IL dumps locate the offending constructor at `Test/forall2Forward@8::.ctor`.
All seven tested caller bodies contain no `newobj`.
Each log repeats its seven dumps, giving 14 caller-body copies and zero caller allocations.
The shared helper therefore causes false negatives and can also conceal false positives in the five allocation-required controls.

| Hypothesis | Evidence and result |
|---|---|
| Nullness repair broke closure elimination at the tested call sites | CI dumps show closure-free caller bodies. No caller regression demonstrated. |
| Shared fixture became allocation-bearing after #20422 | Supported by all six dumps, whole-assembly checker code, and the `List.forall2` diff. Confirm with local merged-state RED/GREEN. |
| Missing baseline updates or a build failure | Rejected for the six collected jobs. Builds succeed and assertions do not read baselines. |

Desktop Batch3 and WindowsNoRealsig Desktop were still running at collection.
Refresh the timeline before implementation and final delivery. Classify any additional failures separately.

### Evidence and existing tools

Current artifacts: `C:\Users\tomasgrosup\.copilot\session-state\61f33263-9eb9-40a0-b21d-926dd61c3f8f\files`.
This directory contains the required branch diff, PR snapshot, build metadata, timeline, six complete logs, and `ci-failure-analysis.json`.
`CI_ERRORS.md` records the current diagnosis and remaining reproduction work.
The old sprint and backlog are archived there.
The runner had already emptied the sprint directory. The requested scoped `rm` cleanup found zero remaining files.

Previous completed validation: `C:\Users\tomasgrosup\.copilot\session-state\0f9743a6-4c3e-4893-ac96-754dc0219e99\files`.
Read `CI_ERRORS.md` there only as historical evidence.
Reuse its existing `vs-host\MSBuild\Current\Bin\amd64\MSBuild.exe` and `run-exact.ps1` host-selection pattern.
The compatible Full MSBuild 18.10.1 toolset is present. Do not repeat its installation investigation.
The repository SDK is `11.0.100-rc.1.26420.103`.

## Approach

Use one recovery sprint with tests and repair together.
Validate the same merged source and FSharp.Core as CI, not cached unmerged binaries.
Preserve the existing PR branch and commits. Integrate the pinned main parent without rewriting history.

Repair only the allocation fixture unless a local counterexample requires more.
Prefer a small, explicitly non-inline test callee over dependence on `List.forall2` remaining non-inline.
Keep all seven no-allocation and five allocation-required scenarios meaningful.
Prove the prelude cannot satisfy allocation-required assertions by itself.
Do not merely delete the negative assertion, skip tests, or change the optimizer.

Run the affected class in Release across standard modern, compressed modern, transparent, no-realsig, and Desktop configurations.
Then run the related IL/optimizer tests, original nullness selections, and complete affected Desktop batch.
Report Windows execution as Windows execution, not native Linux or macOS validation.
Use the collected cross-platform IL evidence to explain equivalent local coverage.

Preserve all 42 modern issue rows, 24 Desktop-active rows, and five representation controls.
Keep the existing compiler behavior and release note. Use the required final expert review.
Commit only validated changes locally, without pushing or publishing.
Keep evidence in session storage. Do not add more orchestration files beyond this requested handoff.

Planning ends with replacement files and a local planning commit.
The implementation sprint owns local merged-state RED/GREEN and the product repair.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 01 | Repair Merged IL Fixture | Reproduce all seven assertions with CI's FSharp.Core, repair the shared fixture, validate the configuration matrix, preserve nullness coverage, review, and commit. |

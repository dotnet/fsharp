# BACKLOG

## Original Request

Repair the existing PR #20562 for https://github.com/dotnet/fsharp/issues/20211 on branch `fix/issue-20211`.

The failing check is `fsharp-ci (Build WindowsCompressedMetadata_Desktop Batch2)`.

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

### Decision: continue the existing implementation

Recovery inspected the fetched branch diff, issue, PR, previous sprint, execution history, and live CI logs on September 16, 2026.

The actual feature is implemented and committed. Do not restart from the original two-line candidate.

| Reference | Revision |
|---|---|
| Existing PR | #20562, `fix/issue-20211` |
| Local HEAD and fetched PR head at inspection | `1dfad28cc4cdc60c5f77fa75c79ac3a07b42c0be` |
| Main and PR base | `b5c530ed6bc42937de6363e3dcc104ebb833893d` |
| CI build | `1598754`, testing merge `e13dc43184169c4e8af3b64402d95882afb154d8` |

The product diff contains six compiler files, 42 regression rows, and a release note linked to #20211 and #20562.

The compiler already requires the representation attribute before accepting an empty union table. It publishes known attributes early and tracks unresolved attributes by union stamp. Deferred checks use trace undo/replay. Signature checking runs the pending checks. Finalized ordinary unions avoid case scans and callbacks.

The supplied `Prompting.fsx`, `Ralph.fsx`, and verifier diff concerns orchestration, not this PR's product code. Do not modify those files.

### Lessons from the previous run

The execution log ends with local verification success, not a compiler crash. The new failure is a CI configuration gap.

Earlier review found three real problems: early attribute publication, deferred attribute aliases, and excessive deferral for finalized unions. Commits through `610b0ea124c` address them. Preserve these changes and their tests.

Repeated modern-runtime runs did not validate Desktop framework metadata. Release configuration alone does not establish configuration parity.

The SDK now exists at `.dotnet\dotnet.exe`, version `11.0.100-rc.1.26420.103`. The old missing-SDK instructions are stale.

The Release `net11.0` test runner exists. The Release `net472` runner does not exist locally. Build it before claiming Desktop validation.

The previous diagnostics server failed on Windows socket initialization. If compiler changes become necessary, invoke `fsharp-diagnostics` and record any limitation honestly.

### CI diagnosis

The only completed failed job at inspection was `WindowsCompressedMetadata_Desktop Batch2`. Desktop Batch3 and WindowsNoRealsig Desktop were still running. Refresh all jobs before implementation and final delivery.

CI used:

```powershell
eng\CIBuildNoPublish.cmd -compressallmetadata -configuration Release -testDesktopBatch 2
```

Log 427 reports successful builds with zero warnings and errors. Component tests target `net472` and execute on `net48|x64`.

The component run reports 2,937 tests: 2,638 passed, 13 failed, and 286 skipped. All 13 failures say:

```text
System.Exception : Operation succeeded (expected to fail).
```

The assertion is `tests\FSharp.Test.Utilities\Compiler.fs:2044`, not an MSBuild compiler error.

| Theory in `Language.NullableRegressions` | Failing rows | Existing assertion line |
|---|---|---|
| `Issue 20211 - nullable constrained keys still warn` | 4 | 127 |
| `Issue 20211 - union constraints in early attribute arguments` | 3, all `expectWarning = true` | 153 |
| `Issue 20211 - union constraints with deferred representation attributes` | 6, all `UseNullAsTrueValue` | 265 |

All failures rely on imported `Dictionary<TKey,TValue>` nullness metadata. The Desktop framework does not expose its modern `notnull` key annotation.

`Utilities.fs:209-213` selects references for the test runner's framework. `typecheck` uses `TargetFramework.Current` through `CompilerAssert.TypeCheckWithOptionsAndName`.

There are no observed `EmittedIL/*.bsl` mismatches. The skill script labels the generic `(Test) Failure running tests` marker as a build error. The actual log disproves that classification.

### Competing hypotheses and evidence

| Hypothesis | Verification | Result |
|---|---|---|
| Compiler build or compressed metadata emission failed | Inspect complete failed-task log and build summaries | Rejected: builds succeeded and tests executed. |
| Desktop references lack the expected Dictionary constraint | Compare identical sources and compiler with Desktop and modern reference sets | Confirmed for constrained aliases, early attributes, and deferred attributes. |
| Release IL baselines changed | Enumerate all 13 failed rows and assertion stacks | Rejected for this failure: every failure is `shouldFail`, not a baseline comparison. |

Eight local FCS probes passed. Dictionary probes produced no diagnostics with `net472` references and FS3261 with `net11.0` references. An explicit F# `not null` constraint produced FS3261 with both reference sets.

The current Release modern-runtime runner also passed all 42 issue rows, with zero skips. These used existing artifacts, not a new build.

The FCS copies used by the probes and component runner have the same SHA256: `0D75E536B421139C0874AE974F5873CC2B9938D2A83B2668C6AE75C337E42303`.

These probes isolate reference metadata. They do not replace a real Desktop test run or prove a CI fix has been applied.

One exploratory local `NN<'T when 'T : not null>` replacement inside `module rec` lost the early-attribute warning. Do not substitute an unfinished local constraint blindly.

### Persistent evidence

Session artifacts are under `C:\Users\tomasgrosup\.copilot\session-state\8a029053-27d4-45ea-8a08-1a91c6b6fb41\files`:

- `ci-1598754-timeline.json` contains the cross-platform timeline.
- `ci-1598754-desktop-batch2.log` contains the complete failed task log.
- `CI_ERRORS.md` records classification, hypotheses, and evidence limits.
- `reference-metadata-probe.fsx` and `.log` preserve the eight reference-set checks.
- `existing-net11-regressions.log` records the 42 passing current-branch tests.

At recovery entry, all sprint files were already absent. The tracked original sprint appeared as deleted. The requested scoped `rm` cleanup found zero remaining files.

Leave the unrelated untracked `.copilot-prompt.txt` and runner files untouched.

## Approach

Use one independently testable recovery sprint. Change the three Dictionary-metadata theories to the existing `TheoryForNETCOREAPP` attribute.

Preserve their sources, row sets, warning promotion, and exact diagnostics. All 42 issue rows must still execute on the modern runtime.

Keep the other 24 rows active on Desktop. They include explicit F# constraints, record-order controls, signature controls, and finalized-union controls.

This is a runtime requirement, not permission to suppress warnings or skip the entire suite. No compiler, harness, project, or baseline edit is justified by the current evidence.

The implementer must reproduce Desktop RED, apply the narrow guards, and validate Release `net472` with `CompressAllMetadata=true`. Then run modern Release regressions and the compatibility controls.

Require the full affected Desktop batch to pass after the focused checks. Its remaining projects were not reached after the failing component run.

Reuse the existing release note. Obtain final expert review of the complete feature and recovery diff. Commit locally without pushing or modifying the existing PR remotely.

Keep exact commands, revisions, exit codes, counts, and review findings in persistent session artifacts. Do not rerun the entire historical implementation workflow.

Planning validation checks file structure, evidence links, paths, scenario coverage, and staged scope. Product changes and real Desktop GREEN remain the implementation sprint's responsibility.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 01 | Repair Desktop Nullness CI | Reproduce the 13 Desktop failures, apply narrow metadata-dependent test guards, verify both Release runtimes and the affected batch, review, and commit. |

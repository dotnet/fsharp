# BACKLOG

## Original Request

A pull request for https://github.com/dotnet/fsharp/issues/20410 already exists on branch fix/issue-20410, and its CI is red. Fix this existing PR. Do not start from scratch or open a new PR.

### FAILING CI CHECKS
fsharp-ci

### REQUIRED APPROACH
1. First, inspect the existing attempt. Run `git fetch origin fix/issue-20410`, then run `git diff origin/main...origin/fix/issue-20410`. Build on the existing changes instead of blindly rewriting them.
2. Before editing, diagnose the root cause of each failing check. Distinguish build errors, test failures, and `EmittedIL/*.bsl` baseline mismatches. If baselines changed, regenerate them, for example with `TEST_UPDATE_BSL=1`, and commit them.
3. The failure can be configuration-specific, such as Release-only. Before finishing, run the affected tests in the same configuration as the failing checks. A Debug-only pass is not sufficient.
4. Use minimal, surgical changes.
5. Validate locally before finishing.
6. Do not push. Only commit.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20410.

Make the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality matters more than time. Take all the time needed. Smaller is better, but not at the cost of correctness. Preserve progress and evidence across execution windows rather than truncate the work.

**Verified root cause.** At main `b5c530ed6bc42937de6363e3dcc104ebb833893d`, `checkRecordFields` validates fields by name in both directions, then calls diagnostic-emitting `checkField` again by position. The first reordered pair produces misleading FS0193 before the correct order diagnostic. The actual order diagnostic is **FS0312**, not the issue's FS0313. FS0313 means a required field is missing. Do not change diagnostic numbers. Eighteen current-main matrix compilations give seven expected RED assertions and eleven passing controls. A separate exact original `ResolvedConfig` compilation confirms only FS0193 at the first displaced field and FS0312 on the type.

**Surgical candidate.** Change only the final record positional predicate in `src/Compiler/Checking/SignatureConformance.fs:640-642`: compare `LogicalName` before calling the existing `checkField`. If names differ, return false without that call. Preserve the existing FS0312 at the implementation type and the false conformance result. Keep the two preceding name-map passes at `631-636` unchanged.

This guard avoids the misleading diagnostic and the wrong cross-field documentation/range mutation. Keep matching-name calls unchanged. Replacing the whole predicate with pure name equality would also remove existing repeated nullness warnings on correctly ordered records, which is outside this issue. The candidate is not yet applied or proven GREEN.

Do not sort fields, accept reordered records, suppress shared FS0193, alter diagnostic resources, or broaden the guard to unions, exceptions, or classes. Same-name type, mutability, accessibility, and attribute checks must still run. Constructor parameter order depends on record declaration order.

**RED-first test plan.** Use `tests/FSharp.Compiler.ComponentTests/Conformance/Signatures/Signatures.fs` and the paired-source compile pipeline. A plain `typecheck` call does not consume `AdditionalSources` at this revision. Use the existing `Fsi` plus implementation-source helper and `compile`, or a demonstrated project-typecheck path. Assert the complete diagnostic list, including code, severity, message, and range. GREEN means correct diagnostics while the invalid permutation still fails compilation.

| Scenario | Required assertion |
|---|---|
| 1. Exact reported `ResolvedConfig` field permutation, with compact local definitions for its dependent types | Exactly FS0312 on `ResolvedConfig`, no positional FS0193. Also use the reduced two-field swap to isolate the cause. |
| 2. A shared correctly ordered prefix followed by a three-field cycle | Exactly FS0312 on the type, no positional FS0193. |
| 3. Swapped fields with the same `int` type | Exactly FS0312. Equal field types do not make declaration order legal. |
| 4. Generic struct record with `'T` and `'T list` fields swapped | Exactly FS0312. Preserve generic remapping and the representation constraint. |
| 5. Permutation plus conflicting attribute arguments on the same named field | Keep FS1200 from attribute reconciliation and FS0312. Remove only positional FS0193. |
| 6. Identical names, declarations, and order | Successful compilation without diagnostics. |
| 7. Real same-name mismatch | Parameterize changed type, changed mutability, and less-accessible representation. Keep genuine FS0193 identifying the same field, without a spurious order error. |
| 8. Different name sets | Missing, extra, and renamed fields keep FS0313, FS0311, and FS0313 respectively. Equal counts alone do not establish a permutation. |
| 9. Warning-only nullness differences | Keep existing same-name warnings. The observed aligned and prefix-before-permutation cases each have three FS3261 warnings. The prefix case adds FS0312 but loses positional FS0193. Do not make warning cleanup part of this change. |
| 10. Non-record and recovery boundaries | Reuse union, exception, object-field, and malformed-field controls. Union and exception diagnostics stay unchanged. Reordered class fields can still compile. Do not refactor recovered duplicate names or list lengths. |

Rows 1-5 provide the primary bug and four meaningful RED variants. Current-main reduced forms already fail the intended expectations. Preserve the exact issue case as a compact fixture too. Rows 6-10 are controls, not additional before-fix failure claims. Use one data-driven regression test and separately named controls. Share source-pair construction; do not copy many near-identical files or test bodies.

First prove RED from the unwanted diagnostic. Then make the local guard and obtain GREEN without weakening tests, changing order-error expectations, suppressing warnings, or refreshing unrelated baselines. Existing field-extended-data, signature-nullness, and attribute-matching sibling selections passed seven rows on the starting main build. Run these and the nearby signature-conformance tests after implementation.

Format only changed F# files. Invoke `fsharp-diagnostics` after compiler edits and invoke the expert-review skill on the final work. Remove noise and duplicate setup using existing helpers. Leave a clean, compact suite and concise release note. No API change, new diagnostic, extra name set, general suppression flag, or broad conformance refactor is needed.

Sources: [issue](https://github.com/dotnet/fsharp/issues/20410), [record-specific checks](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/SignatureConformance.fs#L624-L655), [field checks and side effects](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/SignatureConformance.fs#L559-L623), [diagnostic identities](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/FSComp.txt#L149-L151).

## Analysis

This delivery replaces an obsolete implementation plan with one self-contained CI repair sprint.
It does not implement the CI repair or claim local compiler/test success.
The requested template was read before creating the replacement sprint.

### Existing attempt and branch state

- Ran `git fetch origin fix/issue-20410`, then `git diff origin/main...origin/fix/issue-20410`, before planning changes.
- Existing PR: [#20559](https://github.com/dotnet/fsharp/pull/20559), open, titled "Fix misleading diagnostics for reordered record fields".
- Existing remote head: `5c489dfdb967585f90847a5d6fe7f536d03163cf`. Implementation commit: `2bbf4d6d41f8d6216a59598f23c64a65133e7f17`.
- The local branch initially pointed to `b5c530ed6bc42937de6363e3dcc104ebb833893d`, also the local `origin/main`.
- With a clean tracked worktree, ran `git merge --ff-only origin/fix/issue-20410`. Planning now extends the existing PR history.
- Existing product diff: the record-only logical-name guard, 146 test lines, and one compiler-service release note.
- The source already retains matching-name `checkField` calls, both name-map passes, FS0312, and failed conformance.
- The test suite already has all requested scenarios, shared paired-source construction, and raw exact diagnostic assertions.
- The release note already links both #20410 and #20559. Do not add a duplicate or remove the PR link.
- The old sprint incorrectly says no PR exists and the guard has not been applied. Replace it, rather than leave it executable.

### Verified CI evidence

Build [1597638](https://dev.azure.com/dnceng-public/public/_build/results?buildId=1597638), number `20260915.39`, tested merge SHA `5604d594ed08aa786661166a3fffd1811db0e471`.
The current PR head is not that synthetic merge SHA.
The timeline has 48 jobs: 47 succeeded, one canceled. The aggregate `fsharp-ci` check failed because of that cancellation.

| Surface | Observed result | Classification |
|---|---|---|
| `WindowsNoRealsig_testDesktop`, job `916a2273-64f0-5130-a29e-a4d2f7e48c60` | Agent exceeded the configured 120-minute limit, 15:07:05Z to 17:07:26Z on September 15 | Job timeout, not an observed assertion failure |
| Build task, log 861 | Build summaries show zero errors; solution-wide net472 tests start at 15:27:44Z | No observed compilation failure |
| Component suite in log 861 | 8,284 passed, zero failed, 627 skipped; finished at 16:44:04Z after 76m12s | Successful suite inside canceled job |
| Core and service suites in log 861 | Core: 6,212 passed, 5 skipped; service: 3,487 passed, 306 skipped; both zero failures | Successful suites |
| Legacy `FSharpSuite.Tests` | No completion summary in the canceled job | Remaining workload or runner-lifetime investigation |
| Agent resource warnings | 95.69% memory used at 16:26:44Z and 16:26:49Z | Evidence supporting contention, not proof of a deadlock |
| `WindowsCompressedMetadata_Desktop Batch3`, task log 848 | Isolated legacy suite: 677 total, zero failed; job completed in about 91m18s | Existing isolation pattern succeeds |
| Desktop Batch1 / Batch2 | Jobs completed in about 50m11s / 38m56s | Existing three-batch pattern available |
| `WindowsNoRealsig_testCoreclr`, formatting, ILVerify | All succeeded | No justification to alter these surfaces |
| `EmittedIL` baselines | No observed mismatch in the retrieved failing-task log; component suite passed | Do not regenerate speculatively |
| Test results and binlog publication in canceled job | Those explicit tasks were skipped | Missing artifacts are not proof of passing tests |

The build-status script reports zero build errors and test failures because it filters `failed` tasks, not this `canceled` task.
The raw timeline and canceled-task log are the decisive evidence.
Do not present the script's empty result as a clean CI run.

Public evidence endpoints use `https://dev.azure.com/dnceng-public/public/_apis/build/builds/1597638`.
Append `/timeline?api-version=7.1`, `/logs/861?api-version=7.1`, or `/logs/848?api-version=7.1`.
The sprint embeds the essential evidence and does not depend on this backlog or access to another session.

### Competing hypotheses and prior progress

| Hypothesis | Evidence and next verification |
|---|---|
| Concurrent desktop suites exceed the job budget under memory pressure | Supported by resource warnings and isolated legacy success. Compare unsplit and isolated local runs with identical binaries and settings. |
| A legacy test or runner teardown hangs | No completion result alone cannot distinguish a hang from slowness. Record progress, exit status, child processes, and a dump if progress stops. |
| Release/compiler regression or IL baseline drift causes the failure | No CI assertion or build error supports this. Run the existing regressions and actual desktop batches before dismissing it. |

A bounded history lookup found session `83fb7dbc-211a-47df-8f32-557feaf219c2`.
It reports an unsplit local pass in 97m48s and an isolated legacy pass in 69m23s, with 677 passing tests.
It also reports that the VS engine could not load the SDK, while `-msbuildEngine dotnet` built successfully.
Its last available response says proposed Batch1 validation was blocked by a leftover MSBuild assembly lock.
Batch2, Batch3, and dedicated desktop/net11 signature reruns were still pending in that response.
These are historical reports, not fresh verified logs or evidence that the proposed repair is complete.
Recover matching logs if available. Otherwise rerun the missing evidence without resetting completed source work.

### Repository constraints that matter

- `azure-pipelines-PR.yml` contains the failing job near line 296 and the working desktop matrix near line 438.
- Reuse `eng\templates\batched-test-steps.yml`, `eng\Build.ps1`'s `-testDesktopBatch`, and `eng\tests\TestSplit.fsx`.
- `TestUsingMSBuild` already supplies net472, xUnit reports, binlogs, and five-minute hang dumps.
- Batch1 has residual component tests plus build tests. Batch2 has the remaining components, core, service, and scripting tests.
- Batch3 isolates `tests\fsharp\FSharpSuite.Tests.fsproj`. Preserve complete, nonoverlapping coverage of all six desktop test projects.
- `BUILDING_USING_DOTNET=true` removes net472 from component-project target frameworks. Override it only in the current process for desktop validation.
- The pinned SDK is `11.0.100-rc.1.26420.103`. The other component target is `net11.0`.
- Use supported SDK/build-engine setup. Do not change SDK versions or shared build scripts to hide a local tooling failure.

## Approach

Use one complete vertical repair sprint, including diagnosis, the smallest supported repair, its tests, review, and a local commit.
The leading candidate changes only the `WindowsNoRealsig_testDesktop` job to use the existing three-batch mechanism.
Keep the job's flags, environment, pool, and 120-minute per-job limit. Preserve distinct test and artifact names.
Make that change only after diagnosis supports workload isolation. Investigate a reproducible assertion failure or deadlock instead, if found.

Do not rerun the old implementation sprint or add duplicate regression tests.
Preserve original RED evidence when available. If it is missing, recover it with unchanged regression assertions and an isolated, temporary guard reversal.
Do not leave that reversal in the repair branch or mistake a setup failure for RED.
Require fresh Release/net472 regression and batch execution, followed by targeted Release/net11 checks.
Compare discovered test identities across batches, not counts alone. Discovery is not execution.
Regenerate only proven affected `EmittedIL` baselines and rerun without update mode before staging them.

Keep logs and resumable state under `.tools\ralph\evidence\issue-20410-ci`, outside implementation commits.
The implementation verifier must inspect exact commands, source SHA, flags, test counts, timings, exit codes, and unresolved limitations.
Local passes cannot make an unpushed GitHub check green. The final report must distinguish local validation from the unchanged remote check.

Planning verification checks the requested document structure, one active sprint, source paths, preserved request, and whitespace.
The planning commit changes only the backlog and the replacement sprint, retaining the existing compiler/test/release-note commits.
No build or test execution is claimed for this documentation-only planning pass.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 01 | Repair Desktop CI | Diagnose the existing PR timeout, apply the smallest proven repair, preserve all regression coverage, validate Release desktop batches, review, and commit without pushing. |

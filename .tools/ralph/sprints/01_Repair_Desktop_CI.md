---
---
# Sprint: Repair the existing record-field PR's desktop CI

## Context - WHY this sprint exists

Repair existing PR [#20559](https://github.com/dotnet/fsharp/pull/20559) for issue [#20410](https://github.com/dotnet/fsharp/issues/20410).
Work in `Q:\fsharp-worktrees\issue-878` on `fix/issue-20410`.
This is one complete implementation unit, including diagnosis, repair, tests, review, and a local commit.
You do not need another sprint, the backlog, or another agent's history.
Do not start over, open another PR, push, post reviews, amend commits, or change remote settings.

The PR already contains the compiler guard, regression tests, and release note.
Its remote head at planning was `5c489dfdb967585f90847a5d6fe7f536d03163cf`.
The compiler/test implementation is commit `2bbf4d6d41f8d6216a59598f23c64a65133e7f17`.
The architect fast-forwarded this local branch from main `b5c530ed6bc42937de6363e3dcc104ebb833893d` to the existing PR head.
Planning commits can follow that head. Preserve them and any completed repair work.

### Verified failure, not an assumed diagnostic regression

CI build [1597638](https://dev.azure.com/dnceng-public/public/_build/results?buildId=1597638) tested synthetic merge `5604d594ed08aa786661166a3fffd1811db0e471`.
Its 48 jobs comprise 47 successes and one cancellation.
The failed aggregate `fsharp-ci` check comes from `WindowsNoRealsig_testDesktop`.
Job ID: `916a2273-64f0-5130-a29e-a4d2f7e48c60`.

| Evidence | Observed result |
|---|---|
| Timeline | Agent exceeded 120 minutes, September 15, 2026, 15:07:05Z to 17:07:26Z. |
| Canceled Build task, log 861 | Build summaries have zero errors. Solution-wide Release/net472 tests begin at 15:27:44Z. |
| Component suite | Completed at 16:44:04Z: 8,284 passed, zero failed, 627 skipped, duration 76m12s. |
| Core and service suites | Core: 6,212 passed, 5 skipped. Service: 3,487 passed, 306 skipped. Both have zero failures. |
| Legacy `FSharpSuite.Tests` | No completion summary before cancellation. This does not distinguish slow tests from a runner hang. |
| Memory warnings | 95.69% memory used at 16:26:44Z and 16:26:49Z. |
| Existing desktop Batch3, log 848 | Isolated legacy suite completed 677 tests, zero failures. Whole job took about 91m18s. |
| Existing desktop Batch1 / Batch2 | Successful jobs took about 50m11s / 38m56s. |
| Other checks | No-realsig CoreCLR, formatting, and ILVerify succeeded. No failing-task `EmittedIL` baseline mismatch was observed. |

The build-status script filters failed tasks and can miss canceled tasks. Read the raw canceled-task log as well.
Explicit test-result and binlog publication tasks were skipped after cancellation. Do not interpret missing results as passes.

A prior execution reported local unsplit success in 97m48s and isolated legacy success in 69m23s.
That execution reported a VS/SDK incompatibility, then successful builds with the supported `-msbuildEngine dotnet` option.
Its final report left proposed Batch1 blocked by an MSBuild assembly lock, with other batch and signature reruns unfinished.
These reports are leads, not accepted validation. Reuse matching raw evidence if available, or reproduce it.
No local test or repair execution occurred during this planning pass.

## Description - WHAT to implement with DETAILED guidance

### 1. Inspect and preserve the existing attempt

Run these commands before editing implementation files:

```powershell
Set-Location Q:\fsharp-worktrees\issue-878
git fetch origin fix/issue-20410
git --no-pager diff origin/main...origin/fix/issue-20410
git status --short
git --no-pager log -6 --oneline
gh pr view 20559 --repo dotnet/fsharp --json headRefOid,statusCheckRollup
```

Confirm the existing PR history is an ancestor of local HEAD.
If behind and clean, fast-forward with `git merge --ff-only origin/fix/issue-20410`.
Never reset local repair commits or overwrite another agent's edits.
Keep evidence under `.tools\ralph\evidence\issue-20410-ci`, outside committed implementation files.
Record source SHA, commands, configuration, environment, exit codes, durations, test counts, and pending work.
Use distinct filenames for unsplit, isolated, Batch1, Batch2, Batch3, and signature runs.

| File, relative to the worktree | Required use |
|---|---|
| `azure-pipelines-PR.yml` | Primary candidate edit: `WindowsNoRealsig_testDesktop`, initially near line 296. |
| `eng\templates\batched-test-steps.yml` | Reuse the existing template. Read its parameters and publication conditions. |
| `eng\Build.ps1` | Read `TestUsingMSBuild`, `BuildSolution`, and the `testDesktopBatch` branch. Do not add another runner. |
| `eng\tests\TestSplit.fsx` | Reuse the existing three-batch assignments and `--validate`. Do not change the split without evidence. |
| `eng\CIBuildNoPublish.cmd` | Existing CI entry point, including restore, bootstrap, build, pack, sign, and binary logging. |
| `FSharp.slnx` | Source of the unsplit desktop test-project inventory. |
| `src\Compiler\Checking\SignatureConformance.fs` | Preserve the existing record-only `checkRecordFields` guard. |
| `tests\FSharp.Compiler.ComponentTests\Conformance\Signatures\Signatures.fs` | Preserve and run the existing issue tests in `Conformance.Signatures.SignatureConformance`. |
| `tests\FSharp.Test.Utilities\Compiler.fs` | Existing paired-source and raw diagnostic behavior. Do not change the shared harness. |
| `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md` | Preserve the existing concise entry linking #20410 and #20559. |

Read `.github\instructions\ExpertReview.instructions.md`, `ComponentTests.instructions.md`, and `NoBloat.instructions.md` before any corresponding F# edits.
Those three files are under `.github\instructions`.
Read `eng\common\AGENTS.md` if an edit there becomes necessary. The proposed repair does not edit that directory.

### 2. Diagnose every failing check before choosing a repair

Invoke `pr-build-status` and `hypothesis-driven-debugging`.
Refresh all platforms and jobs, not just the first red check.
For the known build, retrieve:

```powershell
pwsh .github\skills\pr-build-status\scripts\Get-BuildInfo.ps1 -BuildId 1597638
pwsh .github\skills\pr-build-status\scripts\Get-BuildErrors.ps1 -BuildId 1597638
$base = 'https://dev.azure.com/dnceng-public/public/_apis/build/builds/1597638'
Invoke-RestMethod "$base/timeline?api-version=7.1"
Invoke-RestMethod "$base/logs/861?api-version=7.1"
Invoke-RestMethod "$base/logs/848?api-version=7.1"
```

Save raw output and a `CI_ERRORS.md` under the evidence directory.
Classify each failure as build/restore, test assertion, baseline mismatch, timeout, or cancellation.
If a newer build exists, update the evidence instead of treating these IDs as permanently current.

Test these competing hypotheses before editing:

| Hypothesis | Verification |
|---|---|
| Unsplit concurrent desktop suites cause excessive runtime and memory pressure | Compare the unsplit command with isolated legacy execution on identical Release binaries and settings. Record process and memory observations. |
| A legacy case or runner teardown hangs | Check progress, case duration, process exit, and remaining child processes. If progress stops, inspect the existing hang dump or capture the specific owned process. |
| Release conformance or emitted-code behavior regressed | Run existing issue assertions and inspect actual failing test output. Separate expected negative-test diagnostics from runner failures. |

Invoke `binlog-analysis` for actual build, restore, or WarnAsError errors.
A timeout alone does not justify changing the compiler or diagnostic expectations.
If a test fails, reproduce that exact case in isolation before editing it.
If `EmittedIL\*.bsl` differs, inspect expected versus actual IL and identify the semantic cause.
Regenerate only affected baselines with process-local `TEST_UPDATE_BSL=1`, then remove that variable and rerun.
Commit generated baseline changes only when the semantic change is intended. Do not refresh unrelated baselines.

### 3. Reproduce with the correct Windows configuration

Use PowerShell and the repository's pinned SDK `11.0.100-rc.1.26420.103`.
Probe `dotnet --version` first. If missing, use `.\eng\common\dotnet.ps1 --version`.
Use that repository SDK thereafter. Restore tools/packages only after an actual missing-dependency failure.
Do not change `global.json`, package versions, or machine-wide environment variables.

The failed job uses Release, net472, compressed metadata, no-realsig product binaries, and immediate cache eviction.
For desktop commands, set `BUILDING_USING_DOTNET=false` in the current process.
The value `true` removes net472 from the component project's target frameworks.
Do not use `-noVisualStudio` to silently replace desktop coverage with CoreCLR coverage.

Reproduce the existing job before editing, or retain verifiable equivalent logs for this exact source and configuration:

```powershell
$env:BUILDING_USING_DOTNET = 'false'
$env:FSharp_CacheEvictionImmediate = 'true'
$env:NativeToolsOnMachine = 'true'
$env:DOTNET_DbgEnableMiniDump = '1'
$env:DOTNET_DbgMiniDumpType = '2'
$env:DOTNET_DbgMiniDumpName = 'Q:\fsharp-worktrees\issue-878\artifacts\log\Release\issue-20410-%e-%p-%t.dmp'
& .\eng\CIBuildNoPublish.cmd -compressallmetadata -buildnorealsig -testDesktop -configuration Release
```

`eng\Build.ps1` sets `FSHARP_REALSIG=false` for this invocation.
If the installed VS MSBuild cannot load the SDK, preserve that failure and invoke `binlog-analysis`.
Then use the supported `-msbuildEngine dotnet` option with the same command, if it builds all required desktop products.
Record this local engine deviation. Do not change CI's engine or claim an exact CI reproduction for a substituted command.
Do not restore incompatible SDK versions or hide build failures.

After the successful product build, run `tests\fsharp\FSharpSuite.Tests.fsproj` alone with the repository SDK.
Use `-c Release -f net472 --no-build --no-restore`, `FSHARP_REALSIG=false`, and the same cache-eviction setting.
Retain the runner's reports, actual exit code, and duration. The known inventory is 677 cases.
Do not run competing builds/tests in the same artifacts directory during timing comparisons.
A local unsplit pass does not disprove the CI timeout. Compare contention and duration rather than inventing a deterministic failure.

Preserve logs outside `artifacts` before cleaning task-owned build outputs.
If an owned process locks an assembly, identify its PID and origin before stopping that PID.
Never kill processes by name or delete another worktree's outputs.
Resume incomplete runs instead of repeating completed work merely because an execution window ended.

### 4. Apply the smallest evidence-supported repair

The leading candidate is a job-local three-batch conversion in `azure-pipelines-PR.yml`.
Follow the existing `WindowsCompressedMetadata_Desktop` job near line 438.
Reuse `eng\templates\batched-test-steps.yml` and the existing `-testDesktopBatch` option.
Do not add a new splitting algorithm, project, package, script, or test exclusion.

Keep the job name `WindowsNoRealsig_testDesktop`, its pool/demand, and `timeoutInMinutes: 120`.
Add the existing matrix pattern:

```yaml
strategy:
  matrix:
    Batch1:
      batchNumber: 1
    Batch2:
      batchNumber: 2
    Batch3:
      batchNumber: 3
```

Pass this build command to the shared template:

```text
eng\CIBuildNoPublish.cmd -compressallmetadata -buildnorealsig -testDesktopBatch $(batchNumber) -configuration Release
```

Preserve `FSharp_CacheEvictionImmediate`, all three `DOTNET_Dbg*` variables, and `NativeToolsOnMachine` through `buildEnv`.
Use `testRunTitlePrefix: 'WindowsNoRealsig_testDesktop'` and a distinct artifact prefix such as `'WindowsNoRealsig testDesktop'`.
Enable `publishBinLog` and `publishDumps`, retaining the Release build-binlog path from the original job.
Let the template append `Batch$(batchNumber)` to report/artifact names.
Retain the template's checked-in Azure include syntax. Do not hand-copy its publication tasks into the job.
Do not change sibling jobs, global parallelism, shared timeouts, or `continueOnError` to conceal failures.

This candidate is conditional on the diagnosis.
If an isolated legacy test or runner defect reproduces, fix that cause surgically with a regression test in this same sprint.
Do not use batching to hide a reproducible assertion failure, hang, or coverage loss.

### 5. Verify batch coverage and actual Release execution

Run the existing split validator and inspect all three generated command sets:

```powershell
dotnet fsi eng\tests\TestSplit.fsx --validate
dotnet fsi eng\tests\TestSplit.fsx 1 desktop
dotnet fsi eng\tests\TestSplit.fsx 2 desktop
dotnet fsi eng\tests\TestSplit.fsx 3 desktop
```

The validator checks project registration, not complete test execution.
Use MTP discovery to compare the full desktop test inventory with the union of batch selections.
Compare test identities, including theory rows, and require disjoint batch membership.
Counts alone are insufficient. Confirm every discovered test is assigned exactly once.
Preserve all six test projects from `FSharp.slnx`: component, build, core, service, private scripting, and legacy tests.
Batch1 is residual components plus build tests. Batch2 has remaining components, core, service, and private scripting tests.
Batch3 contains legacy tests alone. Issue tests belong to Batch1. EmittedIL belongs to Batch2.
Retain existing exclusions and skip reasons. Add none.

Run each proposed CI batch locally, sequentially, with the environment from section 3:

```powershell
& .\eng\CIBuildNoPublish.cmd -compressallmetadata -buildnorealsig -testDesktopBatch 1 -configuration Release
& .\eng\CIBuildNoPublish.cmd -compressallmetadata -buildnorealsig -testDesktopBatch 2 -configuration Release
& .\eng\CIBuildNoPublish.cmd -compressallmetadata -buildnorealsig -testDesktopBatch 3 -configuration Release
```

Apply the documented supported engine option if necessary.
Stop on any nonzero exit code. Archive each batch's results and binlogs before the next invocation can overwrite them.
Record build and test time separately, plus the whole invocation time.
Each whole invocation must finish below 120 minutes locally, with zero test failures and no missing suite results.
Record available CPU and memory when interpreting timing. Local timing cannot guarantee the unchanged CI host's runtime.
If a batch exceeds the budget, investigate before declaring completion. Do not increase the limit.
A successful build, discovery listing, or partial component pass does not satisfy batch execution.

### 6. Preserve and rerun the issue contract

Do not add duplicate tests. Existing `Signatures.fs` contains a six-row permutation theory and separately named controls.
The existing shared helpers are `recordSignaturePair`, `assertRecordDiagnostics`, `recordOrderDiagnostic`, and `fieldMismatch`.
They compile `Fsi` plus `FsSource` using `withAdditionalSourceFile`.
Plain `typecheck` ignores the additional implementation source at this revision.
Raw diagnostics preserve duplicate warnings, zero-based columns, message text, source basename, and order.
Do not replace these assertions with presence checks, sorted lists, or deduplicated diagnostics.

| Existing scenario | Required result |
|---|---|
| Exact `ResolvedConfig`: signature `Config; Settings; EditorConfigFiles; Problems`, implementation `Config; EditorConfigFiles; Problems; Settings` | Exactly FS0312 on the implementation type, no positional FS0193. Keep compact dependent-type definitions. |
| Reduced `A:int; B:string` swap; matching prefix plus three-field cycle; same-type `int` swap | Exactly FS0312. Every invalid permutation still fails compilation. |
| Generic struct with `'T` and `'T list` reversed | Exactly FS0312, retaining generic remapping and representation constraints. |
| Reordering plus different `Obsolete` arguments on same named field | Existing FS1200 warning and FS0312, without positional FS0193. |
| Identical declarations | Successful compilation, no diagnostics. |
| Same-name type, mutability, and less-accessible representation mismatches | Genuine FS0193 with unchanged field, message, and range. No spurious FS0312. |
| Missing, extra, renamed fields | FS0313, FS0311, FS0313 respectively. |
| Aligned nullness difference and prefix-before-permutation | Exactly three FS3261 warnings each. Only the permutation adds FS0312. |
| Union, exception, class fields, duplicate record name | Existing FS0193/FS0036, FS0193/FS0063, successful class compilation, and FS0037 respectively. |

Retain both record name-map passes and this final predicate in `checkRecordFields`:

```fsharp
implField.LogicalName = sigField.LogicalName
&& checkField aenv infoReader implTycon sigTycon implField sigField
```

Do not use pure name equality, sort fields, accept reordering, suppress shared FS0193, or change diagnostic numbers.
Do not extend the guard to unions, exceptions, or classes.
Keep matching-name checks and their documentation/range effects. Do not refactor recovery or list lengths.

Preserve original RED/GREEN evidence when available, including seven expected RED assertions caused by positional FS0193.
If unavailable, reconstruct RED using unchanged current tests and only a temporary guard reversal in an isolated task-owned worktree.
Do not remove the fix from the delivery branch or share build outputs between RED and GREEN.
A build/setup failure is not RED. Return to the guarded compiler and rerun unchanged expectations for GREEN.

After a verified product build, run the issue tests and nearby signature selection in Release/net472 and Release/net11.0.
Use the SDK's MTP syntax, for example:

```powershell
$env:BUILDING_USING_DOTNET = 'false'
$env:FSHARP_REALSIG = 'false'
$env:FSharp_CacheEvictionImmediate = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net472 --no-build -- --filter-method "*Issue 20410*"
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net472 --no-build -- --filter-class "Conformance.Signatures.*"
```

Repeat for `-f net11.0` only after verifying matching Release binaries exist.
If syntax differs, consult local `--help` and adjust syntax without changing the selection.
Run `FieldNotContainedDiagnosticExtendedData 01` from `ErrorMessages\ExtendedDiagnosticDataTests.fs`.
Run `Signature conformance` and `Micro compilation` from `Language\Nullness\NullableRegressionTests.fs`.
These paths are under `tests\FSharp.Compiler.ComponentTests`.
The nearby signature selection includes `AttributeMatching01 - attribute mismatch between signature and implementation`.
The supplied sibling baseline comprises seven rows. Verify their actual discovery, names, and results.
Require nonzero discovery for every selection. Never use stale binaries after compiler edits.

### 7. Review and commit locally

If any compiler `.fs` file changes, immediately invoke `fsharp-diagnostics`, then rebuild and rerun affected tests.
Format only changed F# files with `dotnet fantomas <changed-files>`.
A YAML-only repair needs no F# reformatting. Do not format the whole repository.
Validate YAML structure with available repository tooling and compare flags, environment, and expanded batch coverage against the original job.
Do not introduce a validation dependency solely for this small YAML edit.

Invoke `reviewing-compiler-prs` and the `expert-reviewer` agent for the final local work.
Request review of the preserved record-only behavior and the CI repair's coverage, configuration, runtime evidence, and artifact names.
Keep all review feedback local. Resolve concrete findings and rerun affected validation.
Apply `code-compaction` if new changes introduce duplicated setup or excessive scope.
Invoke `release-notes` to confirm the existing compiler-service entry remains sufficient. Do not duplicate it for CI-only changes.

Run `git diff --check` and inspect the repair diff against the existing PR head.
Stage only necessary repair files and any justified, regenerated baselines.
Do not commit raw logs, dumps, generated discovery data, or temporary probes.
Commit on `fix/issue-20410`, for example `Isolate no-realsig desktop CI test batches (#20559)`.
Include `Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>` and `Copilot-Session: <implementer-session-id>` trailers.
Preserve the existing implementation and planning commits. Do not amend, push, open a PR, post a comment, or request a remote rerun.
Report the local commit and validation accurately. Remote checks remain unchanged until an authorized later push.

## Definition of Done

- The existing PR branch and full diff were inspected before editing, and the repair extends its history.
- Every failed or canceled CI job has a recorded classification and raw evidence, including the known desktop timeout.
- Competing contention, runner-hang, and Release-regression hypotheses were tested before selecting the repair.
- Local reproduction uses Release/net472, compressed metadata, no-realsig binaries, and immediate cache eviction, with any engine deviation recorded.
- The repair changes only the proven cause and does not increase timeouts, ignore errors, drop tests, or change unrelated jobs.
- If batching is used, all three generated selections preserve each desktop test identity exactly once across all six projects.
- All three proposed desktop batch commands complete locally below 120 minutes each, with zero failures and complete suite results.
- The existing issue regressions and nearby signature tests pass with nonzero discovery in Release/net472 and Release/net11.0.
- Field-extended-data, signature-nullness, micro-compilation, and attribute-matching siblings pass with recorded selections and counts.
- Exact diagnostic lists, invalid-permutation failure, three repeated nullness warnings, and non-record controls remain unchanged.
- Original diagnostic RED evidence is retained or reconstructed without weakening tests, followed by GREEN on the preserved guard.
- No build error, test assertion, or EmittedIL baseline mismatch remains unresolved; any necessary baseline regeneration is committed and passes without update mode.
- Existing compiler and release-note changes remain intact unless a reproduced failure requires a surgical correction.
- Changed compiler files pass fsharp-diagnostics and rebuilt tests; only changed F# files are formatted, if any.
- YAML structure, preserved environment, distinct report/artifact names, and git diff --check pass.
- Local expert review is complete, concrete findings are resolved, and affected validation is rerun.
- Commands, source revisions, timing, exit codes, test results, and pending-state history persist outside implementation commits.
- The verified repair is committed locally on fix/issue-20410 with required trailers, with no push or other remote mutation.

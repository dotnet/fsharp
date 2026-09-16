---
---

# Sprint: Repair the merged-state IL fixture on the existing PR

## Context

Repair PR #20562 for issue #20211 on `fix/issue-20211`.
Work in `Q:\fsharp-worktrees\issue-876`. Do not push, open another PR, amend commits, or publish remotely.

The existing repair head is `d7af5d9f109806cac12ae9d47da48ce416cd2cb7`.
The original feature base is `b5c530ed6bc42937de6363e3dcc104ebb833893d`.
CI build `1599006` tests merge `53091b2e594a7d0be25ab037bedff06243318a01`.
Its other parent is main `c9213bc6f2ebae94a59d563319483c6ce8c527c7`.
Both the PR branch and CI merge have been fetched locally.

The prior recovery is complete: three Dictionary-dependent theories already use `TheoryForNETCOREAPP`.
It passed all 42 modern issue rows, 24 Desktop rows, and the exact compressed Desktop Batch2.
However, those binaries precede the three main commits included in the current CI merge.
Do not repeat the old guard repair or treat old-head GREEN as merged-state validation.

Preserve the existing nullness implementation.
It requires the representation attribute before accepting provisional union cases and publishes attributes before early constraints.
It tracks unresolved union attributes, preserves deferred-check undo/replay, and runs pending checks for signatures.
Finalized ordinary unions avoid case scans and callbacks.
Earlier reviews found the counterexamples behind these changes. Do not return to the original two-line candidate.

### Collected failures

All six current failed jobs have the same seven failures and exact assertion:

```text
EmittedIL.InlineIfLambdaClosureForms+DoesNotAllocate
Found in actual IL: 'newobj'
```

| Job | Release configuration | Task log |
|---|---|---:|
| Linux | net11.0 | 162 |
| MacOS Batch2 | net11.0 | 147 |
| WindowsCompressedMetadata coreclr_release | compressed, net11.0 | 773 |
| WindowsCompressedMetadata transparent_compiler_release | compressed, net11.0, TEST_TRANSPARENT_COMPILER=1 | 772 |
| WindowsCompressedMetadata_Desktop Batch2 | compressed, net472 on net48 x64 | 267 |
| WindowsNoRealsig_testCoreclr | compressed, BuildNoRealsig, net11.0 | 798 |

The seven methods cover direct-apply HOF lambda, partial application, forward pipe, direct call, back pipe, inline delegation, and instance-member back pipe.
Every build succeeds. No failure is an `EmittedIL\*.bsl` mismatch.
The old 13 Dictionary diagnostic assertions no longer fail.
Desktop Batch3 and WindowsNoRealsig Desktop were still running. Refresh every job before editing and delivery.

Complete logs are available at `https://dev.azure.com/dnceng-public/public/_apis/build/builds/1599006/logs/<LOG_ID>?api-version=7.1`.
Saved evidence is under `C:\Users\tomasgrosup\.copilot\session-state\61f33263-9eb9-40a0-b21d-926dd61c3f8f\files`.
Read `CI_ERRORS.md`, `ci-failure-analysis.json`, and the six `ci-*.log` files as needed.

### Root cause and evidence boundary

`tests\FSharp.Compiler.ComponentTests\EmittedIL\Inlining\InlineIfLambdaClosureForms.fs:14-42` adds one shared prelude to every source.
Its `forall2Forward` assumes that `List.forall2` is non-inline.
Main commit `ec437d5ac2f` (#20422) makes that library function inline with opaque-callback adaptation.
The fixture source itself did not change.

`tests\FSharp.Test.Utilities\ILChecker.fs:204-221` searches the entire assembly for `newobj`.
All six CI logs identify the same allocation: `Test/forall2Forward@8::.ctor`.
The seven `Test.test` bodies contain no `newobj`.
Each log repeats the seven dumps. The analysis found 14 caller-body copies and zero caller allocations per job.

The shared helper causes false failures outside the tested caller.
It can also make allocation-required tests pass for the wrong reason.
This diagnosis is based on source and CI IL. Local merged-state reproduction remains required.

The supplied `Prompting.fsx`, `Ralph.fsx`, and verifier diff concerns the runner, not the product.
Do not change those files.

## Description

### Establish the correct RED state

First run:

```powershell
git fetch origin fix/issue-20211
git diff origin/main...origin/fix/issue-20211
git status --short --branch
gh pr checks 20562 --repo dotnet/fsharp --json name,state,link
```

Invoke `pr-build-status` and collect any new failed-task logs.
Record build errors, test assertions, and baseline mismatches separately.
Invoke `hypothesis-driven-debugging`. Preserve these three hypotheses:
caller optimization regressed, the shared fixture allocates, or a baseline/build failed.

Keep the same PR branch. Integrate pinned main `c9213bc6f2ebae94a59d563319483c6ce8c527c7` without rewriting existing commits.
A normal local merge is sufficient. Keep the upstream merge separate from the surgical repair.
Do not cherry-pick isolated FSharp.Core files or test against mismatched compiler/Core artifacts.
Record the tested tree, compiler path, FSharp.Core path, configuration, and hashes.

Use the pinned `.dotnet\dotnet.exe`. Rebuild the compiler, FSharp.Core, and component runner through the repository build.
Do not reuse stale `--no-build` artifacts to establish RED.
For Desktop, unset `BUILDING_USING_DOTNET` so the project includes `net472`.
If bootstrap output is stale, archive evidence before cleaning only this worktree's generated `artifacts`.

Run the affected class first in modern Release:

```powershell
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net11.0 --filter-class "*InlineIfLambdaClosureForms*"
```

Require actual test execution and the seven CI assertions.
Inspect the emitted `forall2Forward` and `test` bodies. Confirm the allocation location before editing.
Use one failing method in isolation if the class output obscures the cause.
A build failure or zero discovered tests is not RED. Use `binlog-analysis` for real build failures.

### Make the smallest reliable fixture repair

Read `.github\instructions\ComponentTests.instructions.md` and `.github\instructions\NoBloat.instructions.md`.
The expected edit is confined to `EmittedIL\Inlining\InlineIfLambdaClosureForms.fs`.

Make the forwarding probe independent of library inline policy.
Prefer a small, top-level, non-inline recursive predicate callee inside the test source.
Have `forall2Forward` call that callee instead of relying on `List.forall2` remaining non-inline.
Verify its compiled definition does not introduce a closure into every assembly.
Keep the empty-list, unequal-length, and short-circuit behavior needed by the probe.
Use the existing `compile`, `withOptimize`, `verifyILPresent`, and `verifyILNotPresent` mechanisms.

Treat this as a candidate until execution proves it.
If fixture isolation gives a smaller correct solution, use it instead.
Do not add an IL parser, generalized harness API, referenced-project scaffolding, or compiler transformation.
Do not replace the seven negative assertions with success checks or platform guards.
Do not move tests between outcome groups merely to obtain GREEN.

Preserve all 12 existing scenarios: seven closure-free and five allocation-required.
Prove the shared fixture alone cannot satisfy a positive `newobj` assertion.
Inspect each positive scenario's allocation location so fixture allocations cannot hide a false pass.
Retain a compact fixture-only control if needed to enforce that invariant.
Correct the directly affected comment about the non-inline callee. Avoid unrelated comment or formatting cleanup.

### Reuse the working build host

The pinned SDK reports `11.0.100-rc.1.26420.103`.
The previous recovery already supplied compatible Windows Full MSBuild 18.10.1 x64.
Its executable exists at:

```text
C:\Users\tomasgrosup\.copilot\session-state\0f9743a6-4c3e-4893-ac96-754dc0219e99\files\vs-host\MSBuild\Current\Bin\amd64\MSBuild.exe
```

Read that session's `run-exact.ps1` for PATH and VSINSTALLDIR selection.
Reuse the host, but save new command logs and results in the current session.
Do not rerun the old tool-deployment investigation or overwrite its historical evidence.
Do not change `global.json`, bypass SDK version checks, or modify shared Visual Studio installations.

### Validate every affected configuration

Rebuild after the repair. Run the full affected class with nonzero counts in each configuration:

| Local run | Required settings |
|---|---|
| Standard modern | Release, net11.0, current merged compiler and Core |
| Compressed modern | Release, net11.0, CompressAllMetadata=true |
| Transparent modern | Same compressed build, TEST_TRANSPARENT_COMPILER=1 |
| No-realsig modern | Release, net11.0, compressed, BuildNoRealsig=true |
| Desktop | Release, net472, compressed, compatible Windows Full MSBuild |

Set and clear environment variables per process.
`CompilerAssert.fs:342-345` selects the transparent checker through `TEST_TRANSPARENT_COMPILER`.
Rebuild before changing compiler build modes. Do not share mutable build outputs between concurrent configurations.

The Windows CI commands are:

```powershell
eng\CIBuildNoPublish.cmd -compressallmetadata -configuration Release -testCoreclr
eng\CIBuildNoPublish.cmd -compressallmetadata -buildnorealsig -testCoreclr -configuration Release
eng\CIBuildNoPublish.cmd -compressallmetadata -configuration Release -testDesktopBatch 2
```

The transparent job uses the first command with its environment variable set.
The affected class must pass under each mode. A Debug-only or old-Core pass is insufficient.
Run the complete Release component suite and exact compressed Desktop Batch2 after focused GREEN.
The Desktop component failure prevents later projects in that batch from running.
Report native Linux/macOS execution only if it actually occurs.
Otherwise document equivalent Windows configuration runs and the identical cross-platform CI IL evidence.

Run neighboring `*InlineIfLambda*` and merged `*OptimizeClosureIfNotInlined*` classes in Release.
Also rerun `*Issue 20211*` and classes `Language.NullableRegressions`, `Language.NullableReferenceTypes`, and `Language.NullableCSharpImport`.
Run nullness selections on both modern and Desktop runners.
Require all 42 modern issue rows without skips and all 24 Desktop-applicable rows.
Keep the three existing metadata gates and all exact FS3261 diagnostic assertions.
Preserve local F# constraints with `--checknulls-`, record ordering, signatures, nullable payloads, and completed-type controls.
Run `*UseNullAsTrueValue*` and `*Nullable attr for Option clones*` in modern Release. Preserve all five controls.

No current failure requires a baseline update.
If a genuine baseline mismatch appears, inspect it before changing expectations.
Regenerate only justified affected baselines with `TEST_UPDATE_BSL=1`.
Unset update mode, rerun normally, and commit any required baselines.

### Review and deliver locally

Format only changed F# files.
Invoke `code-compaction` if the repair grows beyond a small fixture change.
If compiler edits become necessary, read applicable instructions and invoke `fsharp-diagnostics` immediately afterward.
Record tooling limitations rather than claiming a failed diagnostics server succeeded.

Invoke `reviewing-compiler-prs` and its expert-review workflow on the final work.
Provide the feature diff relative to the main merge parent and the separate CI-repair diff.
Require review of assertion strength, fixture isolation, and preserved nullness behavior.
Resolve concrete findings and rerun affected tests.
Invoke `release-notes`. Reuse the existing #20211/#20562 entry in `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md`.
Do not add a duplicate user-facing entry for a fixture-only repair.

Record commands, revisions, configurations, counts, exit codes, and review results in session artifacts.
Run `git diff --check` and inspect the staged scope.
Commit validated changes with the required co-author and current session trailers.
Refresh CI before delivery. Do not describe an unpushed local fix as remote GREEN.
Leave unrelated user and runner changes untouched. Do not push.

## Definition of Done

- The existing PR and CI merge were inspected, and every completed failed job has an accurate classification.
- The existing nullness implementation and three Desktop metadata guards remain intact.
- Local merged-state Release execution reproduces the seven `newobj` assertions before the fixture repair.
- The repair removes fixture contamination without weakening any of the 12 existing allocation scenarios.
- The prelude cannot independently satisfy positive allocation assertions, and positive allocation locations are verified.
- The affected class passes with nonzero counts in standard, compressed, transparent, no-realsig, and Desktop Release configurations.
- The Release component suite and exact compressed Desktop Batch2 pass after the final edit.
- Related IL/optimizer tests, all 42 modern issue rows, all 24 Desktop issue rows, neighboring nullness selections, and five representation controls pass.
- Any necessary baseline update is justified, regenerated through the harness, and verified without update mode.
- Only changed F# files are formatted, final expert review has no unresolved concrete findings, and the release note remains correct.
- Persistent evidence distinguishes old-head tests, current merged-state tests, native platforms, and equivalent local configurations.
- The repair is committed on the existing branch without pushing, rewriting history, publishing, or staging unrelated changes.

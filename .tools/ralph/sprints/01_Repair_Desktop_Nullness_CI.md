---
---

# Sprint: Repair Desktop nullness CI on the existing PR

## Context

Repair PR #20562 for issue #20211 on `fix/issue-20211`. Work in `Q:\fsharp-worktrees\issue-876`. Do not push, create another PR, or post remotely.

The implementation already exists. The inspected PR head is `1dfad28cc4cdc60c5f77fa75c79ac3a07b42c0be`. Its base is `b5c530ed6bc42937de6363e3dcc104ebb833893d`.

First inspect the current branch:

```powershell
git fetch origin fix/issue-20211
git diff origin/main...origin/fix/issue-20211
git status --short --branch
```

Preserve all existing compiler fixes. They require the representation attribute before accepting provisional union cases, publish attributes early, and track unresolved attributes. Deferred checks preserve trace undo/replay and signature behavior. Finalized ordinary unions avoid extra case scans and callbacks.

Earlier review found the early-attribute, deferred-alias, and performance defects. Existing commits address them. Do not return to the original two-line candidate.

The previous run ended with successful local verification. It missed Desktop framework metadata, despite repeated Release runs. The supplied orchestration diff is not the feature diff.

### Exact CI failure

Azure build `1598754` tests merge `e13dc43184169c4e8af3b64402d95882afb154d8`. The failing check is `fsharp-ci (Build WindowsCompressedMetadata_Desktop Batch2)`.

CI command:

```powershell
eng\CIBuildNoPublish.cmd -compressallmetadata -configuration Release -testDesktopBatch 2
```

The affected component runner targets `net472` and executes on `net48|x64`. Log 427 reports successful builds with zero warnings and errors.

The component run contains 2,937 tests: 2,638 passed, 13 failed, and 286 skipped. Every failure says:

```text
System.Exception : Operation succeeded (expected to fail).
```

The assertion is `tests\FSharp.Test.Utilities\Compiler.fs:2044`. The failures are:

| Theory in `Language.NullableRegressions` | Failed source rows |
|---|---|
| `Issue 20211 - nullable constrained keys still warn` | All four: direct/deferred attributed union, option, nullable string |
| `Issue 20211 - union constraints in early attribute arguments` | Three `UseNullAsTrueValue` rows: module, recursive module, recursive namespace |
| `Issue 20211 - union constraints with deferred representation attributes` | Six `UseNullAsTrueValue` rows: three scopes, each in `.fs` and `.fsi` |

These are test assertions, not compiler build errors or `EmittedIL/*.bsl` mismatches. The build-status script misclassifies the generic `(Test) Failure running tests` marker.

Desktop `Dictionary<TKey,TValue>` has no modern `notnull` key annotation. These tests therefore expect diagnostics from a constraint that is absent.

The test harness chooses framework references in `tests\FSharp.Test.Utilities\Utilities.fs:209-213`. `typecheck` uses current-framework references through `CompilerAssert.TypeCheckWithOptionsAndName`.

Eight local reference-set probes confirm the distinction. Dictionary aliases and early/deferred attribute arguments warn with modern references, but not Desktop references. Explicit F# `not null` constraints warn with both.

The existing Release `net11.0` runner passed all 42 issue rows. No product source was edited during planning. These existing-artifact runs do not replace a fresh Desktop build.

At inspection, Desktop Batch3 and WindowsNoRealsig Desktop were still running. Refresh every CI job before editing and before delivery. Diagnose any additional failure separately.

Evidence is under `C:\Users\tomasgrosup\.copilot\session-state\8a029053-27d4-45ea-8a08-1a91c6b6fb41\files`: `CI_ERRORS.md`, `ci-1598754-timeline.json`, `ci-1598754-desktop-batch2.log`, `reference-metadata-probe.fsx`, `reference-metadata-probe.log`, and `existing-net11-regressions.log`.

The full task log is also available at:
`https://dev.azure.com/dnceng-public/public/_apis/build/builds/1598754/logs/427?api-version=7.1`

## Description

### Minimal repair

Read `.github\instructions\ComponentTests.instructions.md` and `.github\instructions\NoBloat.instructions.md` before editing.

Modify only `tests\FSharp.Compiler.ComponentTests\Language\Nullness\NullableRegressionTests.fs`, unless new reproduced evidence requires another file.

Replace `[<Theory>]` with `[<TheoryForNETCOREAPP>]` on the three theories listed above. The attribute already exists in `FSharp.Test`, which this file opens.

Preserve every source row and exact assertion. The three theories contain 18 source rows, including five positive controls. Their metadata prerequisite applies to the entire theory.

This narrow runtime gate follows the framework contract. Do not skip the entire module or change expected warnings into success.

Do not add a platform predicate, helper, framework override, or new harness. Do not change `withVersionAndCheckNulls`, warning promotion, or compiler constraints.

Do not blindly replace Dictionary with a same-group F# constrained type inside an early attribute. Planning tried this and lost the warning because the local constraint was unfinished.

No compiler change is indicated. Keep `CanHaveUseNullAsTrueValueAttribute`, the `unit` case, completed-union classification, and public APIs unchanged.

### Preserve the original requirements

All existing issue coverage must remain:

| Scenario | Required behavior |
|---|---|
| Exact recursive-module source | Preserve struct `Hole`, original `Value` member, and Dictionary alias. No diagnostics. |
| Exact issue-comment source | Preserve non-recursive module and `and Substitution`. No diagnostics. |
| Reference union without member | No false representation warning. |
| Generic struct with nullable payload | No false warning or added payload constraint. |
| Record before union | No false warning for its Dictionary field. |
| Explicit F# constraint with `--checknulls-` | Still executes on both runtimes and has no false warning. |
| Completed-type controls | Preserve non-recursive original and imported `Choice`. |
| Same-group attributed union before finalization | Preserve exact genuine representation FS3261 where the constraint exists. |
| Option constrained key | Preserve genuine representation FS3261 on modern Dictionary metadata. |
| Nullable-string key | Preserve the distinct supports-null FS3261 on modern Dictionary metadata. |

All 42 issue rows must execute and pass on `net11.0`, with no skips.

The other 24 issue rows must execute and pass on Desktop. Preserve the 12 local-F# record-constraint rows, four finalized-union rows, and eight ordinary-union rows.

The record controls retain eight genuine FS3261 expectations across declaration order and `.fs`/`.fsi`. They must not acquire platform guards.

Also retain `Notnull constraint and inline annotated value` in `NullableReferenceTypesTests.fs`. It checks option representation and nullable-string constraints without relying on Dictionary metadata.

### Reproduce before editing

Invoke `pr-build-status`. Collect all failures, not only the first failed job:

```powershell
gh pr checks 20562 --repo dotnet/fsharp --json name,state,link
& .github\skills\pr-build-status\scripts\Get-BuildInfo.ps1 -BuildId 1598754 -FailedOnly
& .github\skills\pr-build-status\scripts\Get-BuildErrors.ps1 -BuildId 1598754
```

Record three hypotheses: build failure, framework-reference mismatch, and IL baseline mismatch. Use the complete logs to classify them.

Use the pinned SDK. Do not install another SDK or alter `global.json`.

```powershell
$env:DOTNET_ROOT = (Resolve-Path .dotnet).Path
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
if (Test-Path Env:\BUILDING_USING_DOTNET) {
    Remove-Item Env:\BUILDING_USING_DOTNET
}
.\.dotnet\dotnet.exe --version
.\.dotnet\dotnet.exe msbuild tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -getProperty:TargetFrameworks
```

Require `net472` in the framework list. `BUILDING_USING_DOTNET=true` removes Desktop from this project.

A Release `net11.0` runner exists locally. A Release `net472` runner does not yet exist. Build Desktop before using `--no-build`.

Use the exact CI command above to establish the compressed-metadata Desktop build and RED result. It also builds the required Desktop compiler tools.

Then isolate each of the three failing theory methods on the built Desktop runner:

```powershell
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net472 --no-build --filter-method "*Issue 20211 - nullable constrained keys still warn*"
```

Repeat with each other failing method name. Record four, three, and six failures respectively. Capture their exact assertions and framework identity.

If a genuine build failure occurs, invoke `binlog-analysis` and resolve it before counting test outcomes. Do not count build failure or zero discovered tests as RED.

### Apply, rebuild, and validate

Apply the three narrow theory guards. Format only the changed F# file:

```powershell
.\.dotnet\dotnet.exe fantomas tests\FSharp.Compiler.ComponentTests\Language\Nullness\NullableRegressionTests.fs
```

Inspect formatting changes and exclude unrelated churn.

Rebuild and run the issue selection in both configurations:

```powershell
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net472 -p:CompressAllMetadata=true --filter-method "*Issue 20211*"
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net11.0 -p:CompressAllMetadata=true --filter-method "*Issue 20211*"
```

If dotnet-hosted build tasks require the Windows build route, use `eng\CIBuildNoPublish.cmd`. Do not change repository targets to bypass them.

Desktop must execute 24 applicable issue rows with zero failures. Only the three metadata-dependent theories are excluded there. The runner can report skips by theory rather than by source row.

Modern Release must execute all 42 rows with zero failures and zero skips. Verify the three guarded theories independently there as well.

Run neighboring nullness checks after rebuilding:

```powershell
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net472 --no-build --filter-class "Language.NullableRegressions" "Language.NullableReferenceTypes" "Language.NullableCSharpImport"
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net11.0 --no-build --filter-class "Language.NullableRegressions" "Language.NullableReferenceTypes" "Language.NullableCSharpImport"
```

Run the existing representation controls in modern Release:

```powershell
.\.dotnet\dotnet.exe test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -f net11.0 --no-build --filter-method "*UseNullAsTrueValue*" "*Nullable attr for Option clones*"
```

Require the same five controls: runtime DU properties, signature agreement, invalid-attribute diagnostics, C# nullable metadata, and the Option-clone IL baseline.

Finally rerun the complete affected batch:

```powershell
eng\CIBuildNoPublish.cmd -compressallmetadata -configuration Release -testDesktopBatch 2
```

Batch2 includes component tests, FSharp.Core tests, compiler service tests, and private scripting tests. The CI component failure stopped subsequent project execution.

Current evidence requires no `.bsl` change. If a real baseline mismatch appears, inspect expected and actual output before deciding its cause.

Regenerate only intentional affected baselines with `$env:TEST_UPDATE_BSL = '1'`. Remove the variable and rerun normally. Commit required baselines, not unrelated updates.

### Review and local delivery

Reuse the release note in `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md`. It already links #20211 and #20562. Invoke `release-notes` to confirm no duplicate entry is needed.

Invoke `reviewing-compiler-prs` for the final work and follow its expert-review workflow. Supply the complete feature diff and the separate recovery diff. Request local findings only.

Review must confirm the runtime prerequisite, all original assertions, active Desktop controls, and preserved declaration-phase fixes. Resolve concrete findings and rerun affected tests.

If compiler `.fs` edits become necessary, invoke `fsharp-diagnostics` immediately. Read the applicable compiler instructions first. Record any Windows tooling failure and real fallback evidence.

Do not claim the diagnostics server passed if it failed to start. Do not expand compiler behavior merely to make Desktop Dictionary emit a warning.

Keep commands, revisions, exit codes, test counts, and review output in persistent session artifacts. Do not add logs or SDK files to the PR.

Run `git diff --check`, inspect the staged diff, and commit the validated repair with the required session and co-author trailers.

Stage only task-owned changes. Leave `.copilot-prompt.txt`, runner files, and unrelated changes untouched. Do not amend existing commits.

Refresh CI status before delivery and record any remaining remote failures against the old head. Do not claim remote GREEN for an unpushed local fix.

## Definition of Done

- The existing PR branch was fetched and its diff inspected before edits, with no new branch or PR.
- All completed failed CI jobs have recorded classifications, exact errors, and configuration details.
- The original Desktop selection reproduces the 13 expected assertion failures before the guards.
- Only the three metadata-dependent theories receive the existing runtime attribute, with all sources and assertions preserved.
- Release `net11.0` executes all 42 issue rows with zero failures and zero skips.
- Compressed-metadata Release Desktop executes all 24 applicable issue rows with zero failures.
- Local F# constraint, record-order, signature, and finalized-union controls remain active on Desktop.
- Both Release runtimes pass the neighboring nullness selection with nonzero test counts.
- The five existing representation controls pass in modern Release without unjustified baseline changes.
- The complete compressed-metadata Desktop Batch2 command succeeds locally after the final edit.
- Any necessary baseline update was inspected, regenerated through the harness, and rerun without update mode.
- Existing compiler behavior, public APIs, diagnostics, and the correctly linked release note remain intact.
- Only changed F# files were formatted, final expert review completed, and `git diff --check` passes.
- Persistent evidence distinguishes CI results, local builds, existing-artifact probes, runtime exclusions, and validation limits.
- The validated repair is committed with required trailers, without pushing, remote publication, or unrelated staged changes.

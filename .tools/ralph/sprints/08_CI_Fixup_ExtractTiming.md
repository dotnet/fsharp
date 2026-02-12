---
---
# Sprint: CI Fixup - Fix ExtractTimingsFromBinlog dotnet fsi failure and false failure reports

## Context

Build 1292059 showed that ALL regression test Build tasks SUCCEEDED (the --nowarn:75 fix from previous sprints works). However, the "Extract compiler timing from binlogs" step fails with exit code 1 on every job that produces binlogs. The error is:

```
The application 'fsi' does not exist or is not a managed .dll or .exe.
The .NET SDK could not be found, please run ./eng/common/dotnet.sh.
```

Root cause: The Extract timing step runs `dotnet fsi` from `$(Build.SourcesDirectory)` (the F# repo root), which has a `global.json` requiring SDK 10.0.101. The `UseDotNet@2` tasks only install 10.0.100, so `dotnet fsi` fails with SDK version mismatch.

Secondary issue: The Report step checks `$env:AGENT_JOBSTATUS -eq "Succeeded"`, but `continueOnError: true` on the Extract step makes job status `SucceededWithIssues`, which causes the Report step to falsely report "Regression test failed" even though the actual build succeeded.

## Description

### Files Modified
- `eng/templates/regression-test-jobs.yml` - Two targeted fixes

### Changes Made

1. **Fix dotnet fsi SDK resolution**: Add `DOTNET_ROLL_FORWARD: LatestMajor` env var to the Extract timing step. This tells dotnet to use whatever SDK version is available instead of requiring the exact version from global.json.

2. **Fix false failure reporting**: Change the Report step condition from `$env:AGENT_JOBSTATUS -eq "Succeeded"` to also accept `"SucceededWithIssues"`, since `continueOnError: true` steps (like timing extraction) can cause this status without indicating an actual build failure.

### What Was NOT Changed
- No compiler source code changes
- No changes to ExtractTimingsFromBinlog.fsx or PrepareRepoForRegressionTesting.fsx
- No test changes

## Definition of Done
- Extract compiler timing step no longer fails with SDK not found error
- Report step correctly shows SUCCESS when the Build task succeeded
- Regression test jobs show as succeeded (not succeededWithIssues from false failure reports)

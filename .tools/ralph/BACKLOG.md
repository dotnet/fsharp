# BACKLOG

## Original Request
Check AzDo failures on the PR, web copilot struggles. Fix it, reproduce locally, retest. Goal is to collect binlogs from regression test runs and then process those binlogs and extract their `<OtherFlags>--times<>` output from the F# compiler. Keep it short. I will push committed changes once you are done and see how CI reacts, no worries.

## Analysis

PR #19273 adds `--times` to `<OtherFlags>` in third-party repos via `PrepareRepoForRegressionTesting.fsx`. This flag is an **internal** compiler option that emits **FS0075** as a warning: *"The command-line option 'times' is for test purposes only"*.

Both IcedTasks and FsToolkit.ErrorHandling have `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in their `Directory.Build.props`, which promotes FS0075 to an error, causing build failures.

The binlog collection and extraction pipeline (`ExtractTimingsFromBinlog.fsx`, YAML steps) is already correct. The only problem is the FS0075 warning being promoted to error.

## Approach

In `PrepareRepoForRegressionTesting.fsx`, when injecting the `--times` OtherFlag, also inject `<NoWarn>$(NoWarn);0075</NoWarn>` into the same PropertyGroup. This suppresses the internal-option warning regardless of the third-party repo's TreatWarningsAsErrors setting.

## Sprint Overview
| # | Name | Purpose |
|---|------|---------|
| 01 | Fix FS0075 NoWarn | Add NoWarn for 0075 in PrepareRepoForRegressionTesting.fsx so --times doesn't fail with TreatWarningsAsErrors |
| 02 | CI Fixup NoWarn Robustness | Move --nowarn:75 into OtherFlags instead of separate NoWarn element, because NoWarn can be overridden by project files |
| 04 | Remove Self-Explanatory Comments | Clean up AI-generated verbose comments from scripts |
| 05 | Address NO-LEFTOVERS Findings | Remove last remaining self-explanatory comments and redundant doc lines |
| 06 | Fix Binlog Overwrite + XPath | Fix unique binlog filenames and robust XPath import detection |
| 08 | CI Fixup ExtractTiming | Fix dotnet fsi SDK mismatch (DOTNET_ROLL_FORWARD) and false failure reports (SucceededWithIssues) |

## CI Failure Analysis (2026-02-12)

### Build 1290542: FS0075 errors (FIXED by sprints 01-06)
Build 1290542 failed with `FSC : error FS0075` across all 4 regression test jobs. Fixed by `--nowarn:75` in OtherFlags.

### Build 1291753: Canceled (superseded)

### Build 1292059: ExtractTimingsFromBinlog.fsx fails (FIXED by sprint 08)
All regression test **Build tasks SUCCEEDED** — the `--nowarn:75` fix works. However, the "Extract compiler timing from binlogs" step fails with:
```
The application 'fsi' does not exist or is not a managed .dll or .exe.
The .NET SDK could not be found, please run ./eng/common/dotnet.sh.
```

**Root cause 1**: The Extract step runs `dotnet fsi` from `$(Build.SourcesDirectory)` (F# repo root) where `global.json` requires SDK 10.0.101, but `UseDotNet@2` only installs 10.0.100.
**Fix**: Add `DOTNET_ROLL_FORWARD: LatestMajor` env var to the Extract timing step.

**Root cause 2**: Report step checks `AGENT_JOBSTATUS -eq "Succeeded"` but `continueOnError: true` on the Extract step sets status to `SucceededWithIssues`, causing false "Regression test failed" reports.
**Fix**: Accept both `"Succeeded"` and `"SucceededWithIssues"` in the Report step condition.

**Status**: Sprint 08 applied. Awaiting CI confirmation.

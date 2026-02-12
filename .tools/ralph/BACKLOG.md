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

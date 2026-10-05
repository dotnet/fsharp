# Visual Studio backport branches

`scripts\Get-VSBackportBranchPoint.ps1` finds the F# source commit that produced
the payload currently inserted into a VS branch. It can create an F# backport
branch, configure its insertion target, and pin its VS minor version.

## Prerequisites

- Git, PowerShell, and an authenticated Azure CLI (`az login`).
- Permission to read DevDiv's VS repository and dnceng/internal F# build metadata.
  Signing in alone does not grant either permission.
- A local F# clone containing the source branch and commit being serviced.

## Preview and create a branch

Supply the minor version the target VS branch will release. It is not inferred
from the branch name or from the version of the currently inserted F# payload.

```powershell
.\eng\release\scripts\Get-VSBackportBranchPoint.ps1 `
    -VSBranch rel/insiders `
    -FSharpRepoPath Q:\source\fsharp `
    -NewBranchName release/dev18.11 `
    -VSMinorVersion 11
```

The default is a preview. Add `-Execute` to create and check out the F# branch,
update `FSharpReleaseBranchName` and `VSInsertionTargetBranchName` in
`azure-pipelines.yml`, and fix `VSMinorVersion` in `eng\Versions.props`.
The script does not commit, push, or insert anything into VS. Execution requires
a clean tracked F# worktree and must not overwrite untracked files.

Optionally supply `-VSRepoPath Q:\source\VS`. Without it, the script uses an
authenticated shallow VS clone in an owned `.vs-backport-<guid>` directory beneath
the invocation's working directory, and removes it when finished, including on
failure. Initial depth is 100 commits; it deepens by 100 commits up to ten times.
If the history limit is reached, supply a clone with sufficient history. Preview
can fetch refs and use temporary disk space but does not edit the F# worktree.

The caller is responsible for choosing the correct release minor. Two VS release
branches can legitimately target the same minor. A backport has
`UseVSScheduledMinorVersion=false`, so later builds retain the supplied minor even
when they receive a build timestamp.

## Main's automatic minor version

`eng\VSMinorVersion.props` selects `VSMinorVersion` during MSBuild evaluation when
`UseVSScheduledMinorVersion=true` and `VSBuildTimestampUtc` is supplied.
Official builds supply one timestamp across restore, build, and pack. Local
builds without an explicit timestamp retain the literal minor in
`eng\Versions.props`.

The nominal main-to-insiders snap is **Patch Tuesday (the second Tuesday of each
month) minus 11 calendar days**, at noon Pacific. The minor changes **24 elapsed
hours later**. All snap dates are calculated; there is no release schedule table.
Pacific daylight-saving time is taken into account when converting to UTC.

To reproduce a selection without compiling:

```powershell
dotnet msbuild eng\release\VSVersion.proj -t:PrintVSVersion `
    -p:VSBuildTimestampUtc=2026-09-11T12:00:00Z
```

This selects minor 12. Use the same property on a build command to override the
timestamp. Timestamps must include `Z` or a numeric UTC offset. Version selection
needs no network access.

Minor numbering uses a single calibration: October 2026's Patch Tuesday cycle
maps to minor 13. Each earlier/later month decrements/increments that number once.
This reference establishes version numbers, not snap dates; it requires no
monthly updates. The snap can fall in the preceding calendar month.
Automatic selections emit advisory warning `FSVS1001`, including when general
MSBuild warnings are treated as errors, because the heuristic cannot confirm the
actual state of the VS branches.

Input validation also runs during evaluation-only queries such as `-getProperty`:
missing offsets, unsupported majors, and out-of-range minors fail rather than
falling back to a literal version. These queries do not execute warning targets;
read `VSMinorVersionIsHeuristic` to distinguish an automatic estimate (`true`)
from the local fallback or a fixed backport minor (`false`).

Estimates do not account for holidays or rescheduled snaps and may run ahead of
or behind actual VS development. Pacific conversion
uses the US DST rule: second Sunday in March through first Sunday in November;
update it if that rule changes. The calibration covers VS major 18; changing the
major or minor numbering convention requires updating `_VSMinorBaseYear`,
`_VSMinorBaseMonth`, and `_VSMinorBase` in `eng\VSMinorVersion.props`. Computed
minors outside 0..65534 are rejected.

For reproducibility, retain both the source revision (including its calibration)
and the build timestamp. Do not add raw private schedule documents to the repo.

## Tests

Run the PowerShell tests using the existing Pester installation:

```powershell
Invoke-Pester eng\release\scripts
```

The tests use local Git fixtures and MSBuild evaluation; no VS insertion is
performed.

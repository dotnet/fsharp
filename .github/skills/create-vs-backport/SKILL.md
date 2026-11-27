---
name: create-vs-backport
description: Create an F# backport or servicing branch for a Visual Studio release. Trace the target VS branch's inserted F# payload to its exact source commit, configure insertion mappings, and pin the VS minor version.
---

# Create a Visual Studio backport branch

Start from the **F# source commit that produced the payload currently inserted
into the target VS branch**. Do not start from today's F# main, the latest
successful build, the VS insertion commit, or an estimated snap date.

This workflow prepares a branch and configuration edits. It does not commit,
push, cherry-pick fixes, create a PR, queue a build, or insert into VS unless
the user also requests those actions.

## Required inputs and access

Obtain these inputs before making changes; ask for missing release decisions:

- Target VS branch, without `refs/heads/`, for example `rel/insiders`.
- New F# release branch, for example `release/dev18.11`. Use `release/*` so CI
  triggers and the release-branch handling in `eng\Build.ps1` apply.
- Target VS minor, explicitly confirmed by the user or release owner. Require
  an integer in `0..65534`. Do not infer it from the branch name, today's date,
  or the currently inserted payload's version. Two VS branches can share a minor.
- Approved F# checkout and, optionally, an existing VS clone.

Require Git, PowerShell, Azure CLI authentication, and permission to read both
DevDiv's VS repository and dnceng/internal F# build metadata. Authentication
alone does not grant these permissions. Report access failures; never substitute
public PR builds or guessed source commits.

Respect the host's workspace and branch-management rules. Do not switch a
shared/main checkout or bypass app-managed branch naming. If the host cannot
create the requested branch at the exact source SHA, report that limitation and
the required operation instead of creating a differently based/named branch.

Check `$LASTEXITCODE` after every native command; PowerShell's
`$ErrorActionPreference = 'Stop'` does not make native-command failures fatal.

## 1. Freeze the target VS snapshot

Refresh the target branch from its verified VS remote without changing the VS
checkout. Resolve its fetched remote-tracking ref to a full commit SHA and keep
that SHA as `$VSSnapshot` for every subsequent read. Do not trust a stale local
branch or a detached HEAD as the current release state.

If no VS clone is available, use an invocation-owned temporary directory and
an authenticated shallow clone of:

```text
https://devdiv.visualstudio.com/DefaultCollection/DevDiv/_git/VS
```

Clone only the requested branch with `--no-checkout --single-branch --no-tags
--depth=100`. Deepen if needed; do not check out the large VS tree.

Use the configured credential manager where possible. If bearer authentication
is needed for Git, supply it through process-scoped `GIT_CONFIG_*` settings for
`http.extraHeader`, disable Git tracing, and restore the previous settings in
`finally`. Never put tokens in command arguments, URLs, persistent Git config,
files, or output. Delete only temporary clones created by this invocation,
including on failure; never remove a supplied VS clone.

Read the pinned snapshot's `.corext/Configs/components.json` and extract
`Components."Microsoft.FSharp"`:

```powershell
$ComponentsPath = '.corext/Configs/components.json' # Git tree path
$components = (git -C $VSRepo show "${VSSnapshot}:$ComponentsPath") -join "`n" |
    ConvertFrom-Json
$component = $components.Components.'Microsoft.FSharp'
$PayloadUrl = $component.url
```

Require a nonempty component URL. Record the component version if present, but
do not use that version to choose the target release minor.

Locate the first-parent change that introduced the current URL:

```powershell
git -C $VSRepo log $VSSnapshot --first-parent -n 1 --format=%H "-S$PayloadUrl" -- $ComponentsPath
```

Inspect the candidate and its first parent: the F# component must acquire the
current payload at that change. Record the insertion SHA, date, and subject.
A shallow boundary can look like an introduction because Git treats it as a
root. Check `git rev-parse --git-path shallow`; reject candidates at a boundary
or with an unavailable parent. Deepen the target branch and retry with a bounded
number of attempts, retaining `$VSSnapshot`. If provenance remains unprovable,
stop and request sufficient history rather than using the boundary as evidence.

## 2. Resolve the payload to an F# build and source SHA

The established payload URL contains
`/dotnet-fsharp/<source-branch>/<build-number>;...`. Preserve slashes within the
source branch and URI-decode it:

```powershell
$match = [regex]::Match(
    $PayloadUrl,
    '/dotnet-fsharp/(?<sourceBranch>.+)/(?<buildNumber>[^/;]+);',
    [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
if (-not $match.Success) { throw 'Unrecognized F# payload URL; inspect its metadata.' }
$SourceBranch = [Uri]::UnescapeDataString($match.Groups['sourceBranch'].Value)
$BuildNumber = $match.Groups['buildNumber'].Value
```

Query the official `fsharp-ci` build using **both** the exact build number and
`refs/heads/$SourceBranch`. Its established definition ID is `499` in
`https://dev.azure.com/dnceng/internal`; verify the pipeline identity rather
than silently accepting a different definition if the configuration has moved.

For Azure DevOps REST authentication, acquire a token into a variable without
printing it:

```powershell
$token = az account get-access-token --resource 499b84ac-1321-427f-aa17-267ca6975798 --query accessToken --only-show-errors -o tsv
if ($LASTEXITCODE -ne 0 -or -not $token) { throw 'Azure DevOps authentication failed.' }
try {
    $number = [Uri]::EscapeDataString($BuildNumber)
    $branch = [Uri]::EscapeDataString("refs/heads/$SourceBranch")
    $uri = "https://dev.azure.com/dnceng/internal/_apis/build/builds?definitions=499&buildNumber=$number&branchName=$branch&api-version=7.1"
    $response = Invoke-RestMethod -Uri $uri -Headers @{ Authorization = "Bearer $token" }
}
finally {
    Remove-Variable token -ErrorAction SilentlyContinue
}
```

Require exactly one matching build, handling pagination if returned. Verify its
definition, build number, source branch, and expected F# repository or official
mirror. Use its `sourceVersion` as `$SourceSha`, requiring a full 40-digit
hexadecimal Git commit SHA. Record the build ID as well as its number.

Resolve that commit in the approved F# checkout. Inspect remote URLs; prefer a
verified `upstream`, then `origin`, containing the recorded source branch.
Fetch the exact branch if cached refs do not contain the commit. Deepen a
shallow clone when necessary; do not change the checkout during discovery.

Prove that `$SourceSha` exists as a commit and is an ancestor of the resolved
source-branch ref using `git cat-file -e` and `git merge-base --is-ancestor`.
If the commit or ancestry cannot be verified, stop. The branch tip is evidence
of ancestry, **not** the branch point.

## 3. Preflight and present the plan

Before creating anything:

- Validate the proposed branch with `git check-ref-format --branch`. Check
  local branches, other worktrees, and the intended publishing remote for an
  existing branch. Do not reset or overwrite one; ask whether to use it.
- Require a clean destination worktree. Do not automatically stash, discard,
  or commit user changes. Protect untracked and ignored paths from collisions.
- Read `azure-pipelines.yml`, `eng\Versions.props`, and the applicable build
  entry points **at `$SourceSha`**, not just in the current checkout.
- Confirm each pipeline mapping exists once and that the version layout can
  be pinned unambiguously. If the historical layout differs, explain the
  required adaptation rather than replacing whole files with today's copies.

Present the VS branch and snapshot, current payload identity, insertion commit,
F# build ID/number and source branch, exact F# source SHA, new branch name,
confirmed minor, and the two planned file edits. Redact credentials or signed
URL parameters. If the user requested only discovery/preview, stop here.
Otherwise proceed only within their authorization to create this branch.

## 4. Create the branch and configure servicing

In an approved standalone checkout, the Git operation is:

```powershell
git -C $FSharpRepo switch --no-overwrite-ignore -c $NewFSharpBranch $SourceSha
```

For a managed workspace, use its supported equivalent with the same exact base
and branch name. Do not merge main into the new branch.

Make only these configuration changes, preserving formatting and unrelated
content:

1. In `azure-pipelines.yml`, set `FSharpReleaseBranchName` to the new **F#**
   branch and `VSInsertionTargetBranchName` to the target **VS** branch, both
   without `refs/heads/`. These are different mappings: they control which F#
   branch can insert and where its payload is sent.
2. In `eng\Versions.props`, set one **unconditional literal**
   `<VSMinorVersion>11</VSMinorVersion>` using the confirmed minor, not
   necessarily `11`. Replace any conditional `UNSET` default, including its
   `Condition`; merely changing its text would still allow an environment
   value to win. Preserve `VSMajorVersion` unless a major-version change is
   explicitly part of the release decision.

The date-based `eng\Build.ps1` implementation skips automatic minor selection
for `release/*`, including PRs targeting a release branch. The unconditional
pin also protects direct MSBuild/IDE builds from inherited environment values.
If the historical base instead has `UseVSScheduledMinorVersion` and a schedule
import, disable that existing mechanism before its import. Do not add that
removed property or copy the removed backport utility into the newer layout.

## 5. Verify and hand off

Before any commit, verify `HEAD` is still exactly `$SourceSha`. Run
`git diff --check` and inspect the diff: branch preparation should change only
the two configuration files above.

Evaluate the actual pinned properties without compiling:

```powershell
dotnet msbuild .\eng\Versions.props -nologo -getProperty:VSMajorVersion,VSMinorVersion,VSAssemblyVersion
```

For major `18` and minor `11`, expect `VSMinorVersion=11` and
`VSAssemblyVersion=18.11.0.0`. Repeat in a child process with the
`VSMinorVersion` environment variable set to a different number; the pin must
still win. If the base has date/timestamp-driven versioning, also evaluate with
a later date/timestamp to prove the release remains pinned. Do not test this
using `/p:VSMinorVersion=...`: an MSBuild command-line global property overrides
even an unconditional project assignment.

Confirm both insertion mappings, the `release/*` trigger coverage, and the
absence of unintended compiler/Core/FCS version changes. Do not run a signed
build or an insertion merely to validate branch preparation.

Report the created branch, exact base SHA and build provenance, fixed VS
version, insertion target, and whether edits remain uncommitted. If creation
succeeds but edits fail, report the partial state explicitly; do not perform a
destructive automatic rollback. Clean up invocation-owned temporary clones and
leave unrelated checkouts and credentials unchanged.

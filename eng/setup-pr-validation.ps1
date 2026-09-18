# Sets up the working tree on a DartLab test machine to validate a specific GitHub PR: fetches the
# PR merge commit, optionally enforces that the PR head still matches the reviewed SHA, and checks
# it out. Referenced by eng/pipelines/apex-integration/stage.yml (only when a PR number is supplied).
# Adapted from dotnet/roslyn's eng/setup-pr-validation.ps1.
[CmdletBinding(PositionalBinding=$false)]
param (
  [string]$sourceBranchName,
  [string]$prNumber,
  [string]$commitSHA,
  [boolean]$enforceLatestCommit)

try {
    # name and email are only used for the merge commit; the values do not matter.
    git config user.name "FSharpValidation"
    git config user.email "validation@fsharp.net"

    if ($commitSHA.Length -lt 7) {
      Write-Host "##vso[task.LogIssue type=error;]The PR Commit SHA must be at least 7 characters long."
      exit 1
    }

    git remote add gh https://github.com/dotnet/fsharp.git

    Write-Host "Getting the hash of refs/pull/$prNumber/head..."
    $remoteRef = git ls-remote gh refs/pull/$prNumber/head
    Write-Host ($remoteRef | Out-String)

    $prHeadSHA = $remoteRef.Split()[0]

    if ($enforceLatestCommit) {
      Write-Host "Validating the PR head matches the specified commit SHA ($commitSHA)..."
      if (!$prHeadSHA.StartsWith($commitSHA)) {
        Write-Host "##vso[task.LogIssue type=error;]The PR's Head SHA ($prHeadSHA) does not begin with the specified commit SHA ($commitSHA). Unreviewed changes may have been pushed to the PR."
        exit 1
      }
    }

    Write-Host "Setting up the build for PR validation by fetching refs/pull/$prNumber/merge..."
    git fetch gh refs/pull/$prNumber/merge
    if (!$?) {
      Write-Host "##vso[task.LogIssue type=error;]Fetching ref refs/pull/$prNumber/merge failed."
      exit 1
    }

    git checkout FETCH_HEAD
    if (!$?) {
      Write-Host "##vso[task.LogIssue type=error;]Checking out FETCH_HEAD for refs/pull/$prNumber/merge failed."
      exit 1
    }

    if (!$enforceLatestCommit) {
      if ($prHeadSHA.StartsWith($commitSHA)) {
        Write-Host "PR head SHA ($prHeadSHA) already matches the specified commit SHA ($commitSHA), skipping checkout."
      }
      else {
        Write-Host "Checking out the specified commit SHA ($commitSHA)..."
        git checkout $commitSHA
        if (!$?) {
          Write-Host "##vso[task.LogIssue type=error;]Checking out commit SHA $commitSHA failed."
          exit 1
        }
      }
    }
}
catch {
  Write-Host $_
  Write-Host $_.Exception
  Write-Host $_.ScriptStackTrace
  exit 1
}

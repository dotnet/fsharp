[CmdletBinding(PositionalBinding=$false)]
param (
    [Parameter(Mandatory=$true)]
    [ValidatePattern('^[1-9][0-9]*$')]
    [string]$prNumber,
    [Parameter(Mandatory=$true)]
    [ValidatePattern('^[0-9a-f]{40}$')]
    [string]$headSha
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-Git {
    # Windows PowerShell treats native stderr (including successful fetch progress) as errors.
    $ErrorActionPreference = 'Continue'
    $output = & git @args 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "git $args failed: $output"
    }
    $output | ForEach-Object { "$_" }
}

function Get-PrHeadSha {
    $refName = "refs/pull/$prNumber/head"
    $refs = @(Invoke-Git ls-remote $repository $refName)
    Write-Host 'Advertised PR head ref (tabs shown as <TAB>):'
    foreach ($ref in $refs) {
        Write-Host "  $($ref.Replace("`t", '<TAB>'))"
    }
    if ($refs.Count -ne 1 -or $refs[0] -cnotmatch "\A([0-9a-f]{40})`t$refName\z") {
        throw 'Git did not advertise exactly one valid PR head ref.'
    }
    $Matches[1]
}

function Get-ValidatedPr {
    $pr = Invoke-RestMethod -Uri "https://api.github.com/repos/dotnet/fsharp/pulls/$prNumber" -Headers @{
        Accept = 'application/vnd.github+json'
        'User-Agent' = 'FSharp-Apex-Validation'
        'X-GitHub-Api-Version' = '2022-11-28'
    }
    Write-Host "GitHub PR: state=$($pr.state) target=$($pr.base.repo.full_name):$($pr.base.ref) head=$($pr.head.sha) base=$($pr.base.sha)"
    if ($pr.state -cne 'open' -or $pr.base.repo.full_name -cne 'dotnet/fsharp' -or
        $pr.base.ref -cne 'main') {
        throw 'The PR is closed or no longer targets dotnet/fsharp main.'
    }
    if ($pr.head.sha -isnot [string] -or $pr.head.sha -cnotmatch '\A[0-9a-f]{40}\z') {
        throw 'GitHub did not return a full PR head commit SHA.'
    }
    $pr
}

Write-Host "Requested snapshot: PR #$prNumber reviewed=$headSha"
$checkoutSha = Invoke-Git rev-parse HEAD
Write-Host "Initial checkout: $checkoutSha"
if ($checkoutSha -ne $headSha) {
    throw 'Expected the reviewed PR checkout at the captured head SHA.'
}

$repository = 'https://github.com/dotnet/fsharp.git'
$pr = Get-ValidatedPr
$currentHead = Get-PrHeadSha
$testedSha = $headSha
$mode = 'commit'
if ($currentHead -ceq $headSha) {
    Invoke-Git fetch --no-tags --depth=2 $repository "refs/pull/$prNumber/merge" | Out-Null
    $mergeSha = Invoke-Git rev-parse FETCH_HEAD
    $parents = (Invoke-Git show -s --format=%P $mergeSha) -split ' '
    Write-Host "Fetched merge: $mergeSha parents=$($parents -join ' ')"
    $currentHead = Get-PrHeadSha
    if ($currentHead -ceq $headSha) {
        if ($parents.Count -ne 2 -or $parents[1] -cne $headSha) {
            throw 'The fetched PR merge does not contain the reviewed head as its second parent.'
        }
        $testedSha = $mergeSha
        $mode = 'merge'
    }
}

if ($mode -eq 'commit') {
    $found = $false
    for ($page = 1; $page -le 3; $page++) {
        $commits = Invoke-RestMethod -Uri "https://api.github.com/repos/dotnet/fsharp/pulls/$prNumber/commits?per_page=100&page=$page" -Headers @{
            Accept = 'application/vnd.github+json'
            'User-Agent' = 'FSharp-Apex-Validation'
            'X-GitHub-Api-Version' = '2022-11-28'
        }
        if ($commits -isnot [array] -or $commits.Count -gt 100) { throw 'GitHub returned an invalid PR commit list.' }
        foreach ($commit in $commits) {
            if ($commit -isnot [System.Management.Automation.PSCustomObject] -or
                $commit.PSObject.Properties.Name -cnotcontains 'sha' -or
                $commit.sha -isnot [string] -or $commit.sha -cnotmatch '\A[0-9a-f]{40}\z') { throw 'GitHub returned an invalid PR commit SHA.' }
            if ($commit.sha -ceq $headSha) { $found = $true }
        }
        if ($found -or $commits.Count -lt 100) { break }
    }
    if (!$found) { throw 'The reviewed SHA was not found in the PR commit list (GitHub exposes at most 250 commits).' }
    Write-Host "PR head observations: API=$($pr.head.sha) Git=$currentHead; testing reviewed commit directly."
}

$pr = Get-ValidatedPr
Invoke-Git checkout --detach $testedSha | Out-Null
Write-Host "PR #$prNumber`: reviewed=$headSha mode=$mode tested=$testedSha"
Write-Host "##vso[task.setvariable variable=FSharp.TestedCommitSha]$testedSha"
Write-Host "##vso[task.setvariable variable=FSharp.TestedCommitMode]$mode"

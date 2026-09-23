[CmdletBinding(PositionalBinding=$false)]
param (
    [Parameter(Mandatory=$true)]
    [ValidatePattern('^[1-9][0-9]*$')]
    [string]$prNumber,
    [Parameter(Mandatory=$true)]
    [ValidatePattern('^[0-9a-f]{40}$')]
    [string]$headSha,
    [Parameter(Mandatory=$true)]
    [ValidatePattern('^[0-9a-f]{40}$')]
    [string]$baseSha
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

if ((Invoke-Git rev-parse HEAD) -ne $baseSha) {
    throw 'Expected the trusted main checkout at the captured base SHA.'
}

$repository = 'https://github.com/dotnet/fsharp.git'
Invoke-Git fetch --no-tags --depth=2 $repository "refs/pull/$prNumber/merge" | Out-Null
$mergeSha = Invoke-Git rev-parse FETCH_HEAD
$parents = (Invoke-Git show -s --format=%P $mergeSha) -split ' '
if ($parents.Count -ne 2 -or $parents[0] -ne $baseSha -or $parents[1] -ne $headSha) {
    throw 'The fetched PR merge does not match the captured base/head. Request a new /dart or /pr-val run.'
}

$pr = Invoke-RestMethod -Uri "https://api.github.com/repos/dotnet/fsharp/pulls/$prNumber" -Headers @{
    Accept = 'application/vnd.github+json'
    'User-Agent' = 'FSharp-Apex-Validation'
    'X-GitHub-Api-Version' = '2022-11-28'
}
if ($pr.state -ne 'open' -or $pr.base.repo.full_name -ne 'dotnet/fsharp' -or
    $pr.base.ref -ne 'main' -or $pr.head.sha -ne $headSha -or $pr.base.sha -ne $baseSha) {
    throw 'The PR is closed or its head/base changed. Request a new /dart or /pr-val run.'
}
$refs = Invoke-Git ls-remote $repository refs/heads/main "refs/pull/$prNumber/head"
foreach ($expected in @("$baseSha`trefs/heads/main", "$headSha`trefs/pull/$prNumber/head")) {
    if ($refs -cnotcontains $expected) {
        throw 'The current head/base refs do not match the snapshot. Request a new /dart or /pr-val run.'
    }
}

Invoke-Git checkout --detach $mergeSha | Out-Null
Write-Host "PR #$prNumber`: head=$headSha base=$baseSha merge=$mergeSha"
Write-Host "##vso[task.setvariable variable=FSharp.PrMergeSha]$mergeSha"

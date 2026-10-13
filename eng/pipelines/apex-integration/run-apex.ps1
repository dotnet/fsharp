[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..\..')
Set-Location $repositoryRoot

$mergeSha = (& git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $mergeSha -notmatch '^[0-9a-f]{40}$') {
    throw 'Unable to determine the verified PR merge SHA.'
}
Write-Host "Running Apex against verified PR merge $mergeSha."

$env:NativeToolsOnMachine = 'true'
& .\eng\SetupVSHive.ps1
& .\eng\CIBuildNoPublish.cmd -configuration $Configuration -deployExtensions -testApex
if ($LASTEXITCODE -ne 0) {
    throw "Apex build and tests failed with exit code $LASTEXITCODE."
}

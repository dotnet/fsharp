[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$gitDirectory = 'C:\Test\Git\cmd'
if (-not (Test-Path -LiteralPath (Join-Path $gitDirectory 'git.exe') -PathType Leaf)) {
    throw 'DartLab Git was not provisioned before running Apex.'
}
$env:PATH = "$gitDirectory;$env:PATH"

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..\..')
Set-Location $repositoryRoot

$testedSha = (& git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $testedSha -notmatch '^[0-9a-f]{40}$') {
    throw 'Unable to determine the selected PR test commit SHA.'
}
Write-Host "Running Apex against selected PR test commit $testedSha."

$env:NativeToolsOnMachine = 'true'
& .\eng\SetupVSHive.ps1
& .\eng\CIBuildNoPublish.cmd -configuration $Configuration -deployExtensions -testApex
if ($LASTEXITCODE -ne 0) {
    throw "Apex build and tests failed with exit code $LASTEXITCODE."
}

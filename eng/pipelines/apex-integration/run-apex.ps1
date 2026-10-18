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
$env:AGENT_WORKFOLDER = $repositoryRoot.Path

$testedSha = (& git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $testedSha -notmatch '^[0-9a-f]{40}$') {
    throw 'Unable to determine the selected PR test commit SHA.'
}
Write-Host "Running Apex against selected PR test commit $testedSha."

$resultsDirectory = Join-Path $repositoryRoot.Path "artifacts\TestResults\$Configuration"
$destination = $env:LoggingDirectory
if ([string]::IsNullOrWhiteSpace($destination)) {
    throw 'CloudTest LoggingDirectory is required to publish Apex test results.'
}

$env:NativeToolsOnMachine = 'true'
$buildFailure = $null
try {
    & .\eng\SetupVSHive.ps1
    & .\eng\CIBuildNoPublish.cmd -configuration $Configuration -deployExtensions -testApex
    if ($LASTEXITCODE -ne 0) {
        throw "Apex build and tests failed with exit code $LASTEXITCODE."
    }
}
catch {
    $buildFailure = $_
    throw
}
finally {
    try {
        $trxFiles = @()
        if (Test-Path -LiteralPath $resultsDirectory -PathType Container) {
            $trxFiles = @(Get-ChildItem -LiteralPath $resultsDirectory -Filter '*.trx' -File -Recurse)
            New-Item -ItemType Directory -Path $destination -Force | Out-Null
            Get-ChildItem -LiteralPath $resultsDirectory -Force |
                Copy-Item -Destination $destination -Recurse -Force
        }
        Write-Host "Apex result handoff: $($trxFiles.Count) TRX file(s) from '$resultsDirectory' to '$destination'."
        if (!$buildFailure -and $trxFiles.Count -eq 0) {
            throw "Apex build succeeded but no TRX results were produced in '$resultsDirectory'."
        }
    }
    catch {
        $message = "Failed to hand off Apex results from '$resultsDirectory' to '$destination': $_"
        if (!$buildFailure) { throw $message }
        Write-Error -Message $message -ErrorAction Continue
    }
}

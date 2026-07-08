<#
  F# 15.9 VS IDE unit tests (vsintegration/tests/UnitTests/VisualFSharp.UnitTests).

  Exercises the vsintegration assemblies the infra changes actually touched (FSharp.Editor + the serviced
  Roslyn 2.10 editor services, the legacy LanguageService/ProjectSystem, and the FCS service tests). Runs
  AFTER the vsintegration insertion build (CIBuild Step 5), so the vsint src projects + serviced Roslyn are
  already restored/built. Restores + builds the test with the same devdiv 'VS' feed the VSIX build uses
  (passed as --source; nothing written into a committed config, so CFS stays green), then runs it headless
  via the NUnit console in an STA apartment (the editor/WPF tests require STA). Emits NUnit XML.
#>
[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$RepoRoot,
    [string]$ResultsDir
)

$ErrorActionPreference = 'Stop'
if (-not $RepoRoot) {
    $scriptDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
    $RepoRoot = (Resolve-Path (Join-Path $scriptDir '..')).Path
}
if (-not $ResultsDir) { $ResultsDir = Join-Path $RepoRoot 'artifacts\TestResults' }
New-Item -ItemType Directory -Force -Path $ResultsDir | Out-Null

# Use the Arcade-provisioned SDK (matches global.json), consistent with CIBuild Step 5.
$dotnet = Join-Path $RepoRoot '.dotnet\dotnet.exe'
if (-not (Test-Path $dotnet)) { $dotnet = 'dotnet' }
$env:DOTNET_ROOT = Join-Path $RepoRoot '.dotnet'
$env:DOTNET_MULTILEVEL_LOOKUP = '0'
$env:NUGET_PACKAGES = (Join-Path $RepoRoot '.packages\')

$proj = Join-Path $RepoRoot 'vsintegration\tests\UnitTests\VisualFSharp.UnitTests.fsproj'

# Restore feeds = the devdiv 'VS' feed (serviced Roslyn 2.10) + every approved feed from the committed
# NuGet.Config, all passed as --source (matches CIBuild Step 5; keeps the committed config CFS-clean).
$devdivFeed = 'https://devdiv.pkgs.visualstudio.com/_packaging/VS/nuget/v3/index.json'
$srcArgs = @('--source', $devdivFeed)
foreach ($u in ([xml](Get-Content -Raw (Join-Path $RepoRoot 'NuGet.Config'))).configuration.packageSources.add.value) {
    if ($u) { $srcArgs += @('--source', $u) }
}

Write-Host "==================== Restoring VisualFSharp.UnitTests ===================="
& $dotnet restore $proj --configfile (Join-Path $RepoRoot 'NuGet.Config') @srcArgs
if ($LASTEXITCODE -ne 0) { Write-Host "VS IDE tests: restore FAILED"; exit 1 }

Write-Host "==================== Building VisualFSharp.UnitTests ===================="
# Use `dotnet build` directly (NOT build.ps1 -ci, which self-reports failure to the pipeline). The test's
# ProjectReferences rebuild the vsint assemblies, so pass /p:GeneratePkgDefFile=false to skip CreatePkgDef
# (matches the VSIX build; CreatePkgDef fails with ReflectionTypeLoadException on FSharp.Editor here) and
# supply the RID the VS-SDK RID check wants. -warnaserror off so vsint build warnings don't fail the build.
& $dotnet build $proj -c $Configuration -m:1 `
    /p:GeneratePkgDefFile=false /p:DisableLocalization=true /p:TreatWarningsAsErrors=false `
    /p:RuntimeIdentifiers=win
if ($LASTEXITCODE -ne 0) { Write-Host "VS IDE tests: build FAILED"; exit 1 }

$dll = Get-ChildItem (Join-Path $RepoRoot "artifacts\bin\VisualFSharp.UnitTests\$Configuration") -Recurse -Filter 'VisualFSharp.UnitTests.dll' -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $dll) { Write-Host "VS IDE tests: VisualFSharp.UnitTests.dll not produced"; exit 1 }
Write-Host "test dll: $($dll.FullName)"

# NUnit 3 console runner.
$runner = @(
    (Join-Path $RepoRoot 'packages\NUnit.Console.3.0.0\tools\nunit3-console.exe'),
    (Join-Path $env:USERPROFILE '.nuget\packages\nunit.console\3.0.0\tools\nunit3-console.exe')
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $runner) {
    $runner = Get-ChildItem (Join-Path $RepoRoot 'packages'), (Join-Path $env:USERPROFILE '.nuget\packages'), (Join-Path $RepoRoot '.packages') -Recurse -Filter 'nunit3-console.exe' -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $runner) { Write-Host "VS IDE tests: nunit3-console.exe not found"; exit 1 }
Write-Host "NUnit runner: $runner"

$resultXml = Join-Path $ResultsDir 'VisualFSharp.UnitTests.xml'
# STA apartment + single worker: the editor/WPF/VS-shell tests are not thread-safe and require STA.
& $runner $dll.FullName "--result=$resultXml" '--noheader' '--workers=1' '--domain=Single' '--x86:false'
$code = $LASTEXITCODE
if ($code -ne 0) { Write-Host "VS IDE tests: FAILED (nunit exit $code)"; exit 1 }
Write-Host "VS IDE tests: passed"
exit 0

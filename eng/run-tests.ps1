<#
  F# 15.9 unit tests.

  Builds and runs the 15.9-era NUnit unit-test suites against the product that CIBuild.cmd just built
  (the net9-proto fsc + the real FSharp.Core / FSharp.Compiler.Private). Produces NUnit result XML for
  PublishTestResults. Fails the pipeline on any test failure.

  Per-project signing rule (matches the assemblies' references):
    * FSharp.Core.UnitTests  -> built UNSIGNED: it references the unsigned FsCheck 3.0.0-alpha3, and a
      strong-named assembly cannot reference an unsigned one.
    * FSharp.Compiler.UnitTests -> built SIGNED (default): it needs the keyed InternalsVisibleTo from
      FSharp.Compiler.Private and does not reference FsCheck.

  The proto builds only fsc.exe, so FsiToolPath points at the fully built product's fsi.exe for the
  legacy post-build subst.fsx step.
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

# Pick a dotnet that actually satisfies global.json here: the Arcade-provisioned .dotnet on CI (system
# dotnet may be too old for global.json's pinned SDK), or the system dotnet locally (a dev box may have a
# broken/partial .dotnet). Probe each candidate with `dotnet --version` from the repo root and use the
# first that resolves global.json successfully.
Push-Location $RepoRoot
$prevEAP = $ErrorActionPreference
$ErrorActionPreference = 'Continue'
try {
    $dotnet = $null
    foreach ($cand in @((Join-Path $RepoRoot '.dotnet\dotnet.exe'), (Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'), (Get-Command dotnet -ErrorAction SilentlyContinue).Source)) {
        if (-not $cand -or -not (Test-Path $cand)) { continue }
        & $cand --version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) { $dotnet = $cand; break }
    }
} finally { $ErrorActionPreference = $prevEAP; Pop-Location }
if (-not $dotnet) { Write-Host "TESTS FAIL: no dotnet resolves global.json (tried .dotnet, ProgramFiles, PATH)"; exit 1 }
Write-Host "dotnet: $dotnet"
$protoBin = Join-Path $RepoRoot 'Proto\net40\bin'      # has fsc.exe
$prodBin  = Join-Path $RepoRoot 'release\net40\bin'    # has fsi.exe + runtime deps

# Locate the NUnit 3.0.0 console runner (packages.config restore or the global cache).
$runner = @(
    (Join-Path $RepoRoot 'packages\NUnit.Console.3.0.0\tools\nunit3-console.exe'),
    (Join-Path $env:USERPROFILE '.nuget\packages\nunit.console\3.0.0\tools\nunit3-console.exe'),
    'Q:\.tools\.nuget\packages\nunit.console\3.0.0\tools\nunit3-console.exe'
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $runner) {
    $runner = Get-ChildItem (Join-Path $RepoRoot 'packages'), (Join-Path $env:USERPROFILE '.nuget\packages') -Recurse -Filter 'nunit3-console.exe' -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $runner) { Write-Host "TESTS FAIL: nunit3-console.exe not found (NUnit.Console 3.0.0)"; exit 1 }
Write-Host "NUnit runner: $runner"

# Test suites: project path, and whether to build unsigned (StrongNames=false).
$suites = @(
    @{ Name = 'FSharp.Core.UnitTests';     Proj = 'tests\FSharp.Core.UnitTests\FSharp.Core.Unittests.fsproj';        Unsigned = $true  },
    @{ Name = 'FSharp.Compiler.UnitTests'; Proj = 'tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj'; Unsigned = $false }
)

$anyFailed = $false

foreach ($s in $suites) {
    Write-Host "`n==================== $($s.Name) ===================="
    $proj = Join-Path $RepoRoot $s.Proj

    $buildArgs = @(
        'msbuild', $proj,
        "/p:Configuration=$Configuration",
        '/p:DisableAutoSetFscCompilerPath=true',
        "/p:FscToolPath=$protoBin", '/p:FscToolExe=fsc.exe',
        "/p:FsiToolPath=$prodBin",  '/p:FsiToolExe=fsi.exe',
        '/p:DisableLocalization=true', '/m:1', '/v:m'
    )
    if ($s.Unsigned) { $buildArgs += @('/p:StrongNames=false', '/p:SignAssembly=false', '/p:DelaySign=false') }

    & $dotnet @buildArgs
    # The Pdb2Pdb post-build step warns (exit 2) but the test DLL is produced; verify by output, not exit code.

    $dll = Get-ChildItem (Join-Path $RepoRoot "artifacts\bin\$($s.Name)\$Configuration") -Filter "$($s.Name).dll" -ErrorAction SilentlyContinue |
           Select-Object -First 1
    if (-not $dll) {
        # Fall back to a case-insensitive match (FSharp.Core.Unittests vs UnitTests).
        $dll = Get-ChildItem (Join-Path $RepoRoot "artifacts\bin") -Recurse -Filter "$($s.Name).dll" -ErrorAction SilentlyContinue | Select-Object -First 1
    }
    if (-not $dll) { Write-Host "TESTS FAIL: $($s.Name) test DLL not produced"; $anyFailed = $true; continue }

    # Co-locate runtime dependencies the test DLL needs but that aren't emitted next to it.
    $outDir = $dll.Directory.FullName
    foreach ($dep in @('System.ValueTuple.dll', 'FSharp.Core.dll')) {
        $dst = Join-Path $outDir $dep
        if (-not (Test-Path $dst)) {
            $src = Join-Path $prodBin $dep
            if (Test-Path $src) { Copy-Item $src $dst -Force }
        }
    }

    $resultXml = Join-Path $ResultsDir "$($s.Name).xml"
    & $runner $dll.FullName "--result=$resultXml" '--noheader'
    if ($LASTEXITCODE -ne 0) { Write-Host "$($s.Name): FAILED (nunit exit $LASTEXITCODE)"; $anyFailed = $true }
    else { Write-Host "$($s.Name): passed" }
}

if ($anyFailed) { Write-Host "`nUnit tests FAILED."; exit 1 }
Write-Host "`nAll unit tests passed."
exit 0

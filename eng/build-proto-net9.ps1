#requires -Version 5
<#
  Block 9i: F# 15.9 proto/bootstrap build using the .NET 9 SDK's bundled F# toolset.

  This replaces the legacy FSharp.Compiler.Tools 4.1.27 LKG proto build. It needs NO
  nuget.org-only package: the .NET 9 SDK's fsc builds the 15.9 proto compiler, and
  fslex/fsyacc are built from source (src/buildtools). See Block 9g for the full recipe.

  Phase 0: build fslex + fsyacc from source (-> artifacts\bin\{fslex,fsyacc}\Proto\*.exe)
  Phase 1: build FSharp.Core(proto) + FSharp.Build-proto + Fsc-proto with the SDK fsc
           (-> Proto\net40\bin\fsc.exe, the genuine "F# 4.5" compiler the real build uses)
#>
param(
  [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)
$ErrorActionPreference = 'Stop'
Set-Location $RepoRoot

# --- locate a usable .NET SDK (>= 9.x) that ships the F# toolset (FSharp\fsc.dll).
#     Prefer the global.json-pinned version; search the repo-local Arcade SDK (.dotnet)
#     first, then a machine-wide install. The 15.9 proto recipe needs a >= 9.x fsc
#     (the from-source fslex/fsyacc target net9.0). ---
$pinned = $null
try { $pinned = (Get-Content (Join-Path $RepoRoot 'global.json') -Raw | ConvertFrom-Json).tools.dotnet } catch {}
$roots = @((Join-Path $RepoRoot '.dotnet'), (Join-Path $env:ProgramFiles 'dotnet')) |
  Where-Object { Test-Path (Join-Path $_ 'dotnet.exe') }
if (-not $roots) { throw "dotnet.exe not found (looked in .dotnet and Program Files)" }

$dotnetRoot = $null; $sdkFSharp = $null
# 1) exact pinned version, in either root
foreach ($root in $roots) {
  if ($pinned -and (Test-Path (Join-Path $root "sdk\$pinned\FSharp\fsc.dll"))) {
    $dotnetRoot = $root; $sdkFSharp = Join-Path $root "sdk\$pinned\FSharp"; break
  }
}
# 2) else highest >= 9.x SDK that ships FSharp\fsc.dll, in either root
if (-not $sdkFSharp) {
  foreach ($root in $roots) {
    $cand = Get-ChildItem (Join-Path $root 'sdk') -Directory -ErrorAction SilentlyContinue |
      Where-Object { ($_.Name -replace '-.*$','') -as [version] -and [version]($_.Name -replace '-.*$') -ge [version]'9.0.0' -and (Test-Path (Join-Path $_.FullName 'FSharp\fsc.dll')) } |
      Sort-Object { [version]($_.Name -replace '-.*$') } -Descending | Select-Object -First 1
    if ($cand) { $dotnetRoot = $root; $sdkFSharp = Join-Path $cand.FullName 'FSharp'; break }
  }
}
if (-not $sdkFSharp) { throw "No >= 9.x .NET SDK with FSharp\fsc.dll found under: $($roots -join '; ')" }
$dotnet = Join-Path $dotnetRoot 'dotnet.exe'

# The from-source build tools (fslex/fsyacc/fssrgen) are framework-dependent net9.0 apphosts.
# On a clean CI agent the .NET 9 runtime lives under the Arcade-installed .dotnet (NOT globally
# registered), so the apphosts fail with "You must install or update .NET to run this application".
# Point DOTNET_ROOT at the resolved dotnet root (and put it first on PATH) so they find the runtime.
$env:DOTNET_ROOT = $dotnetRoot
$env:DOTNET_ROOT_X64 = $dotnetRoot
$env:DOTNET_MULTILEVEL_LOOKUP = '0'
$env:PATH = "$dotnetRoot;$env:PATH"

# 8.3 short paths: the legacy Fsc MSBuild task splits a path on its first space
# (e.g. "C:\Program Files\..."), so feed it space-free short paths.
$fso = New-Object -ComObject Scripting.FileSystemObject
$sdkFSharpShort = $fso.GetFolder($sdkFSharp).ShortPath
$dotnetDirShort = $fso.GetFolder($dotnetRoot).ShortPath

Write-Host "dotnet        : $dotnet"
Write-Host "SDK FSharp    : $sdkFSharp"
Write-Host "SDK FSharp 8.3: $sdkFSharpShort"

# --- Phase 0: fslex + fsyacc + fssrgen from source ---
Write-Host "`n==== Phase 0: build fslex/fsyacc/fssrgen from source ====" -ForegroundColor Cyan
& $dotnet build (Join-Path $RepoRoot 'src\buildtools\fslex\fslex.fsproj')   -c Proto /p:DisableLocalization=true --nologo /v:minimal
if ($LASTEXITCODE) { throw "fslex build failed ($LASTEXITCODE)" }
& $dotnet build (Join-Path $RepoRoot 'src\buildtools\fsyacc\fsyacc.fsproj') -c Proto /p:DisableLocalization=true --nologo /v:minimal
if ($LASTEXITCODE) { throw "fsyacc build failed ($LASTEXITCODE)" }
& $dotnet build (Join-Path $RepoRoot 'src\buildtools\fssrgen\fssrgen.fsproj') -c Proto /p:DisableLocalization=true --nologo /v:minimal
if ($LASTEXITCODE) { throw "fssrgen build failed ($LASTEXITCODE)" }

# --- Phase 1: proto/bootstrap compiler ---
Write-Host "`n==== Phase 1: build proto compiler ====" -ForegroundColor Cyan
$protoBinlog = Join-Path $RepoRoot 'artifacts\log\proto-net9.binlog'
New-Item -ItemType Directory -Force -Path (Split-Path $protoBinlog) | Out-Null
& $dotnet msbuild (Join-Path $RepoRoot 'src\fsharp-proto-build.proj') `
  /p:Configuration=Proto `
  /p:VisualStudioVersion=15.0 `
  /p:SystemCollectionsImmutableVersion=1.5.0 `
  /p:DisableLocalization=true `
  /p:TargetFrameworkVersion=v4.7.2 `
  /p:TreatWarningsAsErrors=false `
  /p:SignAssembly=false `
  /p:DelaySign=false `
  /p:PublicSign=false `
  "/p:FSharpTargetsPath=$sdkFSharpShort\Microsoft.FSharp.Targets" `
  "/p:FSharpBuildAssemblyFile=$sdkFSharpShort\FSharp.Build.dll" `
  "/p:DotnetFscCompilerPath=$sdkFSharpShort\fsc.dll" `
  "/p:FscToolPath=$dotnetDirShort" `
  /p:FscToolExe=dotnet.exe `
  /nologo /v:minimal `
  "/bl:$protoBinlog"
if ($LASTEXITCODE) { throw "proto build failed ($LASTEXITCODE)" }

if (-not (Test-Path (Join-Path $RepoRoot 'Proto\net40\bin\fsc.exe'))) {
  throw "proto build did not produce Proto\net40\bin\fsc.exe"
}
Write-Host "`nPROTO BUILD SUCCEEDED -> Proto\net40\bin\fsc.exe" -ForegroundColor Green

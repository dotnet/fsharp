[CmdletBinding(PositionalBinding=$false)]
Param(
  [string] $configuration = "Debug",
  [string] $solution = "",
  [string] $verbosity = "minimal",
  [switch] $restore,
  [switch] $deployDeps,
  [switch] $build,
  [switch] $rebuild,
  [switch] $deploy,
  [switch] $test,
  [switch] $integrationTest,
  [switch] $sign,
  [switch] $pack,
  [switch] $ci,
  [switch] $prepareMachine,
  [switch] $log,
  [switch] $help,
  [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

set-strictmode -version 2.0
$ErrorActionPreference = "Stop"

function Print-Usage() {
    Write-Host "Common settings:"
    Write-Host "  -configuration <value>  Build configuration Debug, Release"
    Write-Host "  -verbosity <value>      Msbuild verbosity (q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic])"
    Write-Host "  -help                   Print help and exit"
    Write-Host ""

    Write-Host "Actions:"
    Write-Host "  -restore                Restore dependencies"
    Write-Host "  -build                  Build solution"
    Write-Host "  -rebuild                Rebuild solution"
    Write-Host "  -deploy                 Deploy built VSIXes"
    Write-Host "  -deployDeps             Deploy dependencies (Roslyn VSIXes for integration tests)"
    Write-Host "  -test                   Run all unit tests in the solution"
    Write-Host "  -integrationTest        Run all integration tests in the solution"
    Write-Host "  -sign                   Sign build outputs"
    Write-Host "  -pack                   Package build outputs into NuGet packages and Willow components"
    Write-Host ""

    Write-Host "Advanced settings:"
    Write-Host "  -solution <value>       Path to solution to build"
    Write-Host "  -ci                     Set when running on CI server"
    Write-Host "  -log                    Enable logging (by default on CI)"
    Write-Host "  -prepareMachine         Prepare machine for CI run"
    Write-Host ""
    Write-Host "Command line arguments not listed above are passed thru to msbuild."
    Write-Host "The above arguments can be shortened as much as to be unambiguous (e.g. -co for configuration, -t for test, etc.)."
}

if ($help -or (($properties -ne $null) -and ($properties.Contains("/help") -or $properties.Contains("/?")))) {
  Print-Usage
  exit 0
}

$RepoRoot = Join-Path $PSScriptRoot "..\"
$SourceRoot = Join-Path $RepoRoot "src"
$ToolsRoot = Join-Path $RepoRoot ".tools"
$BuildProj = Join-Path $PSScriptRoot "build.proj"
$DependenciesProps = Join-Path $PSScriptRoot "Versions.props"
$ArtifactsDir = Join-Path $RepoRoot "artifacts"
$LogDir = Join-Path (Join-Path $ArtifactsDir $configuration) "log"
$TempDir = Join-Path (Join-Path $ArtifactsDir $configuration) "tmp"

function Create-Directory([string[]] $path) {
  if (!(Test-Path -path $path)) {
    New-Item -path $path -force -itemType "Directory" | Out-Null
  }
}

function GetVSWhereVersion {
  [xml]$xml = Get-Content $DependenciesProps
  return $xml.Project.PropertyGroup.VSWhereVersion
}

function LocateVisualStudio {  
  $vswhereVersion = GetVSWhereVersion
  $vsWhereDir = Join-Path $ToolsRoot "vswhere\$vswhereVersion"
  $vsWhereExe = Join-Path $vsWhereDir "vswhere.exe"
  
  if (!(Test-Path $vsWhereExe)) {
    Create-Directory $vsWhereDir   
    Invoke-WebRequest "http://github.com/Microsoft/vswhere/releases/download/$vswhereVersion/vswhere.exe" -OutFile $vswhereExe
  }
  
  $vsInstallDir = & $vsWhereExe -latest -property installationPath -requires Microsoft.Component.MSBuild -requires Microsoft.VisualStudio.Component.VSSDK -requires Microsoft.Net.Component.4.6.TargetingPack -requires Microsoft.VisualStudio.Component.Roslyn.Compiler -requires Microsoft.VisualStudio.Component.VSSDK
  
  if (!(Test-Path $vsInstallDir)) {
    throw "Failed to locate Visual Studio (exit code '$lastExitCode')."
  }
  
  return $vsInstallDir
}

function InstallDotNet {
  $dotnetUrl = "https://download.microsoft.com/download/1/B/4/1B4DE605-8378-47A5-B01B-2C79D6C55519/dotnet-sdk-2.0.0-win-x64.zip"
  $toolsDir = Join-Path (Split-Path -Parent $PSScriptRoot) "Tools"
  $dotnetArchive = Join-Path $toolsDir "dotnet.zip"
  $dotnetDir = Join-Path $toolsDir "dotnet"
  $dotnetExe = Join-Path $dotnetDir "dotnet.exe"
  If (-Not (Test-Path $toolsDir)) {
    New-Item -ItemType Directory -Force -Path $toolsDir
  }
  If (-Not (Test-Path $dotnetExe)) {
    ((New-Object System.Net.WebClient).DownloadFile($dotnetUrl, $dotnetArchive))
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($dotnetArchive, $dotnetDir)
  }
}

function BuildAndPublish([string] $projectFile, [string] $publishDir, [string] $output) {
  $toolsDir = Join-Path $RepoRoot "Tools"
  $dotnetDir = Join-Path $toolsDir "dotnet"
  $dotnetExe = Join-Path $dotnetDir "dotnet.exe"
  If (-Not (Test-Path $output)) {
    & $dotnetExe restore $projectFile
    & $dotnetExe publish $projectFile -o $publishDir
  }
}

function BuildTools {
  $toolsDir = Join-Path $RepoRoot "Tools"
  $buildToolsDir = Join-Path $SourceRoot "buildtools"
  BuildAndPublish "$buildToolsDir\fssrgen\fssrgen.fsproj" "$toolsDir\fssrgen" (Join-Path $toolsDir "fssrgen\fssrgen.dll")
  BuildAndPublish "$buildToolsDir\fslex\fslex.fsproj" "$toolsDir\fslex" (Join-Path $toolsDir "fslex\fslex.dll")
  BuildAndPublish "$buildToolsDir\fsyacc\fsyacc.fsproj" "$toolsDir\fsyacc" (Join-Path $toolsDir "fsyacc\fsyacc.dll")
}

function BuildProto {
  $vsInstallDir = LocateVisualStudio
  $msbuildExe = Join-Path $vsInstallDir "MSBuild\15.0\Bin\msbuild.exe"
  Write-Host "Building proto"
  & $msbuildExe "$SourceRoot/fsharp-proto-build.proj" /p:Configuration=Proto
}

function Build {
  $vsInstallDir = LocateVisualStudio
  $msbuildExe = Join-Path $vsInstallDir "MSBuild\15.0\Bin\msbuild.exe"

  if ($ci -or $log) {
    Create-Directory($logDir)
    $logCmd = "/bl:" + (Join-Path $LogDir "Build.binlog")
  } else {
    $logCmd = ""
  }

  $nodeReuse = !$ci

  & $msbuildExe $BuildProj /m /nologo /clp:Summary /nodeReuse:$nodeReuse /warnaserror /v:$verbosity $logCmd /p:Configuration=$configuration /p:SolutionPath=$solution /p:Restore=$restore /p:DeployDeps=$deployDeps /p:Build=$build /p:Rebuild=$rebuild /p:Deploy=$deploy /p:Test=$test /p:IntegrationTest=$integrationTest /p:Sign=$sign /p:Pack=$pack /p:CIBuild=$ci $properties
}

function Stop-Processes() {
  Write-Host "Killing running build processes..."
  Get-Process -Name "msbuild" -ErrorAction SilentlyContinue | Stop-Process
  Get-Process -Name "vbcscompiler" -ErrorAction SilentlyContinue | Stop-Process
}

function Clear-NuGetCache() {
  # clean nuget packages -- necessary to avoid mismatching versions of swix microbuild build plugin and VSSDK on Jenkins
  $nugetRoot = (Join-Path $env:USERPROFILE ".nuget\packages")
  if (Test-Path $nugetRoot) {
    Remove-Item $nugetRoot -Recurse -Force
  }
}

try {
  if ($ci) {
    Create-Directory $TempDir
    $env:TEMP = $TempDir
    $env:TMP = $TempDir
  }

  # Preparation of a CI machine
  if ($prepareMachine) {
    Clear-NuGetCache
  }

  InstallDotNet
  Build
  #BuildTools
  #Build
  exit $lastExitCode
}
catch {
  Write-Host $_
  Write-Host $_.Exception
  Write-Host $_.ScriptStackTrace
  exit 1
}
finally {
  Pop-Location
  if ($ci -and $prepareMachine) {
    Stop-Processes
  }
}


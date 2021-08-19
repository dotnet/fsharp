# This script verifies that subsequent calls to `Build.cmd` don't cause assemblies to be unnecessarily rebuilt.

[CmdletBinding(PositionalBinding=$false)]
param (
    [string][Alias('c')]$configuration = "Debug",
    [parameter(ValueFromRemainingArguments=$true)][string[]]$properties
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

try {
    $RepoRoot = Join-Path $PSScriptRoot ".." | Join-Path -ChildPath ".." -Resolve
    $BuildScript = Join-Path $RepoRoot "Build.cmd"

    # do first build
    & $BuildScript -configuration $configuration @properties
    if ($LASTEXITCODE -ne 0) {
        Write-Host "##[error](NETCORE_ENGINEERING_TELEMETRY=Build) Error running first build."
        exit 1
    }

    # gather assembly timestamps
    $ArtifactsBinDir = Join-Path $RepoRoot "artifacts" | Join-Path -ChildPath "bin" -Resolve
    $FSharpAssemblyDirs = Get-ChildItem -Path $ArtifactsBinDir -Filter "FSharp.*"
    $FscAssemblyDir = Get-ChildItem -Path $ArtifactsBinDir -Filter "fsc"
    $FscAnyCpuAssemblyDir = Get-ChildItem -Path $ArtifactsBinDir -Filter "fscAnyCpu"
    $FsiAssemblyDir = Get-ChildItem -Path $ArtifactsBinDir -Filter "fsi"
    $FsiAnyCpuAssemblyDir = Get-ChildItem -Path $ArtifactsBinDir -Filter "fsiAnyCpu"
    $ProjectSystemAssemblyDirs = Get-ChildItem -Path $ArtifactsBinDir -Filter "ProjectSystem*"
    $FSharpDirs = @($FSharpAssemblyDirs) + @($FscAssemblyDir) + @($FscAnyCpuAssemblyDir) + @($FsiAssemblyDir) + @($FsiAnyCpuAssemblyDir) + @($ProjectSystemAssemblyDirs)
    $FSharpDllPaths = $FSharpDirs | ForEach-Object { Get-ChildItem -Path (Join-Path $ArtifactsBinDir $_) -Recurse -Filter "*.dll" } | ForEach-Object { $_.FullName }
    $FSharpExePaths = $FSharpDirs | ForEach-Object { Get-ChildItem -Path (Join-Path $ArtifactsBinDir $_) -Recurse -Filter "*.exe" } | ForEach-Object { $_.FullName }
    $FSharpAssemblyPaths = @($FSharpDllPaths) + @($FSharpExePaths)

    $InitialAssembliesAndTimes = @{}
    foreach ($asm in $FSharpAssemblyPaths) {
        Write-Host "Assembly : $asm"
        $LastWriteTime = (Get-Item $asm).LastWriteTimeUtc
        $InitialAssembliesAndTimes.Add($asm, $LastWriteTime)
    }

    $InitialCompiledCount = $FSharpAssemblyPaths.Length

    # build again
    & $BuildScript -configuration $configuration @properties
    if ($LASTEXITCODE -ne 0) {
        Write-Host "##[error](NETCORE_ENGINEERING_TELEMETRY=Build) Error running second build."
        exit 1
    }

    # gather assembly timestamps again
    $FinalAssembliesAndTimes = @{}
    foreach ($asm in $FSharpAssemblyPaths) {
        $LastWriteTime = (Get-Item $asm).LastWriteTimeUtc
        $FinalAssembliesAndTimes.Add($asm, $LastWriteTime)
    }

    # validate that assembly timestamps haven't changed
    $RecompiledFiles = @()
    foreach ($asm in $InitialAssembliesAndTimes.keys) {
        $InitialTime = $InitialAssembliesAndTimes[$asm]
        $FinalTime = $FinalAssembliesAndTimes[$asm]
        if ($InitialTime -ne $FinalTime) {
            Write-Host "Rebuilt assembly: $asm"
            $RecompiledFiles += $asm
        }
    }

    $RecompiledCount = $RecompiledFiles.Length
    if ($RecompiledCount -ge 0) {
        Write-Host "##[error](NETCORE_ENGINEERING_TELEMETRY=Test) $RecompiledCount of $InitialCompiledCount assemblies were re-compiled."
        $RecompiledFiles | ForEach-Object { Write-Host "    $_" }
        exit $RecompiledCount
    } else {
        Write-Host "No assemblies rebuilt."
        exit 0
    }
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}

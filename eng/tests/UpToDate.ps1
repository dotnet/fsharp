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
        Write-Host "Error running first build."
        exit 1
    }

    # gather assembly timestamps
    $ArtifactsBinDir = Join-Path $RepoRoot "artifacts" | Join-Path -ChildPath "bin" -Resolve
    $FSharpAssemblyDirs = Get-ChildItem -Path $ArtifactsBinDir -Filter "FSharp.*"
    $FSharpAssemblyPaths = $FSharpAssemblyDirs | ForEach-Object { Get-ChildItem -Path (Join-Path $ArtifactsBinDir $_) -Recurse -Filter "$_.dll" } | ForEach-Object { $_.FullName }

    $InitialAssembliesAndTimes = @{}
    foreach ($asm in $FSharpAssemblyPaths) {
        $LastWriteTime = (Get-Item $asm).LastWriteTimeUtc
        $InitialAssembliesAndTimes.Add($asm, $LastWriteTime)
    }

    $InitialCompiledCount = $FSharpAssemblyPaths.Length

    # build again
    & $BuildScript -configuration $configuration @properties
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error running second build."
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
            $RecompiledFiles += $asm
        }
    }

    $RecompiledCount = $RecompiledFiles.Length
    Write-Host "$RecompiledCount of $InitialCompiledCount assemblies were re-compiled"
    $RecompiledFiles | ForEach-Object { Write-Host "    $_" }
    exit $RecompiledCount
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}

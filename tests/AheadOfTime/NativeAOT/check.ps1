# Publish the test project with NativeAOT and check that it runs.
#
# The point of this check is that the publish succeeds: a string-typed interpolated string
# must lower to a reflection-free form (System.String.Concat), not the reflection-based
# printf engine. If it regresses to printf, FSharp.Reflection becomes statically reachable,
# NativeAOT analysis emits IL2026/IL2070/IL3050, TreatWarningsAsErrors turns them into errors,
# and this publish fails.

$ErrorActionPreference = "Stop"

$root = "NativeAOT_Test"

$cwd = Get-Location
Set-Location $PSScriptRoot

dotnet publish -restore -c release "$root.fsproj" -bl:"$PSScriptRoot/../../../artifacts/log/Release/AheadOfTime/NativeAOT/$root.binlog"
if (-not ($LASTEXITCODE -eq 0)) {
    Set-Location $cwd
    Write-Error "NativeAOT publish failed with exit code $LASTEXITCODE" -ErrorAction Stop
}

# Read the frameworks from the project; passing -f above would override TargetFramework globally.
$props = dotnet msbuild "$root.fsproj" -getProperty:TargetFramework -getProperty:FSharpCoreShippedNetTargetFramework -nologo | ConvertFrom-Json
$tfm = $props.Properties.TargetFramework
$coreAsset = "lib/$($props.Properties.FSharpCoreShippedNetTargetFramework)/FSharp.Core.dll"

# Assert on the selected compile/runtime asset, not the "libraries" manifest, which lists every
# lib folder and would false-pass on the netstandard2.1 fallback.
$assets = Join-Path $PSScriptRoot "obj/project.assets.json"
$assetsJson = Get-Content $assets -Raw | ConvertFrom-Json
$netAssetSelected = $false
foreach ($target in $assetsJson.targets.PSObject.Properties) {
    foreach ($package in $target.Value.PSObject.Properties) {
        if ($package.Name -like "FSharp.Core/*") {
            $compileKeys = if ($package.Value.compile) { @($package.Value.compile.PSObject.Properties.Name) } else { @() }
            $runtimeKeys = if ($package.Value.runtime) { @($package.Value.runtime.PSObject.Properties.Name) } else { @() }
            # FSharp.Core packs only lib/{tfm}, so both must resolve there; anything else is a fallback.
            if (($compileKeys -contains $coreAsset) -and ($runtimeKeys -contains $coreAsset)) {
                $netAssetSelected = $true
            }
        }
    }
}
if (-not $netAssetSelected) {
    Set-Location $cwd
    Write-Error "The $tfm consumer did not resolve the $coreAsset FSharp.Core asset (it fell back to a lower target framework). The shipped net FSharp.Core target framework is not being dogfooded. See $assets." -ErrorAction Stop
}

$exe = Join-Path $PSScriptRoot "bin/release/$tfm/win-x64/publish/$root.exe"
$output = (& $exe) -join "`n"
$exitCode = $LASTEXITCODE
Set-Location $cwd

# The app prints a "FAILED" line per mismatch and "Finished" last, so its output is exactly "Finished" only if all checks passed.
if (-not ($exitCode -eq 0)) {
    Write-Error "NativeAOT app crashed with exit code $exitCode.`nOutput:`n$output" -ErrorAction Stop
}

if ($output.Trim() -ne "Finished") {
    Write-Error "NativeAOT behavior checks failed.`nOutput:`n$output" -ErrorAction Stop
}

Write-Host "NativeAOT test passed."

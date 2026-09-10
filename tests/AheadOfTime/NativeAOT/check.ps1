# Publish the test project with NativeAOT and check that it runs.
#
# The point of this check is that the publish succeeds: a string-typed interpolated string
# must lower to a reflection-free form (System.String.Concat), not the reflection-based
# printf engine. If it regresses to printf, FSharp.Reflection becomes statically reachable,
# NativeAOT analysis emits IL2026/IL2070/IL3050, TreatWarningsAsErrors turns them into errors,
# and this publish fails.

$ErrorActionPreference = "Stop"

$root = "NativeAOT_Test"
$tfm = "net10.0"

$cwd = Get-Location
Set-Location $PSScriptRoot

dotnet publish -restore -c release -f:$tfm "$root.fsproj" -bl:"$PSScriptRoot/../../../artifacts/log/Release/AheadOfTime/NativeAOT/$root.binlog"
if (-not ($LASTEXITCODE -eq 0)) {
    Set-Location $cwd
    Write-Error "NativeAOT publish failed with exit code $LASTEXITCODE" -ErrorAction Stop
}

# Prove this is a genuine net10 dogfood: a net10.0 consumer must resolve the locally-packed
# FSharp.Core's lib/net10.0 asset, not the netstandard2.1 fallback. Assert on the *selected*
# compile/runtime asset under "targets" (the "libraries" manifest lists every lib folder, so a
# plain substring search would false-pass even when ns2.1 was chosen). Without this the test would
# still succeed on the ns2.1 asset and the net10 target framework would go unexercised.
$assets = Join-Path $PSScriptRoot "obj/project.assets.json"
$assetsJson = Get-Content $assets -Raw | ConvertFrom-Json
$net10Selected = $false
foreach ($target in $assetsJson.targets.PSObject.Properties) {
    foreach ($package in $target.Value.PSObject.Properties) {
        if ($package.Name -like "FSharp.Core/*") {
            $compileKeys = if ($package.Value.compile) { @($package.Value.compile.PSObject.Properties.Name) } else { @() }
            $runtimeKeys = if ($package.Value.runtime) { @($package.Value.runtime.PSObject.Properties.Name) } else { @() }
            # FSharp.Core packs only lib/{tfm} (no ref/, no runtimes/), so compile and runtime resolve
            # from the same folder; require both to be net10 - anything else means a fallback was chosen.
            if (($compileKeys -contains "lib/net10.0/FSharp.Core.dll") -and
                ($runtimeKeys -contains "lib/net10.0/FSharp.Core.dll")) {
                $net10Selected = $true
            }
        }
    }
}
if (-not $net10Selected) {
    Set-Location $cwd
    Write-Error "The net10.0 consumer did not resolve the lib/net10.0 FSharp.Core asset (it fell back to a lower target framework). The net10 FSharp.Core target framework is not being dogfooded. See $assets." -ErrorAction Stop
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
    Write-Error "NativeAOT interpolation checks failed.`nOutput:`n$output" -ErrorAction Stop
}

Write-Host "NativeAOT interpolated-string test passed."

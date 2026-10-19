# Publish the test project with NativeAOT and check that it runs.
#
# The point of this check is that the publish succeeds: a string-typed interpolated string
# must lower to a reflection-free form (System.String.Concat), not the reflection-based
# printf engine. If it regresses to printf, FSharp.Reflection becomes statically reachable,
# NativeAOT analysis emits IL2026/IL2070/IL3050, TreatWarningsAsErrors turns them into errors,
# and this publish fails.

$ErrorActionPreference = "Stop"

$root = "NativeAOT_Test"
$tfm = "net9.0"

$cwd = Get-Location
Set-Location $PSScriptRoot

dotnet publish -restore -c release -f:$tfm "$root.fsproj" -bl:"$PSScriptRoot/../../../artifacts/log/Release/AheadOfTime/NativeAOT/$root.binlog"
if (-not ($LASTEXITCODE -eq 0)) {
    Set-Location $cwd
    Write-Error "NativeAOT publish failed with exit code $LASTEXITCODE" -ErrorAction Stop
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

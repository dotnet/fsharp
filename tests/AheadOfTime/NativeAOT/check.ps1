# Publish the test project with NativeAOT and check that it runs.
#
# The point of this check is that the publish succeeds: a string-typed interpolated string
# must lower to a reflection-free form (System.String.Concat), not the reflection-based
# printf engine. If it regresses to printf, FSharp.Reflection becomes statically reachable,
# NativeAOT analysis emits IL2026/IL2070/IL3050, TreatWarningsAsErrors turns them into errors,
# and this publish fails.

$ErrorActionPreference = "Stop"

$root = "NativeAOT_Test"

# net9.0 is the stable control; the shipped net pin (FSharpCoreShippedNetTargetFramework) adds the
# net-TFM leg exercising lib/<pin> under NativeAOT. Derived from the knob so it follows a pin bump.
$netPin = (Select-String -Path "$PSScriptRoot/../../../eng/TargetFrameworks.props" -Pattern 'FSharpCoreShippedNetTargetFramework[^>]*>(net\d+\.0)<').Matches[0].Groups[1].Value
$tfms = @("net9.0", $netPin)

$cwd = Get-Location
Set-Location $PSScriptRoot
try {
    foreach ($tfm in $tfms) {
        Write-Host "NativeAOT publish + run: $tfm"

        dotnet publish -restore -c release -f:$tfm "$root.fsproj" -bl:"$PSScriptRoot/../../../artifacts/log/Release/AheadOfTime/NativeAOT/${root}_${tfm}.binlog"
        if (-not ($LASTEXITCODE -eq 0)) {
            Write-Error "NativeAOT publish failed for $tfm with exit code $LASTEXITCODE" -ErrorAction Stop
        }

        $exe = Join-Path $PSScriptRoot "bin/release/$tfm/win-x64/publish/$root.exe"
        $output = (& $exe) -join "`n"
        $exitCode = $LASTEXITCODE

        # The app prints a "FAILED" line per mismatch and "Finished" last, so its output is exactly "Finished" only if all checks passed.
        if (-not ($exitCode -eq 0)) {
            Write-Error "NativeAOT app crashed for $tfm with exit code $exitCode.`nOutput:`n$output" -ErrorAction Stop
        }

        if ($output.Trim() -ne "Finished") {
            Write-Error "NativeAOT interpolation checks failed for $tfm.`nOutput:`n$output" -ErrorAction Stop
        }

        Write-Host "NativeAOT interpolated-string test passed for $tfm."
    }
}
finally {
    Set-Location $cwd
}

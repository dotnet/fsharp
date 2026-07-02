<#
  Register strong-name verification skipping for the F# public key, so the delay-signed product
  binaries (fsc.exe / fsi.exe / FSharp.Core.dll, public-key-signed with b03f5f7f11d50a3a but not yet
  real-signed) can actually run on a clean machine for the test phase.

  This mirrors what the original 15.9 src\update.cmd did ("adding required strong name verification
  skipping"). Without it, running the built compiler on an agent that enforces strong-name validation
  fails with 0x8013141A (SecurityException: Strong name validation failed). The real signed build applies
  genuine signatures later; this is only for running tests against the CI (non-signed) build output.

  No-op-safe: if sn.exe is missing or registration is already present it does not fail the build.
#>
[CmdletBinding()]
param(
    # F# / Microsoft public key token used to delay-sign the product assemblies.
    [string[]]$PublicKeyTokens = @('b03f5f7f11d50a3a')
)

$ErrorActionPreference = 'Continue'

# Find sn.exe for both bitnesses (the 32-bit and 64-bit strong-name caches are independent).
function Find-Sn([bool]$x64) {
    $roots = @(
        "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows",
        "${env:ProgramFiles}\Microsoft SDKs\Windows"
    ) | Where-Object { $_ -and (Test-Path $_) }
    foreach ($root in $roots) {
        $cands = Get-ChildItem $root -Recurse -Filter 'sn.exe' -ErrorAction SilentlyContinue
        foreach ($c in $cands) {
            $isX64 = $c.FullName -match '\\x64\\'
            if ($isX64 -eq $x64) { return $c.FullName }
        }
    }
    return $null
}

$sn32 = Find-Sn $false
$sn64 = Find-Sn $true
Write-Host "sn.exe (x86): $sn32"
Write-Host "sn.exe (x64): $sn64"

if (-not $sn32 -and -not $sn64) {
    Write-Host "WARNING: sn.exe not found; cannot register strong-name verification skip."
    exit 0
}

foreach ($pkt in $PublicKeyTokens) {
    foreach ($sn in @($sn32, $sn64)) {
        if (-not $sn) { continue }
        Write-Host "Registering SN verification skip: `"$sn`" -Vr *,$pkt"
        & $sn -q -Vr "*,$pkt"
        if ($LASTEXITCODE -ne 0) { Write-Host "  (sn -Vr returned $LASTEXITCODE; continuing)" }
    }
}

Write-Host "Strong-name verification skip registered."
exit 0

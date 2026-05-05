# scripts/compare-insertion-vs-rtm.ps1
# D9 / G5: Compares produced Release\insertion against locally-cached 15.9 RTM insertion archive.
# Reports relative-path diffs and component-ID drift in .vsman.
# Allows: timestamp differences, PDB id differences, signing cert differences, build numbers in versioned package names.

[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [string]$Built,

  [Parameter(Mandatory=$true)]
  [string]$Rtm
)
Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Built)) { throw "Built dir not found: $Built" }
if (-not (Test-Path $Rtm))   { throw "Rtm dir not found:   $Rtm" }

function Get-RelativeFiles($base) {
  $b = (Resolve-Path $base).Path.TrimEnd('\') + '\'
  Get-ChildItem $base -Recurse -File | ForEach-Object {
    # Normalize: relative path, replace build-number-bearing version stamps with '*'
    $rel = $_.FullName.Substring($b.Length)
    # Strip dot-version-build-number patterns like '15.9.0.20260505.1' -> '15.9.0.*.*'
    $rel = $rel -replace '\.(\d{8}\.\d+)', '.*.*'
    $rel
  } | Sort-Object -Unique
}

$builtRel = Get-RelativeFiles $Built
$rtmRel   = Get-RelativeFiles $Rtm

$diffs = Compare-Object $rtmRel $builtRel
$missing = $diffs | Where-Object { $_.SideIndicator -eq '<=' } | ForEach-Object { $_.InputObject }
$extra   = $diffs | Where-Object { $_.SideIndicator -eq '=>' } | ForEach-Object { $_.InputObject }

Write-Host "RTM file count:   $($rtmRel.Count)"
Write-Host "Built file count: $($builtRel.Count)"
Write-Host "Missing in built (present in RTM but absent here): $($missing.Count)"
Write-Host "Extra in built (here but not in RTM):              $($extra.Count)"

if ($missing.Count -gt 0) {
  Write-Host ""
  Write-Host "MISSING:"
  $missing | ForEach-Object { Write-Host "  - $_" }
}
if ($extra.Count -gt 0) {
  Write-Host ""
  Write-Host "EXTRA (review case-by-case):"
  $extra | ForEach-Object { Write-Host "  + $_" }
}

# Component ID drift in .vsman files
$builtVsman = Get-ChildItem $Built -Recurse -Filter '*.vsman'
$rtmVsman   = Get-ChildItem $Rtm   -Recurse -Filter '*.vsman'

if ($builtVsman -and $rtmVsman) {
  $b = (Get-Content $builtVsman[0].FullName -Raw | ConvertFrom-Json)
  $r = (Get-Content $rtmVsman[0].FullName   -Raw | ConvertFrom-Json)

  $bIds = $b.packages | ForEach-Object { $_.id } | Sort-Object -Unique
  $rIds = $r.packages | ForEach-Object { $_.id } | Sort-Object -Unique

  $idDiff = Compare-Object $rIds $bIds
  $idMissing = $idDiff | Where-Object { $_.SideIndicator -eq '<=' } | ForEach-Object { $_.InputObject }
  $idExtra   = $idDiff | Where-Object { $_.SideIndicator -eq '=>' } | ForEach-Object { $_.InputObject }

  Write-Host ""
  Write-Host ".vsman packages: built=$($bIds.Count), rtm=$($rIds.Count)"
  if ($idMissing) { Write-Host "Component IDs missing from built:"; $idMissing | ForEach-Object { Write-Host "  - $_" } }
  if ($idExtra)   { Write-Host "Component IDs extra in built:";    $idExtra   | ForEach-Object { Write-Host "  + $_" } }
}

if ($missing.Count -gt 0 -or ($idMissing -and $idMissing.Count -gt 0)) {
  throw "compare-insertion-vs-rtm.ps1 failed: missing files or component IDs vs RTM (G5 violated)"
}

Write-Host ""
Write-Host "OK: built insertion matches RTM at file-list and component-ID level."

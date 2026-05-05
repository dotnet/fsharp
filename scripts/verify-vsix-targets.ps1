# scripts/verify-vsix-targets.ps1
# D8: Asserts every produced VSIX manifest has the right shape for VS 2017 15.9.
# Used by L4 step 4.3 in plan §7.

[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [string]$InsertionDir
)
Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if (-not (Test-Path $InsertionDir)) {
  throw "InsertionDir not found: $InsertionDir"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

$vsixes = Get-ChildItem $InsertionDir -Recurse -Filter '*.vsix'
if ($vsixes.Count -eq 0) {
  throw "No .vsix files found under $InsertionDir"
}

Write-Host "Checking $($vsixes.Count) VSIX files..."
$failures = @()

foreach ($vsix in $vsixes) {
  $tmp = New-Item -ItemType Directory -Path (Join-Path $env:TEMP "vsix-verify-$([guid]::NewGuid())")
  try {
    [IO.Compression.ZipFile]::ExtractToDirectory($vsix.FullName, $tmp.FullName)
    $manifestPath = Join-Path $tmp.FullName 'extension.vsixmanifest'
    if (-not (Test-Path $manifestPath)) {
      $failures += "$($vsix.Name): missing extension.vsixmanifest"
      continue
    }
    $xml = [xml](Get-Content $manifestPath)
    $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $ns.AddNamespace('v', 'http://schemas.microsoft.com/developer/vsx-schema/2011')

    $installation = $xml.SelectSingleNode('//v:Installation', $ns)
    if (-not $installation) {
      $failures += "$($vsix.Name): no <Installation> element"
      continue
    }

    # Constraint 4 / R17: Experimental MUST be absent or false in production VSIX
    $expAttr = $installation.GetAttribute('Experimental')
    if ($expAttr -and $expAttr -ieq 'true') {
      $failures += "$($vsix.Name): <Installation Experimental=`"true`"> not stripped at finalize (R17 violated)"
    }

    # Every InstallationTarget must target [15.0]
    $targets = $xml.SelectNodes('//v:InstallationTarget', $ns)
    if ($targets.Count -eq 0) {
      $failures += "$($vsix.Name): no <InstallationTarget>"
    }
    foreach ($t in $targets) {
      $ver = $t.GetAttribute('Version')
      if (-not ($ver -match '^\[15\.0' -or $ver -match '^15\.0' -or $ver -eq '[15.0]' -or $ver -eq '[15.0,16.0)')) {
        $failures += "$($vsix.Name): InstallationTarget Version='$ver' is not 15.0-compatible"
      }
    }
  } finally {
    Remove-Item -Recurse -Force $tmp.FullName
  }
}

if ($failures.Count -gt 0) {
  Write-Host "FAILURES ($($failures.Count)):"
  $failures | ForEach-Object { Write-Host "  - $_" }
  throw "verify-vsix-targets.ps1 failed: $($failures.Count) violations"
}

Write-Host "All $($vsixes.Count) VSIXes pass: target=[15.0], no Experimental=true, manifest readable."

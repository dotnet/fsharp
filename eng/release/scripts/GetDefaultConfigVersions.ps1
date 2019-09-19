[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$packagesDir
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

try {
    $packages = @()
    $regex = "^(.*?)\.((?:\.?[0-9]+){3,}(?:[-a-z0-9]+)?)\.nupkg$"
    Get-Item -Path "$packagesDir\*" -Filter "*.nupkg" | ForEach-Object {
        $fileName = Split-Path $_ -Leaf
        If ($fileName -Match $regex) {
            $entry = $Matches[1] + "=" + $Matches[2]
            $packages += $entry
        }
    }

    $final = $packages -Join ","
    Write-Host "Setting InsertConfigValues to $final"
    Write-Host "##vso[task.setvariable variable=InsertConfigValues]$final"
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}

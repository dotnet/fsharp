<#
.SYNOPSIS
    Deploys a VSIX to the Visual Studio experimental instance.
.DESCRIPTION
    Installs a VSIX into the experimental hive, waits for the installer to
    finish, then clears caches and updates the configuration so VS picks up
    the new extension on next launch.
.PARAMETER Path
    Path to the .vsix file to install.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Path)) {
    Write-Error "VSIX not found: $Path"
    exit 1
}

# --- 1. Install the VSIX ---------------------------------------------------
Write-Host "Installing VSIX: $Path" -ForegroundColor Cyan
VSIXInstaller.exe /rootSuffix:exp $Path /q
Write-Host "VSIXInstaller.exe launched (exit code $LASTEXITCODE)."

# --- 2. Wait for the background installer to finish ------------------------
#   VSIXInstaller.exe returns immediately while a background process
#   completes the actual installation. Poll until it exits.
Write-Host "Waiting for VSIXInstaller.exe to finish..." -ForegroundColor Cyan
while (Get-Process -Name VSIXInstaller -ErrorAction SilentlyContinue) {
    Start-Sleep -Seconds 5
}
Write-Host "VSIX installation complete."

# --- 3. Clear caches and update configuration ------------------------------
# Find devenv.exe dynamically — VS folders use numeric versions (e.g. 18),
# not marketing years, and editions vary (IntPreview, Enterprise, etc.).
$devenv = Get-Command devenv.exe -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty Source
if (-not $devenv) {
    $devenv = Get-ChildItem "$env:ProgramFiles\Microsoft Visual Studio","${env:ProgramFiles(x86)}\Microsoft Visual Studio" `
        -Filter "devenv.exe" -Recurse -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -match 'Common7\\IDE\\devenv\.exe$' } |
        Select-Object -First 1 -ExpandProperty FullName
}
if (-not $devenv) {
    Write-Error "devenv.exe not found. Run from a Developer PowerShell or install Visual Studio."
    exit 1
}
Write-Host "Clearing VS experimental instance caches..." -ForegroundColor Cyan
& $devenv /rootsuffix exp /clearcache
& $devenv /rootsuffix exp /updateconfiguration
Write-Host "Caches cleared and configuration updated." -ForegroundColor Green

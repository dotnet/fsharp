#Requires -Version 5
[CmdletBinding()]
param(
    [string] $DotNet = "dotnet"
)

$ErrorActionPreference = "Stop"
$key = [Microsoft.Win32.Registry]::CurrentUser.CreateSubKey("Software\Microsoft\VisualStudio\FSharp")
$valueName = "UseNetSdkCompiler"
$saved = $key.GetValue($valueName, $null, [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames)
if ($null -ne $saved) { $savedKind = $key.GetValueKind($valueName) }

try {
    foreach ($initial in @($null, 0, 1, '%PATH%')) {
        if ($null -eq $initial) {
            $key.DeleteValue($valueName, $false)
        } else {
            $kind = if ($initial -is [int]) { [Microsoft.Win32.RegistryValueKind]::DWord }
                    else { [Microsoft.Win32.RegistryValueKind]::ExpandString }
            $key.SetValue($valueName, $initial, $kind)
        }

        $output = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File `
            "$PSScriptRoot\RegistryCompilerSelection.Tests.ps1" -DotNet $DotNet 2>&1 | Out-String
        if ($LASTEXITCODE -ne 0) { throw "Initial preference '$initial': $output" }

        $actual = $key.GetValue($valueName, $null, [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames)
        if ($actual -cne $initial) { throw "Preference was not restored: expected '$initial', got '$actual'" }
        if ($null -ne $initial -and $key.GetValueKind($valueName) -ne $kind) {
            throw "Preference kind was not restored for '$initial'"
        }
        Write-Host "PASS: Windows PowerShell 5.1, initial preference '$initial'"
    }
} finally {
    try {
        if ($null -eq $saved) { $key.DeleteValue($valueName, $false) }
        else { $key.SetValue($valueName, $saved, $savedKind) }
    } finally { $key.Dispose() }
}

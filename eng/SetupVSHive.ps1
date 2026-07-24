. $PSScriptRoot\common\tools.ps1

$vsInfo = LocateVisualStudio
if ($null -eq $vsInfo) {
  throw "Unable to locate required Visual Studio installation"
}

$vsDir = $vsInfo.installationPath.TrimEnd("\")

$vsRegEdit = Join-Path (Join-Path (Join-Path $vsDir 'Common7') 'IDE') 'VSRegEdit.exe'

$hive = "RoslynDev"
&$vsRegEdit set "$vsDir" $hive HKCU "Roslyn\Internal\OnOff\Features" OOP64Bit dword 0

# Suppress modal dialogs and online-profile prompts that block UI automation (Apex) and cause the
# integration tests to hang until they time out. These mirror the settings dotnet/roslyn applies to its
# RoslynDev hive before running VS integration tests.

# Disable the editor "report exceptions" dialog: fail silently and keep the test running instead of
# popping a modal that stalls automation.
&$vsRegEdit set "$vsDir" $hive HKCU "Text Editor" "Report Exceptions" dword 0

# Disable roaming settings so an online user profile / sign-in prompt can't interfere on CI.
&$vsRegEdit set "$vsDir" $hive HKCU "ApplicationPrivateSettings\Microsoft\VisualStudio" RoamingEnabled string "1*System.Boolean*False"

# Disable background download UI to avoid toasts appearing over the IDE during a run.
&$vsRegEdit set "$vsDir" $hive HKCU "FeatureFlags\Setup\BackgroundDownload" Value dword 0

# Disable targeted (remote settings) notifications. This one can't be set via VsRegEdit, so write it
# directly to the user hive (it is not hive-suffix specific).
reg add hkcu\Software\Microsoft\VisualStudio\RemoteSettings /f /t REG_DWORD /v TurnOffSwitch /d 1 | Out-Null

Write-Host "-- VS Info --"
$isolationIni = Join-Path (Join-Path (Join-Path $vsDir 'Common7') 'IDE') 'devenv.isolation.ini'
Get-Content $isolationIni | Write-Host
Write-Host "-- /VS Info --"

. $PSScriptRoot\common\tools.ps1

$vsInfo = LocateVisualStudio
if ($null -eq $vsInfo) {
  throw "Unable to locate required Visual Studio installation"
}

$vsDir = $vsInfo.installationPath.TrimEnd("\")

$vsRegEdit = Join-Path (Join-Path (Join-Path $vsDir 'Common7') 'IDE') 'VSRegEdit.exe'

$hive = "RoslynDev"
&$vsRegEdit set "$vsDir" $hive HKCU "Roslyn\Internal\OnOff\Features" OOP64Bit dword 0

# VS/UI hardening for headless integration test runs, ported from dotnet/roslyn eng\build.ps1.
# These reduce interference (toasts, dialogs, roaming) that can disrupt the editor and the
# error list / light bulb during code-action tests.

# Disable roaming settings to avoid interference from the online user profile
&$vsRegEdit set "$vsDir" $hive HKCU "ApplicationPrivateSettings\Microsoft\VisualStudio" RoamingEnabled string "1*System.Boolean*False"

# Disable IntelliCode line completions to avoid interference with completion testing
&$vsRegEdit set "$vsDir" $hive HKCU "ApplicationPrivateSettings\Microsoft\VisualStudio\IntelliCode" wholeLineCompletions string "0*System.Int32*2"

# Disable IntelliCode RepositoryAttachedModels since it requires authentication which can fail in CI
&$vsRegEdit set "$vsDir" $hive HKCU "ApplicationPrivateSettings\Microsoft\VisualStudio\IntelliCode" repositoryAttachedModels string "0*System.Int32*2"

# Disable background download UI to avoid toasts
&$vsRegEdit set "$vsDir" $hive HKCU "FeatureFlags\Setup\BackgroundDownload" Value dword 0

# Disable text spell checker to avoid spurious warnings in the error list
&$vsRegEdit set "$vsDir" $hive HKCU "FeatureFlags\Editor\EnableSpellChecker" Value dword 0

# Disable text editor error reporting because it pops up a dialog. We want to fail fast or
# fail silently and continue testing, never block on a modal dialog.
&$vsRegEdit set "$vsDir" $hive HKCU "Text Editor" "Report Exceptions" dword 0

# Disable targeted notifications (does not work via vsregedit, so set the registry directly)
reg add hkcu\Software\Microsoft\VisualStudio\RemoteSettings /f /t REG_DWORD /v TurnOffSwitch /d 1

Write-Host "-- VS Info --"
$isolationIni = Join-Path (Join-Path (Join-Path $vsDir 'Common7') 'IDE') 'devenv.isolation.ini'
Get-Content $isolationIni | Write-Host
Write-Host "-- /VS Info --"

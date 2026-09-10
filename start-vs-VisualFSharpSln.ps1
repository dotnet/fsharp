# Launch VS on VisualFSharp.slnx, building the F# VS extension against the Roslyn your installed VS
# ships (not the newer one flowed into the repo) so F5 loads it. See DEVGUIDE.md. Needs VS Roslyn >= 5.10.
# -RoslynVersion forces a version (e.g. if the exact VS build isn't on a feed); any 5.Y.* binds identically.
[CmdletBinding()]
param(
    [string]$RoslynVersion,
    [string]$DevEnv,
    [string]$Solution = 'VisualFSharp.slnx',
    [switch]$DryRun
)
Set-StrictMode -Version Latest; $ErrorActionPreference = 'Stop'; $root = $PSScriptRoot

if (-not $DevEnv) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $DevEnv = if ($env:DevEnvDir) { Join-Path $env:DevEnvDir 'devenv.exe' }
              elseif (Test-Path $vswhere) { & $vswhere -latest -prerelease -property productPath 2>$null }
}
if (-not ($DevEnv -and (Test-Path $DevEnv))) { throw 'devenv.exe not found; run from a VS Developer prompt or pass -DevEnv.' }

if (-not $RoslynVersion) {
    $dll = "$(Split-Path $DevEnv)\CommonExtensions\Microsoft\VBCSharp\LanguageServices\Microsoft.CodeAnalysis.dll"
    if (-not (Test-Path $dll)) { throw "Can't detect your VS Roslyn version; pass -RoslynVersion." }
    $RoslynVersion = ([System.Diagnostics.FileVersionInfo]::GetVersionInfo($dll).ProductVersion -split '\+')[0]
}
$minor = [version](($RoslynVersion -split '-')[0])
if ($minor -lt [version]'5.10.0') {
    throw "Your VS ships Roslyn $RoslynVersion but the repo references packages from >= 5.10 (unified ExternalAccess, #20099). Update VS or deploy a local Roslyn."
}
Write-Host "Building the F# extension against Roslyn $RoslynVersion ($DevEnv)."

# Repoint every Roslyn package (versions set in eng/Version.Details.props) via a props file MSBuild
# imports after it through the CustomAfterMicrosoftCommonProps hook the launched VS inherits.
$names = 'MicrosoftCodeAnalysis', 'MicrosoftCodeAnalysisCompilers', 'MicrosoftCodeAnalysisCSharp',
    'MicrosoftCodeAnalysisEditorFeatures', 'MicrosoftCodeAnalysisEditorFeaturesText', 'MicrosoftCodeAnalysisFeatures',
    'MicrosoftVisualStudioLanguageServices', 'MicrosoftVisualStudioLanguageServicesExternalAccess'
$override = Join-Path $root 'artifacts\RoslynOverride.props'
New-Item -ItemType Directory -Force (Split-Path $override) | Out-Null
"<Project><PropertyGroup>$(-join ($names | ForEach-Object { "<${_}Version>$RoslynVersion</${_}Version>" }))</PropertyGroup></Project>" |
    Set-Content -LiteralPath $override -Encoding UTF8

# Apply to THIS process only, restoring in finally so it can't leak into a later build.cmd/CI run
# (which must keep the flowed Roslyn); the launched VS snapshots the env for its F5/restore builds.
$vars = @{
    CustomAfterMicrosoftCommonProps = $override
    DOTNET_ROOT                     = Join-Path $root '.dotnet'
    'DOTNET_ROOT(x86)'              = Join-Path $root '.dotnet\x86'
    PATH                            = "$(Join-Path $root '.dotnet');$env:PATH"
    RunNetFrameworkApiCompat        = 'false'
    RunRefApiCompat                 = 'false'
}
$saved = @{}; foreach ($k in $vars.Keys) { $saved[$k] = [Environment]::GetEnvironmentVariable($k) }
try {
    foreach ($k in $vars.Keys) { Set-Item -LiteralPath "Env:\$k" -Value $vars[$k] }
    if ($DryRun) { Write-Host "DryRun: $override"; return }
    & (Join-Path $root 'Restore.cmd')
    if ($LASTEXITCODE) { throw "Restore failed for Roslyn $RoslynVersion; try another 5.$($minor.Minor).* build via -RoslynVersion." }
    Start-Process $DevEnv "`"$(Join-Path $root $Solution)`""
    Write-Host 'Launched VS. Set VisualFSharpDebug as the startup project, then F5 / Ctrl+F5.'
}
finally {
    foreach ($k in $saved.Keys) {
        if ($null -eq $saved[$k]) { Remove-Item -LiteralPath "Env:\$k" -ErrorAction SilentlyContinue } else { Set-Item -LiteralPath "Env:\$k" -Value $saved[$k] }
    }
}

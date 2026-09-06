# Launch VS on VisualFSharp.slnx so that F5 builds the F# VS extension against the Roslyn it will
# actually run against, and deploys it to $RootSuffix. See DEVGUIDE.md.
#
# What F5 runs against is the hive's Roslyn, not the installed VS's: deploying a locally built Roslyn
# into the hive stamps it 42.42.42.42 and redirects every reference to itself, so the hive wins. Only
# when the hive has no Roslyn of its own does the installation's version apply. This picks whichever
# of the two is in force and matches the package versions to it - or leaves the repo's flowed versions
# alone when they already match, which is the common case for a hive built from this same source.
#
# -RoslynVersion forces a package version; any 5.Y.* binds identically within a minor.
[CmdletBinding()]
param(
    [string]$RoslynVersion,
    [string]$DevEnv,
    [string]$RootSuffix = 'RoslynDev',
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

$ideDir = Split-Path $DevEnv
$flowed = ([xml](Get-Content -LiteralPath (Join-Path $root 'eng\Version.Details.props') -Raw)
    ).GetElementsByTagName('MicrosoftCodeAnalysisPackageVersion')[0].InnerText.Trim()

if (-not $RoslynVersion) {
    $ini = Get-Content -LiteralPath (Join-Path $ideDir 'devenv.isolation.ini') -Raw
    if ($ini -notmatch '(?m)^InstallationID=(?<id>\S+)') { throw 'No InstallationID in devenv.isolation.ini.' }
    $installationId = $Matches.id
    if ($ini -notmatch '(?m)^InstallationVersion=(?<v>\d+)') { throw 'No InstallationVersion in devenv.isolation.ini.' }
    $hive = Join-Path $env:LOCALAPPDATA ('Microsoft\VisualStudio\{0}.0_{1}{2}' -f $Matches.v, $installationId, $RootSuffix)

    $hiveRoslyn = Get-ChildItem (Join-Path $hive 'Extensions') -Recurse -Filter 'Microsoft.CodeAnalysis.dll' -ErrorAction SilentlyContinue |
        Where-Object { $_.DirectoryName -like '*Roslyn Language Services*' } | Select-Object -First 1

    if ($hiveRoslyn) {
        $target = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($hiveRoslyn.FullName).ProductVersion
        $source = "deployed in $RootSuffix"
    }
    else {
        $dll = Join-Path $ideDir 'CommonExtensions\Microsoft\VBCSharp\LanguageServices\Microsoft.CodeAnalysis.dll'
        if (-not (Test-Path $dll)) { throw "Can't detect the Roslyn version for $RootSuffix; pass -RoslynVersion." }
        $target = ([System.Diagnostics.FileVersionInfo]::GetVersionInfo($dll).ProductVersion -split '\+')[0]
        $source = 'shipped with the installed VS'
    }
    Write-Host "Roslyn in force: $target ($source)."

    # A dev build's "5.12.0-dev" is no package version, so only its minor is usable - and when that is
    # the minor the repo already flows, the flowed packages are the match and no override belongs here.
    $targetMinor = [version](($target -split '-')[0])
    $flowedMinor = [version](($flowed -split '-')[0])
    if ($targetMinor.Major -eq $flowedMinor.Major -and $targetMinor.Minor -eq $flowedMinor.Minor) {
        Write-Host "Repo already flows Roslyn $flowed - building against it, no override."
    }
    elseif ($target -match '-dev$') {
        throw "$RootSuffix has a locally built Roslyn $target but the repo flows $flowed; pass -RoslynVersion with a $($targetMinor.Major).$($targetMinor.Minor).* package version to build against that minor."
    }
    else { $RoslynVersion = $target }
}

if ($RoslynVersion) {
    $minor = [version](($RoslynVersion -split '-')[0])
    if ($minor -lt [version]'5.10.0') {
        throw "Roslyn $RoslynVersion is older than the 5.10 the repo's sources expect (unified ExternalAccess, #20099). Update VS or deploy a local Roslyn."
    }
    Write-Host "Building the F# extension against Roslyn $RoslynVersion ($DevEnv)."
}

# Repoint every Roslyn package (versions set in eng/Version.Details.props) via a props file MSBuild
# imports after it through the CustomAfterMicrosoftCommonProps hook the launched VS inherits.
$names = 'MicrosoftCodeAnalysis', 'MicrosoftCodeAnalysisCompilers', 'MicrosoftCodeAnalysisCSharp',
    'MicrosoftCodeAnalysisEditorFeatures', 'MicrosoftCodeAnalysisEditorFeaturesText', 'MicrosoftCodeAnalysisFeatures',
    'MicrosoftVisualStudioLanguageServices', 'MicrosoftVisualStudioLanguageServicesExternalAccess'
# Its own file: build-vs-VisualFSharpSln.ps1 writes an override too, and one path for both means
# whichever ran last decides what a long-lived VS session restores against.
$override = Join-Path $root 'artifacts\RoslynOverride.start-vs.props'
if ($RoslynVersion) {
    New-Item -ItemType Directory -Force (Split-Path $override) | Out-Null
    "<Project><PropertyGroup>$(-join ($names | ForEach-Object { "<${_}Version>$RoslynVersion</${_}Version>" }))</PropertyGroup></Project>" |
        Set-Content -LiteralPath $override -Encoding UTF8
}

# Apply to THIS process only, restoring in finally so it can't leak into a later build.cmd/CI run
# (which must keep the flowed Roslyn); the launched VS snapshots the env for its F5/restore builds.
$vars = @{
    DOTNET_ROOT                     = Join-Path $root '.dotnet'
    'DOTNET_ROOT(x86)'              = Join-Path $root '.dotnet\x86'
    PATH                            = "$(Join-Path $root '.dotnet');$env:PATH"
    RunNetFrameworkApiCompat        = 'false'
    RunRefApiCompat                 = 'false'
}
if ($RoslynVersion) { $vars.CustomAfterMicrosoftCommonProps = $override }
$saved = @{}; foreach ($k in $vars.Keys) { $saved[$k] = [Environment]::GetEnvironmentVariable($k) }
try {
    foreach ($k in $vars.Keys) { Set-Item -LiteralPath "Env:\$k" -Value $vars[$k] }
    if ($DryRun) {
        if ($RoslynVersion) { Write-Host "DryRun: wrote $override" } else { Write-Host 'DryRun: no override needed' }
        Write-Host "Would restore, then open $Solution in $DevEnv"
        return
    }
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

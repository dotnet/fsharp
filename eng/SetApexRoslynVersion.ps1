# Detects the Roslyn version of the *installed* Visual Studio and exports it as the pipeline variable
# FSHARP_APEX_ROSLYN_VERSION so the Apex integration-test leg builds the F# VSIX against it.
#
# Why: the repo pins Roslyn forward (to the VS being inserted into), which is newer than any VS on the CI
# scout image. A VSIX built against that pin cannot bind Microsoft.VisualStudio.LanguageServices at
# runtime in the older CI VS, so FSharpPackage/FSharpProjectPackage fail to load and every Apex test
# fails. The F# editor references Roslyn compile-only (ExcludeAssets=runtime), so building against the
# VS's own Roslyn version is sufficient and ships nothing extra. The eng/Versions.props override consumes
# this variable; the committed pin is unchanged. If detection fails we leave the variable unset (the
# build falls back to the committed pin) rather than break the build.
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

function Write-Skip([string] $message) {
    Write-Host "##vso[task.logissue type=warning]$message"
    Write-Host "Apex Roslyn override not set; the VSIX will build against the repo's pinned Roslyn."
}

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) {
    Write-Skip "vswhere.exe not found; cannot detect the installed VS Roslyn version."
    return
}

# Select the same install the Apex gate runs against: the lowest matching (default major '18') VS that
# actually has devenv.exe. On CI there is a single VS, so any selection resolves to it.
$prefix = if (${env:FSHARP_APEX_VS_VERSION}) { ${env:FSHARP_APEX_VS_VERSION} } else { '18' }
$versions = @(& $vswhere -all -prerelease -products * -property installationVersion)
$paths    = @(& $vswhere -all -prerelease -products * -property installationPath)

$candidates = for ($i = 0; $i -lt $versions.Count; $i++) {
    if ($versions[$i] -like "$prefix.*") {
        [PSCustomObject]@{ Version = $versions[$i]; Path = $paths[$i] }
    }
}
$candidates = @($candidates | Sort-Object { [version]$_.Version })

$vsDir = $null
foreach ($c in $candidates) {
    if (Test-Path (Join-Path $c.Path 'Common7\IDE\devenv.exe')) { $vsDir = $c.Path.TrimEnd('\'); break }
}
if (-not $vsDir) {
    Write-Skip "No installed VS $prefix.x with devenv.exe found; cannot detect the Roslyn version."
    return
}

$lsDll = Join-Path $vsDir 'Common7\IDE\CommonExtensions\Microsoft\VBCSharp\LanguageServices\Microsoft.VisualStudio.LanguageServices.dll'
if (-not (Test-Path $lsDll)) {
    $lsDll = Get-ChildItem -Path $vsDir -Recurse -Filter 'Microsoft.VisualStudio.LanguageServices.dll' -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
}
if (-not $lsDll -or -not (Test-Path $lsDll)) {
    Write-Skip "Microsoft.VisualStudio.LanguageServices.dll not found under '$vsDir'."
    return
}

$asmVersion = [System.Reflection.AssemblyName]::GetAssemblyName($lsDll).Version
$majorMinor = "$($asmVersion.Major).$($asmVersion.Minor)"
Write-Host "Installed VS '$vsDir' ships Roslyn assembly $asmVersion (major.minor $majorMinor)."

# Resolve a matching Microsoft.CodeAnalysis NuGet version. The whole Roslyn family is published together
# with the same version, and every '<major>.<minor>.0-*' build has assembly version <major>.<minor>.0.0,
# so any such package binds to the installed VS at runtime; pick the newest for freshness.
$feed = 'https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3'
try {
    $allVersions = (Invoke-RestMethod -Uri "$feed/flat2/microsoft.codeanalysis.externalaccess.fsharp/index.json" -UseBasicParsing).versions
} catch {
    Write-Skip "Failed to query the dotnet-tools feed for Roslyn versions: $($_.Exception.Message)"
    return
}

$selected = @($allVersions | Where-Object { $_ -match "^$([regex]::Escape($majorMinor))\.0-" }) |
    Sort-Object | Select-Object -Last 1
if (-not $selected) {
    Write-Skip "No Roslyn package '$majorMinor.0-*' found on the feed to match the installed VS."
    return
}

Write-Host "Selected Roslyn NuGet version '$selected' for the Apex VSIX build (matches VS Roslyn $majorMinor)."
Write-Host "##vso[task.setvariable variable=FSHARP_APEX_ROSLYN_VERSION]$selected"

#Requires -Version 5
<#
  Verifies src/FSharp.Build/Microsoft.FSharp.Targets selects the F# compiler correctly based on the
  HKCU\Software\Microsoft\VisualStudio\FSharp\UseNetSdkCompiler value and the FSharp_Shim_Present gate.
  Windows-only (registry + VS shim). Evaluates the SOURCE targets via `dotnet msbuild -getProperty`
  from a temp directory OUTSIDE the repo so the repo's global.json (which pins an SDK that may not be
  installed) does not apply.
#>
[CmdletBinding()]
param(
    [string] $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path,
    [string] $DotNet = "dotnet"
)

$ErrorActionPreference = "Stop"

$shim    = Join-Path $RepoRoot "vsintegration\shims\Microsoft.FSharp.ShimHelpers.props"
$targets = Join-Path $RepoRoot "src\FSharp.Build\Microsoft.FSharp.Targets"
if (-not (Test-Path $shim))    { throw "Shim not found: $shim" }
if (-not (Test-Path $targets)) { throw "Targets not found: $targets" }

$work = Join-Path ([System.IO.Path]::GetTempPath()) ("fsc_regsel_" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Force -Path $work | Out-Null
$probe = Join-Path $work "probe.proj"
@"
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Language>F#</Language>
    <Configuration>Debug</Configuration>
    <FSharpCompilerPath></FSharpCompilerPath>
  </PropertyGroup>
  <Import Project="$shim" />
  <Import Project="$targets" />
</Project>
"@ | Set-Content -Path $probe -Encoding UTF8

$regKey = "HKCU\Software\Microsoft\VisualStudio\FSharp"
$regVal = "UseNetSdkCompiler"

# Save & later restore any pre-existing value so we never corrupt a developer's setting.
$saved = (reg query $regKey /v $regVal 2>$null | Select-String $regVal)

function Clear-Reg { reg delete $regKey /v $regVal /f 2>$null | Out-Null }
function Set-Reg([int]$v) { reg add $regKey /v $regVal /t REG_DWORD /d $v /f | Out-Null }

function Get-Props([string[]]$extra) {
    Push-Location $work
    try {
        $args = @("msbuild", "probe.proj",
                  "-getProperty:MSBuildToolsPath",
                  "-getProperty:FSharp_Shim_Present",
                  "-getProperty:FSharpPreferNetFrameworkTools",
                  "-getProperty:FscToolPath",
                  "-getProperty:FscToolExe",
                  "-getProperty:DotnetFscCompilerPath") + $extra
        $json = & $DotNet @args 2>&1 | Out-String
        if ($LASTEXITCODE -ne 0) { throw "MSBuild probe failed: $json" }
        return ($json | ConvertFrom-Json).Properties
    } finally { Pop-Location }
}

$failures = New-Object System.Collections.Generic.List[string]
function Check($name, $cond, $detail) {
    if ($cond) { Write-Host "PASS: $name" -ForegroundColor Green }
    else { Write-Host "FAIL: $name -- $detail" -ForegroundColor Red; $failures.Add($name) }
}

try {
    $sdk = Get-Props @()
    $sdkRoot = (Split-Path (Split-Path $sdk.MSBuildToolsPath -Parent) -Parent) + '\'
    $sdkVersion = Split-Path $sdk.MSBuildToolsPath -Leaf
    $sdkCases = @(
        @{ Name = 'resolved SDK'; Root = $sdkRoot; Version = $sdkVersion },
        @{ Name = 'missing root'; Root = ''; Version = $sdkVersion },
        @{ Name = 'missing version'; Root = $sdkRoot; Version = '' },
        @{ Name = 'non-SDK project'; Root = ''; Version = '' }
    )
    foreach ($registry in @($null, 0, 1)) {
        if ($null -eq $registry) { Clear-Reg } else { Set-Reg $registry }
        foreach ($sdkCase in $sdkCases) {
            foreach ($explicit in @('', 'true', 'false')) {
                $properties = @("-p:NetCoreRoot=$($sdkCase.Root)", "-p:NETCoreSdkVersion=$($sdkCase.Version)")
                if ($explicit -ne '') { $properties += "-p:FSharpPreferNetFrameworkTools=$explicit" }
                $p = Get-Props $properties
                $desktop = $explicit -eq 'true' -or
                    ($explicit -eq '' -and ($registry -eq 0 -or $sdkCase.Root -eq '' -or $sdkCase.Version -eq ''))
                if ($desktop) {
                    $correct = $p.FSharpPreferNetFrameworkTools -eq 'true' -and
                        $p.FscToolExe -match '^fsc.*\.exe$' -and
                        $p.FscToolPath -like '*/Common7/IDE/CommonExtensions/Microsoft/FSharp/Tools/' -and
                        $p.DotnetFscCompilerPath -eq ''
                } else {
                    $correct = $p.FSharpPreferNetFrameworkTools -eq 'false' -and
                        $p.FscToolExe -eq 'dotnet.exe' -and $p.FscToolPath -eq $sdkCase.Root -and
                        $p.DotnetFscCompilerPath -eq "`"$($sdkCase.Root)sdk/$($sdkCase.Version)/FSharp/fsc.dll`""
                }
                Check "$($sdkCase.Name), registry='$registry', explicit='$explicit'" `
                    ($p.FSharp_Shim_Present -eq 'true' -and $correct) `
                    ("got: $($p | ConvertTo-Json -Compress)")
            }
        }
    }

    # Case 5 (NEGATIVE): CLI build (shim absent) -> selection blocks skipped, unaffected
    Set-Reg 0
    $p = Get-Props @("-p:FSharpCompilerPath=C:\Explicit\")
    Check "Case5 CLI (shim absent) -> unaffected (no fsc selection)" `
        ($p.FSharp_Shim_Present -eq '' -and $p.FscToolExe -eq '' -and $p.DotnetFscCompilerPath -eq '') `
        ("got: $($p | ConvertTo-Json -Compress)")
}
finally {
    # restore registry
    if ($saved) {
        $v = ($saved -split '\s+')[-1]
        if ($v -match '^0x') { $v = [Convert]::ToInt32($v,16) }
        Set-Reg ([int]$v)
    } else { Clear-Reg }
    Remove-Item -Recurse -Force $work -ErrorAction SilentlyContinue
}

if ($failures.Count -gt 0) { Write-Error "Registry compiler-selection matrix FAILED: $($failures -join ', ')"; exit 1 }
Write-Host "All registry compiler-selection cases passed." -ForegroundColor Green

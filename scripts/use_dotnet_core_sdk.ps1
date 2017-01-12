[cmdletbinding()]
param(
   [string] $Version = "",
   [string] $OutPath = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop"
$ProgressPreference="SilentlyContinue"

$rootDir = Split-Path -Parent $PSScriptRoot

if ($Version -eq "") { $Version = Get-Content .\DotnetCoreSdkVersion.txt }
echo "Using .NET Core Sdk version $Version"

$global:OutFilePath = $OutPath
$global:Version = $Version

function Check-Dotnet
{
    $dotnet = $args[0]
    $dotnetDir = Split-Path -Parent $dotnet
    try { $dotnetVersion = iex '& "$dotnet" --version' -ErrorAction SilentlyContinue } catch { }
    if (-not $?) { $dotnetVersion = "" }
    if ($dotnetVersion -eq $global:Version) {
        echo "Found the .NET Core Sdk $dotnetVersion in $($args[1])"
        if ($global:OutFilePath -eq "") { #OutFilePath not set, add directly dir to PATH
            if ($dotnetDir -ne "") { #is already in PATH, no need
                $env:Path = "$dotnetDir;$env:Path"
            }
        } else { # write dir to OutFilePath
            [System.IO.File]::WriteAllText("$global:OutFilePath", "$dotnetDir")
        }
        exit 0
    } else {
        echo ".NET Core Sdk not found in $($args[1]), was '$dotnetVersion'"
    }
}

# check in PATH
Check-Dotnet "dotnet" "PATH"

# check in .dotnetsdk dir
md -Force -Path "$rootDir\.dotnetsdk" | Out-Null

$installDir = "$rootDir\.dotnetsdk" | Convert-Path

Check-Dotnet "$installDir\dotnet" "$installDir"

# not found, install it
$sdkBranch = ($Version.Split('-') | Select -First 2) -Join '-'
$installScriptPath = "$rootDir\scripts\dotnet-install.$sdkBranch.ps1"

Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/$sdkBranch/scripts/obtain/dotnet-install.ps1" -OutFile "$installScriptPath"

iex '& $installScriptPath -InstallDir "$installDir" -Channel "preview" -version "$Version"'

Check-Dotnet "$installDir\dotnet" "$installDir"

echo "Error, the .NET Core Sdk $dotnetVersion was installed but not found in '$installDir' directory"
exit 1

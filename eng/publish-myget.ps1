[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$artifactsDir,
    [string]$apiKey,
    [string]$configuration
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

$shippingDir = "$artifactsDir\packages\$configuration\Shipping"
$packages = @(
    "$shippingDir\FSharp.Compiler.Scripting.*.nupkg",
    "$shippingDir\FSharp.Core.*.nupkg"
)

. (Join-Path $PSScriptRoot "publish-myget.ps1") -apiKey "$apiKey" -feedUrl "https://dotnet.myget.org/F/fsharp/api/v2/package" $packages

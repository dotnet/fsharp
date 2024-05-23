Write-Host "AheadOfTime: check1.ps1"

$savedNUGET_PACKAGES=$env:NUGET_PACKAGES

$env:NUGET_PACKAGES=Join-Path $PSScriptRoot "../../artifacts/nuget/AOT/"
dotnet nuget locals global-packages --clear

Equality\check.ps1
Trimming\check.ps1
$env:NUGET_PACKAGES=$savedNUGET_PACKAGES

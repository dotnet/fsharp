Write-Host "AheadOfTime: check1.ps1"

# the NUGET_PACKAGES environment variable tells dotnet nuget where the global package is
# So save the current setting, we'll reset it after the tests are complete
# Then clear the global cache so that we can grab the FSharp.Core nuget we built earlier
$savedNUGET_PACKAGES=$env:NUGET_PACKAGES
$env:NUGET_PACKAGES=Join-Path $PSScriptRoot "../../artifacts/nuget/AOT/"
dotnet nuget locals global-packages --clear

Equality\check.ps1
Trimming\check.ps1
$env:NUGET_PACKAGES=$savedNUGET_PACKAGES

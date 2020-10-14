Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

try {
    $fakeBuildId = (Get-Date -Format "yyyyMMdd") + ".0"
    $visualStudioDropName = "Products/mock/dotnet-fsharp/branch/$fakeBuildId"
    & "$PSScriptRoot\Build.ps1" -build -restore -ci -bootstrap -binaryLog -pack -configuration Release /p:OfficialBuildId=$fakeBuildId /p:VisualStudioDropName=$visualStudioDropName
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    Write-Host "##[error](NETCORE_ENGINEERING_TELEMETRY=Build) Error doing mock official build."
    exit 1
}

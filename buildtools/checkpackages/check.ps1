# ENTRY POINT MAIN()
Param(
    [Parameter(Mandatory=$True)]
    [String] $project
)
& dotnet restore $project 2>$null

if ($LASTEXITCODE -eq 0)
{
    $package = Get-Content -Path .\Version.txt
    Write-Error "
        Package restore succeded for '${package}', expected to fail.
        This usually means that the package has been already published.
        Please, bump the version to fix this failure." -ErrorAction Stop
}

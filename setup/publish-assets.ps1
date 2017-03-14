<#
.SYNOPSIS
Publishes the VSIX package on MyGet.

.PARAMETER binariesPath
The root directory where the build outputs are written.

.PARAMETER branchName
The name of the branch that is being built.

.PARAMETER apiKey
The API key used to authenticate with MyGet.

#>

Param(
    [string]$binariesPath = $null,
    [string]$branchName = $null,
    [string]$apiKey = $null
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

try {
    $branchName = $branchName -Replace "refs/heads/" # get rid of prefix

    switch ($branchName) {
        "microbuild" { }
        default {
            Write-Host "Branch [$branchName] is not supported for publishing."
            exit 0 # non-fatal
        }
    }

    $branchName = $branchName.Replace("/", "_") # can't have slashes in the branch name
    $requestUrl = "https://dotnet.myget.org/F/fsharp/vsix/upload"
    $vsix = Join-Path $binariesPath "net40\bin\VisualFSharpFull.vsix"

    Write-Host "  Uploading '$vsix' to '$requestUrl'."

    $response = Invoke-WebRequest -Uri $requestUrl -Headers @{"X-NuGet-ApiKey"=$apiKey} -ContentType "multipart/form-data" -InFile $vsix -Method Post -UseBasicParsing
    if ($response.StatusCode -ne 201) {
        Write-Error "Failed to upload VSIX: $vsix.  Upload failed with status cude: $response.StatusCode."
        exit 1
    }
}
catch [exception] {
    Write-Host $_.Exception
    exit -1
}

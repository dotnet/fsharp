[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$apiKey,
    [string]$feedUrl,

    [parameter(ValueFromRemainingArguments=$true)][string[]]$packages
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

$failedPackages = 0
foreach ($package in $packages) {
    $response = Invoke-WebRequest -Uri $feedUrl -Headers @{"X-NuGet-ApiKey"=$apiKey} -ContentType "multipart/form-data" -InFile "$package" -Method Post -UseBasicParsing
    if ($response.StatusCode -ne 201) {
        Write-Error "Failed to upload package.  Upload failed with status code: $response.StatusCode."
        $failedPackages++
    }
}

exit $failedPackages

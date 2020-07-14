[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$accessToken,
    [string]$buildId,
    [string]$insertionDir
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

function Invoke-WebRequestWithAccessToken([string] $uri, [string] $accessToken, [int] $retryCount = 5) {
    Write-Host "Fetching content from $uri"
    $base64 = [Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes(":$accessToken"))
    $headers = @{
        Authorization = "Basic $base64"
    }

    for ($i = 0; $i -lt $retryCount; $i++) {
        try {
            return Invoke-WebRequest -Method Get -Uri $uri -Headers $headers -UseBasicParsing
        }
        catch {
            Write-Host "Invoke-WebRequest failed: $_"
            Start-Sleep -Seconds 1
        }
    }

    throw "Unable to fetch $uri after $retryCount tries."
}

try {
    # build map of all *.vsman files to their `info.buildVersion` values
    $manifestVersionMap = @{}
    Get-ChildItem -Path "$insertionDir\*" -Filter "*.vsman" | ForEach-Object {
        $manifestName = Split-Path $_ -Leaf
        $vsmanContents = Get-Content $_ | ConvertFrom-Json
        $buildVersion = $vsmanContents.info.buildVersion
        $manifestVersionMap.Add($manifestName, $buildVersion)
    }

    # find all publish URLs
    $manifests = @()
    $seenManifests = @{}
    $json = Invoke-WebRequestWithAccessToken -uri "https://dev.azure.com/dnceng/internal/_apis/build/builds/$buildId/logs?api-version=5.1" -accessToken $accessToken | ConvertFrom-Json
    foreach ($l in $json.value) {
        $logUrl = $l.url
        $log = (Invoke-WebRequestWithAccessToken -uri $logUrl -accessToken $accessToken).Content
        If ($log -Match "(https://vsdrop\.corp\.microsoft\.com/[^\r\n;]+);([^\r\n]+)\r?\n") {
            $manifestShortUrl = $Matches[1]
            $manifestName = $Matches[2]
            $manifestUrl = "$manifestShortUrl;$manifestName"
            If (-Not $seenManifests.Contains($manifestUrl)) {
                $seenManifests.Add($manifestUrl, $true)
                $buildVersion = $manifestVersionMap[$manifestName]
                $manifestEntry = "$manifestName{$buildVersion}=$manifestUrl"
                $manifests += $manifestEntry
            }
        }
    }

    $final = $manifests -Join ","
    Write-Host "Setting InsertJsonValues to $final"
    Write-Host "##vso[task.setvariable variable=InsertJsonValues]$final"
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}

[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$accessToken,
    [string]$buildId,
    [string]$insertionDir
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

$dropUrlRegex = "(https://vsdrop\.corp\.microsoft\.com/[^\r\n;]+);([^\r\n]+)\r?\n"

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

# this function has to download ~500 individual logs and check each one; prone to timeouts
function Get-ManifestsViaIndividualLogs([PSObject] $manifestVersionMap, [string] $buildId, [string] $accessToken) {
    $manifests = @()
    $seenManifests = @{}
    $json = Invoke-WebRequestWithAccessToken -uri "https://dev.azure.com/dnceng/internal/_apis/build/builds/$buildId/logs?api-version=5.1" -accessToken $accessToken | ConvertFrom-Json
    foreach ($l in $json.value) {
        $logUrl = $l.url
        $log = (Invoke-WebRequestWithAccessToken -uri $logUrl -accessToken $accessToken).Content
        If ($log -Match $dropUrlRegex) {
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

    return $manifests
}

# this function only has to download 1 file and look at a very specific file
function Get-ManifestsViaZipLog([PSObject] $manifestVersionMap, [string] $buildId, [string] $accessToken) {
    # create temporary location
    $guid = [System.Guid]::NewGuid().ToString()
    $tempDir = Join-Path ([System.IO.Path]::GetTempPath()) $guid
    New-Item -ItemType Directory -Path $tempDir | Out-Null

    # download the logs
    $base64 = [Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes(":$accessToken"))
    $headers = @{
        Authorization = "Basic $base64"
    }
    $uri = "https://dev.azure.com/dnceng/internal/_apis/build/builds/$buildId/logs?`$format=zip"
    Invoke-WebRequest -Uri $uri -Method Get -Headers $headers -UseBasicParsing -OutFile "$tempDir/logs.zip"

    # expand the logs
    New-Item -ItemType Directory -Path "$tempDir/logs" | Out-Null
    Expand-Archive -Path "$tempDir/logs.zip" -DestinationPath "$tempDir/logs"

    # parse specific logs
    $logDir = "$tempDir/logs"
    $manifests = @()
    $seenManifests = @{}
    Get-ChildItem $logDir -r -inc "*Upload VSTS Drop*" | ForEach-Object {
        $result = Select-String -Path $_ -Pattern "(https://vsdrop\.corp\.microsoft\.com[^;]+);(.*)" -AllMatches
        $result.Matches | ForEach-Object {
            $manifestShortUrl = $_.Groups[1].Value
            $manifestName = $_.Groups[2].Value
            $manifestUrl = "$manifestShortUrl;$manifestName"
            If (-Not $seenManifests.Contains($manifestUrl)) {
                $seenManifests.Add($manifestUrl, $true)
                $buildVersion = $manifestVersionMap[$manifestName]
                $manifestEntry = "$manifestName{$buildVersion}=$manifestUrl"
                $manifests += $manifestEntry
            }
        }
    }

    Remove-Item -Path $tempDir -Recurse

    return $manifests
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
    #$manifests = Get-ManifestsViaIndividualLogs -manifestVersionMap $manifestVersionMap -buildId $buildId -accessToken $accessToken
    $manifests = Get-ManifestsViaZipLog -manifestVersionMap $manifestVersionMap -buildId $buildId -accessToken $accessToken

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

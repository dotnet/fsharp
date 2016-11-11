Param (
    [Parameter(Mandatory = $true)]
    [string] $path,
    [string] $format = "nunit"
)
# Upload results to AppVeyor

Get-Item Env:APPVEYOR_* | Write-Verbose
Write-Verbose ""

# See <http://www.appveyor.com/docs/running-tests>
# and <http://www.appveyor.com/docs/environment-variables>
$url = "$env:APPVEYOR_URL/api/testresults/$format/$($env:APPVEYOR_JOB_ID)"
Write-Output "Uploading results $path to $url"

$wc = New-Object 'System.Net.WebClient'
$wc.UploadFile($url, (Resolve-Path $path))

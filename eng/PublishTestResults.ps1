param(
    [Parameter(Mandatory = $true)][string]$ResultsDirectory,
    [Parameter(Mandatory = $true)][string]$RunTitle,
    [int]$TimeoutSeconds = 300
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Net.Http

function ConvertTo-VsoValue([string]$Value) {
    $Value.Replace('%', '%AZP25').Replace("`r", '%0D').Replace("`n", '%0A').Replace(';', '%3B').Replace(']', '%5D')
}

function Get-TestRunResponse([string]$Uri, [datetime]$Deadline) {
    $timeout = 30
    for ($attempt = 1; $attempt -le 3; $attempt++) {
        try {
            return Invoke-RestMethod -Uri $Uri -Headers $headers -TimeoutSec $timeout
        }
        catch [System.Net.Http.HttpRequestException] {
            $cause = $_.Exception.GetBaseException()
            if ($cause -isnot [System.Net.Sockets.SocketException] -or
                $cause.SocketErrorCode -ne [System.Net.Sockets.SocketError]::HostNotFound -or
                $attempt -eq 3 -or [DateTime]::UtcNow.AddSeconds(5) -ge $Deadline) {
                throw
            }
            Write-Warning "DNS failure while verifying test results. Retrying within the existing deadline: $($_.Exception.Message)"
            Start-Sleep -Seconds 5
            $timeout = [Math]::Min(30, [Math]::Floor(($Deadline - [DateTime]::UtcNow).TotalSeconds))
            if ($timeout -lt 1) { throw }
        }
    }
}

$files = @(if (Test-Path -LiteralPath $ResultsDirectory) {
    Get-ChildItem -LiteralPath $ResultsDirectory -Filter '*.xml' -File | Sort-Object Name
})
if ($files.Count -eq 0) {
    Write-Warning "No test result XML files found in $ResultsDirectory."
    return
}

if (!$env:SYSTEM_ACCESSTOKEN) {
    throw 'SYSTEM_ACCESSTOKEN is required to verify published test runs.'
}

$headers = @{ Authorization = "Bearer $env:SYSTEM_ACCESSTOKEN" }
$project = [uri]::EscapeDataString($env:SYSTEM_TEAMPROJECT)
$build = [uri]::EscapeDataString($env:BUILD_BUILDURI)
$runsUrl = "$env:SYSTEM_COLLECTIONURI$project/_apis/test/runs"
$runIds = @()

foreach ($file in $files) {
    $xml = [xml](Get-Content -LiteralPath $file.FullName -Raw)
    # Azure's XUnit parser coalesces results with the same case-sensitive test name.
    $testNames = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($test in $xml.SelectNodes('/assemblies/assembly/collection/test')) {
        $null = $testNames.Add($test.GetAttribute('name'))
    }
    $expected = $testNames.Count
    if ($expected -eq 0) {
        throw "No test cases found in $($file.FullName)."
    }
    if ($file.FullName.Contains(',')) {
        throw "The test result path cannot contain a comma: $($file.FullName)"
    }

    $title = "$RunTitle $($file.Name) $env:SYSTEM_JOBID.$env:SYSTEM_JOBATTEMPT"
    $commandTitle = ConvertTo-VsoValue $title
    $commandFile = ConvertTo-VsoValue $file.FullName
    Write-Host "##vso[results.publish type=XUnit;mergeResults=false;runTitle=$commandTitle;publishRunAttachments=true;]$commandFile"

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $response = Get-TestRunResponse "${runsUrl}?buildUri=$build&`$top=10000&api-version=7.1" $deadline
        $runs = @($response.value | Where-Object { $_.name -eq $title -or $_.name -eq "${title}_1" })
        if ($runs.Count -gt 1) {
            throw "Multiple test runs found for $($file.Name)."
        }
        if ($runs.Count -eq 1) {
            $run = Get-TestRunResponse "$runsUrl/$($runs[0].id)?api-version=7.1" $deadline
            if ($runIds -contains $run.id) {
                throw "Test run $($run.id) was reused for $($file.Name)."
            }
            if ($run.state -eq 'Completed') {
                if ($run.totalTests -ne $expected -or $run.incompleteTests -ne 0) {
                    throw "Run $($run.id) contains $($run.totalTests) tests ($($run.incompleteTests) incomplete); expected $expected from $($file.Name)."
                }
                $runIds += $run.id
                Write-Host "Verified $($file.Name): run $($run.id), $expected results, Completed."
                break
            }
        }
        if ([DateTime]::UtcNow -ge $deadline) {
            throw "Timed out waiting for a complete test run for $($file.Name)."
        }
        Start-Sleep -Seconds 5
    } while ($true)
}

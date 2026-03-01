<#
.SYNOPSIS
    Continuously kills dotnet.exe processes.

.DESCRIPTION
    Runs in a loop, checking every second for dotnet.exe processes and killing them.
    Optionally filters processes by their command line arguments.

.PARAMETER ArgumentFilter
    Optional filter to match against the command line arguments of dotnet.exe processes.
    Only processes whose command line contains this string will be killed.
    Example: "MyFoo.dll" will kill processes running "dotnet.exe MyFoo.dll"

.PARAMETER MaxTime
    Maximum time in seconds to run the loop. Default is 30 seconds.
    Use -1 to run indefinitely.

.EXAMPLE
    .\kill-dotnet.ps1
    Kills all dotnet.exe processes for 30 seconds.

.EXAMPLE
    .\kill-dotnet.ps1 -ArgumentFilter "MyFoo.dll"
    Kills only dotnet.exe processes that have "MyFoo.dll" in their command line.

.EXAMPLE
    .\kill-dotnet.ps1 -MaxTime -1
    Kills all dotnet.exe processes indefinitely.

.EXAMPLE
    .\kill-dotnet.ps1 -MaxTime 60 -ArgumentFilter "MyFoo.dll"
    Kills matching processes for 60 seconds.

.NOTES
    Press Ctrl+C to stop the script.
#>
param(
    [string]$ArgumentFilter,
    [int]$MaxTime = 30
)

$startTime = Get-Date

while ($MaxTime -eq -1 -or ((Get-Date) - $startTime).TotalSeconds -lt $MaxTime) {
    $processes = Get-CimInstance Win32_Process -Filter "Name = 'dotnet.exe'" -ErrorAction SilentlyContinue
    if ($ArgumentFilter) {
        $processes = $processes | Where-Object { $_.CommandLine -like "*$ArgumentFilter*" }
    }
    $count = ($processes | Measure-Object).Count
    if ($count -gt 0) {
        $processes | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
        Write-Host "Killed $count dotnet process(es)"
    }
    Start-Sleep -Seconds 1
}

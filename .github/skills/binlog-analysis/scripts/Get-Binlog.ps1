<#
.SYNOPSIS
    Resolve a build's .binlog and print its path, for live analysis via the
    `binlog-mcp` MCP server (Microsoft.AITools.BinlogMcp). Works on a local
    build's binlog or on a failed dotnet/fsharp Azure DevOps PR build's
    published binlog.

.DESCRIPTION
    This skill's job is ACQUISITION only — it does not analyze. It locates (and,
    for Azure DevOps, downloads) the binary log and prints the path(s). Hand each
    path to the binlog-mcp MCP tools (binlog_overview, binlog_diagnose,
    binlog_errors, ...) with `binlog_file: <path>`.

    Source (pick one):
      * Local  — pass -BinlogPath, or run with no arguments to auto-discover the
                 newest *.binlog under <repo>/artifacts/log.
      * Azure DevOps — pass -PrNumber (latest failed `fsharp-ci` build) or an
                 explicit -BuildId; the build-leg binlog artifact is downloaded
                 to a temp folder and KEPT, so the MCP can read it afterwards.

.PARAMETER BinlogPath
    Local binlog source: a .binlog file, a directory (newest *.binlog inside, or
    all with -AllLegs), or a glob. No download is performed.

.PARAMETER PrNumber
    GitHub PR number in dotnet/fsharp. The latest failed build is used.

.PARAMETER BuildId
    Explicit Azure DevOps build id.

.PARAMETER AllLegs
    Include every binlog rather than just the build leg / newest: all binlog
    artifacts for an AzDo build, or every *.binlog in a local directory.

.PARAMETER Json
    Emit the resolved path(s) as JSON (`{ "binlogs": [ ... ] }`).

.EXAMPLE
    # Newest local build binlog (build first, e.g. ./build.sh --binaryLog):
    pwsh Get-Binlog.ps1

.EXAMPLE
    pwsh Get-Binlog.ps1 -BinlogPath artifacts/log/Debug/Build.binlog

.EXAMPLE
    pwsh Get-Binlog.ps1 -PrNumber 19941

.EXAMPLE
    pwsh Get-Binlog.ps1 -BuildId 1462217 -Json
#>
[CmdletBinding(DefaultParameterSetName = 'Local')]
param(
    [Parameter(ParameterSetName = 'ByPath', Mandatory, Position = 0)]
    [string[]]$BinlogPath,

    [Parameter(ParameterSetName = 'ByPr', Mandatory, Position = 0)]
    [int]$PrNumber,

    [Parameter(ParameterSetName = 'ByBuild', Mandatory, Position = 0)]
    [long]$BuildId,

    [string]$Org = 'dnceng-public',
    [string]$Project = 'public',
    [int]$Definition = 90,
    [switch]$AllLegs,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$api = "https://dev.azure.com/$Org/$Project/_apis/build"

function Resolve-BuildId([int]$pr) {
    $url = "$api/builds?definitions=$Definition&reasonFilter=pullRequest&statusFilter=completed&`$top=100&api-version=7.1"
    $builds = (Invoke-RestMethod -Uri $url).value |
        Where-Object { $_.triggerInfo.'pr.number' -eq "$pr" }
    if (-not $builds) { throw "No completed PR builds found for PR #$pr (definition $Definition)." }
    $failed = $builds | Where-Object { $_.result -eq 'failed' } | Sort-Object finishTime -Descending
    $chosen = if ($failed) { $failed[0] } else { ($builds | Sort-Object finishTime -Descending)[0] }
    Write-Host "PR #$pr -> build $($chosen.id) ($($chosen.result), finished $($chosen.finishTime))"
    return $chosen.id
}

function Get-RepoRoot { (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path }

function Resolve-LocalBinlogs([string[]]$paths, [bool]$all) {
    $out = [System.Collections.Generic.List[string]]::new()
    foreach ($p in $paths) {
        if (Test-Path -LiteralPath $p -PathType Leaf) {
            if ([IO.Path]::GetExtension($p) -eq '.binlog') { $out.Add((Resolve-Path -LiteralPath $p).Path) }
            else { Write-Warning "Skipping non-binlog file: $p" }
        }
        elseif (Test-Path -LiteralPath $p -PathType Container) {
            $found = Get-ChildItem -LiteralPath $p -Recurse -Filter *.binlog -ErrorAction SilentlyContinue |
                Sort-Object LastWriteTime -Descending
            if (-not $found) { throw "No *.binlog under directory: $p" }
            if ($all) { $found | ForEach-Object { $out.Add($_.FullName) } } else { $out.Add($found[0].FullName) }
        }
        else {
            $glob = Get-ChildItem -Path $p -ErrorAction SilentlyContinue | Where-Object { $_.Extension -eq '.binlog' }
            if (-not $glob) { throw "Path not found or no .binlog match: $p" }
            $glob | ForEach-Object { $out.Add($_.FullName) }
        }
    }
    return $out
}

$binlogs = [System.Collections.Generic.List[string]]::new()

switch ($PSCmdlet.ParameterSetName) {
    'Local' {
        $logDir = Join-Path (Get-RepoRoot) 'artifacts/log'
        if (-not (Test-Path $logDir)) {
            throw "No local build logs at '$logDir'. Build with a binary log first " +
                  "(e.g. ./build.sh --binaryLog or eng/Build.ps1 -binaryLog), or pass -BinlogPath."
        }
        Write-Host "Auto-discovering newest binlog under $logDir ..."
        $binlogs = Resolve-LocalBinlogs @($logDir) $AllLegs.IsPresent
    }
    'ByPath' {
        $binlogs = Resolve-LocalBinlogs $BinlogPath $AllLegs.IsPresent
    }
    default {
        # Azure DevOps modes (ByPr / ByBuild): download the build-leg binlog
        # artifact and KEEP it so the binlog-mcp MCP server can read it.
        if ($PSCmdlet.ParameterSetName -eq 'ByPr') { $BuildId = Resolve-BuildId $PrNumber }
        $artifacts = (Invoke-RestMethod -Uri "$api/builds/$BuildId/artifacts?api-version=7.1").value
        $selected = if ($AllLegs) {
            $artifacts | Where-Object { $_.name -match 'binlog' }
        } else {
            $build = $artifacts | Where-Object { $_.name -match 'build binlog' }
            if ($build) { $build } else { $artifacts | Where-Object { $_.name -match 'binlog' } }
        }
        if (-not $selected) { throw "Build $BuildId has no binlog artifacts." }

        $downloadDir = Join-Path ([IO.Path]::GetTempPath()) "binlog-analysis-$BuildId"
        if (Test-Path $downloadDir) { Remove-Item -Recurse -Force $downloadDir }
        New-Item -ItemType Directory -Force -Path $downloadDir | Out-Null
        foreach ($a in $selected) {
            $zip = Join-Path $downloadDir "$($a.name -replace '[^\w.-]', '_').zip"
            Write-Host "Downloading artifact: $($a.name)"
            Invoke-WebRequest -Uri $a.resource.downloadUrl -OutFile $zip
            $dest = Join-Path $downloadDir ($a.name -replace '[^\w.-]', '_')
            Expand-Archive -Path $zip -DestinationPath $dest -Force
            Get-ChildItem $dest -Recurse -Filter *.binlog | ForEach-Object { $binlogs.Add($_.FullName) }
        }
        Write-Host "Kept under: $downloadDir"
    }
}

if ($binlogs.Count -eq 0) { throw "No .binlog files resolved." }

if ($Json) {
    [pscustomobject]@{ binlogs = @($binlogs) } | ConvertTo-Json
    return
}

Write-Host ''
Write-Host "Resolved $($binlogs.Count) binlog(s):"
foreach ($b in $binlogs) { Write-Host "  $b" }
Write-Host ''
Write-Host 'Next: analyze with the binlog-mcp MCP server (arg name is binlog_file):'
Write-Host '  binlog_overview { binlog_file: "<path>" }   # build status + error/warning counts'
Write-Host '  binlog_diagnose { binlog_file: "<path>" }   # categorized root causes + next steps'
Write-Host '  binlog_errors   { binlog_file: "<path>" }   # structured errors (code/file/line/project)'
Write-Host 'If the build succeeded / 0 errors (e.g. a test-only or formatting failure), there is nothing to analyze.'

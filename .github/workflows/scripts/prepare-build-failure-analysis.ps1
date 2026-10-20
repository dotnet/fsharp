[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateRange(1, 2147483647)]
    [int]$PrNumber,

    [ValidateRange(1, 9223372036854775807)]
    [long]$BuildId
)

$ErrorActionPreference = 'Stop'
$root = Split-Path (Split-Path (Split-Path $PSScriptRoot))
$directory = Join-Path ([IO.Path]::GetTempPath()) "fsharp-build-failure-analysis-$([Guid]::NewGuid())"
$output = Join-Path $directory 'fetch-output.txt'
$binlogs = Join-Path $directory 'binlogs'
$variables = @{
    GH_AW_REPO = 'dotnet/fsharp'
    RESOLVE_MODE = $(if ($BuildId) { 'dispatch' } else { 'latest' })
    PR_NUMBER = "$PrNumber"
    DISPATCH_BUILD_ID = "$BuildId"
    BINLOG_DIR = $binlogs
    GITHUB_OUTPUT = $output
    GH_TOKEN = [Environment]::GetEnvironmentVariable('GH_TOKEN')
}
$previous = @{}

try {
    if (-not $variables.GH_TOKEN) {
        $variables.GH_TOKEN = gh auth token
        if ($LASTEXITCODE -ne 0 -or -not $variables.GH_TOKEN) {
            throw 'Authenticate GitHub CLI before fetching a validated PR build.'
        }
    }
    foreach ($name in $variables.Keys) {
        $previous[$name] = [Environment]::GetEnvironmentVariable($name)
        [Environment]::SetEnvironmentVariable($name, $variables[$name])
    }
    New-Item -ItemType Directory -Path $directory | Out-Null
    Push-Location $directory
    try {
        dotnet fsi --exec (Join-Path $PSScriptRoot 'fetch-build-binlogs.fsx') |
            ForEach-Object { [Console]::Error.WriteLine($_) }
        if ($LASTEXITCODE -ne 0) {
            throw "Binlog collection failed (exit $LASTEXITCODE); see the fetcher's error above."
        }
    }
    finally {
        Pop-Location
    }
    $metadata = @{}
    foreach ($line in Get-Content -LiteralPath $output) {
        $key, $value = $line -split '=', 2
        $metadata[$key] = $value
    }
    if ($metadata['binlog-found'] -ne 'true') {
        throw 'No complete, current binlog evidence was recovered; no agent was started.'
    }
    $paths = @(Get-ChildItem -LiteralPath $binlogs -Filter '*.binlog' -File | Sort-Object Name)
    if (-not $paths.Count) {
        throw 'The successful fetch has no regular binlogs.'
    }
    $context = @{
        GH_AW_LOCAL_RUN = 'true'
        GITHUB_REPOSITORY = 'dotnet/fsharp'
        GH_AW_BUILD_OUTCOME = 'failure'
        GH_AW_BINLOG_DIR = $binlogs
        GH_AW_BINLOG_LIST = ($paths.FullName -join "`n")
        GH_AW_BINLOG_PATH = $paths[0].FullName
        GH_AW_BINLOG_HOST_PATH = $metadata['ado-build-url']
        GH_AW_PR_NUMBER = $metadata['pr-number']
        GH_AW_PR_HEAD_SHA = $metadata['pr-head-sha']
        GH_AW_PR_MERGE_SHA = $metadata['pr-merge-sha']
        GH_AW_PR_HEAD_REF = $metadata['pr-head-ref']
        GH_AW_PR_HEAD_REPO = $metadata['pr-head-repo']
        GH_AW_WORKSPACE = $root
    }
    $path = Join-Path $directory 'context.json'
    $context | ConvertTo-Json | Set-Content -LiteralPath $path -Encoding utf8
    $path
}
finally {
    foreach ($name in $previous.Keys) {
        $value = $previous[$name]
        # .NET 9+ distinguishes an absent variable from an empty string.
        if ($null -eq $value) { $value = [NullString]::Value }
        [Environment]::SetEnvironmentVariable($name, $value)
    }
}

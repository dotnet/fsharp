<#
.SYNOPSIS
    Retrieves Azure DevOps build IDs associated with a GitHub PR.

.DESCRIPTION
    Queries GitHub PR checks and extracts the Azure DevOps build IDs,
    pipeline names, states, and links for each unique build.

.PARAMETER PrNumber
    The GitHub Pull Request number.

.PARAMETER Repo
    The GitHub repository in 'owner/repo' format. Defaults to 'dotnet/fsharp'.

.EXAMPLE
    ./Get-PrBuildIds.ps1 -PrNumber 33251

.EXAMPLE
    ./Get-PrBuildIds.ps1 -PrNumber 33251 -Repo "dotnet/fsharp"

.OUTPUTS
    Array of objects with Pipeline, BuildId, State, Detail, and Link properties.
    The State represents the worst-case state across all jobs in the build:
    - FAILURE if any job failed
    - IN_PROGRESS if any job is still running or queued
    - SUCCESS if all jobs completed successfully
    The Detail field shows the count of jobs in each state.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [int]$PrNumber,

    [Parameter(Mandatory = $false)]
    [string]$Repo = "dotnet/fsharp"
)

$ErrorActionPreference = "Stop"

# Validate prerequisites
if (-not (Get-Command "gh" -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) is not installed. Install from https://cli.github.com/"
    exit 1
}

# Get PR checks from GitHub
$checksJson = gh pr checks $PrNumber --repo $Repo --json name,link,state 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to get PR checks: $checksJson"
    exit 1
}

$checks = $checksJson | ConvertFrom-Json

# Filter to Azure DevOps checks and extract build IDs
$builds = $checks | Where-Object { $_.link -match "dev\.azure\.com" } | ForEach-Object {
    $buildId = if ($_.link -match "buildId=(\d+)") { $matches[1] } else { $null }
    $pipeline = ($_.name -split " ")[0]
    
    [PSCustomObject]@{
        Pipeline = $pipeline
        BuildId  = $buildId
        State    = $_.state
        Link     = $_.link
    }
} | Group-Object BuildId | ForEach-Object {
    $jobs = $_.Group
    $states = $jobs | Select-Object -ExpandProperty State -Unique
    
    # Determine overall state (worst case wins)
    $overall = if ($states -contains "FAILURE") { 
        "FAILURE" 
    }
    elseif ($states -contains "IN_PROGRESS" -or $states -contains "QUEUED") { 
        "IN_PROGRESS" 
    }
    else { 
        "SUCCESS" 
    }
    
    $first = $jobs | Select-Object -First 1
    
    [PSCustomObject]@{
        Pipeline = $first.Pipeline
        BuildId  = $first.BuildId
        State    = $overall
        Detail   = ($jobs | Group-Object State | ForEach-Object { "$($_.Count) $($_.Name)" }) -join ", "
        Link     = ($first.Link -replace "\&view=.*", "")
    }
}

$builds
<#
.SYNOPSIS
    Finds the F# source commit corresponding to the latest F# insertion on a Visual Studio branch.

.DESCRIPTION
    Refreshes the Visual Studio branch's configured upstream, finds the first-parent commit that
    introduced its current F# payload, and queries Azure DevOps pipeline definition 499 for the
    payload build's source commit.

    By default, prints the command and pipeline changes needed to create a backport branch. When
    -Execute is specified, creates and checks out the branch and updates azure-pipelines.yml.
    Requires the Azure CLI to be installed and authenticated.

.PARAMETER VSBranch
    The Visual Studio branch whose latest F# insertion should be located.

.PARAMETER VSRepoPath
    The path to a local clone of the Visual Studio repository.

.PARAMETER FSharpRepoPath
    The path to a local clone of the dotnet/fsharp repository.

.PARAMETER NewBranchName
    The name of the F# backport branch to create.

.PARAMETER Execute
    Creates the branch and updates azure-pipelines.yml. Without this switch, no changes are made.

.EXAMPLE
    .\Get-VSBackportBranchPoint.ps1 `
        -VSBranch rel/insiders `
        -VSRepoPath Q:\source\VS `
        -FSharpRepoPath Q:\source\fsharp `
        -NewBranchName release/dev18.10

.EXAMPLE
    .\Get-VSBackportBranchPoint.ps1 `
        -VSBranch rel/insiders `
        -VSRepoPath Q:\source\VS `
        -FSharpRepoPath Q:\source\fsharp `
        -NewBranchName release/dev18.10 `
        -Execute
#>

[CmdletBinding(PositionalBinding = $false)]
param (
    [Parameter(Mandatory = $true)]
    [string]$VSBranch,

    [Parameter(Mandatory = $true)]
    [string]$VSRepoPath,

    [Parameter(Mandatory = $true)]
    [string]$FSharpRepoPath,

    [Parameter(Mandatory = $true)]
    [string]$NewBranchName,

    [switch]$Execute
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

function Invoke-Git {
    param (
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $output = & git -C $RepositoryPath @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "git -C `"$RepositoryPath`" $($Arguments -join ' ') failed:`n$($output -join "`n")"
    }

    return $output
}

function Resolve-GitRepository {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw "$Name repository path does not exist or is not a directory: $Path"
    }

    $repositoryRoot = Invoke-Git -RepositoryPath $Path -Arguments @("rev-parse", "--show-toplevel") |
        Select-Object -First 1

    return [IO.Path]::GetFullPath($repositoryRoot)
}

function Test-GitRef {
    param (
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [string]$Ref
    )

    & git -C $RepositoryPath rev-parse --verify --quiet "$Ref^{commit}" 2>$null | Out-Null
    return $LASTEXITCODE -eq 0
}

function Test-GitAncestor {
    param (
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [string]$Ancestor,

        [Parameter(Mandatory = $true)]
        [string]$Descendant
    )

    & git -C $RepositoryPath merge-base --is-ancestor $Ancestor $Descendant 2>$null
    return $LASTEXITCODE -eq 0
}

function Resolve-VisualStudioBranch {
    param (
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [string]$Branch
    )

    $localBranchRef = "refs/heads/$Branch"
    if (-not (Test-GitRef -RepositoryPath $RepositoryPath -Ref $localBranchRef)) {
        if (-not (Test-GitRef -RepositoryPath $RepositoryPath -Ref $Branch)) {
            throw "Visual Studio branch or ref '$Branch' was not found."
        }

        Write-Warning "Using Visual Studio ref '$Branch' without fetching because it is not a local branch with a configured upstream."
        return $Branch
    }

    $upstream = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
        "for-each-ref",
        "--format=%(upstream:short)",
        $localBranchRef
    ) | Select-Object -First 1

    if (-not $upstream) {
        Write-Warning "Using local Visual Studio branch '$Branch' without fetching because it has no configured upstream."
        return $Branch
    }

    $remote = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
        "for-each-ref",
        "--format=%(upstream:remotename)",
        $localBranchRef
    ) | Select-Object -First 1

    $null = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
        "fetch",
        $remote,
        "--quiet"
    )

    return $upstream
}

function Resolve-FSharpBranch {
    param (
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [string]$Branch,

        [Parameter(Mandatory = $true)]
        [string]$Commit
    )

    $candidates = @(
        "upstream/$Branch",
        "origin/$Branch",
        $Branch
    )

    foreach ($candidate in $candidates) {
        if ((Test-GitRef -RepositoryPath $RepositoryPath -Ref $candidate) -and
            (Test-GitAncestor -RepositoryPath $RepositoryPath -Ancestor $Commit -Descendant $candidate)) {
            return $candidate
        }
    }

    throw "F# source commit '$Commit' was not found on a local, upstream, or origin ref for branch '$Branch'. Fetch the F# repository and try again."
}

function ConvertTo-PowerShellLiteral {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    return "'$($Value.Replace("'", "''"))'"
}

function Set-PipelineVariable {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Content,

        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    $escapedName = [regex]::Escape($Name)
    $pattern = "(?m)(^\s*-\s+name:\s*$escapedName\s*\r?\n\s*value:\s*)[^\r\n]+"
    $matches = [regex]::Matches($Content, $pattern)

    if ($matches.Count -ne 1) {
        throw "Expected one '$Name' variable in azure-pipelines.yml; found $($matches.Count)."
    }

    return [regex]::Replace(
        $Content,
        $pattern,
        { param($match) $match.Groups[1].Value + $Value }
    )
}

function Get-AzureDevOpsToken {
    $token = & az account get-access-token `
        --resource "499b84ac-1321-427f-aa17-267ca6975798" `
        --query accessToken `
        -o tsv 2>&1

    if ($LASTEXITCODE -ne 0 -or -not $token) {
        throw "Unable to acquire an Azure DevOps access token. Run 'az login' and try again.`n$($token -join "`n")"
    }

    return $token | Select-Object -First 1
}

function Get-FSharpBuild {
    param (
        [Parameter(Mandatory = $true)]
        [string]$BuildNumber,

        [Parameter(Mandatory = $true)]
        [string]$SourceBranch
    )

    $definitionId = 499
    $escapedBuildNumber = [Uri]::EscapeDataString($BuildNumber)
    $escapedSourceBranch = [Uri]::EscapeDataString("refs/heads/$SourceBranch")
    $url = "https://dev.azure.com/dnceng/internal/_apis/build/builds" +
        "?definitions=$definitionId" +
        "&buildNumber=$escapedBuildNumber" +
        "&branchName=$escapedSourceBranch" +
        "&api-version=7.1"
    $webClient = [System.Net.WebClient]::new()

    try {
        $webClient.Headers.Add("Authorization", "Bearer $(Get-AzureDevOpsToken)")
        $response = $webClient.DownloadString($url) | ConvertFrom-Json
    }
    catch {
        throw "Failed to query fsharp-ci build '$BuildNumber' for branch '$SourceBranch' from Azure DevOps: $($_.Exception.Message)"
    }
    finally {
        $webClient.Dispose()
    }

    $builds = @($response.value)
    if ($builds.Count -ne 1) {
        throw "Expected one fsharp-ci build numbered '$BuildNumber' for branch '$SourceBranch'; found $($builds.Count)."
    }

    return $builds[0]
}

$vsRepository = Resolve-GitRepository -Path $VSRepoPath -Name "Visual Studio"
$fsharpRepository = Resolve-GitRepository -Path $FSharpRepoPath -Name "F#"

$null = Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
    "check-ref-format",
    "--branch",
    $NewBranchName
)

if (Test-GitRef -RepositoryPath $fsharpRepository -Ref "refs/heads/$NewBranchName") {
    throw "Local F# branch '$NewBranchName' already exists."
}

if ($NewBranchName -notmatch "^(feature|release)/") {
    Write-Warning "Branch '$NewBranchName' is not covered by the pipeline's feature/* or release/* CI triggers."
}

if ($Execute) {
    $trackedChanges = Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
        "status",
        "--porcelain",
        "--untracked-files=no"
    )

    if ($trackedChanges) {
        throw "The F# repository has tracked working-tree changes. Commit or stash them before using -Execute."
    }
}

$vsBranchRef = Resolve-VisualStudioBranch -RepositoryPath $vsRepository -Branch $VSBranch

$componentsPath = ".corext/Configs/components.json"
$branchComponentsJson = (Invoke-Git -RepositoryPath $vsRepository -Arguments @(
    "show",
    "${vsBranchRef}:$componentsPath"
)) -join "`n"
$branchComponents = $branchComponentsJson | ConvertFrom-Json
$fsharpComponent = $branchComponents.Components."Microsoft.FSharp"

if (-not $fsharpComponent) {
    throw "The F# component was not found in $componentsPath on Visual Studio branch '$VSBranch'."
}

$insertionCommit = Invoke-Git -RepositoryPath $vsRepository -Arguments @(
    "log",
    $vsBranchRef,
    "--first-parent",
    "-n",
    "1",
    "--format=%H",
    "-S$($fsharpComponent.url)",
    "--",
    $componentsPath
)

if (-not $insertionCommit) {
    throw "No F# insertion was found on Visual Studio branch '$VSBranch'."
}

$insertionCommit = $insertionCommit | Select-Object -First 1
$insertionDate = (Invoke-Git -RepositoryPath $vsRepository -Arguments @(
    "show",
    "-s",
    "--format=%cI",
    $insertionCommit
)) | Select-Object -First 1
$insertionSubject = (Invoke-Git -RepositoryPath $vsRepository -Arguments @(
    "show",
    "-s",
    "--format=%s",
    $insertionCommit
)) | Select-Object -First 1

$buildNumberMatch = [regex]::Match(
    $fsharpComponent.url,
    "/dotnet-fsharp/(?<sourceBranch>.+)/(?<buildNumber>[^/;]+);",
    [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
)
if (-not $buildNumberMatch.Success) {
    throw "Unable to extract the F# source branch and build number from component URL '$($fsharpComponent.url)'."
}

$sourceBranch = [Uri]::UnescapeDataString($buildNumberMatch.Groups["sourceBranch"].Value)
$buildNumber = $buildNumberMatch.Groups["buildNumber"].Value
$build = Get-FSharpBuild -BuildNumber $buildNumber -SourceBranch $sourceBranch
$branchPoint = $build.sourceVersion

if ($branchPoint -notmatch "^[0-9a-f]{40}$") {
    throw "Build '$buildNumber' returned invalid source version '$branchPoint'."
}

$fsharpBranchRef = Resolve-FSharpBranch `
    -RepositoryPath $fsharpRepository `
    -Branch $sourceBranch `
    -Commit $branchPoint

$branchPointDate = (Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
    "show",
    "-s",
    "--format=%cI",
    $branchPoint
)) | Select-Object -First 1
$branchPointSubject = (Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
    "show",
    "-s",
    "--format=%s",
    $branchPoint
)) | Select-Object -First 1
$componentVersionProperty = $fsharpComponent.PSObject.Properties["version"]
$fsharpRepoLiteral = ConvertTo-PowerShellLiteral -Value $fsharpRepository
$newBranchLiteral = ConvertTo-PowerShellLiteral -Value $NewBranchName
$branchPointLiteral = ConvertTo-PowerShellLiteral -Value $branchPoint
$gitCommand = "git -C $fsharpRepoLiteral switch -c $newBranchLiteral $branchPointLiteral"
$action = "Preview"

if ($Execute) {
    $pipelinePath = Join-Path $fsharpRepository "azure-pipelines.yml"
    # Validate both mappings before creating the branch; edit the checked-out file afterward to preserve its formatting.
    $branchPointPipelineContent = (Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
        "show",
        "${branchPoint}:azure-pipelines.yml"
    )) -join "`n"
    $branchPointPipelineContent = Set-PipelineVariable `
        -Content $branchPointPipelineContent `
        -Name "FSharpReleaseBranchName" `
        -Value $NewBranchName
    $null = Set-PipelineVariable `
        -Content $branchPointPipelineContent `
        -Name "VSInsertionTargetBranchName" `
        -Value $VSBranch

    $null = Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
        "switch",
        "-c",
        $NewBranchName,
        $branchPoint
    )

    $pipelineContent = [IO.File]::ReadAllText($pipelinePath)
    $pipelineContent = Set-PipelineVariable `
        -Content $pipelineContent `
        -Name "FSharpReleaseBranchName" `
        -Value $NewBranchName
    $pipelineContent = Set-PipelineVariable `
        -Content $pipelineContent `
        -Name "VSInsertionTargetBranchName" `
        -Value $VSBranch
    [IO.File]::WriteAllText($pipelinePath, $pipelineContent)
    $action = "Executed"
}

[PSCustomObject]@{
    Action                  = $action
    VSBranch                = $VSBranch
    VSResolvedBranchRef     = $vsBranchRef
    VSInsertionCommit       = $insertionCommit
    VSInsertionDate         = $insertionDate
    VSInsertionSubject      = $insertionSubject
    FSharpComponentVersion  = if ($componentVersionProperty) { $componentVersionProperty.Value } else { $null }
    FSharpComponentUrl      = $fsharpComponent.url
    FSharpSourceBranch      = $sourceBranch
    FSharpResolvedBranchRef = $fsharpBranchRef
    FSharpBuildNumber       = $buildNumber
    FSharpBuildId           = $build.id
    FSharpBranchPoint       = $branchPoint
    FSharpBranchPointDate   = $branchPointDate
    FSharpBranchPointSubject = $branchPointSubject
    NewBranchName           = $NewBranchName
    GitCommand              = $gitCommand
    PipelineFSharpBranch    = $NewBranchName
    PipelineVSBranch        = $VSBranch
}

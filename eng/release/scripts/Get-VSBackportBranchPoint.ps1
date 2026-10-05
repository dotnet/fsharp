<#
.SYNOPSIS
    Finds the F# source commit corresponding to the latest F# insertion on a Visual Studio branch.

.DESCRIPTION
    Refreshes the Visual Studio branch's configured upstream, finds the first-parent commit that
    introduced its current F# payload, and queries Azure DevOps pipeline definition 499 for the
    payload build's source commit.

    Without VSRepoPath, uses an owned shallow, no-checkout clone, deepening at most ten times
    by 100 commits. The clone is removed even on failure. Requires Azure CLI authentication
    with access to DevDiv/VS and dnceng/internal.

    By default, reports the command and proposed pipeline/version edits without changing F#.
    -Execute creates and checks out the branch, updates azure-pipelines.yml, and fixes the
    caller-supplied minor in eng\Versions.props. It does not commit, push, or insert into VS.

.PARAMETER VSBranch
    The Visual Studio branch whose latest F# insertion should be located.

.PARAMETER VSRepoPath
    Optional local Visual Studio clone. Its checkout is not changed. Otherwise a temporary
    clone of https://devdiv.visualstudio.com/DefaultCollection/DevDiv/_git/VS is used.

.PARAMETER FSharpRepoPath
    The path to a local clone of the dotnet/fsharp repository.

.PARAMETER NewBranchName
    The name of the F# backport branch to create.

.PARAMETER VSMinorVersion
    Required integer from 0 through 65534 for the target VS release. The caller owns this
    mapping; different branches may use the same minor. Scheduled minor selection is disabled.

.PARAMETER Execute
    Creates the branch and updates both files. Without this switch, no F# changes are made.

.EXAMPLE
    .\Get-VSBackportBranchPoint.ps1 `
        -VSBranch rel/insiders `
        -VSRepoPath Q:\source\VS `
        -FSharpRepoPath Q:\source\fsharp `
        -NewBranchName release/dev18.11 -VSMinorVersion 11

.EXAMPLE
    .\Get-VSBackportBranchPoint.ps1 `
        -VSBranch rel/insiders `
        -VSRepoPath Q:\source\VS `
        -FSharpRepoPath Q:\source\fsharp `
        -NewBranchName release/dev18.11 -VSMinorVersion 11 `
        -Execute
#>

[CmdletBinding(PositionalBinding = $false)]
param (
    [Parameter(Mandatory = $true)]
    [string]$VSBranch,

    [string]$VSRepoPath,

    [Parameter(Mandatory = $true)]
    [string]$FSharpRepoPath,

    [Parameter(Mandatory = $true)]
    [string]$NewBranchName,

    [Parameter(Mandatory = $true)]
    [ValidateScript({ $_ -cmatch '^[0-9]{1,5}$' -and [int]$_ -le 65534 })]
    [string]$VSMinorVersion,

    [switch]$Execute
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

function Invoke-Git {
    param (
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,

        [string]$AccessToken
    )

    $savedEnvironment = @{}
    try {
        if ($AccessToken) {
            # Git's process configuration keeps the header out of argv and repository config.
            $environment = @{
                GIT_CONFIG_COUNT = '1'
                GIT_CONFIG_KEY_0 = 'http.extraHeader'
                GIT_CONFIG_VALUE_0 = "Authorization: Bearer $AccessToken"
                GIT_TERMINAL_PROMPT = '0'
                GIT_TRACE = '0'
                GIT_TRACE_CURL = '0'
                GIT_CURL_VERBOSE = '0'
                GIT_TRACE_PACKET = '0'
                GIT_TRACE2 = '0'
                GIT_TRACE2_EVENT = '0'
                GIT_TRACE2_PERF = '0'
            }
            foreach ($name in $environment.Keys) {
                $savedEnvironment[$name] = [Environment]::GetEnvironmentVariable($name, 'Process')
                [Environment]::SetEnvironmentVariable($name, $environment[$name], 'Process')
            }
        }
        # Windows PowerShell wraps native stderr as errors, including successful Git messages.
        $ErrorActionPreference = 'Continue'
        $output = & git -C $RepositoryPath @Arguments 2>&1
        $exitCode = $LASTEXITCODE
        $ErrorActionPreference = 'Stop'
        if ($exitCode -ne 0) {
            if ($AccessToken) { throw "Authenticated Git operation failed. Check Azure CLI identity and repository access." }
            throw "git -C `"$RepositoryPath`" $($Arguments -join ' ') failed:`n$($output -join "`n")"
        }
        return $output
    }
    catch {
        if ($AccessToken) { throw "Authenticated Git operation failed. Check Azure CLI identity and repository access." }
        throw
    }
    finally {
        foreach ($name in $savedEnvironment.Keys) {
            [Environment]::SetEnvironmentVariable($name, $savedEnvironment[$name], 'Process')
        }
    }
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
        [string]$Branch,

        [string]$AccessToken
    )

    $Branch = $Branch -replace '^refs/heads/', ''
    $localBranchRef = "refs/heads/$Branch"
    if (-not (Test-GitRef -RepositoryPath $RepositoryPath -Ref $localBranchRef)) {
        if (-not (Test-GitRef -RepositoryPath $RepositoryPath -Ref $Branch)) {
            throw "Visual Studio branch or ref '$Branch' was not found."
        }

        Write-Warning "Using Visual Studio ref '$Branch' without fetching because it is not a local branch with a configured upstream."
        $fullRef = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @("rev-parse", "--symbolic-full-name", $Branch)
        if ($fullRef -notmatch '^refs/remotes/([^/]+)/(.+)$') {
            throw "Supply a Visual Studio branch name, not an arbitrary ref or commit."
        }
        return [PSCustomObject]@{ Ref = $fullRef; Remote = $Matches[1]; RemoteBranch = "refs/heads/$($Matches[2])"; Branch = $Matches[2] }
    }

    $upstream = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
        "for-each-ref",
        "--format=%(upstream:short)",
        $localBranchRef
    ) | Select-Object -First 1

    if (-not $upstream) {
        Write-Warning "Using local Visual Studio branch '$Branch' without fetching because it has no configured upstream."
        return [PSCustomObject]@{ Ref = $localBranchRef; Branch = $Branch; Remote = $null; RemoteBranch = $null }
    }

    $remote = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
        "for-each-ref",
        "--format=%(upstream:remotename)",
        $localBranchRef
    ) | Select-Object -First 1

    $remoteBranch = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
        "for-each-ref", "--format=%(upstream:remoteref)", $localBranchRef
    ) | Select-Object -First 1
    $null = Invoke-Git -RepositoryPath $RepositoryPath -AccessToken $AccessToken -Arguments @(
        "fetch",
        $remote,
        "--quiet",
        "--no-tags",
        "${remoteBranch}:refs/remotes/$upstream"
    )

    return [PSCustomObject]@{ Ref = $upstream; Branch = $remoteBranch -replace '^refs/heads/', ''; Remote = $remote; RemoteBranch = $remoteBranch }
}

function Get-VisualStudioInsertion {
    param (
        [string]$RepositoryPath,
        [string]$Snapshot,
        [string]$PayloadUrl,
        $Branch,
        [string]$AccessToken,
        [int]$DeepenBy = 100,
        [int]$MaxDeepenAttempts = 10
    )

    for ($attempt = 0; $attempt -le $MaxDeepenAttempts; $attempt++) {
        $candidate = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @(
            "log", $Snapshot, "--first-parent", "-n", "1", "--format=%H",
            "-S$PayloadUrl", "--", ".corext/Configs/components.json"
        ) | Select-Object -First 1
        $shallowPath = Invoke-Git -RepositoryPath $RepositoryPath -Arguments @("rev-parse", "--git-path", "shallow")
        if (-not [IO.Path]::IsPathRooted($shallowPath)) { $shallowPath = Join-Path $RepositoryPath $shallowPath }
        $boundaries = if (Test-Path -LiteralPath $shallowPath) { @(Get-Content -LiteralPath $shallowPath) } else { @() }
        # Pickaxe treats a shallow boundary as a root, falsely attributing every payload to it.
        if ($candidate -and $candidate -notin $boundaries) { return $candidate }
        if (-not $boundaries) { throw "No F# insertion was found in the Visual Studio snapshot." }
        if (-not $Branch.Remote -or $attempt -eq $MaxDeepenAttempts) {
            throw "Insufficient Visual Studio history to prove the insertion. Deepen the local clone and retry."
        }
        $null = Invoke-Git -RepositoryPath $RepositoryPath -AccessToken $AccessToken -Arguments @(
            "fetch", "--quiet", "--no-tags", "--deepen=$DeepenBy", $Branch.Remote, $Branch.RemoteBranch
        )
    }
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

function Set-FixedVSMinorVersion {
    param (
        [string]$Content,
        [ValidateScript({ $_ -cmatch '^[0-9]{1,5}$' -and [int]$_ -le 65534 })]
        [string]$Minor
    )

    $document = [xml]$Content
    $minorNodes = $document.SelectNodes('/Project/PropertyGroup/VSMinorVersion')
    $minorPattern = '<VSMinorVersion>\s*[0-9]+\s*</VSMinorVersion>'
    if ($minorNodes.Count -ne 1 -or $minorNodes[0].ParentNode.HasAttribute('Condition') -or
        [regex]::Matches($Content, $minorPattern).Count -ne 1) {
        throw "Expected one unconditional literal VSMinorVersion in eng\Versions.props."
    }
    $Content = [regex]::Replace($Content, $minorPattern, "<VSMinorVersion>$([int]$Minor)</VSMinorVersion>")
    $flagPattern = '<UseVSScheduledMinorVersion(?:\s+Condition="[^"]*")?>[^<]*</UseVSScheduledMinorVersion>'
    $flagNodes = $document.SelectNodes('/Project/PropertyGroup/UseVSScheduledMinorVersion')
    $flag = '<UseVSScheduledMinorVersion>false</UseVSScheduledMinorVersion>'
    if ($flagNodes.Count -eq 0) {
        $newline = if ($Content.Contains("`r`n")) { "`r`n" } else { "`n" }
        $Content = [regex]::Replace($Content, "(?m)^([ `t]*)($minorPattern)", {
            param($match)
            $match.Value + $newline + $match.Groups[1].Value + $flag
        })
    }
    elseif ($flagNodes.Count -eq 1 -and -not $flagNodes[0].ParentNode.HasAttribute('Condition') -and
        [regex]::Matches($Content, $flagPattern).Count -eq 1) {
        $Content = [regex]::Replace($Content, $flagPattern, $flag)
    }
    else { throw "Unsupported scheduled minor layout in eng\Versions.props." }

    $flagIndex = $Content.IndexOf($flag, [StringComparison]::Ordinal)
    $minorIndex = $Content.IndexOf('<VSMinorVersion>', [StringComparison]::Ordinal)
    $scheduleImport = [regex]::Match($Content, '<Import\b[^>]*\bProject="[^"]*VSMinorVersion\.props"[^>]*/>')
    if ($flagIndex -lt 0 -or ($scheduleImport.Success -and
        ($scheduleImport.Index -lt $flagIndex -or $scheduleImport.Index -lt $minorIndex))) {
        throw "The fixed minor and schedule opt-out must precede the schedule import in eng\Versions.props."
    }
    return $Content
}

function Get-AzureDevOpsToken {
    $ErrorActionPreference = 'Continue'
    $token = & az account get-access-token `
        --resource "499b84ac-1321-427f-aa17-267ca6975798" `
        --query accessToken `
        --only-show-errors `
        -o tsv 2>&1
    $exitCode = $LASTEXITCODE
    $ErrorActionPreference = 'Stop'

    if ($exitCode -ne 0 -or -not $token) {
        throw "Unable to acquire an Azure DevOps access token. Run 'az login' and try again."
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

function Invoke-VSBackportBranchPoint {
    param (
        [Parameter(Mandatory = $true)][string]$VSBranch,
        [string]$VSRepoPath,
        [Parameter(Mandatory = $true)][string]$FSharpRepoPath,
        [Parameter(Mandatory = $true)][string]$NewBranchName,
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $_ -cmatch '^[0-9]{1,5}$' -and [int]$_ -le 65534 })]
        [string]$VSMinorVersion,
        [switch]$Execute
    )

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

    $ownedRepository = $null
    try {
        $accessToken = Get-AzureDevOpsToken
        if ($VSRepoPath) {
            $vsRepository = Resolve-GitRepository -Path $VSRepoPath -Name "Visual Studio"
        }
        else {
            $VSBranch = $VSBranch -replace '^refs/heads/', ''
            $null = Invoke-Git -RepositoryPath $fsharpRepository -Arguments @("check-ref-format", "refs/heads/$VSBranch")
            # Keep scratch storage invocation-owned and under the caller's working directory.
            $clonePath = Join-Path (Get-Location).Path (".vs-backport-" + [Guid]::NewGuid().ToString('N'))
            $null = New-Item -ItemType Directory -Path $clonePath
            $ownedRepository = $clonePath
            $null = Invoke-Git -RepositoryPath $ownedRepository -AccessToken $accessToken -Arguments @(
                "clone", "--quiet", "--depth=100", "--single-branch", "--no-tags", "--no-checkout",
                "--branch", $VSBranch, "https://devdiv.visualstudio.com/DefaultCollection/DevDiv/_git/VS", "."
            )
            $vsRepository = $ownedRepository
        }

        $vsBranchInfo = Resolve-VisualStudioBranch -RepositoryPath $vsRepository -Branch $VSBranch -AccessToken $accessToken
        $vsBranchRef = $vsBranchInfo.Ref
        $VSBranch = $vsBranchInfo.Branch
        $vsSnapshot = Invoke-Git -RepositoryPath $vsRepository -Arguments @("rev-parse", "$vsBranchRef^{commit}") |
            Select-Object -First 1

        $componentsPath = ".corext/Configs/components.json"
        $branchComponentsJson = (Invoke-Git -RepositoryPath $vsRepository -Arguments @(
            "show",
            "${vsSnapshot}:$componentsPath"
        )) -join "`n"
        $branchComponents = $branchComponentsJson | ConvertFrom-Json
        $fsharpComponent = $branchComponents.Components."Microsoft.FSharp"

        if (-not $fsharpComponent) {
            throw "The F# component was not found in $componentsPath on Visual Studio branch '$VSBranch'."
        }

        $insertionCommit = Get-VisualStudioInsertion -RepositoryPath $vsRepository -Snapshot $vsSnapshot `
            -PayloadUrl $fsharpComponent.url -Branch $vsBranchInfo -AccessToken $accessToken
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

        # Preflight both files at the exact source SHA, even in preview.
        $branchPointPipelineContent = (Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
            "show", "${branchPoint}:azure-pipelines.yml"
        )) -join "`n"
        $branchPointPipelineContent = Set-PipelineVariable `
            -Content $branchPointPipelineContent -Name "FSharpReleaseBranchName" -Value $NewBranchName
        $null = Set-PipelineVariable `
            -Content $branchPointPipelineContent -Name "VSInsertionTargetBranchName" -Value $VSBranch
        $branchPointVersionsContent = (Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
            "show", "${branchPoint}:eng/Versions.props"
        )) -join "`n"
        $null = Set-FixedVSMinorVersion -Content $branchPointVersionsContent -Minor $VSMinorVersion

        if ($Execute) {
            $targetFiles = @(Invoke-Git -RepositoryPath $fsharpRepository -Arguments @("ls-tree", "-r", "--name-only", $branchPoint))
            $untrackedPaths = @(Invoke-Git -RepositoryPath $fsharpRepository -Arguments @("ls-files", "--others", "--directory", "--no-empty-directory"))
            foreach ($path in $untrackedPaths) {
                foreach ($target in $targetFiles) {
                    if ($target -eq $path -or $target.StartsWith($path.TrimEnd('/') + '/', [StringComparison]::OrdinalIgnoreCase) -or
                        $path.StartsWith($target + '/', [StringComparison]::OrdinalIgnoreCase)) {
                        throw "Untracked or ignored path '$path' collides with the selected F# source commit."
                    }
                }
            }
            $null = Invoke-Git -RepositoryPath $fsharpRepository -Arguments @(
                "switch",
                "--no-overwrite-ignore",
                "-c",
                $NewBranchName,
                $branchPoint
            )

            try {
                foreach ($file in @("azure-pipelines.yml", "eng\Versions.props")) {
                    $path = Join-Path $fsharpRepository $file
                    $content = [IO.File]::ReadAllText($path)
                    if ($file -eq "azure-pipelines.yml") {
                        $content = Set-PipelineVariable -Content $content -Name "FSharpReleaseBranchName" -Value $NewBranchName
                        $content = Set-PipelineVariable -Content $content -Name "VSInsertionTargetBranchName" -Value $VSBranch
                    }
                    else {
                        $content = Set-FixedVSMinorVersion -Content $content -Minor $VSMinorVersion
                    }
                    $bytes = [IO.File]::ReadAllBytes($path)
                    $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 239 -and $bytes[1] -eq 187 -and $bytes[2] -eq 191
                    [IO.File]::WriteAllText($path, $content, [Text.UTF8Encoding]::new($hasBom))
                }
            }
            catch {
                throw "Branch '$NewBranchName' was created, but edits failed and may be partial. Inspect the worktree; no rollback was attempted. $($_.Exception.Message)"
            }
            $action = "Executed"
        }

        [PSCustomObject]@{
            Action                    = $action
            VSBranch                  = $VSBranch
            VSResolvedBranchRef       = $vsBranchRef
            VSSnapshot                = $vsSnapshot
            VSInsertionCommit         = $insertionCommit
            VSInsertionDate           = $insertionDate
            VSInsertionSubject        = $insertionSubject
            FSharpComponentVersion    = if ($componentVersionProperty) { $componentVersionProperty.Value } else { $null }
            FSharpComponentUrl        = $fsharpComponent.url
            FSharpSourceBranch        = $sourceBranch
            FSharpResolvedBranchRef   = $fsharpBranchRef
            FSharpBuildNumber         = $buildNumber
            FSharpBuildId             = $build.id
            FSharpBranchPoint         = $branchPoint
            FSharpBranchPointDate     = $branchPointDate
            FSharpBranchPointSubject  = $branchPointSubject
            NewBranchName             = $NewBranchName
            GitCommand                = $gitCommand
            PipelineFSharpBranch      = $NewBranchName
            PipelineVSBranch          = $VSBranch
            VSMinorVersion            = [int]$VSMinorVersion
            UseVSScheduledMinorVersion = $false
            PlannedEdits              = @("azure-pipelines.yml: FSharpReleaseBranchName=$NewBranchName; VSInsertionTargetBranchName=$VSBranch",
                                         "eng\Versions.props: VSMinorVersion=$([int]$VSMinorVersion); UseVSScheduledMinorVersion=false")
        }
    }
    finally {
        if ($ownedRepository) { Remove-Item -LiteralPath $ownedRepository -Recurse -Force }
    }
}

if ($MyInvocation.InvocationName -ne '.') {
    Invoke-VSBackportBranchPoint @PSBoundParameters
}

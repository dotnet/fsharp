param([Parameter(Mandatory)][ValidateSet('Prepare', 'Membership', 'Queue', 'Report')][string]$Mode)

$ErrorActionPreference = 'Stop'

function Test-SafeId($Value) {
    ($Value -is [long] -or $Value -is [int] -or $Value -is [double]) -and
        $Value -gt 0 -and $Value -le 9007199254740991 -and [Math]::Truncate($Value) -eq $Value
}

function Invoke-Api($Uri, $Token, $Method = 'GET', $Body = $null, $ExpectedStatus = 200) {
    $options = @{
        Uri = $Uri; Method = $Method; Headers = @{ Authorization = "Bearer $Token"; Accept = 'application/json' }
        UserAgent = 'fsharp-pr-validation'; ContentType = 'application/json'
        MaximumRedirection = 0; SkipHttpErrorCheck = $true; ErrorAction = 'Stop'
    }
    if ($null -ne $Body) { $options.Body = ConvertTo-Json -InputObject $Body -Depth 10 -Compress }
    try { $response = Invoke-WebRequest @options }
    catch { throw "HTTP request failed for $Uri." }
    if ($response.StatusCode -notin $ExpectedStatus) { throw "HTTP $($response.StatusCode) from $Uri." }
    if ($response.StatusCode -ne 204) { ConvertFrom-Json -InputObject $response.Content -NoEnumerate }
}

if ($Mode -ne 'Queue') {
    $event = Get-Content -LiteralPath $env:GITHUB_EVENT_PATH -Raw | ConvertFrom-Json -NoEnumerate
    $prNumber = $event.issue.number
    if ($env:GITHUB_REPOSITORY -cne 'dotnet/fsharp' -or $event -is [array] -or
        $event.issue.pull_request -isnot [System.Management.Automation.PSCustomObject] -or !(Test-SafeId $prNumber)) {
        throw 'Only dotnet/fsharp PRs with a valid PR number are supported.'
    }
    # Always use the original commenter, not the actor rerunning the workflow.
    $login = [Uri]::EscapeDataString($event.comment.user.login)
    $repoApi = 'https://api.github.com/repos/dotnet/fsharp'
}

switch ($Mode) {
    'Prepare' {
        if ($event.comment.user.type -isnot [string] -or $event.comment.user.type -cne 'User') { throw 'Only user comments can request validation.' }
        if ($event.comment.body -isnot [string] -or $event.comment.body.Trim() -cnotmatch '\A/(dart|pr-val) ([0-9a-f]{40})\z') {
            throw 'Use a standalone /dart <sha> or /pr-val <sha> with the full 40-character reviewed PR commit SHA.'
        }
        $requestedSha = $Matches[2]
        $permission = Invoke-Api "$repoApi/collaborators/$login/permission" $env:GITHUB_TOKEN
        if ($permission -is [array] -or $permission.permission -isnot [string] -or
            $permission.permission -cnotin 'write', 'maintain', 'admin') { throw 'The requester needs repository write access.' }
        $pr = Invoke-Api "$repoApi/pulls/$prNumber" $env:GITHUB_TOKEN
        if ($pr -is [array] -or $pr.state -isnot [string] -or $pr.state -cne 'open' -or
            $pr.base.repo.full_name -isnot [string] -or $pr.base.repo.full_name -cne 'dotnet/fsharp' -or
            $pr.base.ref -isnot [string] -or $pr.base.ref -cne 'main') {
            throw 'Only open PRs targeting dotnet/fsharp main are supported.'
        }
        if ($pr.head.sha -isnot [string] -or $pr.head.sha -cnotmatch '\A[0-9a-f]{40}\z') {
            throw 'GitHub did not return a full PR head commit SHA.'
        }
        $found = $false
        for ($page = 1; $page -le 3; $page++) {
            $commits = Invoke-Api "$repoApi/pulls/$prNumber/commits?per_page=100&page=$page" $env:GITHUB_TOKEN
            if ($commits -isnot [array] -or $commits.Count -gt 100) { throw 'GitHub returned an invalid PR commit list.' }
            foreach ($commit in $commits) {
                if ($commit -isnot [System.Management.Automation.PSCustomObject] -or
                    $commit.PSObject.Properties.Name -cnotcontains 'sha' -or
                    $commit.sha -isnot [string] -or $commit.sha -cnotmatch '\A[0-9a-f]{40}\z') { throw 'GitHub returned an invalid PR commit SHA.' }
                if ($commit.sha -ceq $requestedSha) { $found = $true }
            }
            if ($found -or $commits.Count -lt 100) { break }
        }
        if (!$found) {
            throw 'The requested SHA was not found in the PR commit list (GitHub exposes at most 250 commits). Review a listed commit and request a new run.'
        }
        $request = @{
            resources = @{ repositories = @{ self = @{ refName = "refs/pull/$prNumber/head"; version = $requestedSha } } }
            templateParameters = @{ prNumber = "$prNumber"; headSha = $requestedSha }
        }
        $json = ConvertTo-Json -InputObject $request -Depth 10 -Compress
        Add-Content -LiteralPath $env:GITHUB_OUTPUT -Encoding utf8 -Value "body=$json", "head=$requestedSha"
    }
    'Membership' {
        $null = Invoke-Api "https://api.github.com/orgs/microsoft/members/$login" $env:GITHUB_TOKEN -ExpectedStatus 204
    }
    'Queue' {
        if ($env:PIPELINE_ID -cnotmatch '\A[1-9][0-9]*\z') { throw 'Configure FSHARP_APEX_PIPELINE_ID in the fsharp_pr_validation environment.' }
        if ([string]::IsNullOrWhiteSpace($env:AZDO_TOKEN)) { throw 'Azure did not return an access token.' }
        $request = ConvertFrom-Json -InputObject $env:REQUEST_BODY -NoEnumerate
        $run = Invoke-Api "https://dev.azure.com/devdiv/DevDiv/_apis/pipelines/$env:PIPELINE_ID/runs?api-version=7.1" `
            $env:AZDO_TOKEN -Method POST -Body $request -ExpectedStatus (200..299)
        if ($run -is [array] -or $run.PSObject.Properties.Name -cnotcontains 'id' -or !(Test-SafeId $run.id)) {
            throw 'Azure DevOps did not return a valid run ID.'
        }
        Add-Content -LiteralPath $env:GITHUB_OUTPUT -Encoding utf8 -Value "url=https://dev.azure.com/devdiv/DevDiv/_build/results?buildId=$($run.id)"
    }
    'Report' {
        $workflowUrl = "$env:GITHUB_SERVER_URL/dotnet/fsharp/actions/runs/$env:GITHUB_RUN_ID"
        $body = if ($env:RUN_URL) {
            "[F# Apex run]($env:RUN_URL) requested by @$($event.comment.user.login).`n`nReviewed commit and pipeline YAML: ``$env:HEAD_SHA```n`nThe run tests a PR merge containing this commit if it is still the PR head; otherwise it tests this commit directly. The setup log records the selected mode and tested SHA. New commits require review and a new ``/dart <sha>`` or ``/pr-val <sha>`` comment."
        }
        else {
            "F# Apex request failed. [Workflow details]($workflowUrl). Use a standalone ``/dart <sha>`` or ``/pr-val <sha>`` with a full commit SHA listed in the PR after reviewing code and pipeline YAML. The PR must target ``main`` and the requester needs repository write access and Microsoft-org membership."
        }
        $null = Invoke-Api "$repoApi/issues/$prNumber/comments" $env:GITHUB_TOKEN -Method POST -Body @{ body = $body } -ExpectedStatus 201
    }
}

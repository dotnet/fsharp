$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot ('.pr-validation-tests-' + [Guid]::NewGuid())
$count = 0
New-Item -ItemType Directory $root | Out-Null

function Assert($Condition, $Message) {
    if (!$Condition) { throw $Message }
}

function Assert-Snapshot($Request) {
    Assert ($Request.resources.repositories.self.refName -ceq 'refs/heads/main') 'Wrong YAML ref'
    Assert ($Request.resources.repositories.self.version -ceq ('b' * 40)) 'Wrong YAML revision'
    Assert ($Request.templateParameters.prNumber -is [string] -and $Request.templateParameters.prNumber -ceq '42') 'PR number must be a string'
    Assert ($Request.templateParameters.headSha -ceq ('a' * 40)) 'Wrong head snapshot'
    Assert ($Request.templateParameters.baseSha -ceq ('b' * 40)) 'Wrong base snapshot'
}

function Invoke-WebRequest {
    [CmdletBinding()]
    param($Uri, $Method, $Headers, $UserAgent, $ContentType, $MaximumRedirection, [switch]$SkipHttpErrorCheck, $Body)
    $calls.Add($PSBoundParameters)
    Assert ($responses.ContainsKey($Uri)) "Unexpected URL: $Uri"
    Assert ($Headers.Authorization -ceq "Bearer $expectedToken") 'Wrong bearer token'
    Assert ($Headers.Accept -ceq 'application/json' -and $ContentType -ceq 'application/json') 'Wrong JSON headers'
    Assert ($UserAgent -ceq 'fsharp-pr-validation') 'Missing user agent'
    Assert ($MaximumRedirection -ceq 0 -and $SkipHttpErrorCheck -and $PSBoundParameters.ErrorAction -eq 'Stop') 'HTTP handling is not fail-closed'
    $response = $responses[$Uri]
    Assert ($Method -ceq $response.Method) 'Wrong HTTP method'
    if ($response.Failure) { throw 'Network failed (test-token)' }
    @{
        StatusCode = $response.Status
        Content = if ($response.ContainsKey('Raw')) { $response.Raw } else { ConvertTo-Json -InputObject $response.Body -Depth 10 -Compress }
    }
}

function Test-Case($Name, [scriptblock]$Change = {}, $ExpectedError = '', $Mode = 'Prepare', $CallCount = -1) {
    $event = @{
        comment = @{ body = '/dart'; user = @{ login = 'maintainer'; type = 'User' } }
        issue = @{ number = 42; pull_request = @{} }
    }
    $pr = @{
        state = 'open'
        head = @{ sha = 'a' * 40; repo = @{ full_name = 'contributor/fsharp' } }
        base = @{ sha = 'b' * 40; ref = 'main'; repo = @{ full_name = 'dotnet/fsharp' } }
    }
    $permission = @{ permission = 'write' }
    $request = @{
        resources = @{ repositories = @{ self = @{ refName = 'refs/heads/main'; version = 'b' * 40 } } }
        templateParameters = @{ prNumber = '42'; headSha = 'a' * 40; baseSha = 'b' * 40 }
    }
    $permissionUri = 'https://api.github.com/repos/dotnet/fsharp/collaborators/maintainer/permission'
    $pullUri = 'https://api.github.com/repos/dotnet/fsharp/pulls/42'
    $memberUri = 'https://api.github.com/orgs/microsoft/members/maintainer'
    $queueUri = 'https://dev.azure.com/devdiv/DevDiv/_apis/pipelines/123/runs?api-version=7.1'
    $reportUri = 'https://api.github.com/repos/dotnet/fsharp/issues/42/comments'
    $responses = @{
        $permissionUri = @{ Method = 'GET'; Status = 200; Body = $permission }
        $pullUri = @{ Method = 'GET'; Status = 200; Body = $pr }
        $memberUri = @{ Method = 'GET'; Status = 204 }
        $queueUri = @{ Method = 'POST'; Status = 200; Body = @{ id = 456 } }
        $reportUri = @{ Method = 'POST'; Status = 201; Body = @{ id = 789 } }
    }
    $calls = [Collections.Generic.List[object]]::new()
    $env:GITHUB_EVENT_PATH = Join-Path $root 'event.json'
    $env:GITHUB_OUTPUT = Join-Path $root 'output'
    $env:GITHUB_REPOSITORY = 'dotnet/fsharp'; $env:GITHUB_ACTOR = 'rerunning-admin'
    $env:GITHUB_SERVER_URL = 'https://github.com'; $env:GITHUB_RUN_ID = '987'
    $env:GITHUB_TOKEN = if ($Mode -eq 'Membership') { 'membership-token' } else { 'github-token' }
    $env:PIPELINE_ID = '123'; $env:AZDO_TOKEN = 'test-token'
    $env:REQUEST_BODY = ConvertTo-Json -InputObject $request -Depth 10 -Compress
    $env:RUN_URL = ''; $env:HEAD_SHA = 'a' * 40; $env:BASE_SHA = 'b' * 40
    $expectedToken = if ($Mode -eq 'Queue') { $env:AZDO_TOKEN } else { $env:GITHUB_TOKEN }
    . $Change
    ConvertTo-Json -InputObject $event -Depth 10 | Set-Content -LiteralPath $env:GITHUB_EVENT_PATH -Encoding utf8
    Remove-Item -LiteralPath $env:GITHUB_OUTPUT -ErrorAction SilentlyContinue
    $failure = $null
    try { $stdout = @(& "$PSScriptRoot\pr-validation.ps1" -Mode $Mode) }
    catch { $failure = "$_" }
    if ($ExpectedError) {
        Assert ($failure -and $failure -match $ExpectedError) "$Name failed for wrong reason: $failure"
        Assert (!(Test-Path -LiteralPath $env:GITHUB_OUTPUT)) "$Name wrote outputs on failure"
        Assert ($failure -notmatch 'test-token|github-token|membership-token') 'Leaked token'
    }
    else {
        Assert (!$failure) "$Name failed: $failure"
        Assert ($stdout.Count -eq 0) "$Name leaked stdout"
        switch ($Mode) {
            'Prepare' {
                $lines = @(Get-Content -LiteralPath $env:GITHUB_OUTPUT)
                Assert ($lines.Count -eq 3 -and $lines[1] -ceq "head=$('a' * 40)" -and $lines[2] -ceq "base=$('b' * 40)") 'Wrong snapshot outputs'
                Assert-Snapshot (ConvertFrom-Json $lines[0].Substring(5))
                Assert ($calls[0].Uri -ceq $permissionUri -and $calls[1].Uri -ceq $pullUri) 'Wrong requester or PR'
            }
            'Queue' {
                Assert-Snapshot (ConvertFrom-Json $calls[0].Body)
                Assert ((Get-Content -LiteralPath $env:GITHUB_OUTPUT -Raw).Trim() -ceq 'url=https://dev.azure.com/devdiv/DevDiv/_build/results?buildId=456') 'Wrong run link'
            }
            'Report' {
                $body = (ConvertFrom-Json $calls[0].Body).body
                $expected = if ($env:RUN_URL) { '[F# Apex run](https://example.invalid/run) requested by @maintainer.' }
                else { 'F# Apex request failed. [Workflow details](https://github.com/dotnet/fsharp/actions/runs/987).' }
                Assert ($body.StartsWith($expected)) 'Wrong PR feedback'
                Assert ($body.Contains('`/dart`') -and $body.Contains('`/pr-val`')) 'Missing retry guidance'
                if ($env:RUN_URL) { Assert ($body.Contains("PR head: ``$env:HEAD_SHA``") -and $body.Contains("Base: ``$env:BASE_SHA``")) 'Missing snapshot identifiers' }
            }
        }
        if ($CallCount -lt 0) { $CallCount = if ($Mode -eq 'Prepare') { 2 } else { 1 } }
    }
    if ($CallCount -ge 0) { Assert ($calls.Count -eq $CallCount) "$Name made unexpected HTTP calls" }
    $script:count++
    Write-Host "PASS: $Name"
}

try {
    foreach ($command in '/dart', '/pr-val', " `n/pr-val `n") {
        foreach ($authorRepo in 'dotnet/fsharp', 'contributor/fsharp') {
            Test-Case "$command from $authorRepo" { $event.comment.body = $command; $pr.head.repo.full_name = $authorRepo }
        }
    }
    foreach ($command in '/dart abc1234', 'please /dart', '`/dart`', "/dart`n/pr-val", '/dart-extra', '/DART', "/dart`nextra", '', $null, 42) {
        Test-Case "reject command '$command'" { $event.comment.body = $command } 'standalone' -CallCount 0
    }
    foreach ($access in 'write', 'maintain', 'admin') { Test-Case "permission $access" { $permission.permission = $access } }
    foreach ($access in 'read', 'triage', 'WRITE', '', $null, @('write')) { Test-Case "reject permission '$access'" { $permission.permission = $access } 'write access' -CallCount 1 }
    Test-Case 'bot' { $event.comment.user.type = 'Bot' } 'user comments' -CallCount 0
    Test-Case 'array user type' { $event.comment.user.type = @('User') } 'user comments' -CallCount 0
    Test-Case 'issue' { $event.issue.Remove('pull_request') } 'dotnet/fsharp PRs' -CallCount 0
    Test-Case 'false PR marker' { $event.issue.pull_request = $false } 'dotnet/fsharp PRs' -CallCount 0
    Test-Case 'array event' { $event = @($event) } 'dotnet/fsharp PRs' -CallCount 0
    Test-Case 'array permission response' { $responses[$permissionUri].Body = @($permission) } 'write access'
    Test-Case 'array PR response' { $responses[$pullUri].Body = @($pr) } 'open PRs'
    foreach ($repo in 'other/fsharp', 'dotnet/other', 'DotNet/fsharp') {
        Test-Case "wrong repo $repo" { $env:GITHUB_REPOSITORY = $repo } 'dotnet/fsharp PRs' -CallCount 0
    }
    foreach ($id in '1; command', '42', $null, 0, -1, 1.5, $true, 9007199254740992) {
        Test-Case "bad PR number '$id'" { $event.issue.number = $id } 'PR number' -CallCount 0
    }
    Test-Case 'closed PR' { $pr.state = 'closed' } 'open PRs'
    Test-Case 'uppercase state' { $pr.state = 'OPEN' } 'open PRs'
    Test-Case 'array state' { $pr.state = @('open') } 'open PRs'
    Test-Case 'array target' { $pr.base.repo.full_name = @('dotnet/fsharp') } 'main'
    Test-Case 'array branch' { $pr.base.ref = @('main') } 'main'
    foreach ($branch in 'release/test', 'Main') { Test-Case "non-main $branch" { $pr.base.ref = $branch } 'main' }
    Test-Case 'foreign target' { $pr.base.repo.full_name = 'other/fsharp' } 'main'
    foreach ($side in 'head', 'base') {
        foreach ($sha in 'abcdef0', $null, ('A' * 40), (('a' * 40) + "`n"), 123, @('a' * 40)) {
            Test-Case "bad $side SHA '$sha'" { $pr[$side].sha = $sha } 'full head/base'
        }
    }
    Test-Case 'membership original commenter' -Mode Membership
    foreach ($status in 200, 201, 301, 302, 307, 308, 401, 403, 404, 429, 500) {
        Test-Case "membership HTTP $status" { $responses[$memberUri].Status = $status } "HTTP $status" Membership
    }
    foreach ($endpoint in 'permissionUri', 'pullUri', 'memberUri', 'queueUri', 'reportUri') {
        $mode = switch ($endpoint) { memberUri { 'Membership' }; queueUri { 'Queue' }; reportUri { 'Report' }; default { 'Prepare' } }
        Test-Case "$endpoint network failure" { $responses[(Get-Variable $endpoint -ValueOnly)].Failure = $true } 'HTTP request failed' $mode
        Test-Case "$endpoint API failure" { $responses[(Get-Variable $endpoint -ValueOnly)].Status = 500 } 'HTTP 500' $mode
    }
    Test-Case 'queue snapshot' -Mode Queue
    Test-Case 'queue created' { $responses[$queueUri].Status = 201 } -Mode Queue
    foreach ($json in '{"id":456.0}', '{"id":4.56e2}') {
        Test-Case "integral JSON number $json" { $responses[$queueUri].Raw = $json } -Mode Queue
    }
    foreach ($pipeline in '', '0', '123/path', "123`n", ' 123', '+123') {
        Test-Case "invalid pipeline '$pipeline'" { $env:PIPELINE_ID = $pipeline } 'Configure FSHARP_APEX_PIPELINE_ID' Queue 0
    }
    foreach ($token in '', ' ') { Test-Case 'missing queue token' { $env:AZDO_TOKEN = $token } 'access token' Queue 0 }
    Test-Case 'invalid request JSON' { $env:REQUEST_BODY = '{' } 'JSON' Queue 0
    foreach ($status in 301, 302, 303, 307, 308, 401, 403, 429, 500) {
        Test-Case "queue HTTP $status" { $responses[$queueUri].Status = $status } "HTTP $status" Queue
    }
    foreach ($id in $null, 0, -1, 1.5, '456', $true, 9007199254740992, @(456)) {
        Test-Case "invalid run ID '$id'" { $responses[$queueUri].Body.id = $id } 'valid run ID' Queue
    }
    foreach ($json in '{}', 'null', '[{"id":456}]', '{"id":[456]}', '{"id":{}}', '{"ID":456}') {
        Test-Case "invalid run $json" { $responses[$queueUri].Raw = $json } 'valid run ID' Queue
    }
    Test-Case 'invalid response JSON' { $responses[$queueUri].Raw = '<html>' } 'JSON' Queue
    Test-Case 'report success' { $env:RUN_URL = 'https://example.invalid/run' } -Mode Report
    Test-Case 'report failure' -Mode Report
    Write-Host "$count tests passed."
}
finally { Remove-Item -LiteralPath $root -Recurse -Force }

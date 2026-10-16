# Compatible with the repository's Pester 3.x runner.
# Invoke-Pester .\eng\release\scripts\Get-VSBackportBranchPoint.Tests.ps1
. "$PSScriptRoot\Get-VSBackportBranchPoint.ps1" -VSBranch fixture -FSharpRepoPath fixture -NewBranchName fixture -VSMinorVersion 0
$script:realGit = ${function:Invoke-Git}
$script:realSetMinor = ${function:Set-FixedVSMinorVersion}

Describe 'Fixed VS minor editing' {
    $literal = @'
<Project>
  <PropertyGroup>
    <VSMajorVersion>18</VSMajorVersion>
    <VSMinorVersion>8</VSMinorVersion>
    <Unrelated>keep</Unrelated>
  </PropertyGroup>
</Project>
'@

    It 'accepts zero and the maximum component, preserving unrelated content' {
        foreach ($minor in @('0', '65534')) {
            $result = Set-FixedVSMinorVersion $literal $minor
            $result | Should Match "<VSMinorVersion>$minor</VSMinorVersion>"
            $result | Should Match '<UseVSScheduledMinorVersion>false</UseVSScheduledMinorVersion>'
            $result | Should Match '<VSMajorVersion>18</VSMajorVersion>'
            $result | Should Match '<Unrelated>keep</Unrelated>'
        }
    }

    It 'rejects invalid and fractional inputs before numeric coercion' {
        foreach ($minor in @('-1', '65535', '1.5', '1e1', ' 11', 'NaN', '')) {
            { Invoke-VSBackportBranchPoint -VSBranch fixture -FSharpRepoPath fixture -NewBranchName fixture -VSMinorVersion $minor } |
                Should Throw 'VSMinorVersion'
        }
        { Invoke-VSBackportBranchPoint -VSBranch fixture -FSharpRepoPath fixture -NewBranchName fixture -VSMinorVersion 1.5 } |
            Should Throw 'VSMinorVersion'
    }

    It 'allows the same minor on multiple branches and preserves CRLF' {
        $content = $literal.Replace("`r`n", "`n").Replace("`n", "`r`n")
        $result = Set-FixedVSMinorVersion $content 8
        (Set-FixedVSMinorVersion $result 8) | Should Be $result
        $result.Replace("`r`n", '') | Should Not Match "`n"
    }

    It 'replaces a conditional scheduled default before its import' {
        $content = $literal.Replace('<Unrelated>keep</Unrelated>', @'
<UseVSScheduledMinorVersion Condition="'$(UseVSScheduledMinorVersion)' == ''">true</UseVSScheduledMinorVersion>
'@).Replace('</Project>', '  <Import Project="VSMinorVersion.props" /></Project>')
        $result = Set-FixedVSMinorVersion $content 11
        $result | Should Match '<VSMinorVersion>11</VSMinorVersion>'
        $result.IndexOf('<UseVSScheduledMinorVersion>false') | Should BeLessThan $result.IndexOf('<Import')
        $result | Should Not Match 'Condition='
    }

    It 'rejects missing, duplicate, conditional, computed and late version layouts' {
        foreach ($content in @(
            $literal.Replace('<VSMinorVersion>8</VSMinorVersion>', ''),
            $literal.Replace('<VSMinorVersion>8</VSMinorVersion>', '<VSMinorVersion>8</VSMinorVersion><VSMinorVersion>9</VSMinorVersion>'),
            $literal.Replace('<PropertyGroup>', '<PropertyGroup Condition="false">'),
            $literal.Replace('<VSMinorVersion>8', '<VSMinorVersion>$(Computed)'),
            $literal.Replace('<PropertyGroup>', '<Import Project="VSMinorVersion.props" /><PropertyGroup>')
        )) {
            { Set-FixedVSMinorVersion $content 11 } | Should Throw 'eng\Versions.props'
        }
    }

    It 'freezes the actual Versions.props when a later build timestamp is supplied' {
        $eng = Join-Path $PSScriptRoot '..\..'
        $versions = Set-FixedVSMinorVersion ([IO.File]::ReadAllText((Join-Path $eng 'Versions.props'))) 11
        $path = Join-Path $TestDrive 'Versions.props'
        [IO.File]::WriteAllText($path, $versions)
        Copy-Item (Join-Path $eng 'Version.Details.props') $TestDrive
        Copy-Item (Join-Path $eng 'VSMinorVersion.props') $TestDrive
        $output = & dotnet msbuild $path -nologo -t:ValidateVSScheduledMinorVersion `
            -p:VSBuildTimestampUtc=2026-12-02T19:00:00Z -getProperty:VSMinorVersion,UseVSScheduledMinorVersion,VSAssemblyVersion
        $LASTEXITCODE | Should Be 0
        $properties = (($output -join "`n") | ConvertFrom-Json).Properties
        $properties.VSMinorVersion | Should Be '11'
        $properties.UseVSScheduledMinorVersion | Should Be 'false'
        $properties.VSAssemblyVersion | Should Be '18.11.0.0'
    }
}

Describe 'Backport Git fixtures' {
    BeforeEach {
        $script:fixtureRoot = Join-Path (Get-Location).Path ('.vs-backport-tests-' + [Guid]::NewGuid().ToString('N'))
        $script:fsRepo = Join-Path $fixtureRoot 'fsharp'
        $script:vsRepo = Join-Path $fixtureRoot 'vs'
        foreach ($repo in @($fsRepo, $vsRepo)) {
            $null = New-Item -ItemType Directory -Path $repo -Force
            $null = Invoke-Git $repo @('init', '--quiet', '--initial-branch=main')
            $null = Invoke-Git $repo @('config', 'user.name', 'Fixture')
            $null = Invoke-Git $repo @('config', 'user.email', 'fixture@example.invalid')
            $null = Invoke-Git $repo @('config', 'core.autocrlf', 'false')
            $null = Invoke-Git $repo @('config', 'commit.gpgsign', 'false')
        }
        $null = New-Item -ItemType Directory -Path "$fsRepo\eng"
        [IO.File]::WriteAllText("$fsRepo\azure-pipelines.yml", "variables:`r`n- name: FSharpReleaseBranchName`r`n  value: main`r`n- name: VSInsertionTargetBranchName`r`n  value: main`r`n")
        [IO.File]::WriteAllText("$fsRepo\eng\Versions.props", "<Project>`r`n  <PropertyGroup>`r`n    <VSMajorVersion>18</VSMajorVersion>`r`n    <VSMinorVersion>8</VSMinorVersion>`r`n  </PropertyGroup>`r`n</Project>`r`n")
        $null = Invoke-Git $fsRepo @('add', '.')
        $null = Invoke-Git $fsRepo @('commit', '--quiet', '-m', 'source')
        $script:source = Invoke-Git $fsRepo @('rev-parse', 'HEAD')
        $null = New-Item -ItemType Directory -Path "$vsRepo\.corext\Configs" -Force
        $script:payload = 'https://example.invalid/dotnet-fsharp/main/20260911.1;payload'
        [IO.File]::WriteAllText("$vsRepo\.corext\Configs\components.json", '{"Components":{"Microsoft.FSharp":{"url":"old"}}}')
        $null = Invoke-Git $vsRepo @('add', '.')
        $null = Invoke-Git $vsRepo @('commit', '--quiet', '-m', 'old payload')
        [IO.File]::WriteAllText("$vsRepo\.corext\Configs\components.json", ('{"Components":{"Microsoft.FSharp":{"url":"' + $payload + '","version":"1"}}}'))
        $null = Invoke-Git $vsRepo @('commit', '--quiet', '-am', 'insert payload')
        $script:insertion = Invoke-Git $vsRepo @('rev-parse', 'HEAD')
        1..3 | ForEach-Object { $null = Invoke-Git $vsRepo @('commit', '--quiet', '--allow-empty', '-m', "later $_") }
        $script:arguments = @{ VSBranch = 'refs/heads/main'; VSRepoPath = $vsRepo; FSharpRepoPath = $fsRepo; NewBranchName = 'release/fixture'; VSMinorVersion = '11' }
        Mock Get-AzureDevOpsToken { 'fixture-only-token' }
        Mock Get-FSharpBuild {
            param($BuildNumber, $SourceBranch)
            if ($BuildNumber -ne '20260911.1' -or $SourceBranch -ne 'main') { throw 'Incorrect build lookup' }
            [PSCustomObject]@{ sourceVersion = $script:source; id = 42 }
        }
    }

    AfterEach {
        Remove-Item -LiteralPath $fixtureRoot -Recurse -Force
    }

    It 'previews the exact source and edits without changing either checkout' {
        $vsHead = Invoke-Git $vsRepo @('rev-parse', 'HEAD')
        $result = Invoke-VSBackportBranchPoint @arguments
        $result.Action | Should Be 'Preview'
        $result.VSBranch | Should Be 'main'
        $result.VSSnapshot | Should Be $vsHead
        $result.VSInsertionCommit | Should Be $insertion
        $result.FSharpBranchPoint | Should Be $source
        $result.VSMinorVersion | Should Be 11
        $result.PlannedEdits.Count | Should Be 2
        (Invoke-Git $fsRepo @('status', '--porcelain')) | Should BeNullOrEmpty
        (Test-GitRef $fsRepo 'refs/heads/release/fixture') | Should Be $false
        (Invoke-Git $vsRepo @('rev-parse', 'HEAD')) | Should Be $vsHead
    }

    It 'reports visible progress separately from the structured result without credentials' {
        $output = @(Invoke-VSBackportBranchPoint @arguments 6>&1)
        $progress = @($output | Where-Object { $_ -is [System.Management.Automation.InformationRecord] })
        $results = @($output | Where-Object { $_ -isnot [System.Management.Automation.InformationRecord] })
        $results.Count | Should Be 1
        $results[0].Action | Should Be 'Preview'
        $messages = ($progress | ForEach-Object { $_.MessageData }) -join "`n"
        $messages | Should Match 'Acquiring Azure DevOps credentials'
        $messages | Should Match 'Looking up fsharp-ci build'
        $messages | Should Match 'Validating pipeline mappings'
        $messages | Should Match 'Preview complete'
        $messages | Should Not Match 'fixture-only-token|Authorization'
    }

    It 'executes only in the fixture, preserving newlines and leaving uncommitted edits' {
        $result = Invoke-VSBackportBranchPoint @arguments -Execute
        $result.Action | Should Be 'Executed'
        (Invoke-Git $fsRepo @('branch', '--show-current')) | Should Be 'release/fixture'
        (Invoke-Git $fsRepo @('rev-parse', 'HEAD')) | Should Be $source
        $content = [IO.File]::ReadAllText("$fsRepo\eng\Versions.props")
        $content | Should Match '<VSMinorVersion>11</VSMinorVersion>'
        $content | Should Match '<UseVSScheduledMinorVersion>false</UseVSScheduledMinorVersion>'
        $content.Replace("`r`n", '') | Should Not Match "`n"
        ([IO.File]::ReadAllText("$fsRepo\azure-pipelines.yml")) | Should Match 'value: release/fixture'
        @(Invoke-Git $fsRepo @('status', '--porcelain')).Count | Should Be 2
    }

    It 'rejects dirty worktrees before creating a branch' {
        Add-Content "$fsRepo\azure-pipelines.yml" '# dirty'
        { Invoke-VSBackportBranchPoint @arguments -Execute } | Should Throw 'tracked working-tree changes'
        (Test-GitRef $fsRepo 'refs/heads/release/fixture') | Should Be $false
    }

    It 'fetches a missing F# source commit without changing checkout or edits' {
        $publisher = Join-Path $fixtureRoot 'publisher'
        $null = Invoke-Git $fixtureRoot @('clone', '--quiet', $fsRepo, $publisher)
        $null = Invoke-Git $publisher @('config', 'user.name', 'Fixture')
        $null = Invoke-Git $publisher @('config', 'user.email', 'fixture@example.invalid')
        $null = Invoke-Git $publisher @('config', 'commit.gpgsign', 'false')
        $null = Invoke-Git $publisher @('commit', '--quiet', '--allow-empty', '-m', 'new inserted source')
        $newSource = Invoke-Git $publisher @('rev-parse', 'HEAD')
        $null = Invoke-Git $fsRepo @('remote', 'add', 'upstream', $publisher)
        [IO.File]::AppendAllText("$fsRepo\azure-pipelines.yml", '# local edit')
        $head = Invoke-Git $fsRepo @('rev-parse', 'HEAD')
        $content = [IO.File]::ReadAllText("$fsRepo\azure-pipelines.yml")
        (Test-GitRef $fsRepo $newSource) | Should Be $false
        (Resolve-FSharpBranch $fsRepo main $newSource) | Should Be 'refs/remotes/upstream/main'
        (Test-GitRef $fsRepo $newSource) | Should Be $true
        (Invoke-Git $fsRepo @('rev-parse', 'HEAD')) | Should Be $head
        ([IO.File]::ReadAllText("$fsRepo\azure-pipelines.yml")) | Should Be $content
    }

    It 'does not fetch when a cached F# ref contains the source commit' {
        $null = Invoke-Git $fsRepo @('remote', 'add', 'upstream', (Join-Path $fixtureRoot 'nonexistent-remote'))
        (Resolve-FSharpBranch $fsRepo main $source) | Should Be 'main'
    }

    It 'rejects existing and invalid branch names' {
        foreach ($name in @('main', 'release/invalid name')) {
            $arguments.NewBranchName = $name
            { Invoke-VSBackportBranchPoint @arguments -Execute } | Should Throw 'branch'
        }
        (Invoke-Git $fsRepo @('branch', '--show-current')) | Should Be 'main'
    }

    It 'preflights malformed pipeline mappings and version layouts' {
        foreach ($file in @('azure-pipelines.yml', 'eng\Versions.props')) {
            $content = [IO.File]::ReadAllText((Join-Path $fsRepo $file))
            [IO.File]::WriteAllText((Join-Path $fsRepo $file), $content.Replace('VSInsertionTargetBranchName', 'Missing').Replace('<VSMinorVersion>8', '<VSMinorVersion>$(Computed)'))
            $null = Invoke-Git $fsRepo @('commit', '--quiet', '-am', 'unsupported file')
            $script:source = Invoke-Git $fsRepo @('rev-parse', 'HEAD')
            { Invoke-VSBackportBranchPoint @arguments -Execute } | Should Throw 'Expected one'
            (Test-GitRef $fsRepo 'refs/heads/release/fixture') | Should Be $false
            $null = Invoke-Git $fsRepo @('revert', '--no-edit', 'HEAD')
        }
    }

    It 'preflights the source SHA rather than a repaired current file' {
        $null = Invoke-Git $fsRepo @('rm', '--quiet', 'eng/Versions.props')
        $null = Invoke-Git $fsRepo @('commit', '--quiet', '-m', 'missing versions')
        $script:source = Invoke-Git $fsRepo @('rev-parse', 'HEAD')
        $null = Invoke-Git $fsRepo @('revert', '--no-edit', 'HEAD')
        { Invoke-VSBackportBranchPoint @arguments -Execute } | Should Throw 'git'
        { Invoke-VSBackportBranchPoint @arguments } | Should Throw 'git'
        (Test-GitRef $fsRepo 'refs/heads/release/fixture') | Should Be $false
    }

    It 'rejects ignored file collisions before switching branches' {
        $null = Invoke-Git $fsRepo @('rm', '--quiet', 'eng/Versions.props')
        $null = Invoke-Git $fsRepo @('commit', '--quiet', '-m', 'remove versions')
        $null = New-Item -ItemType Directory -Path "$fsRepo\eng" -Force
        [IO.File]::WriteAllText("$fsRepo\eng\Versions.props", 'user content')
        [IO.File]::WriteAllText("$fsRepo\.git\info\exclude", 'eng/Versions.props')
        { Invoke-VSBackportBranchPoint @arguments -Execute } | Should Throw 'collides'
        (Test-GitRef $fsRepo 'refs/heads/release/fixture') | Should Be $false
        ([IO.File]::ReadAllText("$fsRepo\eng\Versions.props")) | Should Be 'user content'
    }

    It 'rejects a shallow false introduction and deepens to the real insertion' {
        $clone = Join-Path $fixtureRoot 'shallow'
        $url = ([Uri]($vsRepo + '\')).AbsoluteUri.TrimEnd('/')
        $null = Invoke-Git $fixtureRoot @('clone', '--quiet', '--depth=1', '--no-checkout', $url, $clone)
        $branch = Resolve-VisualStudioBranch $clone main
        $snapshot = Invoke-Git $clone @('rev-parse', 'HEAD')
        { Get-VisualStudioInsertion $clone $snapshot $payload $branch -MaxDeepenAttempts 0 } | Should Throw 'Insufficient'
        { Get-VisualStudioInsertion $clone $snapshot $payload $branch -DeepenBy 1 -MaxDeepenAttempts 1 } | Should Throw 'Insufficient'
        (Get-VisualStudioInsertion $clone $snapshot $payload $branch -DeepenBy 2 -MaxDeepenAttempts 3) | Should Be $insertion
        (Resolve-VisualStudioBranch $clone 'refs/remotes/origin/main').Branch | Should Be 'main'
        (Test-Path "$clone\.corext") | Should Be $false
        (Invoke-Git $clone @('rev-parse', 'HEAD')) | Should Be $snapshot
    }

    It 'uses and cleans an owned no-checkout clone without live services' {
        Mock Get-AzureDevOpsToken { 'fixture-only-token' }
        Mock Invoke-Git {
            param($RepositoryPath, $Arguments, $AccessToken)
            if ($Arguments[0] -eq 'clone') {
                $script:ownedClone = $RepositoryPath
                $Arguments = @($Arguments | ForEach-Object {
                    if ($_ -eq 'https://devdiv.visualstudio.com/DefaultCollection/DevDiv/_git/VS') {
                        ([Uri]($script:vsRepo + '\')).AbsoluteUri.TrimEnd('/')
                    } else { $_ }
                })
            }
            & $script:realGit -RepositoryPath $RepositoryPath -Arguments $Arguments -AccessToken $AccessToken
        }
        $arguments.Remove('VSRepoPath')
        $result = Invoke-VSBackportBranchPoint @arguments
        $result.VSInsertionCommit | Should Be $insertion
        $result.FSharpBranchPoint | Should Be $source
        (Test-Path -LiteralPath $ownedClone) | Should Be $false
        (Test-Path -LiteralPath $vsRepo) | Should Be $true
    }

    It 'cleans only its owned clone on authentication or clone failure' {
        Mock Get-AzureDevOpsToken { 'fixture-only-token' }
        Mock Invoke-Git {
            param($RepositoryPath, $Arguments, $AccessToken)
            if ($Arguments[0] -eq 'clone') {
                $script:ownedClone = $RepositoryPath
                throw 'Fixture clone denied'
            }
            & $script:realGit -RepositoryPath $RepositoryPath -Arguments $Arguments -AccessToken $AccessToken
        }
        $arguments.Remove('VSRepoPath')
        { Invoke-VSBackportBranchPoint @arguments } | Should Throw 'Fixture clone denied'
        (Test-Path -LiteralPath $ownedClone) | Should Be $false
        (Test-Path -LiteralPath $vsRepo) | Should Be $true
    }

    It 'restores process authentication settings and never persists the header' {
        $old = $env:GIT_CONFIG_COUNT
        try {
            $env:GIT_CONFIG_COUNT = '0'
            $null = Invoke-Git $vsRepo @('status', '--porcelain') -AccessToken 'fixture-only-token'
            $env:GIT_CONFIG_COUNT | Should Be '0'
            ([IO.File]::ReadAllText("$vsRepo\.git\config")) | Should Not Match 'fixture-only-token|extraHeader'
            { Invoke-Git $vsRepo @('invalid-command') -AccessToken 'fixture-only-token' } |
                Should Throw 'Authenticated Git operation failed'
            $env:GIT_CONFIG_COUNT | Should Be '0'
        }
        finally { $env:GIT_CONFIG_COUNT = $old }
    }

    It 'fails authentication before creating an owned clone' {
        Mock Get-AzureDevOpsToken { throw 'Fixture authentication denied' }
        $arguments.Remove('VSRepoPath')
        { Invoke-VSBackportBranchPoint @arguments } | Should Throw 'Fixture authentication denied'
        (Test-Path -LiteralPath $vsRepo) | Should Be $true
        (Invoke-Git $fsRepo @('status', '--porcelain')) | Should BeNullOrEmpty
    }

    It 'reports partial writes without rolling back the created branch' {
        Mock Set-FixedVSMinorVersion {
            param($Content, $Minor)
            if ((Invoke-Git $script:fsRepo @('branch', '--show-current')) -eq 'release/fixture') {
                throw 'Fixture write denied'
            }
            & $script:realSetMinor $Content $Minor
        }
        { Invoke-VSBackportBranchPoint @arguments -Execute } | Should Throw 'may be partial'
        (Test-GitRef $fsRepo 'refs/heads/release/fixture') | Should Be $true
        ([IO.File]::ReadAllText("$fsRepo\azure-pipelines.yml")) | Should Match 'value: release/fixture'
        ([IO.File]::ReadAllText("$fsRepo\eng\Versions.props")) | Should Match '<VSMinorVersion>8</VSMinorVersion>'
    }
}

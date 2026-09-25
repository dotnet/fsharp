# Run with: Invoke-Pester eng/tests/PublishTestResults.Tests.ps1 -EnableExit

Describe 'Per-file test result publication' {
    BeforeEach {
        $script:publication = @{ Commands = @(); Runs = @(); Mode = 'complete'; Polls = 0 }
        $script:publisher = Join-Path $PSScriptRoot '..\PublishTestResults.ps1'
        $script:results = Join-Path $TestDrive 'results'
        New-Item $results -ItemType Directory -Force | Out-Null

        Mock Write-Host {
            param($Object)
            $line = [string]$Object
            if ($line -like '##vso[[]results.publish*') {
                if ($publication.Runs | Where-Object state -ne 'Completed') {
                    throw 'The next file was published before the previous run completed.'
                }

                $publication.Commands += $line
                $title, $file = @(
                    [regex]::Match($line, 'runTitle=([^;]+);').Groups[1].Value
                    $line.Substring($line.IndexOf(']') + 1)
                ) | ForEach-Object { $_.Replace('%0D', "`r").Replace('%0A', "`n").Replace('%3B', ';').Replace('%5D', ']').Replace('%AZP25', '%') }
                $xml = [xml][IO.File]::ReadAllText($file)
                $count = @($xml.SelectNodes('//test') | ForEach-Object { $_.GetAttribute('name') } | Select-Object -Unique).Count
                $publication.Runs += [pscustomobject]@{
                    id = $(if ($publication.Mode -eq 'reused') { 1 } else { $publication.Commands.Count })
                    name = $(if ($publication.Mode -eq 'suffix') { "${title}_1" } else { $title })
                    state = 'InProgress'
                    totalTests = $(if ($publication.Mode -eq 'truncated') { $count - 1 } else { $count })
                    incompleteTests = $(if ($publication.Mode -eq 'incomplete') { 1 } else { 0 })
                }
            }
        }
        Mock Invoke-RestMethod {
            $publication.Polls++
            if ($publication.Mode -ne 'stuck' -and ($publication.Mode -ne 'delayed' -or $publication.Polls -gt 2)) {
                $publication.Runs | ForEach-Object { $_.state = 'Completed' }
            }
            if ($Uri -match '/runs/(\d+)\?') {
                return $publication.Runs | Where-Object id -eq $Matches[1] | Select-Object -Last 1
            }
            if ($publication.Mode -eq 'missing' -or ($publication.Mode -eq 'late' -and $publication.Polls -eq 1)) {
                return @{ value = @() }
            }
            if ($publication.Mode -eq 'ambiguous') {
                return @{ value = @($publication.Runs[0], @{ id = 99; name = "$($publication.Runs[0].name)_1" }) }
            }
            return @{ value = $publication.Runs }
        }
        Mock Start-Sleep {}

        $script:savedEnvironment = @{}
        foreach ($name in @('SYSTEM_ACCESSTOKEN', 'SYSTEM_COLLECTIONURI', 'SYSTEM_TEAMPROJECT', 'BUILD_BUILDURI', 'SYSTEM_JOBID', 'SYSTEM_JOBATTEMPT')) {
            $script:savedEnvironment[$name] = [Environment]::GetEnvironmentVariable($name)
            [Environment]::SetEnvironmentVariable($name, 'test')
        }
        $env:SYSTEM_COLLECTIONURI = 'https://dev.azure.com/example/'

        foreach ($count in @(117, 6175)) {
            $tests = (1..$count | ForEach-Object { "<test name='Case$_' result='Pass' />" }) -join ''
            Set-Content (Join-Path $results "$count.xml") "<assemblies><assembly total='$count'><collection>$tests</collection></assembly></assemblies>"
        }
    }

    AfterEach {
        foreach ($name in $savedEnvironment.Keys) {
            [Environment]::SetEnvironmentVariable($name, $savedEnvironment[$name])
        }
    }

    It 'publishes and verifies a separate complete run for each XML (<Mode>)' -TestCases @(
        @{ Mode = 'complete' }
        @{ Mode = 'suffix' }
        @{ Mode = 'duplicate-names' }
        @{ Mode = 'case-distinct-names' }
    ) {
        param($Mode)
        $publication.Mode = $Mode
        if ($Mode -in @('duplicate-names', 'case-distinct-names')) {
            foreach ($file in Get-ChildItem $results -Filter '*.xml') {
                $xml = [xml](Get-Content $file.FullName -Raw)
                $test = $xml.SelectSingleNode('//test').CloneNode($true)
                $collection = $xml.CreateElement('collection')
                $null = $collection.AppendChild($test)
                $null = $xml.assemblies.assembly.AppendChild($collection)
                if ($Mode -eq 'case-distinct-names') { $test.SetAttribute('name', 'case1') }
                $xml.Save($file.FullName)
            }
        }
        & $publisher -ResultsDirectory $results -RunTitle 'Linux Batch1' -TimeoutSeconds 0
        $publication.Commands.Count | Should Be 2
        @($publication.Runs.id | Select-Object -Unique).Count | Should Be 2
        $expected = if ($Mode -eq 'case-distinct-names') { '118,6176' } else { '117,6175' }
        ($publication.Runs.totalTests -join ',') | Should Be $expected
        foreach ($command in $publication.Commands) {
            $command | Should Match 'type=XUnit;mergeResults=false;'
            $command | Should Match 'publishRunAttachments=true;'
        }
        Assert-MockCalled Invoke-RestMethod -Times 2 -Exactly -Scope It -ParameterFilter { $Uri -match '/runs/\d+\?' }
        Assert-MockCalled Invoke-RestMethod -Times 1 -Exactly -Scope It -ParameterFilter { $Uri -match '/runs/1\?' }
        Assert-MockCalled Invoke-RestMethod -Times 1 -Exactly -Scope It -ParameterFilter { $Uri -match '/runs/2\?' }
    }

    It 'rejects <Mode> publication rather than silently losing results' -TestCases @(
        @{ Mode = 'truncated'; Error = 'expected 117'; Published = 1 }
        @{ Mode = 'incomplete'; Error = '1 incomplete'; Published = 1 }
        @{ Mode = 'reused'; Error = 'reused'; Published = 2 }
        @{ Mode = 'stuck'; Error = 'Timed out'; Published = 1 }
        @{ Mode = 'missing'; Error = 'Timed out'; Published = 1 }
        @{ Mode = 'ambiguous'; Error = 'Multiple test runs'; Published = 1 }
    ) {
        param($Mode, $Error, $Published)
        $publication.Mode = $Mode
        { & $publisher -ResultsDirectory $results -RunTitle 'Linux Batch1' -TimeoutSeconds 0 } | Should Throw $Error
        $publication.Commands.Count | Should Be $Published
    }

    It 'waits for <Mode> runs before publishing the next file' -TestCases @(
        @{ Mode = 'delayed' }
        @{ Mode = 'late' }
    ) {
        param($Mode)
        $publication.Mode = $Mode
        & $publisher -ResultsDirectory $results -RunTitle 'Linux Batch1'
        Assert-MockCalled Start-Sleep -Times 1 -Exactly -Scope It
        $publication.Commands.Count | Should Be 2
    }

    It 'escapes logging command properties and file names' {
        Get-ChildItem $results -Filter '*.xml' | Rename-Item -NewName { "space %;] $($_.Name)" }
        & $publisher -ResultsDirectory $results -RunTitle 'Linux %;] Batch1'
        $publication.Commands.Count | Should Be 2
        $publication.Commands[0] | Should Match 'Linux %AZP25%3B%5D Batch1'
        foreach ($command in $publication.Commands) {
            $command | Should Match ']([^]]*)space %AZP25%3B%5D (117|6175)\.xml$'
        }
    }

    It 'warns without publishing when the results directory is <Directory>' -TestCases @(
        @{ Directory = 'empty' }
        @{ Directory = 'missing' }
    ) {
        param($Directory)
        $path = Join-Path $TestDrive $Directory
        if ($Directory -eq 'empty') { New-Item $path -ItemType Directory | Out-Null }
        Mock Write-Warning {}
        & $publisher -ResultsDirectory $path -RunTitle 'Linux Batch1'
        Assert-MockCalled Write-Warning -Times 1 -Exactly -Scope It
        $publication.Commands.Count | Should Be 0
    }
}

Describe 'Test result publishing pipelines' {
    It 'uses isolated publication in <File>' -TestCases @(
        @{ File = 'azure-pipelines-PR.yml'; Calls = 2 }
        @{ File = 'azure-pipelines.yml'; Calls = 1 }
        @{ File = 'eng\templates\batched-test-steps.yml'; Calls = 1 }
    ) {
        param($File, $Calls)
        $yaml = Get-Content (Join-Path $PSScriptRoot "..\..\$File") -Raw
        ($yaml -match 'task: PublishTestResults@2') | Should Be $false
        [regex]::Matches($yaml, 'template: /eng/templates/publish-test-results.yml').Count | Should Be $Calls
    }
}

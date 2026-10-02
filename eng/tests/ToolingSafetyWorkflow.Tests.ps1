# Run with: Invoke-Pester eng/tests/ToolingSafetyWorkflow.Tests.ps1 -EnableExit

Describe 'Tooling safety workflow' {
    BeforeEach {
        $yaml = Get-Content (Join-Path $PSScriptRoot '..\..\.github\workflows\labelops-pr-security-scan.lock.yml') -Raw
        $jobs = @{}
        foreach ($match in [regex]::Matches($yaml, '(?ms)^  (\w+):\r?\n(.*?)(?=^  \w+:|\z)')) {
            $jobs[$match.Groups[1].Value] = $match.Groups[2].Value
        }
        $agent = $jobs['agent']
    }

    It 'does not check out PR code in the API-only agent' {
        ($agent -match 'uses: actions/checkout@|checkout_pr_branch\.cjs') | Should Be $false
    }

    It 'makes the selected PR output available to the post-step' {
        $selection = [regex]::Match($agent, 'SCANNED_PRS: \$\{\{ needs\.(\w+)\.outputs\.(\w+) \}\}')
        $selection.Success | Should Be $true
        $job, $output = $selection.Groups[1].Value, $selection.Groups[2].Value
        $needs = [regex]::Match($agent, '(?m)^    needs:([^\r\n]*(?:\r?\n +\- [^\r\n]+)*)').Groups[1].Value
        $needs | Should Match "\b$job\b"
        $jobs[$job] | Should Match "(?m)^      ${output}: "
    }
}

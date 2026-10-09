# Run with: Invoke-Pester eng/tests/ToolingSafetyWorkflow.Tests.ps1 -EnableExit

Describe 'Tooling safety workflow' {
    function Assert-ScannerBaselineHandoff($Jobs) {
        $Jobs['pre_activation'] | Should Match '(?m)^      - uses: actions/upload-artifact@[^\r\n]+\r?\n        with:\r?\n          name: scanner-categories\r?\n          path: \$\{\{ runner\.temp \}\}/scanner-categories\.json(?:\r?\n|\z)'
        $Jobs['agent'] | Should Match '(?m)^      - uses: actions/download-artifact@[^\r\n]+\r?\n        with:\r?\n          name: scanner-categories\r?\n          path: /tmp/gh-aw/(?:\r?\n|\z)'
        $Jobs['agent'] | Should Match "fs\.readFileSync\('/tmp/gh-aw/scanner-categories\.json', 'utf8'\)"
    }

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

    It 'gates activation on the pre-activation proceed output' {
        $jobs['activation'] | Should Match '(?m)^    needs: pre_activation\r?$'
        $jobs['activation'] | Should Match "needs\.pre_activation\.result == 'success' && needs\.pre_activation\.outputs\.proceed == 'true'"
        $jobs['pre_activation'] | Should Match '(?m)^      proceed: \$\{\{ steps\.gate\.outputs\.proceed \}\}\r?$'
    }

    It 'hands the category baseline from pre-activation to the comment filter' {
        Assert-ScannerBaselineHandoff $jobs
    }

    It 'rejects a <Defect> baseline handoff' -TestCases @(
        @{ Defect = 'missing upload'; Job = 'pre_activation'; From = 'uses: actions/upload-artifact@'; To = 'uses: missing@' }
        @{ Defect = 'missing download'; Job = 'agent'; From = 'uses: actions/download-artifact@'; To = 'uses: missing@' }
        @{ Defect = 'mismatched artifact name'; Job = 'agent'; From = 'name: scanner-categories'; To = 'name: other-categories' }
        @{ Defect = 'mismatched upload path'; Job = 'pre_activation'; From = 'path: ${{ runner.temp }}/scanner-categories.json'; To = 'path: ${{ runner.temp }}/other.json' }
        @{ Defect = 'mismatched download path'; Job = 'agent'; From = 'path: /tmp/gh-aw/'; To = 'path: /tmp/other/' }
        @{ Defect = 'mismatched filter input'; Job = 'agent'; From = "fs.readFileSync('/tmp/gh-aw/scanner-categories.json'"; To = "fs.readFileSync('/tmp/gh-aw/other.json'" }
    ) {
        param($Defect, $Job, $From, $To)
        $broken = $jobs.Clone()
        $broken[$Job] = $broken[$Job].Replace($From, $To)
        { Assert-ScannerBaselineHandoff $broken } | Should Throw
    }
}

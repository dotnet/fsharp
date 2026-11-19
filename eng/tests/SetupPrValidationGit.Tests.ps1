Describe 'PR validation fixture Git discovery' {
    It 'uses one executable when command discovery returns multiple applications' {
        $fixture = Join-Path $PSScriptRoot 'SetupPrValidation.Tests.ps1'
        $application = Get-Command git -CommandType Application | Select-Object -First 1
        Mock Get-Command { @($application, $application) } -ParameterFilter {
            $Name -eq 'git' -and $CommandType -eq 'Application'
        }

        { & $fixture } | Should Not Throw
        Assert-MockCalled Get-Command -Times 1 -Exactly
    }
}

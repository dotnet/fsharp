param([Parameter(Mandatory)][string] $PerformanceRoot)

@{
    sdk10 = @{
        host = "$PerformanceRoot\sdk10\dotnet.exe"
        compiler = "$PerformanceRoot\sdk10\sdk\10.0.100\FSharp"
        runtime = '10.0.0'
        version = '43.10.100-rc2.25523.111+b0f34d51fccc69fd334253924abd8d6853fad7aa'
    }
    sdk11rc1 = @{
        host = "$env:ProgramFiles\dotnet\dotnet.exe"
        compiler = "$env:ProgramFiles\dotnet\sdk\11.0.100-rc.1.26425.128\FSharp"
        runtime = '11.0.0-rc.1.26425.128'
        version = '43.13.101-rc1.26425.128+3551975be08744f0418857c5bed8ab1545c5dd47'
    }
    'vmr-rc2-r2r' = @{
        host = "$env:ProgramFiles\dotnet\dotnet.exe"
        compiler = "$PerformanceRoot\rc2-r2r"
        runtime = '11.0.0-rc.1.26425.128'
        version = '43.13.101+9cd6167a7265ce7264b22719503b7dfa9eb8f83c'
    }
}

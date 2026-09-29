param([Parameter(Mandatory)][string] $PerformanceRoot)

$chains = [ordered]@{}
foreach ($runtime in 'framework', 'net10', 'net11') {
    foreach ($payload in 'old', 'new') {
        $classic = $runtime -eq 'framework'
        $hostPath = if ($classic) { "$PerformanceRoot\framework\collector-bin\Release\net472\Collector.exe" }
            elseif ($runtime -eq 'net10') { "$PerformanceRoot\sdk10\dotnet.exe" }
            else { "$env:ProgramFiles\dotnet\dotnet.exe" }
        $version = if ($runtime -eq 'net10') { '10.0.0' } elseif ($runtime -eq 'net11') { '11.0.0-rc.1.26425.128' } else { '4.8.9345.0' }
        $chains["$payload-$runtime"] = @{
            host=$hostPath; runtime=$version; family=$runtime
            collector=if($classic){$hostPath}else{"$PerformanceRoot\framework\collector-bin\Release\net10.0\Collector.dll"}
            payload="$PerformanceRoot\framework\$payload-payload"
            fcs_version=if($payload -eq 'old'){'43.10.100-rc2.25523.111+b0f34d51fccc69fd334253924abd8d6853fad7aa'}
                else{'43.13.101+9cd6167a7265ce7264b22719503b7dfa9eb8f83c'}
        }
    }
}
$chains

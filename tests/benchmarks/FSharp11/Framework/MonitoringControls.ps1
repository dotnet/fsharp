param([Parameter(Mandatory)][string] $PerformanceRoot)

$ErrorActionPreference = 'Stop'
foreach ($case in 'oxpecker', 'fsharp-compiler-service') {
    $rounds = if ($case -eq 'oxpecker') { 6 } else { 3 }
    for ($round = 0; $round -lt $rounds; $round++) {
        $arms = if ($round % 2 -eq 0) { @('old-framework', 'new-framework') }
            else { @('new-framework', 'old-framework') }
        foreach ($arm in $arms) {
            $modes = if ($round % 2 -eq 0) { @('on', 'off') } else { @('off', 'on') }
            foreach ($mode in $modes) {
                & "$PSScriptRoot\Measure.ps1" -PerformanceRoot $PerformanceRoot -Case $case `
                    -StartRound $round -EndRound ($round + 1) -Arms $arm `
                    -Cohort "monitoring-$mode" -NoAllocation:($mode -eq 'off')
            }
        }
    }
}

param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [string[]] $Cases = @('fsharp-core', 'fsharp-compiler-service', 'fstoolkit', 'oxpecker', 'nu', 'fsautocomplete')
)

$ErrorActionPreference = 'Stop'
$directory = Join-Path $PerformanceRoot 'cli-validation'
New-Item -ItemType Directory -Force $directory | Out-Null
foreach ($id in $Cases) {
    $casePath = Join-Path $PerformanceRoot "case-$id.json"
    $case = Get-Content $casePath -Raw | ConvertFrom-Json
    $outputs = @($case.arguments | Where-Object { $_ -match '^(-o:|--out:)' })
    if ($outputs.Count -ne 1) { throw "Expected one compiler output: $id" }
    $relativeOutput = [regex]::Match($outputs[0], '^(-o:|--out:)(.*)$').Groups[2].Value.Trim('"')
    $output = [IO.Path]::GetFullPath($relativeOutput, $case.working_directory)
    foreach ($arm in @('sdk10', 'sdk11rc1')) {
        $hostPath = if ($arm -eq 'sdk10') { "$PerformanceRoot\sdk10\dotnet.exe" } else { 'C:\Program Files\dotnet\dotnet.exe' }
        $runtime = if ($arm -eq 'sdk10') { '10.0.0' } else { '11.0.0-rc.1.26425.128' }
        $sdk = if ($arm -eq 'sdk10') {
            "$PerformanceRoot\sdk10\sdk\10.0.100\FSharp"
        } else {
            'C:\Program Files\dotnet\sdk\11.0.100-rc.1.26425.128\FSharp'
        }
        $name = "$id-$arm"
        & $hostPath exec --fx-version $runtime --roll-forward Disable "$PerformanceRoot\collector\Collector.dll" `
            $sdk $casePath "$directory\$name-fcs.json" compile 2>&1 | Set-Content "$directory\$name-fcs.log"
        if ($LASTEXITCODE -ne 0) { throw "FCS equivalence run failed: $name" }
        $fcsHash = (Get-FileHash $output).Hash
        $backup = "$directory\$name-fcs.dll"
        Copy-Item -LiteralPath $output -Destination $backup
        [string[]] $compilerArguments = @($case.arguments | Select-Object -Skip 1)
        if ($compilerArguments | Where-Object { $_ -match '[\r\n]' }) { throw 'Multiline compiler argument' }
        $response = "$directory\$name.rsp"
        $compilerArguments | Set-Content $response
        Push-Location $case.working_directory
        try {
            & $hostPath exec --fx-version $runtime --roll-forward Disable "$sdk\fsc.dll" "@$response" `
                2>&1 | Set-Content "$directory\$name-cli.log"
            if ($LASTEXITCODE -ne 0) { throw "CLI equivalence run failed: $name" }
        } finally {
            Pop-Location
        }
        $cliHash = (Get-FileHash $output).Hash
        $record = [ordered]@{
            case=$id; toolchain=$arm; input_sha256=(Get-FileHash $casePath).Hash
            response_sha256=(Get-FileHash $response).Hash
            fcs_output_sha256=$fcsHash; cli_output_sha256=$cliHash; identical=$fcsHash -eq $cliHash
        }
        $record | ConvertTo-Json | Set-Content "$directory\$name-equivalence.json"
        if ($fcsHash -ne $cliHash) { throw "FCS and CLI outputs differ: $name; retained $backup" }
        Remove-Item -LiteralPath $backup
        Write-Output "${name}: identical emitted DLLs."
    }
}
@(Get-ChildItem $directory -Filter '*-equivalence.json' | Sort-Object Name |
    ForEach-Object { Get-Content $_.FullName -Raw | ConvertFrom-Json }) |
    ConvertTo-Json -Depth 4 | Set-Content "$directory\equivalence.json"

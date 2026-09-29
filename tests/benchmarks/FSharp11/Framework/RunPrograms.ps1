param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [switch] $Prepare,
    [switch] $ValidateOnly,
    [string] $ValidationName='validation',
    [ValidateRange(0,2)][int] $Launch=0,
    [string[]] $Arms=@('old-framework','new-framework','old-net10','new-net10','old-net11','new-net11')
)

$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath($PerformanceRoot)
$chains=& "$PSScriptRoot\Toolchains.ps1" -PerformanceRoot $root
if ($Prepare) {
    foreach ($payload in 'old','new') {
        $directory="$root\framework\programs\$payload"
        New-Item -ItemType Directory -Force $directory | Out-Null
        if(Test-Path "$directory\Kernels.dll"){throw 'Compiled kernels already exist.'}
        $arguments=@('fsc.dll','--target:library','--targetprofile:netstandard','--noframework','--simpleresolution',
            '--optimize+','--debug:portable','--deterministic+',"-o:$directory\Kernels.dll",
            "-r:$root\framework\$payload-payload\FSharp.Core.dll")
        $arguments+=@(Get-ChildItem 'C:\Nuget\netstandard.library\2.0.3\build\netstandard2.0\ref\*.dll'|
            Sort-Object Name|ForEach-Object{"-r:$($_.FullName)"})
        $arguments+=(Join-Path (Split-Path $PSScriptRoot) 'Kernels.fs')
        [ordered]@{id="kernels-$payload";working_directory=$directory;arguments=$arguments}|
            ConvertTo-Json -Depth 5|Set-Content "$directory\case.json"
        & "$root\framework\collector-bin\Release\net472\Collector.exe" "$root\framework\$payload-payload" `
            "$directory\case.json" "$directory\compilation.json" compile *> "$directory\compilation.log"
        if($LASTEXITCODE -ne 0){throw "Kernel compilation failed: $payload"}
    }
}
foreach($arm in $Arms){
    if(-not $chains.Contains($arm)){throw "Unknown arm: $arm"}
    $tc=$chains[$arm]
    $payload=($arm -split '-')[0]
    $directory="$root\framework\programs\$arm"
    New-Item -ItemType Directory -Force $directory|Out-Null
    $output=if($ValidateOnly){"$directory\$ValidationName"}else{"$directory\launch-$Launch"}
    if(Test-Path "$output\provenance.json"){throw "Existing suite: $output"}
    $classic=$tc.family -eq 'framework'
    $driver=if($classic){"$root\framework\program-bin\Release\net472\Programs.exe"}
        else{"$root\framework\program-bin\Release\net10.0\Programs.dll"}
    $info=[Diagnostics.ProcessStartInfo]::new($(if($classic){$driver}else{$tc.host}))
    $info.UseShellExecute=$false
    $info.RedirectStandardOutput=$true
    $info.RedirectStandardError=$true
    foreach($key in @($info.Environment.Keys)){
        if($key -match '^(DOTNET_|COMPlus_)'){[void]$info.Environment.Remove($key)}
    }
    if(-not $classic){
        $info.Environment['DOTNET_ROOT']=Split-Path $tc.host
        foreach($arg in @('exec','--fx-version',$tc.runtime,'--roll-forward','Disable',$driver)){$info.ArgumentList.Add($arg)}
    }
    $info.Environment['FSHARP_WORKLOAD_DLL']="$root\framework\programs\$payload\Kernels.dll"
    $info.Environment['FSHARP_WORKLOAD_CORE']="$root\framework\$payload-payload\FSharp.Core.dll"
    $info.Environment['FSHARP_PROGRAM_RESULTS']=$output
    if($ValidateOnly){$info.ArgumentList.Add('--validate')}else{$info.ArgumentList.Add('--filter');$info.ArgumentList.Add('*')}
    $process=[Diagnostics.Process]::Start($info)
    $stdout=$process.StandardOutput.ReadToEndAsync()
    $stderr=$process.StandardError.ReadToEndAsync()
    if(-not $process.WaitForExit(5400000)){
        $process.Kill($true);$process.WaitForExit();throw "Suite timeout: $arm"
    }
    $stdout.GetAwaiter().GetResult()|Set-Content "$output.stdout"
    $stderr.GetAwaiter().GetResult()|Set-Content "$output.stderr"
    $code=$process.ExitCode
    $process.Dispose()
    if($code -ne 0){throw "Suite failed: $arm; inspect $output.stderr"}
    $p=Get-Content "$output\provenance.json" -Raw|ConvertFrom-Json
    $runtime=if($classic){".NET Framework $($tc.runtime)"}else{".NET $($tc.runtime)"}
    if($p.runtime -ne $runtime -or $p.gc_server){throw "Unexpected runtime/application GC: $arm"}
    Write-Output "$arm launch $Launch completed on $runtime."
}

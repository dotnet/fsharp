[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$assemblyVersionsPath
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

try {
    [string[]]$lines = Get-Content -Path $assemblyVersionsPath | ForEach-Object {
        $parts = $_ -Split ",",2
        $asm = $parts[0]
        $ver = $parts[1]
        $asmConst = ($asm -Replace "\.","") + "Version"
        $output = "$asmConst=$ver"
        $output
    }

    $final = $lines -Join ","
    Write-Host "Setting InsertVersionsValues to $final"
    Write-Host "##vso[task.setvariable variable=InsertVersionsValues]$final"
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}
